# How Resolution Works 

## How QLT Resolves and Uses Standard CodeQL installations and Custom bundles.

The layout of the installation repository is as follows:

```
.qlt/repo/packages/ident/codeql        <-- normal CodeQL installs
.qlt/repo/custom-bundle/ident/codeql   <-- custom bundles
```

The general intention is that the tooling is used as follows:

```
qlt codeql install 
```

Will install the distribution version of CodeQL and:

```
qlt codeql install --custom-bundle [--packs pack1 pack2 pack3]
```

This will build and install a custom bundle from the current project using the comma seperated 
list of packs as the packs to include in the bundle. 

Note this will compile all of the queries and may take a long time. In this case, you may create a bundle as follows:

```
qlt codeql install --quick-bundle [--packs pack1 pack2 pack3]
```

Which will not compile the packs and queries. 

In addition to specifying one of the bundle flags, you must set `ExportedCustomizationPacks` in `qlt.conf.json`. 
If the `--pack` option is not specified on the command line, this is the value that is used for selecting which packs
to include in the bundle. 

Note that the versions of what gets installed is taken from `qlt.conf.json`. 

For a normal installation, the mapping of these values is as follows:

- `CodeQLCLI` - The version of the CLI binaries to use
- `CodeQLStandardLibrary` - the version of the standard library to use. 

For a bundle installation the mapping is as follows:

- `CodeQLCLIBundle` - The `github/codeql-action` bundle release tag to base the bundle on. QLT downloads the platform-specific source bundle for the current host.

In all cases, two environment variables are set after a run:
- `QLT_CODEQL_PATH` - The path to the CodeQL binary. (Always set)
- `QLT_CODEQL_HOME` - The root installation of CodeQL. (Always set)

When using custom bundles, one additional environment variable is set after a run:
- `QLT_CODEQL_BUNDLE_PATH` - The path to the current platform bundle created by QLT.

Each invocation creates a bundle only for its current platform. Run QLT on each target operating system when Linux, Windows, and macOS bundles are all required.

## Idents within the Installation Directory 

The layout of the installation repository is as follows:

```
.qlt/repo/packages/ident/codeql        <-- normal CodeQL installs
.qlt/repo/custom-bundle/ident/codeql   <-- custom bundles
```

For the case of package installations, the ident is generated as follows:

```C#
	var ident =  String.Join("", "codeql-cli-" + CLIVersion, "#standard-library-ident-" ,StandardLibraryIdent);
	return StringUtils.CreateMD5(FileUtils.SanitizeFilename(ident)).ToLower();
```

Otherwise the ident is generated as follows:

```C#
    var ident = String.Join("", "codeql-bundle-" + CLIBundle);
    return StringUtils.CreateMD5(FileUtils.SanitizeFilename(ident)).ToLower();
```