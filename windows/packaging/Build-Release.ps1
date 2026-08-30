<#
.SYNOPSIS
    Builds SmartTicker release artifacts under releases/windows/<version>/.

.DESCRIPTION
    Publishes self-contained builds, produces portable ZIP archives, stages a Microsoft Store
    compatible MSIX layout, builds an MSI installer, and writes SHA-256 checksums. MSIX packing
    requires makeappx.exe from the Windows SDK and the MSI requires the WiX CLI
    (dotnet tool install --global wix); when either is unavailable the rest of the artifacts are
    still produced.

.EXAMPLE
    ./Build-Release.ps1 -Version 1.0.0
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version,

    [ValidateSet('win-x64', 'win-arm64')]
    [string[]]$Runtime = @('win-x64', 'win-arm64'),

    [string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$packagingRoot = $PSScriptRoot
$windowsRoot = Split-Path -Parent $packagingRoot
$repositoryRoot = Split-Path -Parent $windowsRoot
$project = Join-Path $windowsRoot 'src/SmartTicker.Desktop/SmartTicker.Desktop.csproj'
$releaseRoot = Join-Path $repositoryRoot "releases/windows/$Version"
$assetsDirectory = Join-Path $packagingRoot 'Assets'

$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue |
    Select-Object -First 1 -ExpandProperty Source)
if (-not $dotnet) {
    throw 'dotnet was not found on PATH. Install the .NET 10 SDK before building a release.'
}

function New-PlaceholderLogo {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][int]$Width,
        [Parameter(Mandatory)][int]$Height
    )

    Add-Type -AssemblyName System.Drawing
    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
    try {
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        try {
            $graphics.Clear([System.Drawing.ColorTranslator]::FromHtml('#10151D'))
            $accent = New-Object System.Drawing.SolidBrush(
                [System.Drawing.ColorTranslator]::FromHtml('#70E1A1'))
            try {
                $barHeight = [Math]::Max(2, [int]($Height / 8))
                $graphics.FillRectangle($accent, 0, [int](($Height - $barHeight) / 2), $Width, $barHeight)
            }
            finally {
                $accent.Dispose()
            }
        }
        finally {
            $graphics.Dispose()
        }

        $bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        $bitmap.Dispose()
    }
}

function Initialize-PackagingAssets {
    $required = @(
        @{ Name = 'StoreLogo.png'; Width = 50; Height = 50 },
        @{ Name = 'Square44x44Logo.png'; Width = 44; Height = 44 },
        @{ Name = 'Square150x150Logo.png'; Width = 150; Height = 150 },
        @{ Name = 'Wide310x150Logo.png'; Width = 310; Height = 150 }
    )

    if (-not (Test-Path $assetsDirectory)) {
        New-Item -ItemType Directory -Path $assetsDirectory -Force | Out-Null
    }

    foreach ($asset in $required) {
        $assetPath = Join-Path $assetsDirectory $asset.Name
        if (-not (Test-Path $assetPath)) {
            Write-Host "Generating placeholder asset $($asset.Name)."
            New-PlaceholderLogo -Path $assetPath -Width $asset.Width -Height $asset.Height
        }
    }
}

$portableDirectory = Join-Path $releaseRoot 'portable'
$msixDirectory = Join-Path $releaseRoot 'msix'
$msiDirectory = Join-Path $releaseRoot 'msi'
$checksumDirectory = Join-Path $releaseRoot 'checksums'
foreach ($directory in @($portableDirectory, $msixDirectory, $msiDirectory, $checksumDirectory)) {
    if ($PSCmdlet.ShouldProcess($directory, 'Create release directory')) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }
}

Initialize-PackagingAssets

$artifacts = [System.Collections.Generic.List[string]]::new()

foreach ($identifier in $Runtime) {
    Write-Host "=== Publishing ${identifier} ==="
    $publishDirectory = Join-Path $releaseRoot "publish/$identifier"

    if ($PSCmdlet.ShouldProcess($identifier, 'dotnet publish')) {
        & $dotnet publish $project `
            --configuration $Configuration `
            --runtime $identifier `
            --self-contained true `
            -p:Version=$Version `
            --output $publishDirectory
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet publish failed for ${identifier} with exit code ${LASTEXITCODE}."
        }
    }

    $archivePath = Join-Path $portableDirectory "SmartTicker-$Version-$identifier.zip"
    if ($PSCmdlet.ShouldProcess($archivePath, 'Create portable archive')) {
        Compress-Archive -Path (Join-Path $publishDirectory '*') -DestinationPath $archivePath -Force
        $artifacts.Add($archivePath)
    }

    $layout = Join-Path $msixDirectory "layout/$identifier"
    if ($PSCmdlet.ShouldProcess($layout, 'Stage MSIX layout')) {
        New-Item -ItemType Directory -Path $layout -Force | Out-Null
        Copy-Item -Path (Join-Path $publishDirectory '*') -Destination $layout -Recurse -Force
        Copy-Item -Path $assetsDirectory -Destination (Join-Path $layout 'Assets') -Recurse -Force

        $architecture = if ($identifier -eq 'win-x64') { 'x64' } else { 'arm64' }
        $manifestPath = Join-Path $layout 'AppxManifest.xml'
        $manifest = Get-Content -Path (Join-Path $packagingRoot 'Package.appxmanifest') -Raw
        $manifest = $manifest.Replace('Version="1.0.0.0"', "Version=""$Version.0""")
        $manifest = $manifest.Replace('ProcessorArchitecture="x64"', "ProcessorArchitecture=""$architecture""")
        Set-Content -Path $manifestPath -Value $manifest -Encoding UTF8
    }
}

function Find-MakeAppx {
    $sdkRoot = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
    $installed = Get-ChildItem -Path $sdkRoot -Filter 'makeappx.exe' -Recurse -ErrorAction SilentlyContinue |
        Sort-Object FullName -Descending |
        Select-Object -First 1 -ExpandProperty FullName
    if ($installed) {
        return $installed
    }

    # The SDK build tools package carries makeappx, so a full Windows SDK install is not required.
    $packages = if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path $env:USERPROFILE '.nuget/packages' }
    Get-ChildItem -Path (Join-Path $packages 'microsoft.windows.sdk.buildtools') -Filter 'makeappx.exe' -Recurse -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -like '*\x64\*' } |
        Sort-Object FullName -Descending |
        Select-Object -First 1 -ExpandProperty FullName
}

$makeAppx = Find-MakeAppx

if ($makeAppx) {
    Write-Host "Packing with $makeAppx"
    foreach ($identifier in $Runtime) {
        $layout = Join-Path $msixDirectory "layout/$identifier"
        $packagePath = Join-Path $msixDirectory "SmartTicker-$Version-$identifier.msix"
        if ($PSCmdlet.ShouldProcess($packagePath, 'Pack MSIX')) {
            & $makeAppx pack /d $layout /p $packagePath /o
            if ($LASTEXITCODE -ne 0) {
                throw "makeappx failed for ${identifier} with exit code ${LASTEXITCODE}."
            }

            $artifacts.Add($packagePath)
        }
    }
}
else {
    Write-Warning 'makeappx.exe was not found in the Windows SDK or the SDK build tools package.'
    Write-Warning 'The MSIX layout was staged but not packed. Install the Windows SDK, or restore'
    Write-Warning 'Microsoft.Windows.SDK.BuildTools, then run: makeappx pack /d <layout> /p <output.msix>'
}

function Find-Wix {
    $onPath = Get-Command wix -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty Source
    if ($onPath) {
        return $onPath
    }

    # A freshly installed global tool is not on PATH until the shell restarts.
    $toolPath = Join-Path $env:USERPROFILE '.dotnet/tools/wix.exe'
    if (Test-Path $toolPath) {
        return $toolPath
    }

    return $null
}

$wix = Find-Wix

if ($wix) {
    Write-Host "Building MSI with $wix"
    $wxs = Join-Path $packagingRoot 'SmartTicker.wxs'
    foreach ($identifier in $Runtime) {
        $publishDirectory = Join-Path $releaseRoot "publish/$identifier"
        $msiPath = Join-Path $msiDirectory "SmartTicker-$Version-$identifier.msi"
        $architecture = if ($identifier -eq 'win-x64') { 'x64' } else { 'arm64' }
        if ($PSCmdlet.ShouldProcess($msiPath, 'Build MSI')) {
            & $wix build $wxs -arch $architecture -o $msiPath `
                -d "Version=$Version" -d "PublishDir=$publishDirectory"
            if ($LASTEXITCODE -ne 0) {
                throw "wix build failed for ${identifier} with exit code ${LASTEXITCODE}."
            }

            $artifacts.Add($msiPath)
        }
    }
}
else {
    Write-Warning 'The WiX CLI was not found, so no MSI was built.'
    Write-Warning 'Install it with: dotnet tool install --global wix'
}

if ($artifacts.Count -gt 0 -and $PSCmdlet.ShouldProcess('SHA256SUMS.txt', 'Write checksums')) {
    $lines = foreach ($artifact in $artifacts) {
        $hash = (Get-FileHash -Path $artifact -Algorithm SHA256).Hash
        $name = Split-Path -Leaf $artifact
        "$hash  $name"
    }

    Set-Content -Path (Join-Path $checksumDirectory 'SHA256SUMS.txt') -Value $lines -Encoding UTF8
}

Write-Host ''
Write-Host "Release artifacts for ${Version} are in ${releaseRoot}."
Write-Host 'Store submissions require the reserved Identity values from Partner Center.'
