param(
    [Parameter(Mandatory = $true)] 
    [string] $Version,

    [Parameter(Mandatory = $true)] 
    [string] $WorkDirectory,      

    [Parameter(Mandatory = $true)] 
    [string] $DestinationDirectory       
)

# Fail on any built-in command failure
$ErrorActionPreference = "Stop"

if (-not (Test-Path $WorkDirectory)) {
    New-Item -ItemType Directory -Path $WorkDirectory | Out-Null
}

if (-not (Test-Path $DestinationDirectory)) {
    New-Item -ItemType Directory -Path $DestinationDirectory | Out-Null
}

# Download a copy of the release from GitHub
gh release download "v$Version" --repo https://github.com/advanced-security/codeql-bundle -D $WorkDirectory -A zip
if ($LASTEXITCODE -ne 0) {
    throw "Failed to download release from GitHub (gh)"
}

# Extract the zip file
Expand-Archive -Path "$WorkDirectory\codeql-bundle-$Version.zip" -DestinationPath $WorkDirectory

# Create path to archive directory (named codeql-bundle-<version>)
$ArchiveDirectory = Join-Path $WorkDirectory "codeql-bundle-$Version"

Push-Location $ArchiveDirectory

# Export the requirements using poetry
poetry self add poetry-plugin-export
if ($LASTEXITCODE -ne 0) {
    throw "Failed to add poetry-plugin-export"
}

poetry export -f requirements.txt --output requirements.txt
if ($LASTEXITCODE -ne 0) {
    throw "Failed to export requirements using poetry"
}

# Install the requirements using pip
pip install -r requirements.txt
if ($LASTEXITCODE -ne 0) {
    throw "Failed to install requirements using pip"
}

# Move into the cli directory
Push-Location "codeql_bundle"

# Build executable with pyinstaller
pyinstaller -F -n codeql_bundle cli.py
if ($LASTEXITCODE -ne 0) {
    throw "PyInstaller build failed"
}

Pop-Location
Pop-Location

# Determine built output binary path
if ($IsWindows) {
    $OutputFile = Join-Path $ArchiveDirectory "codeql_bundle" "dist" "codeql_bundle.exe"
}
else {
    $OutputFile = Join-Path $ArchiveDirectory "codeql_bundle" "dist" "codeql_bundle"
}

# Copy the binary to the destination directory
Copy-Item -Path $OutputFile -Destination $DestinationDirectory
