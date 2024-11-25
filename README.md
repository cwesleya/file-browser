# File & Directory Browser

## Overview

This application consists of a (1) web service API that allows users to query the contents of a directory on the web server and (2) a single-page web app (SPA) that can be used to search and browse folders and files. 

## Features

- **Browse and Search Files & Folders**: Users can browse and search files and folders on the server.
- **Deep-Linkable URL Pattern**: URL reflects the state of the UI. 
- **SPA (Single Page App)**: The application is a single-page app using plain JavaScript that renders html.
- **File Upload**: Users can upload files from the browser.
- **File and Folder Counts and Sizes**: The current view shows file and folder counts and sizes.
- **Delete Files and Folders**: Users can delete files and folders.
- **Configurable Home Directory**: The home directory is configurable via environment variables or configuration files.
- **Performance Enhancements**: `EnumerateDirectories` and `EnumerateFiles` are used instead of `GetDirectories` and `GetFiles`. These methods return an `IEnumerable<string>` that allows for lazy evaluation, meaning entries are processed one at a time rather than all at once. This can help reduce memory usage and improve performance when dealing with large directories. Also, Pagination is used to limit the number of directories and files returned in each request.

## Endpoints

### Browse Directory

**URL**: `/api/test/browse`

**Method**: `GET`

**Query Parameters**:
- `path` (string, optional): The path of the directory to browse.
- `page` (int, optional): The page number for pagination.
- `pageSize` (int, optional): The number of items per page.

**Response**: JSON object containing directories and files.

**Example**:
```sh
curl -X GET "http://localhost:5000/api/test/browse?path=&page=1&pageSize=5"
```

### Search Files

**URL**: `/api/test/search`

**Method**: `GET`

**Query Parameters**:
- `query` (string, optional):  The search query.
- `page` (int, optional): The page number for pagination.
- `pageSize` (int, optional): The number of items per page.

**Response**: JSON object containing directories and files.

**Example**:
```sh
curl -X GET "http://localhost:5000/api/test/search?query=test&page=1&pageSize=5"
```

### Upload File

**URL**: `/api/test/upload`

**Method**: `POST`

**Query Parameters**:
- `path` (string, optional): The path to upload the file to. If no path is specified it will upload to the home directory.

**Form Data**:
- file (file): The file to upload.

**Response**: Success or error message.

**Example**:
```sh
curl -X POST "http://localhost:5000/api/test/upload?path=" -F "file=@/path/to/your/file.txt"
```

### Delete File or Directory

**URL**: `/api/test/delete`

**Method**: `DELETE`

**Query Parameters**:
- `name` (string): The name of the file or directory.
- `isDirectory` (bool, optional): Whether the item is a directory.

**Response**: Success or error message.

**Example**:
```sh
curl -X DELETE "http://localhost:5000/api/test/delete?name=file.txt&isDirectory=false"
```

### Get Home Directory

**URL**: `/api/test/home-directory`

**Method**: `GET`

**Response**: The currently configured home directory path and whether it exists.

**Example**:
```sh
curl -X GET "http://localhost:5000/api/test/home-directory"
```

## Configuration

**Configuring Home Directory**

The home directory can be configured via environment variables or configuration files.

`appsettings.json`
```json
{
  "HomeDirectory": "/app"
}
```
`launchSettings.json`
```json
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "HomeDirectory": "/app"
      }
    }
  }
}
```

You can also make new profiles and target them with `dotnet run --launch-profile <profile name>`.

## Running the Application

**NOTE**: You may need to use `sudo` (or run in Admin mode for Windows) to utilize the Upload functionality.

**Visual Studio**
1. Open the project in Visual Studio.
2. Select the profile you want to use from the dropdown next to the Run button (e.g., `Development`).
3. Click the Run button (or press `F5`) to start the application.

**Command Line**
1. Navigate to the root project directory.
2. Build the appliaction:

```sh
dotnet build
```

3. Set the environment variables in command line (linux shell example) or by specifying the `launchSettings.json` profile, then run the application:

```sh
export HomeDirectory=~/app
export ASPNETCORE_ENVIRONMENT=Development
dotnet run

```
or
```sh
dotnet run --launch-profile development
```

**Docker**

1. Navigate to the directory containing the Dockerfile.
2. Build the Docker image:
```sh
docker build -t filebrowser .
```
3. Run the Docker container. This will work because the launch profile defaults to development:
```sh
docker run -p 8080:80 filebrowser
```
4. You can pass in your own environment variables to docker or specify other profiles like this:
```sh
# Configure by environment variables
docker run -p 8080:80 -e HomeDirectory=/custom/path -e ASPNETCORE_ENVIRONMENT=Production filebrowser
# Configure by profile variable
docker run -p 8080:80 -e -e LAUNCH_PROFILE=Production filebrowser
```
5. Access the application at http://localhost:8080.

**Cleaning Up Docker**
To clean up Docker resources after running the application:

1. Stop and remove the running container:
```sh
docker ps -a  # List all containers
docker stop <container_id_or_name>  # Stop the container
docker rm <container_id_or_name>  # Remove the container
```
2. Remove the Docker image:
```sh
docker images  # List all images
docker rmi <image_id_or_name>  # Remove the image
```
