[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$DeploymentName = "qna-root-deployment",

  [Parameter(Mandatory=$False)]
  [string]$PackagePath = ".\functionpackage\qna-fn-backend.zip"
)

$regionDeploys = $(Get-AzDeployment -Name "qna-root-deployment").Outputs.regionDeployments.Value | ConvertFrom-Json

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$pathForPackage = Join-Path -Path $scriptPath -ChildPath $PackagePath

Foreach ($deploy in $regionDeploys){
  Publish-AzWebapp -ResourceGroupName $deploy.resourceGroup -Name $deploy.functionAppName -ArchivePath $pathForPackage -Force
}

Write-Output "Function app packages deployed to: "
Foreach ($deploy in $regionDeploys){
  Write-Output $deploy.functionAppName
}
