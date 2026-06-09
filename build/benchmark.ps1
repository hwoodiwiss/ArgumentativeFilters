#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param (
    [switch] $PublishResults = $false
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path $PSScriptRoot -Parent
$benchmarkProject = Join-Path $repoRoot "benchmarks" "ArgumentativeFilters.Benchmarks" "ArgumentativeFilters.Benchmarks.csproj"
$artifactsPath = Join-Path $repoRoot "artifacts" "benchmarks"

Push-Location $repoRoot
try {
    dotnet run --project $benchmarkProject --configuration Release -- --filter * --exporters json --artifacts $artifactsPath

    if ($LASTEXITCODE -ne 0) {
        throw "Benchmarks failed with exit code $LASTEXITCODE"
    }
}
finally {
    Pop-Location
}
