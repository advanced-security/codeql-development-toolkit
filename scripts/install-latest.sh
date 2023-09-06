#!/bin/env bash

# create installation root directory 
if [[ -d /opt/ ]] ; then
    echo -e "\e[0;32m[QLT]\e[0m Skipping creation of installation directory..."
else
    echo -e "\e[0;32m[QLT]\e[0m Creating /opt" directory
    mkdir -p /opt/ 
fi

# download the actual bundle 
gh release download -R advanced-security/codeql-development-toolkit --pattern 'qlt-linux-x86_64.zip' -D /opt/codeql-development-lifecycle-toolkit-downloads

# unzip the bundle
unzip /opt/codeql-development-lifecycle-toolkit-downloads/qlt-linux-x86_64.zip -d /opt/qlt/

# ensure qlt is executable 
chmod +x /opt/qlt/qlt
chmod +r -R /opt/qlt/*
# print the version that was installed 
echo -e "\e[0;32m[QLT]\e[0m Installed QLT Version " `cat /opt/qlt/ver.txt`