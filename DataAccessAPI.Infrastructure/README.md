# DataAccessAPI Infrastructure

## Learnings

- Major TIL: Just appending a semi-colon at the end of the braces style namespace auto-converts it lol.
- Configure in OnModelCreating in AppDbContext to offload Guids to DB.
- FindAsync is for primary key search and has tracking by default.
- EF Core already implements Repository + Unit of Work with DbSet + DbConext; avoid over abstracting and use context in service directly.
- Application Layer should not depend on Infra layer, which means you cant use AppContext in service directly
  so create a Application/IAppDbContext abstraction and inherit/implement + register it in Infra/AppDbContext.

## To run migrations: 

```bash
dotnet ef migrations add <Migration Name> -p .\DataAccessAPI.Infrastructure -s .\DataAccessAPI.Api

dotnet ef database update -p .\DataAccessAPI.Infrastructure -s .\DataAccessAPI.Api
```