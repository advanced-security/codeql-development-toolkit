param(
    [Parameter(Mandatory = $true)] 
    [string]
    $Version,
    [Parameter(Mandatory = $true)] 
    [string]
    $WorkDirectory,      

    [Parameter(Mandatory = $true)] 
    [string]
    $DestinationDirectory       
)

if (-not (Test-Path $WorkDirectory)) {
    New-Item -ItemType Directory -Path $WorkDirectory | Out-Null
}

if (-not (Test-Path $DestinationDirectory)) {
    New-Item -ItemType Directory -Path $DestinationDirectory | Out-Null
}

# download a copy of the release from GitHub
gh release download "v$Version" --repo https://github.com/advanced-security/codeql-bundle  -D $WorkDirectory -A zip

# extract the zip file
Expand-Archive -Path "$WorkDirectory\codeql-bundle-$Version.zip" -DestinationPath $WorkDirectory

# creates a directory named `codeql-bundle-<version>`
$ArchiveDirectory = Join-Path $WorkDirectory "codeql-bundle-$Version"

Push-Location $ArchiveDirectory

# at this point python should already be installed as well as poetry
# export the requirements 
poetry self add poetry-plugin-export
poetry export -f requirements.txt > requirements.txt

# install the requirements
pip install -r requirements.txt

Push-Location "codeql_bundle"

# pyinstaller should also be installed  
pyinstaller -F -n codeql_bundle cli.py

Pop-Location 
Pop-Location 

if ($IsWindows) {
    $OutputFile = Join-Path $ArchiveDirectory "codeql_bundle" "dist" "codeql_bundle.exe"
}
else {
    $OutputFile = Join-Path $ArchiveDirectory "codeql_bundle" "dist" "codeql_bundle"
}


# this will output the binary in the `dist` directory - we should copy that binary the toplevel directory.
Copy-Item -Path $OutputFile -Destination $DestinationDirectory


