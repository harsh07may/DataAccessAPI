# DataAccessAPI
Common patterns for working with databases:

## To run Project:

```bash
cd .\DataAccessAPI.Api
dotnet build
dotnet run
```

## To run migrations: 

```bash
dotnet ef migrations add <Migration Name> -p .\DataAccessAPI.Infrastructure -s .\DataAccessAPI.Api
dotnet ef database update -p .\DataAccessAPI.Infrastructure -s .\DataAccessAPI.Api
```

## TODOs

- Unit and Integration Testing (test containers?) 
- Stored Procedure Repository Pattern
- EF Core, Transactions
- Audit Trail System (EF Core Interceptors, Shadow Properties, ChangeTracker)
- Infra(Database provisioning & configs)
- GuardClauses for conn strings etc.
- Maybe Redis or distributed cache ?
