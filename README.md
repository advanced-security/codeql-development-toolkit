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
# How to Use QLT

QLT is designed to be used mainly in the context of an automation environment, such as actions. Locally, you can use the tool to scaffold common workflows, for example, running unit tests or validating query metadata, and within the actions environment, the workflows that get generated will in turn rely on QLT to perform specialized work related to those tasks. 

QLT is divided in to **features**. A typical workflow in QLT is that you first `init` a feature, which will install necessary metadata into your repository that is later used to invoke or support the operation of QLT. 

For example, to initialize unit testing in your repository you can run: 

```
qlt test init  --use-runner ubuntu-latest --num-threads 4 --language c --automation-type actions
```

Which installs automation for running unit tests for C language queries. Please see the following sections for more information about QLT and its operation as well as see common commands you can use to help manage your CodeQL development. 

## Assumptions About Repository Layout

In order to promote consistency and best practices, QLT takes an opinionated approach to repository layout. These recommendations are based on the experience of our team in deploying CodeQL in a diverse set of environments and we feel offer the best balance of flexibility and functionality. 

Firstly, it is assumed each repository has a `codeql-workspace.yml` file in it in the root. This file can be created with the following command:

```
qlt query init
```

Next, it is assumed that each repository has a `qlt.conf.json` file in it, which contains information pertaining to the CodeQL version used by the repository. This file can be created with the command:

```
qlt codeql set version
```

Note this uses the default CodeQL version -- you can customize these values on the command line using the `--help` flag and reviewing the available options. Additionally, you may edit the `qlt.conf.json` file directly, though this is not recommended. 

Next, at the top level it is assumed each language resides in it's own directory. For example, `java` queries should be in a `java` subdirectory. And `c` queries should be in a `c` subdirectory. 

Within each language, queries should be structured so that each query resides in its own directory and has a matching directory in a test directory. The `src` and `test` directories should both be in their own CodeQL packs. 

The illustration below details the suggested structure for a repository with two different query packs, each containing a single query. 

```
Repo Root
│   codeql-workspace.yml
│   qlt.conf.json
│
└───cpp
    ├───package1
    │   ├───src
    │   │   │   qlpack.yml
    │   │   │
    │   │   └───TestQuery
    │   │           TestQuery.ql
    │   │
    │   └───test
    │       │   qlpack.yml
    │       │
    │       └───TestQuery
    │               TestQuery.cpp
    │               TestQuery.expected
    │               TestQuery.qlref
    │
    └───package2
        ├───src
        │   │   qlpack.yml
        │   │
        │   └───TestQuery
        │           TestQuery.ql
        │
        └───test
            │   qlpack.yml
            │
            └───TestQuery
                    TestQuery.cpp
                    TestQuery.expected
                    TestQuery.qlref
```

## Common Tasks / Cookbook 

**Initialize repo for query development**

```
Usage:
  qlt query init [options]

Options:
  --overwrite-existing                    Overwrite exiting files (if they exist). [default: False]
  --base <base>                           The base path to find the query repository. [default: C:\temp\p1]
  --automation-type <actions> (REQUIRED)  The base path to find the query repository. [default: actions]
  -?, -h, --help                          Show help and usage information
```

**Set the Version of CodeQL Used**

```
Usage:
  qlt codeql set version [options]

Options:
  --cli-version <cli-version> (REQUIRED)                  The version of the cli to use. Example: `2.11.6`. [default:
                                                          2.11.6]
  --standard-library-version <standard-library-version>   The version of the standard library to use. Example:
  (REQUIRED)                                              `codeql-cli/v2.11.6`. [default: codeql-cli/v2.11.6]
  --bundle-version <bundle-version> (REQUIRED)            The bundle version to use. Example: `codeql-bundle-20221211`.
                                                          [default: codeql-bundle-20221211]
  --base <base>                                           The base path to find the query repository. [default:
                                                          C:\temp\p1]
  --automation-type <actions> (REQUIRED)                  The base path to find the query repository. [default: actions]
  -?, -h, --help                                          Show help and usage information
```


**Initialize CodeQL CI/CD and Unit Testing For Actions**

This command will install a number of workflows into your repository which include the necessary workflows for using QLT in your automation environment as well as the workflows for running CodeQL unit tests for the specified language. 

```
Usage:
  qlt test init [options]

Options:
  --overwrite-existing                                       Overwrite exiting files (if they exist). [default: False]
  --num-threads <num-threads>                                Number of threads to use during test execution. [default:
                                                             4]
  --use-runner <use-runner>                                  The runner(s) to use. Should be a comma-seperated list of
                                                             actions runners. [default: ubuntu-latest]
  --language <c|cpp|csharp|go|java|javascript|python|ruby>   The language to generate automation for.
  (REQUIRED)
  --codeql-args <codeql-args>                                Extra arguments to pass to CodeQL.
  --base <base>                                              The base path to find the query repository. [default:
                                                             C:\temp\p1]
  --automation-type <actions> (REQUIRED)                     The base path to find the query repository. [default:
                                                             actions]
  -?, -h, --help                                             Show help and usage information
```



Example usage: 
```
qlt test init  --use-runner ubuntu-latest --num-threads 4 --language c --automation-type actions
```


**Validate the metadata in your queries**

```
Usage:
  qlt validation run check-queries [options]

Options:
  --language <c|cpp|csharp|go|java|javascript|python|ruby>   The language to run tests for.
  (REQUIRED)
  --pretty-print (REQUIRED)                                  Pretty prints error output in a pretty compact format.
                                                             [default: False]
  --base <base>                                              The base path to find the query repository. [default:
                                                             C:\temp\p1]
  --automation-type <actions> (REQUIRED)                     The base path to find the query repository. [default:
                                                             actions]
  -?, -h, --help                                             Show help and usage information
```


Example Usage:
```
qlt validation run check-queries --pretty-print --language cpp
```


**Run your unit tests in parallel**

```
Usage:
  qlt test run execute-unit-tests [options]

Options:
  --num-threads <num-threads> (REQUIRED)                     The number of threads to use for runner. For best
                                                             performance, do not exceed the number of physical cores on
                                                             your system. [default: 4]
  --work-dir <work-dir> (REQUIRED)                           Where to place intermediate execution output files.
                                                             [default: C:\Users\jsingleton\AppData\Local\Temp\]
  --language <c|cpp|csharp|go|java|javascript|python|ruby>   The language to run tests for.
  (REQUIRED)
  --runner-os <runner-os> (REQUIRED)                         Label for the operating system running these tests.
  --codeql-args <codeql-args>                                Extra arguments to pass to CodeQL.
  --base <base>                                              The base path to find the query repository. [default:
                                                             C:\temp\p1]
  --automation-type <actions> (REQUIRED)                     The base path to find the query repository. [default:
                                                             actions]
  -?, -h, --help                                             Show help and usage information
```


Example Usage:
```
qlt test run execute-unit-tests  --num-threads 4 --language cpp --runner-os "Linux" --work-dir /tmp/my-project
```

**Validate unit test run data**

```
Usage:
  qlt test run validate-unit-tests [options]

Options:
  --results-directory <results-directory> (REQUIRED)  Where to find the intermediate execution output files.
  --pretty-print (REQUIRED)                           Pretty print test output in a compact format. Note this will not
                                                      exit with a failure code if tests fail. [default: False]
  --base <base>                                       The base path to find the query repository. [default: C:\temp\p1]
  --automation-type <actions> (REQUIRED)              The base path to find the query repository. [default: actions]
  -?, -h, --help                                      Show help and usage information
```


Example Usage:
```
qlt test run validate-unit-tests --pretty-print  --results-directory /tmp/my-project
```

# Contributing

We welcome your contribution! Please see our [guidelines for contributing](CONTRIBUTING.md) for more information.

# License 

This project is release under the MIT OSS License. Please see our [LICENSE](LICENSE) for more information. 
