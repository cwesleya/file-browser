# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official ASP.NET Core build image as a parent image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FileBrowser.csproj", "."]
RUN dotnet restore "FileBrowser.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "FileBrowser.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FileBrowser.csproj" -c Release -o /app/publish

# Use the runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set default environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV HomeDirectory=/app

# Set the entrypoint to use the specified launch profile if provided
ENTRYPOINT ["dotnet", "FileBrowser.dll", "--launch-profile", "${LAUNCH_PROFILE:-Development}"]
