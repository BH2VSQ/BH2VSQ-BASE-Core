param(
    [string]$Unity = 'H:\tools\unity\2022.3.22f1\Editor\Unity.exe'
)
$ErrorActionPreference = 'Stop'
$project = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'Bootstrap-VRChat.ps1')
if (-not (Test-Path -LiteralPath $Unity)) { throw "Unity Editor missing: $Unity" }
$log = Join-Path $project 'unity-build.log'
$arguments = @('-batchmode', '-nographics', '-quit', '-projectPath', ('"' + $project + '"'), '-executeMethod', 'BH2VSQ.Base.Editor.BaseBuildPipeline.Run', '-logFile', ('"' + $log + '"'))
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -Wait -PassThru -WindowStyle Hidden
if ($process.ExitCode -ne 0) { throw "Unity build failed with exit code $($process.ExitCode). See $log" }
$package = Join-Path $project 'Releases\BH2VSQ_BASE_Core.unitypackage'
if (-not (Test-Path -LiteralPath $package)) { throw "Unity finished without exporting $package. See $log" }
Write-Host "Built $package"
