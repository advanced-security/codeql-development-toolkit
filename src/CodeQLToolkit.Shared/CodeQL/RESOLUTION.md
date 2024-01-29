# How QLT Resolves and Uses Standard CodeQL installations and Custom bundles.

The layout of the installation repository is as follows:

```
.qlt/repo/packages/ident        <-- normal CodeQL installs
.qlt/repo/custom-bundle/ident   <-- custom bundles
.qlt/repo/bundle/ident          <-- precompiled CodeQL bundles which include the cli and library. 
```

The general intention is that the tooling is used as follows:

```
qlt codeql install 
```

If the `--use-quick-bundles` flag is specified, QLT will construct a custom bundle in the repository and then set the `QLT_CODEQL_PATH`
variable. This enables one to use this both in automation AND within
a local developer environment. 

Note that `QLT_CODEQL_PATH` being set overrides all resolution attempted by QLT.

If `QLT_CODEQL_PATH` is *not* set, then QLT will attempt to resolve the required version of CodeQL by looking for the 
`qlt.conf.json` variable `CodeQLStandardLibraryIdent`, which is automatically set by QLT when the version of CodeQL is set.

Note that for custom bundles, the variable `EnableCustomCodeQLBundles` controls if custom bundles will be used. In general, 
when using QLT a previously installed version of CodeQL may easily be detected in the repository by looking for the ident. 
However, since there is no ident for a custom bundle, doing a `qlt codeql install` is always necessary prior to invoking the other
functions of QLT.

For cases where one simply wishes to use CodeQL without a custom bundle (MAD extensions, queries, etc) then QLT will attempt to resolve 
the correct version of CodeQL by using the `CodeQLStandardLibraryIdent`. If this cannot be resolved, QLT will notify the user that 
one must first run `qlt codeql install` prior to proceeding. 
