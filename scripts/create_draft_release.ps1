param(
    [Parameter(Mandatory=$false)] 
    [string]
    $Version,
    [Parameter(Mandatory=$false)] 
    [string]
    $Branch = "master",
    [Parameter(Mandatory=$true)] 
    [string]
    $ArchiveDir       
)

$VERSION_FILE = Join-Path "src" "CodeQLToolkit.Core" "ver.txt"

if (-not $Version) {
    Write-Host "Detecting version from version file..."
    $Version = Get-Content $VERSION_FILE
}

Write-Host "Creating draft release for version $Version on branch $Branch..."

# create checksums 
Get-FileHash -Algorithm SHA256 $ArchiveDir/* | ForEach-Object { (Get-Item $_.Path).Name + " (SHA256): " + $_.Hash } | Out-File -Path $ArchiveDir/checksums.txt

gh release create "v$Version" -d --target "$Branch" -t "v$Version" $ArchiveDir/*

