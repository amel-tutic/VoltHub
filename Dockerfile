# One image for the whole system: the ASP.NET Core API also serves the Angular app from wwwroot.
# Build:  docker build -t volthub .
# Run:    see README.md, "Run with Docker"

# ---- 1. Build the Angular frontend
FROM node:24-alpine AS frontend
WORKDIR /frontend
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci
COPY frontend/ ./
RUN npx ng build --configuration production

# ---- 2. Publish the API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /repo
COPY . .
RUN dotnet publish src/VoltHub.Api/VoltHub.Api.csproj -c Release -o /app

# ---- 3. Runtime: the published API, with the Angular build in wwwroot
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=backend /app ./
COPY --from=frontend /frontend/dist/volthub-web/browser ./wwwroot
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "VoltHub.Api.dll"]
