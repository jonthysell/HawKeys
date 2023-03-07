param(
    [string]$Configuration = "Debug"
)

[string] $RepoRoot = Resolve-Path "$PSScriptRoot\.."

[string] $OutputRoot = "bld"

[string] $TargetOutputDirectory = "HawKeys.WinStore"

[string]$ProjectPath = "src\HawKeys.WinStore\HawKeys.WinStore.wapproj"
[string]$PackageCertificateKeyFile = "HawKeys.WinStore_TemporaryKey.pfx"

$StartingLocation = Get-Location
Set-Location -Path $RepoRoot

try
{
    if (Test-Path "$OutputRoot\$TargetOutputDirectory") {
        Write-Host "Clean output folder..."
        Remove-Item "$OutputRoot\$TargetOutputDirectory" -Recurse
    }

    New-Item -Path "$OutputRoot\$TargetOutputDirectory" -ItemType Directory

    Write-Host "Build..."

    msbuild -restore -p:Configuration=$Configuration -p:AppxBundle=Always -p:UapAppxPackageBuildMode=StoreUpload -p:RestoreForWinStore=true -p:PackageCertificateKeyFile="$PackageCertificateKeyFile" -p:AppxPackageDir="$RepoRoot\$OutputRoot\$TargetOutputDirectory" "$ProjectPath"
    if (!$?) {
        throw 'Build failed!'
    }
}
finally
{
    Set-Location -Path "$StartingLocation"
}