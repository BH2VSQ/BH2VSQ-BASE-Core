$ErrorActionPreference = 'Stop'
$project = Split-Path -Parent $PSScriptRoot
$packages = Join-Path $project 'Packages'
$temp = Join-Path $project 'Temp'
$cache = Join-Path $temp 'VRChatPackages'
New-Item -ItemType Directory -Path $cache -Force | Out-Null

$items = @(
    @{ Name = 'com.vrchat.base'; Sha = '86e8187a7d8f5fb5a54d442b64d09899be2fab2ef1b7822a3c202549ff05ff6b' },
    @{ Name = 'com.vrchat.worlds'; Sha = 'a21cdda0202ce05a33f0892eb6cfb0425e1429a4d1cb5bdcceee7994116017aa' }
)

foreach ($item in $items) {
    $target = Join-Path $packages $item.Name
    if (Test-Path -LiteralPath (Join-Path $target 'package.json')) {
        Write-Host "$($item.Name) is already present."
        continue
    }
    $zip = Join-Path $cache "$($item.Name)-3.10.4.zip"
    $url = "https://github.com/vrchat/packages/releases/download/3.10.4/$($item.Name)-3.10.4.zip"
    Invoke-WebRequest -Uri $url -OutFile $zip
    $actual = (Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($actual -ne $item.Sha) { throw "Checksum mismatch for $($item.Name)." }
    New-Item -ItemType Directory -Path $target -Force | Out-Null
    Expand-Archive -LiteralPath $zip -DestinationPath $target -Force
    Remove-Item -LiteralPath $zip
    Write-Host "Installed $($item.Name) 3.10.4"
}
