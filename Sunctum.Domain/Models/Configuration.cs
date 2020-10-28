

using Homura.Core;
using NLog;
using Sunctum.Infrastructure.Data.Yaml;
using System;
using System.IO;
using System.Reflection;

namespace Sunctum.Domain.Models
{
    public class Configuration : BaseObject, ICloneable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private string _WorkingDirectory;
        private string _ConnectionString;
        private bool _ThumbnailParallelGeneration;
        private long _LowerLimitFileSizeThatImageMustBeDisplayAsThumbnail = 1024 * 1024 * 2; //10MB default
        private string _BookSorting;
        private bool _DisplayAuthorPane;
        private bool _DisplayTagPane;
        private bool _DisplayInformationPane;
        private bool _LockFileInImporting;
        private bool _StoreWindowPosition;
        private Rect _WindowRect;
        private int _ThumbnailQuality = 50;
        private int _LibraryHistoryRecordCount = 5;
        private int? _BookListViewItemWidth = 125;
        private int? _BookListViewItemImageHeight = 150;
        private int? _BookListViewItemAuthorHeight = 25;
        private int? _BookListViewItemTitleHeight = 25;
        private int? _BookListViewItemMarginLeft = 0;
        private int? _BookListViewItemMarginTop = 0;
        private int? _BookListViewItemMarginRight = 0;
        private int? _BookListViewItemMarginBottom = 0;
        private int? _ContentListViewItemWidth = 125;
        private int? _ContentListViewItemImageHeight = 150;
        private int? _ContentListViewItemTitleHeight = 25;
        private int? _ContentListViewItemMarginLeft = 0;
        private int? _ContentListViewItemMarginTop = 0;
        private int? _ContentListViewItemMarginRight = 0;
        private int? _ContentListViewItemMarginBottom = 0;
        private string _AuthorSorting;
        private string _TagSorting;

        public static Configuration ApplicationConfiguration { get; set; }

        #region Configuration Data

        [ConfigurationData]
        public string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            set { SetProperty(ref _WorkingDirectory, value); }
        }

        [ConfigurationData]
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { SetProperty(ref _ConnectionString, value); }
        }

        [ConfigurationData]
        public bool ThumbnailParallelGeneration
        {
            get { return _ThumbnailParallelGeneration; }
            set { SetProperty(ref _ThumbnailParallelGeneration, value); }
        }

        [ConfigurationData]
        public long LowerLimitFileSizeThatImageMustBeDisplayAsThumbnail
        {
            get { return _LowerLimitFileSizeThatImageMustBeDisplayAsThumbnail; }
            set { SetProperty(ref _LowerLimitFileSizeThatImageMustBeDisplayAsThumbnail, value); }
        }

        [ConfigurationData]
        public string BookSorting
        {
            get { return _BookSorting; }
            set { SetProperty(ref _BookSorting, value); }
        }

        [ConfigurationData]
        public bool DisplayAuthorPane
        {
            get { return _DisplayAuthorPane; }
            set { SetProperty(ref _DisplayAuthorPane, value); }
        }

        [ConfigurationData]
        public bool DisplayTagPane
        {
            get { return _DisplayTagPane; }
            set { SetProperty(ref _DisplayTagPane, value); }
        }

        [ConfigurationData]
        public bool DisplayInformationPane
        {
            get { return _DisplayInformationPane; }
            set { SetProperty(ref _DisplayInformationPane, value); }
        }

        [ConfigurationData(defaultValue: false)]
        public bool LockFileInImporting
        {
            get { return _LockFileInImporting; }
            set { SetProperty(ref _LockFileInImporting, value); }
        }

        [ConfigurationData]
        public bool StoreWindowPosition
        {
            get { return _StoreWindowPosition; }
            set { SetProperty(ref _StoreWindowPosition, value); }
        }

        [ConfigurationData]
        public Rect WindowRect
        {
            get { return _WindowRect; }
            set { SetProperty(ref _WindowRect, value); }
        }

        [ConfigurationData]
        public int ThumbnailQuality
        {
            get { return _ThumbnailQuality; }
            set { SetProperty(ref _ThumbnailQuality, value); }
        }

        [ConfigurationData]
        public int LibraryHistoryRecordCount
        {
            get { return _LibraryHistoryRecordCount; }
            set { SetProperty(ref _LibraryHistoryRecordCount, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemWidth
        {
            get { return _BookListViewItemWidth; }
            set { SetProperty(ref _BookListViewItemWidth, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemImageHeight
        {
            get { return _BookListViewItemImageHeight; }
            set { SetProperty(ref _BookListViewItemImageHeight, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemAuthorHeight
        {
            get { return _BookListViewItemAuthorHeight; }
            set { SetProperty(ref _BookListViewItemAuthorHeight, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemTitleHeight
        {
            get { return _BookListViewItemTitleHeight; }
            set { SetProperty(ref _BookListViewItemTitleHeight, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemMarginLeft
        {
            get { return _BookListViewItemMarginLeft; }
            set { SetProperty(ref _BookListViewItemMarginLeft, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemMarginTop
        {
            get { return _BookListViewItemMarginTop; }
            set { SetProperty(ref _BookListViewItemMarginTop, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemMarginRight
        {
            get { return _BookListViewItemMarginRight; }
            set { SetProperty(ref _BookListViewItemMarginRight, value); }
        }

        [ConfigurationData]
        public int? BookListViewItemMarginBottom
        {
            get { return _BookListViewItemMarginBottom; }
            set { SetProperty(ref _BookListViewItemMarginBottom, value); }
        }

        [ConfigurationData]
        public int? ContentListViewItemWidth
        {
            get { return _ContentListViewItemWidth; }
            set { SetProperty(ref _ContentListViewItemWidth, value); }
        }

        [ConfigurationData]
        public int? ContentListViewItemImageHeight
        {
            get { return _ContentListViewItemImageHeight; }
            set { SetProperty(ref _ContentListViewItemImageHeight, value); }
        }

        [ConfigurationData]
        public int? ContentListViewItemTitleHeight
        {
            get { return _ContentListViewItemTitleHeight; }
            set { SetProperty(ref _ContentListViewItemTitleHeight, value); }
        }

        [ConfigurationData]
        public int? ContentListViewItemMarginLeft
        {
            get { return _ContentListViewItemMarginLeft; }
            set { SetProperty(ref _ContentListViewItemMarginLeft, value); }
        }

        [ConfigurationData]
        public int? ContentListViewItemMarginTop
        {
            get { return _ContentListViewItemMarginTop; }
            set { SetProperty(ref _ContentListViewItemMarginTop, value); }
        }

        [ConfigurationData]
        public int? ContentListViewItemMarginRight
        {
            get { return _ContentListViewItemMarginRight; }
            set { SetProperty(ref _ContentListViewItemMarginRight, value); }
        }

        [ConfigurationData]
        public int? ContentListViewItemMarginBottom
        {
            get { return _ContentListViewItemMarginBottom; }
            set { SetProperty(ref _ContentListViewItemMarginBottom, value); }
        }

        [ConfigurationData]
        public string AuthorSorting
        {
            get { return _AuthorSorting; }
            set { SetProperty(ref _AuthorSorting, value); }
        }

        [ConfigurationData]
        public string TagSorting
        {
            get { return _TagSorting; }
            set { SetProperty(ref _TagSorting, value); }
        }

        #endregion //Configuration Data

        public Configuration()
        { }

        public static Configuration Load()
        {
            try
            {
                var config = YamlFile.Deserialize<Configuration>(Specifications.APPCONFIG_FILENAME);
                s_logger.Info("Configuration was loaded.");
                return config;
            }
            catch (Exception e)
            {
                s_logger.Error(e, "Failed to load configuration. Initialize configuration by default.");
                var config = new Configuration();
                Save(config);
                return config;
            }
        }

        public static void Save(Configuration obj)
        {
            try
            {
                YamlFile.Serialize(obj, Specifications.APPCONFIG_FILENAME);
                s_logger.Info("Saved configuration.");
            }
            catch (Exception e)
            {
                s_logger.Error(e, "Failed to save configuration.");
                throw;
            }
        }

        public void CreateWorkingDirectory(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
            catch (DirectoryNotFoundException)
            {
                var dirpath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                CreateWorkingDirectory(dirpath);
                WorkingDirectory = dirpath;
            }
        }

        public object Clone()
        {
            Configuration ret = new Configuration();
            CopyConfigurationData(ret);
            return ret;
        }

        protected void CopyConfigurationData(Configuration to)
        {
            VerifyArg(to, "argument 'to' must be not null");

            PropertyInfo[] fromPropertyInfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fromPropertyInfo in fromPropertyInfos)
            {
                var fromTarget = Attribute.GetCustomAttributes(fromPropertyInfo, typeof(ConfigurationDataAttribute));
                PropertyInfo toPropertyInfo = to.GetType().GetProperty(fromPropertyInfo.Name);

                if (fromTarget.Length > 0)
                {
                    if (toPropertyInfo != null)
                    {
                        toPropertyInfo.SetValue(to, fromPropertyInfo.GetValue(this));
                    }
                    else
                    {
                        s_logger.Error($"設定データのコピーに失敗しました. コピー元:{this.ToString()}, コピー先:{to.ToString()}");
                    }
                }
            }
        }

        public static void CopyConfigurationData(Configuration from, Configuration to)
        {
            from.CopyConfigurationData(to);
        }

        public ReadOnlyConfiguration ReadOnly()
        {
            return new ReadOnlyConfiguration(this);
        }
    }
}
