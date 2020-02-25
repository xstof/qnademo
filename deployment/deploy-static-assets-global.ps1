[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$DeploymentName = "qna-root-deployment",

  [Parameter(Mandatory=$False)]
  [string]$DistPath = "..\qna\dist\spa\"  # Path is relative to the location of _this_ ps1 file; not relative to path of the caller
)

$regionDeploys = $(Get-AzDeployment -Name $DeploymentName).Outputs.regionDeployments.Value | ConvertFrom-Json

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Write-Output "script path: $scriptPath"
$pathForDeploymentScript = Join-Path -Path $scriptPath -ChildPath ".\deploy-static-site-onto-az-storage.ps1"
$FullDistPath = [System.IO.Path]::GetFullPath($(Join-Path -Path $scriptPath -ChildPath $DistPath))
Write-Output "FullDistPath: $FullDistPath"

Foreach ($deploy in $regionDeploys){
 & $pathForDeploymentScript -RG $deploy.resourceGroup -StorageAccountName $deploy.storageAccount -PathForFilesToUpload $FullDistPath
}

Write-Output "Websites deployed to: "
Foreach ($deploy in $regionDeploys){
  $StorageAccount = Get-AzStorageAccount -Name $deploy.storageAccount -ResourceGroupName $deploy.resourceGroup
  $StorageAccountStaticWebsiteUrl = $StorageAccount.PrimaryEndpoints.Web
  Write-Output $StorageAccountStaticWebsiteUrl
}
