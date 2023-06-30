# One Way 

How it works:

The core binary `qlt` is called. The subcommands are invoked by `qlt`. 

- Each feature should probaby by required to implement something. 
- Qlt.Features.Query.Core 
	- Program.cs implements (perhaps) Qlt.Core.IFeature

- Qlt.Features.Query.Core `qlt-query-core`
	- Commands
	- Data
	- Lifecycle
	- Scaffolding

- Qlt.Features.Query.Jenkins `qlt-query-jenkins`
	- Commands extends Qlt.Features.Query.Core.Commands
	- Data
	- Lifecycle
	- Scaffolding

- Qlt.Features.Query.Actions `qlt-query-actions` 
	- Commands
	- Data
	- Lifecycle
	- Scaffolding

Example commands:

# Alternate Way

Totally flat, lots of projects.

- CodeQLToolkit.Features.Query.Core  `qlt-query-core`
- Qlt.Features.Query.Core.Commands
- Qlt.Features.Query.Core.Data
- Qlt.Features.Query.Core.Lifecycle
- Qlt.Features.Query.Core.Scaffolding


- Qlt.Features.Query.Jenkins `qlt-query-jenkins`
- Qlt.Features.Query.Jenkins 
- Qlt.Features.Query.Jenkins.Commands
- Qlt.Features.Query.Jenkins.Data
- Qlt.Features.Query.Jenkins.Lifecycle
- Qlt.Features.Query.Jenkins.Scaffolding

- Qlt.Features.Query.Actions `qlt-query-actions` 
- Qlt.Features.Query.Actions 
- Qlt.Features.Query.Actions.Commands
- Qlt.Features.Query.Actions.Data
- Qlt.Features.Query.Actions.Lifecycle
- Qlt.Features.Query.Actions.Scaffolding


# Initialization and Registration Process 

1. A shared project `.Shared` contains the code that must be referenced 
by `.Features` modules and by the `.Core` module. 
2. At startup, a registration command is called 


# Command Calling Convention 

The `qlt` command takes as a second argument a string with a prefix which is one of:
- `command` (run)
- `scaffold` (generate)
- `lifecycle` (manage)
- `data` (show)

Examples:

- `qlt query command:run-query`
- `qlt query command:list`
- `qlt query scaffold up` 
- `qlt codeql upgrade`

No explicit prefix: (this design is much better)
- `qlt query run <query-name>`
- `qlt test run all`
- `qlt query generate new-query <query-name>`
- `qlt codeql manage version `
- `qlt query show all`

Explicit Prefix:
- `qlt query command:<query-name>`
- `qlt test command:all` 
- `qlt query scaffold:new-query <query-name>`
- `qlt codeql lifecycle:version `
- `qlt query data:show all`


