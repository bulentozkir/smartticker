<#
.SYNOPSIS
    Publishes SmartTicker for linux-x64 and packs it into a .deb using WSL.

.DESCRIPTION
    The publish runs on Windows; dpkg-deb runs inside the WSL Debian distribution.
    Requires a WSL distribution with dpkg-deb available (Debian and Ubuntu both have it).

.EXAMPLE
    ./Build-Deb.ps1 -Version 1.0.3
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version,

    [string]$Distribution = 'Debian',

    [string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$linuxRoot = $PSScriptRoot
$repositoryRoot = Split-Path -Parent $linuxRoot
$project = Join-Path $repositoryRoot 'windows/src/SmartTicker.Desktop/SmartTicker.Desktop.csproj'
$releaseRoot = Join-Path $repositoryRoot "releases/linux/$Version"
$publishDirectory = Join-Path $releaseRoot 'publish/linux-x64'
$icon = Join-Path $repositoryRoot 'windows/packaging/Assets/AppIcon256.png'

$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue |
    Select-Object -First 1 -ExpandProperty Source)
if (-not $dotnet) {
    throw 'dotnet was not found on PATH. Install the .NET 10 SDK before building the .deb.'
}

Write-Host '=== Publishing linux-x64 ==='
& $dotnet publish $project `
    --configuration $Configuration `
    --framework net10.0 `
    --runtime linux-x64 `
    --self-contained true `
    -p:Version=$Version `
    --output $publishDirectory
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE."
}

function ConvertTo-WslPath {
    param([Parameter(Mandatory)][string]$Path)

    $full = [System.IO.Path]::GetFullPath($Path)
    $drive = $full.Substring(0, 1).ToLowerInvariant()
    $rest = $full.Substring(2).Replace('\', '/')
    return "/mnt/$drive$rest"
}

$script = Join-Path $linuxRoot 'build-deb.sh'
$debOutput = Join-Path $releaseRoot 'deb'

$wslScript = ConvertTo-WslPath $script
$wslPublish = ConvertTo-WslPath $publishDirectory
$wslOutput = ConvertTo-WslPath $debOutput
$wslIcon = if (Test-Path $icon) { ConvertTo-WslPath $icon } else { '' }

Write-Host "=== Packing .deb in WSL ($Distribution) ==="
# The script is edited on Windows, so CRLF is stripped before bash reads it.
$command = "sed 's/\r$//' '$wslScript' > /tmp/st-build-deb.sh && bash /tmp/st-build-deb.sh '$Version' '$wslPublish' '$wslOutput' '$wslIcon'"
& wsl -d $Distribution -- bash -lc $command
if ($LASTEXITCODE -ne 0) {
    throw "The .deb build failed with exit code $LASTEXITCODE."
}

$package = Join-Path $debOutput "smartticker_${Version}_amd64.deb"
if (Test-Path $package) {
    $checksumDirectory = Join-Path $releaseRoot 'checksums'
    New-Item -ItemType Directory -Path $checksumDirectory -Force | Out-Null
    $hash = (Get-FileHash -Path $package -Algorithm SHA256).Hash
    Set-Content -Path (Join-Path $checksumDirectory 'SHA256SUMS.txt') `
        -Value "$hash  $(Split-Path -Leaf $package)" -Encoding UTF8
}

Write-Host ''
Write-Host "Linux artifacts for $Version are in $releaseRoot."
