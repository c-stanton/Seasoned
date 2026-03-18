# Stage 1: Build the Nuxt 4 frontend (Debian Slim)
FROM node:20-bookworm-slim AS frontend-build
WORKDIR /src/frontend
COPY Seasoned.Frontend/package*.json ./
RUN npm install
COPY Seasoned.Frontend/ .
RUN npm run test
RUN npx nuxi generate 

# Stage 2: Build the .NET 9 backend (Debian Slim)
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS backend-build
WORKDIR /src
COPY Seasoned.Backend/*.csproj ./Seasoned.Backend/
RUN dotnet restore ./Seasoned.Backend/Seasoned.Backend.csproj
COPY . .

RUN dotnet test ./Seasoned.Tests/Seasoned.Tests.csproj --configuration Release --no-restore
# Copy Nuxt static files into .NET wwwroot
COPY --from=frontend-build /src/frontend/.output/public ./Seasoned.Backend/wwwroot

RUN dotnet publish ./Seasoned.Backend/Seasoned.Backend.csproj -c Release -o /app

# Stage 3: Final Runtime (Debian Slim)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim
WORKDIR /app
COPY --from=backend-build /app .

# Essential for PGVector and Gemini logic to run smoothly
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Seasoned.Backend.dll"]