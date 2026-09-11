Write-Host "Rodando testes com coverage..." -ForegroundColor Cyan

dotnet test BJJChain.Tests/BJJChain.Tests.csproj `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover `
  /p:CoverletOutput=./coverage/

Write-Host "Coverage gerado em BJJChain.Tests/coverage/coverage.opencover.xml" -ForegroundColor Green