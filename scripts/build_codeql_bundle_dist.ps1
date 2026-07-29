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

# PyInstaller only freezes Python modules, so any package data files read at
# runtime (via importlib.resources) have to be added explicitly. The separator
# expected by --add-data is os.pathsep, which differs per platform, so it is
# resolved here rather than hardcoded. Older codeql-bundle releases do not ship
# these files, hence the Test-Path guard.
$PathSeparator = [System.IO.Path]::PathSeparator
$DataArgs = @()

foreach ($DataFile in @("supported-codeql-bundles.json", "supported-codeql-bundles.schema.json")) {
    if (Test-Path $DataFile) {
        $DataArgs += "--add-data"
        $DataArgs += "${DataFile}${PathSeparator}codeql_bundle"
    }
    else {
        Write-Host "Note: data file '$DataFile' not present in this release, skipping."
    }
}

# Build executable with pyinstaller
Write-Host "Running: pyinstaller -F -n codeql_bundle $($DataArgs -join ' ') cli.py"
pyinstaller -F -n codeql_bundle @DataArgs cli.py
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
