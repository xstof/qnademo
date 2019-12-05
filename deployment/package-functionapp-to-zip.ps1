[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$ZipDestinationPath = ".\functionpackage\qna-fn-backend.zip",

  [Parameter(Mandatory=$False)]
  [string]$CSProjFileLocation = "..\qna-backend",

  [Parameter(Mandatory=$False)]
  [string]$TempDirectory = "$pwd\published"
)

# Resolve temp path in full path name and create temp path if not exist
If(!(test-path $TempDirectory))
{
      New-Item -ItemType Directory -Force -Path $TempDirectory
}
$FullTempPath = (Resolve-Path $TempDirectory).Path
Write-Output "Temp path is set to: $FullTempPath"

# Make sure destination path exists and if not create it 
$DestinationFolderPath = Split-Path -Path $ZipDestinationPath
If(!(test-path $DestinationFolderPath))
{
      New-Item -ItemType Directory -Force -Path $DestinationFolderPath
}

# Publish the function into the temp dir
dotnet publish $CSProjFileLocation -c Debug -o $FullTempPath

# Zip it up
Compress-Archive -Path "$FullTempPath\*" -DestinationPath $ZipDestinationPath -Force
Remove-Item "$FullTempPath" -ErrorAction SilentlyContinue -Force -Recurse

Write-Output "Function app was packaged into zip file and written to: $ZipDestinationPath"