FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MediTrack.MedicalAnalysisService.API/MediTrack.MedicalAnalysisService.API.csproj", "MediTrack.MedicalAnalysisService.API/"]
RUN dotnet restore "MediTrack.MedicalAnalysisService.API/MediTrack.MedicalAnalysisService.API.csproj"
COPY . .
WORKDIR "/src/MediTrack.MedicalAnalysisService.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediTrack.MedicalAnalysisService.API.dll"]
