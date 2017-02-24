<# 

#>
Param ()

$solutionPath = Split-Path $PSScriptRoot

Get-ChildItem -Path $solutionPath -Directory -Recurse -Hidden | ? { $_.Name -eq ".vs" } | % { Remove-Item $_.FullName -Recurse -Force }
Get-ChildItem -Path $solutionPath -Directory -Recurse -Hidden| ? { $_.Name -eq "bin" -or $_.Name -eq "obj" } | % { Remove-Item $_.FullName -Recurse -Force }
Get-ChildItem -Path $solutionPath -File -Recurse -Hidden | ? { $_.Name.EndsWith("csproj.vspscc") -or $_.Name -eq "StyleCop.Cache" } | % { Remove-Item $_.FullName -Force }