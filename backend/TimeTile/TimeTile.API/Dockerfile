# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln ./

# Copy project files
COPY TimeTile.API/*.csproj ./TimeTile.API/
COPY TimeTile.Core/*.csproj ./TimeTile.Core/
COPY TimeTile.Storage/*.csproj ./TimeTile.Storage/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/TimeTile.API
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl and SkiaSharp dependencies
RUN apt-get update && apt-get install -y \
    curl \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    libxcb1 \
    libxext6 \
    libxrender1 \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*


# Copy the published application
COPY --from=publish /app/publish .

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "TimeTile.API.dll"]