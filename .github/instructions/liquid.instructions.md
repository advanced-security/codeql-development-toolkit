# Liquid Template Instructions for CodeQL Development Toolkit

## Template Purpose and Structure

Liquid templates in this project generate CodeQL-related files, GitHub Actions workflows, and project configuration files. They are organized by feature area and automation type.

## Template Organization

- **Query Templates**: `Templates/Query/` - Generate CodeQL query files, qlpack.yml, test files
- **Test Templates**: `Templates/Test/Actions/` - Generate GitHub Actions for testing workflows
- **Bundle Templates**: `Templates/Bundle/Actions/` - Generate bundle integration test workflows
- **Validation Templates**: `Templates/Validation/Actions/` - Generate validation workflows

## Variable Naming Conventions

Use snake_case for template variables to match existing patterns:
- `query_name`, `query_pack_name`, `language`, `description`
- `num_threads`, `use_runner`, `dev_mode`, `branch`
- `codeql_args`, `extra_args`, `automation_type`

## Common Template Patterns

### CodeQL Query Headers
Always include proper metadata headers for CodeQL queries:
```liquid
/**
 * @id {{language}}/{{query_pack_name}}/{{query_name}}
 * @name {{query_name}}
 * @description {{description}}
 * @kind problem
 * @precision medium
 * @problem.severity error
 * @tags {{query_pack_name}}
 */
```

### GitHub Actions Structure
For workflow templates:
- Use descriptive names and descriptions
- Include proper input/output definitions
- Use composite actions pattern with shell steps
- Include debug logging: `echo "::debug::message"`

### File Path Construction
Use consistent path patterns:
- CodeQL queries: `{{language}}/{{query_pack_name}}/src/{{query_name}}/{{query_name}}.ql`
- Test files: `{{language}}/{{query_pack_name}}/test/{{query_name}}/{{query_name}}.{{language_extension}}`

## Template Rendering Context

Templates are rendered using Scriban engine with:
- `TemplateUtil.TemplateFromFile()` for parsing and rendering
- Variables passed as anonymous objects in C# code
- Access to language-specific helpers through utility classes

## Best Practices

- Keep templates focused on single responsibility
- Use meaningful variable names that reflect the generated content purpose
- Include appropriate comments in generated files
- Ensure generated files follow CodeQL and GitHub Actions best practices
- Use conditional logic sparingly - prefer multiple specialized templates