<div align="center">
<img src="assets/qlt-logo2.png">
</div>

# The CodeQL Development Toolkit (QLT)

*Please Note: This project is an Open Source Tool maintained by the Expert Services Team. It is not an officially supported product of Github, Inc and is provided 
"as is", without warranty of any kind, express or implied, including but not limited to
the warranties of merchantability, fitness ofr a particular purpose and noninfringement.*

The CodeQL Development toolkit is a tool for making common CodeQL development workflows easier. Some of its key features include:

- Creation and management of new CodeQL queries and query packs.
- Creation and management of new CodeQL unit tests and unit test packs. 
- Integration with common automation systems such as GitHub Actions which adds support for:
    - Automated query testing 
    - The ability to test your queries on sets of example databases
    - The ability to do performance testing
    - The ability to do regression testing (false positives / false negatives)

Below is a guide showing you how to perform the most common tasks with the CodeQL Development Toolkit

# Installation

You can install QLT by grabbing a release on the releases page. We currently ship builds for Linux x86_64. There are no dependencies to install. Just unpack the bundle and run the binary `qlt` in the unpacked directory. 

# General Command Structure 

QLT is organized groups of functionality called "Features." Within those features, the functionality of that feature is split into 4 verbs that describe the functionality provided:


Which In the following sections you will find documentation of those features as well as examples of their most common usages. 

## Usage 

```
Description: QLT: The CodeQL Development Toolkit

Usage:
  CodeQLToolkit.Core [command] [options]

Options:
  --base <base>                           The base path to find the query repository. [default: C:\Projects\codeql-development-lifecycle-toolkit]
  --automation-type <actions> (REQUIRED)  The base path to find the query repository. [default: actions]
  --version                               Show version information
  -?, -h, --help                          Show help and usage information

Commands:
  version     Get the current tool version.
  query       Use the features related to query creation and execution.
  codeql      Use the features related to managing the version of CodeQL used by this repository.
  test        Features related to the running and processing of CodeQL Unit Tests.
  pack        Features CodeQL pack management and publication.
  validation  Features related to the validation of CodeQL Development Repositories.
```

# Cookbook 

**Validate the metadata in your queries**

```
qlt validation run check-queries --pretty-print --language cpp
```


**Run your unit tests in parallel**

```
qlt test run execute-unit-tests  --num-threads 4 --language cpp --runner-os "Linux" --work-dir /tmp/my-project
```

**Validate unit test run data**
```
qlt test run validate-unit-tests --pretty-print  --results-directory /tmp/my-project
```

# Contributing

We welcome your contribution! Please see our [guidelines for contributing](CONTRIBUTING.md) for more information.

# License 

This project is release under the MIT OSS License. Please see our [LICENSE](LICENSE) for more information. 
