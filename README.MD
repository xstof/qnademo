# QnA Demo Application

This application demonstrates how a cloud-first modern design allows to combine large geo-distributed scale out of compute with high resilience while not giving in on cost or user experience.  This is made possible by the smart combination of:

- Azure FrontDoor
- Azure APIM (consumption)
- Azure Functions (consumption)
- Cosmos DB
- SignalR

## Setup the demo application

The scripts to set it up with reside in the `./deployment` folder:

~~~ps1
# Note: this script assumes a logged in PowerShell session, to the right subscription
# Set up the infrastructure by deploying the ARM templates:
./deploy-infra -NameRGForNestedTemplates [Name for resource group] -LocationRGForNestedTemplates [Location]  -NameStorageAcctForNestedTemplates [Name for storage account] -ResourceGroupPrefix [Prefix for the generated Resource Groups] -ArtifactPrefix [Prefix For Generated Artifacts] -AADClientId [Client ID for your registered AAD B2C App] -AADB2CIssuer [AAD B2C issuer in form of something like: https://xstofb2c.b2clogin.com/xstofb2c.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_susi]
~~~

## Deploy the Functions app backend

In order to deploy the function code which will serve as the application backend in each of the geos:

~~~ps1
# First package the functions up in a zip package:
./package-functionapp-to-zip.ps1

# Deploy the zip packages to each of the geos:
./deploy-function-package-global
~~~

## Deploy the frontend app 

The frontend app is written in Vue.js (more specifically in Quasar, which is an opinionated convenience layer on top).

In order to build the frontend, follow the build instruction in README.MD the `qna` folder.  In summary

~~~ps1
# first make some changes to frontend to work with your own AAD B2C - see markdown
# then build the frontend:
quasar build

# then deploy it from within the deployment directory:
deploy-static-assets-global.ps1 
~~~
