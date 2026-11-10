# Локальний еквівалент конвеєра CI: ті самі кроки, той самий поріг покриття.
# Запуск із кореня репозиторію: powershell -ExecutionPolicy Bypass -File scripts/check.ps1
$ErrorActionPreference = 'Stop'
function Step([string]$title, [scriptblock]$command) {
    Write-Host "== $title"
    & $command
    if ($LASTEXITCODE -ne 0) { throw "Крок «$title» завершився з кодом $LASTEXITCODE" }
}
Step 'restore' { dotnet restore ShopOrders.sln }
Step 'format' { dotnet format ShopOrders.sln --verify-no-changes --no-restore }
Step 'build' { dotnet build ShopOrders.sln -c Release --no-restore -warnaserror }
Step 'test + coverage' {
    dotnet test ShopOrders.sln -c Release --no-build /p:CollectCoverage=true `
        /p:Threshold=70 /p:ThresholdType=line /p:ThresholdStat=total `
        /p:CoverletOutputFormat=cobertura "/p:CoverletOutput=$PWD/coverage/"
}
Write-Host 'Усі перевірки пройдено.'
