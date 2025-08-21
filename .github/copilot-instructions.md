# CodeQL Development Toolkit (QLT)

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Installation
- Install .NET 6.0 SDK for development and testing:
  ```bash
  wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
  chmod +x dotnet-install.sh
  ./dotnet-install.sh --channel 6.0
  export PATH="$HOME/.dotnet:$PATH"
  ```
- The project requires .NET 6.0 runtime for execution, but can be built with .NET 8.0.
- For production use, download pre-built releases from GitHub releases page (Linux x86_64 only).

### Bootstrap, Build, and Test
- **Restore dependencies**: `dotnet restore` -- takes 20 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- **Build the project**: `dotnet build -c Release --no-restore` -- takes 9 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- **Run tests**: `dotnet test -c Release --no-build` -- takes 3 seconds. Set timeout to 15+ seconds.
- **Note**: Tests require .NET 6.0 runtime. Build will complete with warnings (nullable reference types) but no errors.

### Run the Application
- **During development**: `dotnet run --project src/CodeQLToolkit.Core --configuration Release -- [command]`
- **From published binary**: `dotnet publish src/CodeQLToolkit.Core/CodeQLToolkit.Core.csproj -c Release -o publish-out` then use `./publish-out/CodeQLToolkit.Core [command]`
- **Self-contained build**: `dotnet publish src/CodeQLToolkit.Core/CodeQLToolkit.Core.csproj -c Release -o publish-standalone --self-contained -r linux-x64` -- takes 4 seconds. Creates standalone executable.

### Basic Application Commands
- **Get help**: `qlt --help` or add `--help` to any command
- **Get version**: `qlt version`
- **Initialize query development**: `qlt query init --automation-type actions`
- **Set CodeQL version**: `qlt codeql set version --automation-type actions` (creates `qlt.conf.json`)
- **Initialize testing**: `qlt test init --language [c|cpp|csharp|go|java|javascript|python|ruby] --automation-type actions`
  - Creates `.github/workflows/run-codeql-unit-tests-[language].yml`
  - Creates `.github/actions/install-qlt/action.yml`
  - Example: `qlt test init --language cpp --automation-type actions`

## Validation

### Always Run Before Committing
- Build the project: `dotnet build -c Release`
- Run tests: `dotnet test -c Release`
- The build produces 151 warnings (nullable reference types) but no errors - this is expected.

### Manual Validation Scenarios
After making changes, always test key functionality:
1. **Basic functionality**: Run `qlt version` and `qlt --help` to ensure the application starts correctly.
2. **Repository initialization**: 
   - Create a test directory: `mkdir /tmp/qlt-test && cd /tmp/qlt-test`
   - Run: `qlt query init --automation-type actions`
   - Verify `codeql-workspace.yml` is created
   - Run: `qlt codeql set version --automation-type actions`
   - Verify `qlt.conf.json` is created with expected structure
   - Run: `qlt test init --language cpp --automation-type actions`
   - Verify GitHub Actions workflows are created in `.github/workflows/` and `.github/actions/`
3. **Help system**: Test help for main commands: `qlt query --help`, `qlt test --help`, `qlt codeql --help`
4. **Commands requiring CodeQL**: Note that validation commands like `qlt validation run check-queries` require CodeQL to be installed and will fail with "CodeQL not installed" if not available. This is expected behavior.

### CI/CD Validation
- The project uses GitHub Actions for CI/CD with workflows in `.github/workflows/`
- Build artifacts are created as zip files for distribution
- No specific linting tools beyond standard .NET compiler warnings

## Common Tasks

### Repository Structure
QLT expects CodeQL repositories to follow this structure:
```
Repo Root
│   codeql-workspace.yml
│   qlt.conf.json
│
└───[language] (e.g., cpp, java, javascript)
    ├───[package-name]
    │   ├───src
    │   │   │   qlpack.yml
    │   │   └───[QueryName]
    │   │           QueryName.ql
    │   └───test
    │       │   qlpack.yml
    │       └───[QueryName]
    │               TestFile.cpp
    │               TestFile.expected
    │               TestFile.qlref
```

### Key Features
- **Query Management**: Create, scaffold, and manage CodeQL queries
- **Unit Testing**: Run and validate CodeQL unit tests in parallel
- **CI/CD Integration**: Generate GitHub Actions workflows for automated testing
- **Validation**: Check query metadata and structure
- **Pack Management**: Manage CodeQL packs and dependencies
- **Bundle Creation**: Create custom CodeQL bundles for distribution

### Example Workflows
- **Create new query repository**: `qlt query init && qlt codeql set version`
- **Setup CI/CD for C++ queries**: `qlt test init --language cpp --automation-type actions`
- **Run unit tests locally**: `qlt test run execute-unit-tests --num-threads 4 --language cpp --runner-os "Linux" --work-dir /tmp/test-results`
- **Validate queries**: `qlt validation run check-queries --language cpp --pretty-print`

## Development Tips
- The application is organized into **Features** with lifecycle and command components
- Source code is in `src/` with three main projects: Core, Features, and Shared
- Templates for code generation are in the `Templates/` directory
- The `example/` directory contains sample CodeQL repository structures
- Build produces warnings about nullable reference types - this is expected and should not be "fixed"
- Use `--base` parameter to specify working directory (defaults to current directory)
- Always use `--automation-type actions` for GitHub Actions integration

## Important Files
- `src/CodeQLToolkit.Core/` - Main executable project
- `src/CodeQLToolkit.Features/` - Core functionality and commands
- `src/CodeQLToolkit.Shared/` - Shared utilities and base classes
- `CodeQLToolkit.sln` - Visual Studio solution file
- `.github/workflows/` - CI/CD pipeline definitions
- `scripts/` - Build and release automation scripts
- `example/` - Sample repository structures and configurations