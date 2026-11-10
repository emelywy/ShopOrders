#!/usr/bin/env bash
# Локальний еквівалент конвеєра CI для Linux і macOS.
set -euo pipefail
dotnet restore ShopOrders.sln
dotnet format ShopOrders.sln --verify-no-changes --no-restore
dotnet build ShopOrders.sln -c Release --no-restore -warnaserror
dotnet test ShopOrders.sln -c Release --no-build /p:CollectCoverage=true \
  /p:Threshold=70 /p:ThresholdType=line /p:ThresholdStat=total \
  /p:CoverletOutputFormat=cobertura "/p:CoverletOutput=$PWD/coverage/"
echo "Усі перевірки пройдено."
