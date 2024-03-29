﻿
namespace Sunctum.Domain.Models
{
    public class Specifications
    {
        public static string[] SupportedImageType
        {
            get
            {
                return new string[] {
                    ".bmp",
                    ".jpg",
                    ".jpeg",
                    ".jpe",
                    ".jfif",
                    ".gif",
                    ".tif",
                    ".tiff",
                    ".png"
                };
            }
        }

        public static readonly string APPCONFIG_ID = "AppConfiguration";
        public static readonly string APPCONFIG_FILENAME = "appconfig.yml";
        public static readonly string APP_DB_ID = "AppDB";
        public static readonly string APP_DB_FILENAME = "app.db";
        public static readonly string VC_DB_FILENAME = "vc.db";
        public static readonly string WORKSPACE_DB_ID = "WorkspaceDB";
        public static readonly string WORKSPACE_DB_FILENAME = "library.db";
        public static readonly string MASTER_DIRECTORY = "data";
        public static readonly string CACHE_DIRECTORY = "cache";
        public static readonly string APP_LAYOUT_CONFIG_FILENAME = "app.layout.config";
        public static readonly string LOCK_ICON_FILE = "Assets\\lock.png";

        public static string GenerateAbsoluteLibraryDbFilename(string workingDirectory)
        {
            return $"{workingDirectory}\\{WORKSPACE_DB_FILENAME}";
        }

        public static string GenerateConnectionString(string workingDirectory)
        {
            return $"Data Source={GenerateAbsoluteLibraryDbFilename(workingDirectory)};Enlist=N";
        }

        public static readonly int VERTICAL_SEGMENT_COUNT = 3;
        public static readonly int HORIZONTAL_SEGMENT_COUNT = 3;

        public static readonly string SCRAPPED_NEW_BOOK_TITLE = "新しいブック";
        public static readonly int PAGEINDEX_FIRSTPAGE = 1;
    }
}
