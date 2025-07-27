# ---------- Base runtime image ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# ---------- Build image ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy full solution (including EKE_Backend, Repository, Service)
COPY . .

# Restore using the solution file
RUN dotnet restore EKE_Backend.sln

# Publish the main backend project
RUN dotnet publish EKE_Backend/EKE_Backend.csproj -c Release -o /app/publish

# ---------- Final runtime image ----------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EKE_Backend.dll"]
