# Resolve the directory where the script lives
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
# Resolve the root directory (assuming it's three levels up from tools/Code Coverage)
$RootDir = Resolve-Path (Join-Path $ScriptDir "../../..")

$InfraUnitTestProj = Join-Path $RootDir "tests/Test.App.Common.Infrastructure.UnitTest/Test.App.Common.Infrastructure.UnitTest.csproj"
$UnitTestProj = Join-Path $RootDir "tests/Test.App.Api.Todo.UnitTest/Test.App.Api.Todo.UnitTest.csproj"
$IntegrationTestProj = Join-Path $RootDir "tests/Test.App.Api.Todo.IntegrationTest/Test.App.Todo.Integration.csproj"
$CoverageDir = "$RootDir/infra/tools/coverage/todo-api"
$MergedCoverageFile = "$CoverageDir/integration/coverage.opencover.xml"
$HtmlReportDir = "$CoverageDir/report"
# ========================================

Write-Host "Cleaning previous coverage results..."
Remove-Item -Recurse -Force $CoverageDir -ErrorAction SilentlyContinue

Write-Host "Running infra tests with coverage..."
dotnet test $InfraUnitTestProj `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover `
  /p:CoverletOutputFormatVersion=1.0 `
  /p:CoverletOutput="$CoverageDir/infra/"

Write-Host "Running unit tests with coverage..."
dotnet test $UnitTestProj `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover `
  /p:CoverletOutputFormatVersion=1.0 `
  /p:CoverletOutput="$CoverageDir/unit/" `
  /p:MergeWith="$CoverageDir/infra/coverage.json"

Write-Host "Running integration tests with merged coverage..."
dotnet test $IntegrationTestProj `
   /p:CollectCoverage=true `
   /p:CoverletOutputFormat=opencover `
   /p:CoverletOutputFormatVersion=1.0 `
   /p:CoverletOutput="$CoverageDir/integration/" `
   /p:MergeWith="$CoverageDir/unit/coverage.json"

Write-Host "Coberture path : " $MergedCoverageFile

Write-Host "Generating HTML report locally..."
reportgenerator `
  -reports:$MergedCoverageFile `
  -targetdir:$HtmlReportDir `
  -reporttypes:Html

# Write-Host "Done! View your HTML report at: $HtmlReportDir/index.html"
Write-Host "Done!"