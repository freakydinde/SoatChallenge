<#
.Synopsis
Copy resource to resources directory
 
.Description
use solution folder and target folder from parameters to copy cs, ps1 and dll files to resources folder
 
.Parameter SolutionFolder
Solution folder, containing sln file
 
.Parameter OuputFolder
Target folder, Debug or Release
 
.Example
Copy-ToOutput.ps1 "C:\Tfs\freaky\SoatChallenge\Main\" "C:\Tfs\freaky\SoatChallenge\Main\SoatChallenge\bin\Debug\"
 
.Remarks
post build event command line : %SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe -File $(ProjectDir)\Scripts\Copy-ToOutput.ps1 $(SolutionDir) $(ProjectDir) $(TargetDir)
 
.Notes
    fileName    : Copy-ToOuput.ps1
    version        : 0.002
    author        : Armand Lacore
#>
[CmdletBinding()]
Param ( [Parameter(Mandatory=$true,Position=0)][string]$SolutionFolder,
		[Parameter(Mandatory=$true,Position=1)][string]$ProjectFolder,
		[Parameter(Mandatory=$true,Position=2)][string]$OutputFolder )
 
$csFiles = $ProjectFolder | Get-ChildItem -Filter *.cs -Exclude Temp* -Recurse
$psFiles = $OutputFolder | Get-ChildItem -Filter *.ps1 -Recurse
$dllFiles = $OutputFolder | Get-ChildItem -Filter *.dll -Recurse
 
$ressourceFolder = [IO.Path]::Combine($SolutionFolder, "Resources")
$codeFilesFolder = [IO.Path]::Combine($ressourceFolder, "Code")
 
if (Test-Path $codeFilesFolder)
{
	Remove-Item -Path $codeFilesFolder -Force -Recurse
}
 
New-Item -Path $codeFilesFolder -ItemType Directory -Force
 
Copy-Item -Path $psFiles.FullName -Destination $ressourceFolder
Copy-Item -Path $dllFiles.FullName -Destination $ressourceFolder
Copy-Item -Path $csFiles.FullName -Destination $codeFilesFolder