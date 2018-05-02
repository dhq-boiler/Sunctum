

using System.Collections.ObjectModel;

namespace Sunctum.Domain.Logic.Parse
{
    public interface IDirectoryNameParserManager
    {
        IDirectoryNameParser Get(string directoryName);

        ObservableCollection<DirectoryNameParser> Items { get; }

        void Save();

        void Load();
    }
}
