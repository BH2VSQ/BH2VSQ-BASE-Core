param(
    [string]$Unity = 'H:\tools\unity\2022.3.22f1\Editor\Unity.exe'
)
$ErrorActionPreference = 'Stop'
$project = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'Bootstrap-VRChat.ps1')
if (-not (Test-Path -LiteralPath $Unity)) { throw "Unity Editor missing: $Unity" }
$log = Join-Path $project 'unity-build.log'
if (-not (Get-ChildItem -Path (Join-Path $project 'Library/PackageCache/com.unity.textmeshpro@*/Package Resources/TMP Essential Resources.unitypackage') -ErrorAction SilentlyContinue)) {
    $import = Start-Process -FilePath $Unity -ArgumentList @('-batchmode', '-nographics', '-quit', '-projectPath', ('"' + $project + '"'), '-logFile', ('"' + $log + '"')) -PassThru -WindowStyle Hidden
    $import.WaitForExit()
    if ($import.ExitCode -ne 0) { throw "Unity package import failed with exit code $($import.ExitCode). See $log" }
}
python (Join-Path $PSScriptRoot 'Extract-Tmp-Essentials.py')
if ($LASTEXITCODE -ne 0) { throw 'TextMesh Pro resources could not be prepared.' }
$arguments = @('-batchmode', '-nographics', '-quit', '-projectPath', ('"' + $project + '"'), '-executeMethod', 'BH2VSQ.Base.Editor.BaseBuildPipeline.Run', '-logFile', ('"' + $log + '"'))
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -PassThru -WindowStyle Hidden
$process.WaitForExit()
if ($process.ExitCode -ne 0) { throw "Unity build failed with exit code $($process.ExitCode). See $log" }
$package = Join-Path $project 'Releases\BH2VSQ_BASE_Core.unitypackage'
if (-not (Test-Path -LiteralPath $package)) { throw "Unity finished without exporting $package. See $log" }
Write-Host "Built $package"
