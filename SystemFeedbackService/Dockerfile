FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SystemFeedbackService/SystemFeedbackService.csproj", "SystemFeedbackService/"]
RUN dotnet restore "SystemFeedbackService/SystemFeedbackService.csproj"
COPY . .
WORKDIR "/src/SystemFeedbackService"
RUN dotnet build "SystemFeedbackService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SystemFeedbackService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SystemFeedbackService.dll"]
