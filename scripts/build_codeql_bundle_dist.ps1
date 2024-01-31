param(
    [Parameter(Mandatory=$true)] 
    [string]
    $Version,
    [Parameter(Mandatory=$true)] 
    [string]
    $OutputDirectory       
)


# create output directory if it doesn't exist 
if (-not (Test-Path $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
}

# download a copy of the release from GitHub
gh release download "v$Version" --repo https://github.com/rvermeulen/codeql-bundle  -D $OutputDirectory -A zip

# extract the zip file
Expand-Archive -Path "$OutputDirectory\codeql-bundle-$Version.zip" -DestinationPath $OutputDirectory

# creates a directory named `codeql-bundle-<version>`
$ArchiveDirectory = Join-Path $OutputDirectory "codeql-bundle-$Version"

Push-Location $ArchiveDirectory

# at this point python should already be installed as well as poetry
# export the requirements 
poetry export -f requirements.txt > requirements.txt

# install the requirements
pip install -r requirements.txt

Push-Location "codeql_bundle"

# pyinstaller should also be installed  
pyinstaller -F -n codeql_bundle cli.py


Pop-Location 
Pop-Location 

$OutputFile = Join-Path $ArchiveDirectory "codeql_bundle" "dist" "codeql_bundle.exe"

# this will output the binary in the `dist` directory - we should copy that binary the toplevel directory.
Copy-Item -Path $OutputFile -Destination $OutputDirectory


