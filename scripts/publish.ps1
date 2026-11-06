param([string]$Runtime = "win-x64")
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$output = Join-Path $root "artifacts/$Runtime"
dotnet publish (Join-Path $root "src/ChudoObuv.WinForms/ChudoObuv.WinForms.csproj") -c Release -r $Runtime --self-contained true -o $output
if ($LASTEXITCODE -ne 0) { throw "Publish failed" }
Copy-Item (Join-Path $root "README.md") $output
Compress-Archive -Path "$output/*" -DestinationPath (Join-Path $root "artifacts/ChudoObuv-$Runtime.zip") -Force
Write-Host "Ready: artifacts/ChudoObuv-$Runtime.zip"
