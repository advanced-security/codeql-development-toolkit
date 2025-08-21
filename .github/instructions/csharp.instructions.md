# C# Code Instructions for CodeQL Development Toolkit

## Code Style and Conventions

- Use nullable reference types consistently. Declare properties as nullable (`string?`) when they can be null, or ensure non-nullable properties are initialized.
- Follow existing namespace patterns: `CodeQLToolkit.Features.*`, `CodeQLToolkit.Shared.*`, `CodeQLToolkit.Core.*`
- Use the existing logging pattern: `Log<ClassName>.G().LogInformation()` for logging
- Prefer `Path.Combine()` for file path construction instead of string concatenation

## Architecture Patterns

- **Feature Pattern**: Features are organized under `CodeQLToolkit.Features` with main registration classes like `QueryFeatureMain`, `TestFeatureMain`, etc.
- **Command Pattern**: Use `System.CommandLine` for CLI command implementation with proper option handling
- **Template Pattern**: Lifecycle targets inherit from `BaseLifecycleTarget` and implement specific automation workflows
- **Dependency Injection**: Use constructor injection and register services appropriately

## Key Classes and Patterns

- `BaseLifecycleTarget`: Abstract base for lifecycle automation targets (Actions, Local, etc.)
- `ILifecycleTarget`: Interface for lifecycle operations with `Run()` method
- `TemplateUtil`: Use for rendering Liquid templates with `TemplateFromFile()` and `RawTemplateFromFile()`
- `QLTConfig`: Configuration management with JSON serialization using Newtonsoft.Json
- `Query`: Utility class for managing CodeQL query file paths and structure

## File Organization

- Keep automation-specific logic in separate targets under `Lifecycle/Targets/Actions/`, `Lifecycle/Targets/Local/`
- Templates should be organized by feature: `Templates/Query/`, `Templates/Test/`, `Templates/Bundle/`
- Shared utilities go in `CodeQLToolkit.Shared/Utils/`

## Error Handling

- Use proper exception handling and meaningful error messages
- Log important operations and errors using the established logging framework
- Validate file paths and directory existence before operations

## Testing

- Follow the existing test structure in `test/CodeQLToolkit.*.Tests/`
- Use NUnit testing framework consistently with the existing patterns
- Consider using constraint model: `Assert.That(actual, Is.EqualTo(expected))` instead of classic model