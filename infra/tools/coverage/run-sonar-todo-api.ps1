# Resolve the directory where the script lives
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
# Resolve the root directory (assuming it's three levels up from tools/Code Coverage)
$RootDir = Resolve-Path (Join-Path $ScriptDir "../../..")

# === CONFIGURE YOUR PROJECT INFO HERE ===
$SonarProjectKey = "Todo-Api"
$SonarToken = "sqp_41de6feb399066c73fdf97c08ea2df9371f3b0f2"
$SonarHostUrl = "http://localhost:9000"
$SlnPath = Join-Path $RootDir "src/apps/App.Api.Todo/App.Api.Todo.csproj"
$CoverageDir = "$RootDir/infra/tools/coverage/todo-api"
$MergedCoverageFile = "$CoverageDir/integration/coverage.opencover.xml"

# ========================================

Write-Host "Running SonarScanner begin..."
dotnet sonarscanner begin `
  /k:$SonarProjectKey `
  /d:sonar.token=$SonarToken `
  /d:sonar.host.url=$SonarHostUrl `
  /d:sonar.scanner.scanAll=false `
  /d:sonar.cs.opencover.reportsPaths="$MergedCoverageFile" `
  /d:sonar.coverage.exclusions="**Program.cs,**Startup.cs,**App.Api.Todo.csproj,**App.Api.Todo.Tests.csproj,**App.Api.Todo.IntegrationTests.csproj,**App.Api.Todo.IntegrationTests.csproj" 

Write-Host "Building the project..."
dotnet build $SlnPath

Write-Host "Running SonarScanner end..."
dotnet sonarscanner end /d:sonar.login=$SonarToken

# Write-Host "Done! View your result at http://localhost:9000/tutorials?id=Todo-Api"
Write-Host "Done!"
