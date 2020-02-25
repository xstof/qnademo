[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$DeploymentName = "qna-root-deployment",

  [Parameter(Mandatory=$False)]
  [string]$PackagePath = ".\functionpackage\qna-fn-backend.zip"
)

$regionDeploys = $(Get-AzDeployment -Name $DeploymentName).Outputs.regionDeployments.Value | ConvertFrom-Json

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$pathForPackage = Join-Path -Path $scriptPath -ChildPath $PackagePath

Foreach ($deploy in $regionDeploys){
  # publish function
  Publish-AzWebapp -ResourceGroupName $deploy.resourceGroup -Name $deploy.functionAppName -ArchivePath $pathForPackage -Force

  # (section below is optional, also done by deploy-infra.ps1 script) update function settings:
      $signalRName = $deploy.signalrName
      $functionName = $deploy.functionAppName
      $key = Get-AzSignalRKey -ResourceGroupName $deploy.resourceGroup -Name $signalRName

      # Get Function App Settings:
      $functionApp = Get-AzWebApp -ResourceGroupName $deploy.resourceGroup -Name $functionName
      $functionAppSettings = $functionApp.SiteConfig.AppSettings
      $hashTableWithSettings = @{}
      ForEach ($item in $functionAppSettings){
        if ($item.Name -eq "AzureSignalRConnectionString") {    
          $hashTableWithSettings[$item.Name] = $key.PrimaryConnectionString
        } else {
          $hashTableWithSettings[$item.Name] = $item.Value
        }
      }

      # Set Function App Settings:
      Set-AzWebApp -ResourceGroupName $deploy.resourceGroup -Name $functionName -AppSettings $hashTableWithSettings
      Write-Output "changed function $functionName web app settings for SignalR to: $($key.PrimaryConnectionString)"
}

Write-Output "Function app packages deployed to: "
Foreach ($deploy in $regionDeploys){
  Write-Output $deploy.functionAppName
}
