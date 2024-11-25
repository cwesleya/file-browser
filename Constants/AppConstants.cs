namespace FileBrowser.Constants
{
    public static class AppConstants
    {
        // Default file directory key (matches appsettings.json key)
        public const string DefaultRootDirectoryKey = "HomeDirectory";

        // API Endpoints
        public const string ApiRouteBase = "api/test";
        public const string BrowseEndpoint = "browse";
        public const string SearchEndpoint = "search";
        public const string UploadEndpoint = "upload";
        public const string DeleteEndpoint = "delete";
        public const string DownloadEndpoint = "download";
        public const string StatsEndpoint = "stats";
        public const string HomeDirectoryEndpoint = "home-directory";
        // Response Messages
        public const string DirectoryNotFound = "Directory not found.";
        public const string FileNotFound = "File not found.";
        public const string FileUploadedSuccessfully = "File uploaded successfully!";
        public const string DirectoryDeleted = "Directory deleted successfully.";
        public const string FileDeleted = "File deleted successfully.";
        public const string NoFileUploaded = "No file uploaded.";
        public const string UnauthorizedAccess = "Unauthorized access to the specified path.";
        public const string InternalServerError = "Internal server error: ";
        
        // Miscellaneous
        public const string DefaultContentType = "application/octet-stream";
        public const int DefaultPageSize = 25;
    }
}