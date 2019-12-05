[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$ResourceGroupPrefix="qna"
)

# todo, do this in parallel: https://docs.microsoft.com/en-us/powershell/module/psworkflow/about/about_foreach-parallel?view=powershell-5.1

Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-weur" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-neur" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-weus" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-weus2" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-eaus" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-cenus" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-eaus2" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-wceus" -Force
Remove-AzResourceGroup -Name "$($ResourceGroupPrefix)-nceus" -Force