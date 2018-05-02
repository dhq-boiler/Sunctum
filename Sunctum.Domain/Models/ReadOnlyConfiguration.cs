
namespace Sunctum.Domain.Models
{
    public class ReadOnlyConfiguration : Configuration
    {
        private string _WorkingDirectory;
        private string _ConnectionString;
        private bool _ThumbnailParallelGeneration;
        private bool _LockFileInImporting;
        private bool _StoreWindowPosition;

        [ConfigurationData]
        public new string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            private set { SetProperty(ref _WorkingDirectory, value); }
        }

        [ConfigurationData]
        public new string ConnectionString
        {
            get { return _ConnectionString; }
            private set { SetProperty(ref _ConnectionString, value); }
        }

        [ConfigurationData]
        public new bool ThumbnailParallelGeneration
        {
            get { return _ThumbnailParallelGeneration; }
            private set { SetProperty(ref _ThumbnailParallelGeneration, value); }
        }

        [ConfigurationData]
        public new bool LockFileInImporting
        {
            get { return _LockFileInImporting; }
            private set { SetProperty(ref _LockFileInImporting, value); }
        }

        [ConfigurationData]
        public new bool StoreWindowPosition
        {
            get { return _StoreWindowPosition; }
            private set { SetProperty(ref _StoreWindowPosition, value); }
        }

        public ReadOnlyConfiguration(Configuration configuration)
        {
            CopyConfigurationData(configuration, this);
        }
    }
}
