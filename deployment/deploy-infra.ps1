[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$NameRGForNestedTemplates="qna-deployment",

  [Parameter(Mandatory=$False)]
  [string]$LocationRGForNestedTemplates="westeurope",

  [Parameter(Mandatory=$False)]
  [string]$NameStorageAcctForNestedTemplates="qnastornestedtemplates",

  [Parameter(Mandatory=$False)]
  [string]$ResourceGroupPrefix="qna",

  [Parameter(Mandatory=$False)]
  [string]$ArtifactPrefix="qna",

  [Parameter(Mandatory=$False)]
  [string]$AADClientId="d03fc97e-cc4e-4758-944a-43fe4cf3eecc",

  [Parameter(Mandatory=$False)]
  [string]$AADB2CIssuer="https://xstofb2c.b2clogin.com/xstofb2c.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_susi"
)

$LocationForSubscriptionLevelDeployment="westeurope"
$NestedTemplatesStorageContainerName="nestedtemplates"  # Value is hardcoded in azuredeploy.json - do not modify in one place

# create RG for deployment artifacts
New-AzResourceGroup -Name $NameRGForNestedTemplates -Location $LocationRGForNestedTemplates -Force -ErrorAction SilentlyContinue

# create storage account in RG to deploy nested templates towards
Write-Output "Creating Storage Account for storing nested arm templates"
$StorageAccount = New-AzStorageAccount -Name $NameStorageAcctForNestedTemplates -ResourceGroupName $NameRGForNestedTemplates -Location $LocationRGForNestedTemplates -SkuName Standard_LRS -ErrorAction SilentlyContinue
$StorageAccount = Get-AzStorageAccount -Name $NameStorageAcctForNestedTemplates -ResourceGroupName $NameRGForNestedTemplates
$StorageContext = $StorageAccount.Context
Write-Output "Storage Account created"

# upload nested templates
Write-Output "Creating storage container for nested templates: '$NestedTemplatesStorageContainerName'"
New-AzStorageContainer -Name $NestedTemplatesStorageContainerName -Context $StorageContext -WarningAction SilentlyContinue
Write-Output "Storage container created"
Write-Output "Uploading nested templates"
Get-ChildItem "./nestedTemplates" -File -Recurse | Set-AzStorageBlobContent -Context $StorageContext -Container $NestedTemplatesStorageContainerName -ErrorAction SilentlyContinue -Force
Write-Output "Templates uploaded"

# create sas token
$SasTokenForNestedTemplates = New-AzStorageContainerSASToken -Context $StorageContext -Name $NestedTemplatesStorageContainerName  -Permission r -ExpiryTime (Get-Date).AddMinutes(180)
Write-Output "Sas-token for accessing nested templates: $SasTokenForNestedTemplates"

$NestedTemplatesLocation = "https://$NameStorageAcctForNestedTemplates.blob.core.windows.net"

# deploy template for storage accounts, functions in each geo
$InfraDeployment = New-AzDeployment -Location $LocationForSubscriptionLevelDeployment -Name "qna-root-deployment" -TemplateFile "azuredeploy.json" -TemplateParameterFile "azuredeploy.parameters.json" -artifactsLocation $NestedTemplatesLocation -artifactsLocationSasToken $SasTokenForNestedTemplates -resourceGroupPrefix $ResourceGroupPrefix -artifactPrefix $ArtifactPrefix -aadClientId $AADClientId -aadB2cIssuer $AADB2CIssuer

# deploy template for frontdoor
$regionDeploys = $(Get-AzDeployment -Name "qna-root-deployment").Outputs.regionDeployments.Value | ConvertFrom-Json

# create list of backend addresses for static assets
$staticAssetsBackendAddresses = @()
Foreach ($deploy in $regionDeploys){
  $StorageAccount = Get-AzStorageAccount -Name $deploy.storageAccount -ResourceGroupName $deploy.resourceGroup
  $StorageAccountStaticWebsiteUrl = $StorageAccount.PrimaryEndpoints.Web
  $StorageAccountStaticWebsiteUrl = $StorageAccountStaticWebsiteUrl.Replace("https://", "").Replace("/", "")

  $staticAssetsBackendAddresses += $StorageAccountStaticWebsiteUrl
}

# create list of backend addresses for functions
$functionAppBackendAddresses = @()
Foreach($deploy in $regionDeploys){
  # use the below to configure frontdoor to bypass APIM and go direct to the azurewebsites url
  # $functionAppBackendAddresses += "$($deploy.functionAppName).azurewebsites.net"
  $functionAppBackendAddresses += $deploy.apimUrl
}

New-AzResourceGroupDeployment -ResourceGroupName "$($ResourceGroupPrefix)-fd" -Name "qna-fd-deployment" -TemplateFile "frontdoor-template.json" -frontDoorName "$($ArtifactPrefix)-frontdoor" -staticAssetsBackendAddresses $staticAssetsBackendAddresses -functionAppBackendAddresses $functionAppBackendAddresses

return $InfraDeployment