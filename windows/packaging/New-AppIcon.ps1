<#
.SYNOPSIS
    Draws the SmartTicker app icon and writes every packaging asset from a single definition.

.DESCRIPTION
    Renders three ascending bars over a ticker-tape rule on a rounded plate, using the app's own
    palette. Produces the MSIX PNG assets, the multi-resolution .ico embedded in the executable,
    and the PNG the Debian package installs, so all three platforms stay identical.

.EXAMPLE
    ./New-AppIcon.ps1
#>
[CmdletBinding()]
param(
    [string]$AssetsDirectory = (Join-Path $PSScriptRoot 'Assets'),
    [string]$IconPath = (Join-Path (Split-Path -Parent $PSScriptRoot) 'src/SmartTicker.Desktop/Assets/SmartTicker.ico')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

$plateTop = [System.Drawing.ColorTranslator]::FromHtml('#1B2634')
$plateBottom = [System.Drawing.ColorTranslator]::FromHtml('#0D1219')
$barColors = @('#79C0FF', '#00E5FF', '#3FB950') |
    ForEach-Object { [System.Drawing.ColorTranslator]::FromHtml($_) }
$tapeColor = [System.Drawing.ColorTranslator]::FromHtml('#FFA657')

function New-RoundedPath {
    param(
        [Parameter(Mandatory)][single]$X,
        [Parameter(Mandatory)][single]$Y,
        [Parameter(Mandatory)][single]$Width,
        [Parameter(Mandatory)][single]$Height,
        [Parameter(Mandatory)][single]$Radius
    )

    $radius = [Math]::Min($Radius, [Math]::Min($Width, $Height) / 2)
    $diameter = $radius * 2
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    if ($radius -le 0) {
        $path.AddRectangle((New-Object System.Drawing.RectangleF($X, $Y, $Width, $Height)))
        return $path
    }

    $path.AddArc($X, $Y, $diameter, $diameter, 180, 90)
    $path.AddArc(($X + $Width - $diameter), $Y, $diameter, $diameter, 270, 90)
    $path.AddArc(($X + $Width - $diameter), ($Y + $Height - $diameter), $diameter, $diameter, 0, 90)
    $path.AddArc($X, ($Y + $Height - $diameter), $diameter, $diameter, 90, 90)
    $path.CloseFigure()
    return $path
}

function New-IconBitmap {
    param(
        [Parameter(Mandatory)][int]$Width,
        [Parameter(Mandatory)][int]$Height,
        [switch]$IncludeWordmark
    )

    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        # Grid-fit hinting assumes an opaque background and fringes badly over transparency.
        $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAlias
        $g.Clear([System.Drawing.Color]::Transparent)

        # The motif box is square even on the wide tile, so the mark never distorts.
        $plateSize = [single][Math]::Min($Width, $Height)
        $pad = $plateSize * 0.06
        $side = $plateSize - ($pad * 2)
        $plateX = if ($IncludeWordmark) { $pad } else { ($Width - $side) / 2 }
        $plateY = ($Height - $side) / 2

        # The wide tile carries its own full-width plate so the wordmark never sits on bare canvas.
        $backX = if ($IncludeWordmark) { $pad } else { $plateX }
        $backY = if ($IncludeWordmark) { $pad } else { $plateY }
        $backW = if ($IncludeWordmark) { $Width - ($pad * 2) } else { $side }
        $backH = if ($IncludeWordmark) { $Height - ($pad * 2) } else { $side }

        $platePath = New-RoundedPath -X $backX -Y $backY -Width $backW -Height $backH `
            -Radius ([Math]::Min($backW, $backH) * 0.22)
        try {
            $plateRect = New-Object System.Drawing.RectangleF($backX, $backY, $backW, $backH)
            $plateBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
                $plateRect, $plateTop, $plateBottom, 90.0)
            try { $g.FillPath($plateBrush, $platePath) } finally { $plateBrush.Dispose() }
        }
        finally { $platePath.Dispose() }

        # Bars ascend left to right; heights are a fixed fraction of the plate so every size matches.
        $barWidth = $side * 0.155
        $gap = $side * 0.075
        $baseline = $plateY + ($side * 0.700)
        $heights = @(0.26, 0.40, 0.545)
        $firstX = $plateX + ($side * 0.215)
        for ($i = 0; $i -lt 3; $i++) {
            $barHeight = $side * $heights[$i]
            $x = $firstX + ($i * ($barWidth + $gap))
            $y = $baseline - $barHeight
            $barPath = New-RoundedPath -X $x -Y $y -Width $barWidth -Height $barHeight -Radius ($barWidth / 2)
            try {
                $brush = New-Object System.Drawing.SolidBrush($barColors[$i])
                try { $g.FillPath($brush, $barPath) } finally { $brush.Dispose() }
            }
            finally { $barPath.Dispose() }
        }

        $tapeHeight = $side * 0.070
        $tapeY = $plateY + ($side * 0.760)
        $tapeX = $plateX + ($side * 0.200)
        $tapeWidth = $side * 0.600
        $tapePath = New-RoundedPath -X $tapeX -Y $tapeY -Width $tapeWidth -Height $tapeHeight -Radius ($tapeHeight / 2)
        try {
            $brush = New-Object System.Drawing.SolidBrush($tapeColor)
            try { $g.FillPath($brush, $tapePath) } finally { $brush.Dispose() }
        }
        finally { $tapePath.Dispose() }

        if ($IncludeWordmark) {
            $textX = $plateX + $side + ($plateSize * 0.04)
            $textWidth = ($backX + $backW - ($plateSize * 0.06)) - $textX
            $format = New-Object System.Drawing.StringFormat
            $format.LineAlignment = [System.Drawing.StringAlignment]::Center
            $format.FormatFlags = [System.Drawing.StringFormatFlags]::NoWrap
            try {
                # Shrink to fit rather than trusting a fixed ratio, which clipped the last glyph.
                $fontSize = $plateSize * 0.17
                $font = $null
                while ($true) {
                    $candidate = New-Object System.Drawing.Font(
                        'Segoe UI', $fontSize, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
                    $measured = $g.MeasureString('SmartTicker', $candidate, [int]::MaxValue, $format)
                    if ($measured.Width -le $textWidth -or $fontSize -le ($plateSize * 0.08)) {
                        $font = $candidate
                        break
                    }

                    $candidate.Dispose()
                    $fontSize = $fontSize * 0.95
                }

                try {
                    $brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
                    try {
                        $textRect = New-Object System.Drawing.RectangleF(
                            [single]$textX, 0, [single]$textWidth, [single]$Height)
                        $g.DrawString('SmartTicker', $font, $brush, $textRect, $format)
                    }
                    finally { $brush.Dispose() }
                }
                finally { $font.Dispose() }
            }
            finally { $format.Dispose() }
        }
    }
    finally { $g.Dispose() }

    return $bitmap
}

function Save-Png {
    param(
        [Parameter(Mandatory)][System.Drawing.Bitmap]$Bitmap,
        [Parameter(Mandatory)][string]$Path
    )

    $Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
}

function Save-Ico {
    param(
        [Parameter(Mandatory)][int[]]$Sizes,
        [Parameter(Mandatory)][string]$Path
    )

    $payloads = foreach ($size in $Sizes) {
        $bitmap = New-IconBitmap -Width $size -Height $size
        try {
            $stream = New-Object System.IO.MemoryStream
            try {
                $bitmap.Save($stream, [System.Drawing.Imaging.ImageFormat]::Png)
                , $stream.ToArray()
            }
            finally { $stream.Dispose() }
        }
        finally { $bitmap.Dispose() }
    }

    $file = [System.IO.File]::Create($Path)
    try {
        $writer = New-Object System.IO.BinaryWriter($file)
        try {
            $writer.Write([uint16]0)
            $writer.Write([uint16]1)
            $writer.Write([uint16]$Sizes.Count)

            # Offsets follow the 6 byte header plus one 16 byte directory entry per image.
            $offset = 6 + (16 * $Sizes.Count)
            for ($i = 0; $i -lt $Sizes.Count; $i++) {
                $size = $Sizes[$i]
                $bytes = $payloads[$i]
                $writer.Write([byte]$(if ($size -ge 256) { 0 } else { $size }))
                $writer.Write([byte]$(if ($size -ge 256) { 0 } else { $size }))
                $writer.Write([byte]0)
                $writer.Write([byte]0)
                $writer.Write([uint16]1)
                $writer.Write([uint16]32)
                $writer.Write([uint32]$bytes.Length)
                $writer.Write([uint32]$offset)
                $offset += $bytes.Length
            }

            foreach ($bytes in $payloads) {
                $writer.Write($bytes)
            }
        }
        finally { $writer.Dispose() }
    }
    finally { $file.Dispose() }
}

New-Item -ItemType Directory -Path $AssetsDirectory -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path -Parent $IconPath) -Force | Out-Null

$pngTargets = @(
    @{ Name = 'Square44x44Logo.png'; Width = 44; Height = 44 },
    @{ Name = 'Square150x150Logo.png'; Width = 150; Height = 150 },
    @{ Name = 'StoreLogo.png'; Width = 50; Height = 50 },
    @{ Name = 'AppIcon256.png'; Width = 256; Height = 256 }
)

foreach ($target in $pngTargets) {
    $bitmap = New-IconBitmap -Width $target.Width -Height $target.Height
    try {
        $path = Join-Path $AssetsDirectory $target.Name
        Save-Png -Bitmap $bitmap -Path $path
        Write-Host "Wrote $path ($($target.Width)x$($target.Height))"
    }
    finally { $bitmap.Dispose() }
}

$wide = New-IconBitmap -Width 310 -Height 150 -IncludeWordmark
try {
    $widePath = Join-Path $AssetsDirectory 'Wide310x150Logo.png'
    Save-Png -Bitmap $wide -Path $widePath
    Write-Host "Wrote $widePath (310x150)"
}
finally { $wide.Dispose() }

Save-Ico -Sizes @(16, 24, 32, 48, 64, 128, 256) -Path $IconPath
Write-Host "Wrote $IconPath (16-256)"
