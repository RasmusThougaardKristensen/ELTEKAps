# Template

## Migrations

Create tables for database go to directory "src/Management/Management.Infrastructure" to run EF core migraiton commands

```Powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --verbose --context TaskContext --project . --startup-project ../Management.Api.Service
```


```shell
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --verbose --context TaskContext --project . --startup-project ../Management.Api.Service
```


To add a new migration:

```shell
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add AddUser --context TaskContext --project . --startup-project ../Management.Api.Service
```

```Powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef migrations add AddTask --context TaskContext --project . --startup-project ../Management.Api.Service
```

To remove latest added:
```shell
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations remove --context TaskContext --project . --startup-project ../Management.Api.Service
```

```Powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef migrations remove --context TaskContext --project . --startup-project ../Management.Api.Service
```
