# One Way 

How it works:

The core binary `qlt` is called. The subcommands are invoked by `qlt`. 

- Each feature should probaby by required to implement something. 
- CodeQLToolkit.Features.Query.Core 
	- Program.cs implements (perhaps) CodeQLToolkit.Core.IFeature

- CodeQLToolkit.Features.Query.Core `qlt-query-core`
	- Commands
	- Data
	- Lifecycle
	- Scaffolding

- CodeQLToolkit.Features.Query.Jenkins `qlt-query-jenkins`
	- Commands extends CodeQLToolkit.Features.Query.Core.Commands
	- Data
	- Lifecycle
	- Scaffolding

- CodeQLToolkit.Features.Query.Actions `qlt-query-actions` 
	- Commands
	- Data
	- Lifecycle
	- Scaffolding

Example commands:

# Alternate Way

Totally flat, lots of projects.

- CodeQLToolkit.Features.Query.Core  `qlt-query-core`
- CodeQLToolkit.Features.Query.Core.Commands
- CodeQLToolkit.Features.Query.Core.Data
- CodeQLToolkit.Features.Query.Core.Lifecycle
- CodeQLToolkit.Features.Query.Core.Scaffolding


- CodeQLToolkit.Features.Query.Jenkins `qlt-query-jenkins`
- CodeQLToolkit.Features.Query.Jenkins 
- CodeQLToolkit.Features.Query.Jenkins.Commands
- CodeQLToolkit.Features.Query.Jenkins.Data
- CodeQLToolkit.Features.Query.Jenkins.Lifecycle
- CodeQLToolkit.Features.Query.Jenkins.Scaffolding

- CodeQLToolkit.Features.Query.Actions `qlt-query-actions` 
- CodeQLToolkit.Features.Query.Actions 
- CodeQLToolkit.Features.Query.Actions.Commands
- CodeQLToolkit.Features.Query.Actions.Data
- CodeQLToolkit.Features.Query.Actions.Lifecycle
- CodeQLToolkit.Features.Query.Actions.Scaffolding


# Initialization and Registration Process 

1. A shared project `.Shared` contains the code that must be referenced 
by `.Features` modules and by the `.Core` module. 
2. At startup, a registration command is called 


# Command Calling Convention 

The `qlt` command takes as a second argument a string with a prefix which is one of:
- `command` (run)
- `scaffold` (generate)
- `lifecycle` (manage/set/get/upgrade/register/init)
- `data` (show)

Examples:

- `qlt query command:run-query`
- `qlt query command:list`
- `qlt query scaffold up` 
- `qlt codeql upgrade`

No explicit prefix: (this design is much better)
- `qlt query run <query-name>`
- `qlt test run all` -- flags for slicing and dicing 
- `qlt query generate new-query <query-name>`
- `qlt codeql manage version `
- `qlt query show all`

Explicit Prefix:
- `qlt query command:<query-name>`
- `qlt test command:all` 
- `qlt query scaffold:new-query <query-name>`
- `qlt codeql lifecycle:version `
- `qlt query data:show all`




------------

Do an example:

`qlt query generate new-query`

1. The feature module
2. The submodule 
3. The thing to generate 

-----

Alternate version that uses just one level of commands.

qlt generate query:new-query

the top level commands 


# General Directory Structure


`language`
	- `package name`
		- `src` `scope/package-name`
		- `test` `scope/package-name-test`
