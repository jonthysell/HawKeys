param(
    [string]$Configuration = "Debug",
    [boolean]$CreateZip = $False
)

[string] $RepoRoot = Resolve-Path "$PSScriptRoot\.."

[string] $OutputRoot = "bld"

[string] $TargetOutputDirectory = "HawKeys.Portable"
[string] $TargetOutputPackageName = "HawKeys.Portable.zip"

[string]$ProjectPath = "src\HawKeys\HawKeys.csproj"

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

    msbuild -restore -p:Configuration=$Configuration -p:BuildPortable=true "$ProjectPath"
    if (!$?) {
        throw 'Build failed!'
    }

    Copy-Item "src\HawKeys\bin\$Configuration\net20\HawKeys.exe" -Destination "$OutputRoot\$TargetOutputDirectory\HawKeys.exe"
    Copy-Item "README.md" -Destination "$OutputRoot\$TargetOutputDirectory\ReadMe.txt"
    Copy-Item "LICENSE.md" -Destination "$OutputRoot\$TargetOutputDirectory\License.txt"
    Copy-Item "CHANGELOG.md" -Destination "$OutputRoot\$TargetOutputDirectory\ChangeLog.txt"

    if ($CreateZip) {
        if (Test-Path "$OutputRoot\$TargetOutputPackageName") {
            Write-Host "Clean old zip..."
            Remove-Item "$OutputRoot\$TargetOutputPackageName"
        }

        Write-Host "Create zip..."
        Compress-Archive -Path "$OutputRoot\$TargetOutputDirectory" -DestinationPath "$OutputRoot\$TargetOutputPackageName"
    }
}
finally
{
    Set-Location -Path "$StartingLocation"
}