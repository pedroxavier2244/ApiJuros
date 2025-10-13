FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["ApiJuros.Presentation/ApiJuros.Presentation.csproj", "ApiJuros.Presentation/"]
COPY ["ApiJuros.Application/ApiJuros.Application.csproj", "ApiJuros.Application/"]
COPY ["ApiJuros.Infrastructure/ApiJuros.Infrastructure.csproj", "ApiJuros.Infrastructure/"]
COPY ["ApiJuros.Domain/ApiJuros.Domain.csproj", "ApiJuros.Domain/"]
RUN dotnet restore "ApiJuros.Presentation/ApiJuros.Presentation.csproj"

COPY . .
WORKDIR "/src/ApiJuros.Presentation"
RUN dotnet publish "ApiJuros.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

RUN apk add --no-cache icu-libs tzdata

ENV TZ=America/Sao_Paulo

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiJuros.Presentation.dll"]