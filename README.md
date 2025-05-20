# test-dotnet-project
This is a test project on .NET 9. WEB API service, implementing API CRUD methods on Users entity.
## How to start?
Create `.env` file in root, consisting of 3 lines, substituting values:
```
ADMIN_USER_LOGIN=<...>
ADMIN_USER_PASSWORD=<...>
JWT_SECRET=<...random long line...>
```
Run `docker compose up -d`, then navigate to `http://localhost:8080/swagger`.
