### Integration tests
Integration tests are mainly intended as a help for developers to have a quick turn-around when developing features. The tests can also be used to verify if a service has an issue.

The integration tests uses [UserSecrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) to avoid checked in credentials.

To get started, make sure you are in the directory of the testproject and use the following commands

```
dotnet user-secrets set Database {database}
dotnet user-secrets set Key {key}
dotnet user-secrets set Uri {uri}
dotnet user-secrets set Collcetion {collection}
```

Replace the {...} (including brackets) with your parameters for the CosmosDB you want to test against.