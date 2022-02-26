FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["AlertExecutionEngine.Cli/AlertExecutionEngine.Cli.csproj", "AlertExecutionEngine.Cli/"]
RUN dotnet restore "AlertExecutionEngine.Cli/AlertExecutionEngine.Cli.csproj"

COPY ["AlertExecutionEngine.Domain/AlertExecutionEngine.Domain.csproj", "AlertExecutionEngine.Domain/"]
RUN dotnet restore "AlertExecutionEngine.Domain/AlertExecutionEngine.Domain.csproj"

COPY ["AlertExecutionEngine.Domain.Contracts/AlertExecutionEngine.Domain.Contracts.csproj", "AlertExecutionEngine.Domain.Contracts/"]
RUN dotnet restore "AlertExecutionEngine.Domain.Contracts/AlertExecutionEngine.Domain.Contracts.csproj"

COPY ["AlertExecutionEngine.AlertsService.Http/AlertExecutionEngine.AlertsService.Http.csproj", "AlertExecutionEngine.AlertsService.Http/"]
RUN dotnet restore "AlertExecutionEngine.AlertsService.Http/AlertExecutionEngine.AlertsService.Http.csproj"

COPY . .
WORKDIR "/src/AlertExecutionEngine.Cli"
RUN dotnet build "AlertExecutionEngine.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlertExecutionEngine.Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlertExecutionEngine.Cli.dll"]
