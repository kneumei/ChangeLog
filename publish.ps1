dotnet publish ChangeLog.Web\ChangeLog.Web.csproj -o site

$publishPath = "$PSScriptRoot\ChangeLog.Web\site"

Add-Type -Assembly System.IO.Compression.FileSystem
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
[System.IO.Compression.ZipFile]::CreateFromDirectory($publishPath, "$PSScriptRoot/site.zip", $compressionLevel, $false)

Remove-Item $publishPath -Recurse -Confirm:$false

$tempPath = "$PSScriptRoot\temp"

if (Test-Path $tempPath) {
    Remove-Item $tempPath -Recurse -Confirm:$false
}

New-Item -ItemType Directory -Path (Join-Path $PSScriptRoot "temp") | Out-Null
Move-Item -Path "$PSScriptRoot\site.zip" -Destination $tempPath
Copy-Item -Path "$PSScriptRoot\aws-windows-deployment-manifest.json" -Destination $tempPath
#Copy-Item -Path "$PSScriptRoot\.ebextensions" -Destination $tempPath -Recurse

if (Test-Path "$PSScriptRoot/changelog.zip") {
    Remove-Item "$PSScriptRoot/changelog.zip"
}

[System.IO.Compression.ZipFile]::CreateFromDirectory($tempPath, "$PSScriptRoot/changelog.zip", $compressionLevel, $false)

Remove-Item $tempPath -Recurse -Confirm:$false