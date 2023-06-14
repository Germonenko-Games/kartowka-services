# Kartowka web API

## Environment setup

First of all, you need to setup project configuration.
Here is user secrets template:
```json
{
  "ConnectionStrings": {
    "Core": "{CORE_DATABASE_CONNECTION_STRING_HERE}"
  }
}
```

## Adding new migrations

To add a new migrations add the following comand from withing the root of the repository.
```shell
dotnet ef migrations add {MIGRATION_NAME} --project src/Kartowka.Migrations --startup-project src/Kartowka.Api -o ./
```
