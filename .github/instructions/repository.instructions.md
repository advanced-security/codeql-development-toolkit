---
applyTo: "**/*"
description: "High-level project context, repository structure, and development workflow guidance"
---
# CodeQL Development Toolkit Repository Instructions

## Project Overview

The CodeQL Development Toolkit (QLT) is a command-line tool for CodeQL query development, testing, and automation. It provides scaffolding, lifecycle management, and CI/CD integration for CodeQL projects.

## Repository Structure

- `src/CodeQLToolkit.Core/` - Main CLI application entry point
- `src/CodeQLToolkit.Features/` - Feature implementations (Query, Test, Bundle, Validation)
- `src/CodeQLToolkit.Shared/` - Shared utilities, configuration, and base classes
- `test/` - Unit tests for each component
- `.github/workflows/` - CI/CD workflows for building and testing
- `.github/actions/` - Reusable actions for CodeQL and QLT installation

## Key Dependencies

- **.NET 6.0+**: Primary development framework
- **System.CommandLine**: CLI framework for command parsing
- **Scriban**: Liquid template engine for code generation
- **Newtonsoft.Json**: JSON serialization for configuration
- **NUnit**: Testing framework

## Development Workflow

1. **Building**: Use `dotnet build` and `dotnet restore` 
2. **Testing**: Use `dotnet test` for unit tests
3. **CLI Usage**: The main executable is `qlt` with features: query, test, bundle, validation, pack, codeql

## Feature Architecture

Each feature follows a consistent pattern:
- `*FeatureMain.cs` - Feature registration and command setup
- `Commands/` - Command implementations 
- `Lifecycle/` - Automation lifecycle targets
- `Templates/` - Liquid templates for file generation

## Automation Types

The toolkit supports multiple automation platforms:
- **Actions**: GitHub Actions integration
- **Local**: Local development workflows

## Configuration

- `qlt.conf.json` - Project configuration file
- Environment variables and CLI options for runtime configuration
- Template-based configuration file generation

## Contributing Guidelines

- Follow existing code patterns and conventions
- Add unit tests for new functionality
- Update documentation for user-facing changes
- Ensure compatibility with existing workflows
- Test changes against sample CodeQL projects

## CLI Command Structure

```
qlt <feature> <action> [options]
```

Features: query, test, bundle, validation, pack, codeql
Common actions: init, create, validate, run