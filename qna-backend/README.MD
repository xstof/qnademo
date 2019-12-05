# Getting started with the QnA Functions backend

Until this is fixed, there's some steps to do to hook this up to your AAD B2C app registration and tenant:
- go into `WireModels/Configuration.cs` and change this to reflect your AAD B2C information

## Testing the functions locally

- Make sure to run the `./deployment/deploy-infra.ps1` script, which will setup all the Azure resources (with the exception of AAD B2C)
- Install the Azure Functions CLI and the VS Code plugin
- Use the plugin to download the app settings from the environment in Azure; this will pull in all the connection data towards Cosmos / SignalR etc...
- Run the functions using `func host start`
