#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param (
    [switch] $PublishResults,
    [Parameter(Mandatory = $false)][string] $Framework = "net10.0"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path $PSScriptRoot -Parent
$artifactsPath = Join-Path $repoRoot "artifacts" "benchmarks"


if ($PublishResults) {
    $additionalArgs += "--exporters", "json"
    $additionalArgs += "--artifacts", $artifactsPath
}

$benchmarkProject = Join-Path $repoRoot "benchmarks" "ArgumentativeFilters.Benchmarks" "ArgumentativeFilters.Benchmarks.csproj"

dotnet run --project $benchmarkProject --configuration Release --framework $Framework -- $additionalArgs --% --filter *

exit $LASTEXITCODE
