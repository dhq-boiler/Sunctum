using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models.Managers;
using System.Collections.ObjectModel;
using Unity;

namespace Sunctum.Domain.Logic.Parse
{
    public class DirectoryNameParserManager : IDirectoryNameParserManager
    {
        [Dependency]
        public IDataAccessManager DataAccessManager { get; set; }

        public ObservableCollection<DirectoryNameParser> Items { get; private set; } = new ObservableCollection<DirectoryNameParser>();

        public IDirectoryNameParser Get(string directoryName)
        {
            foreach (var item in Items)
            {
                if (item.Match(directoryName))
                {
                    return item;
                }
            }

            return new DefaultDirectoryNameParser();
        }

        public void Load()
        {
            var dao = new DirectoryNameParserDao();
            Items = new ObservableCollection<DirectoryNameParser>(dao.FindAll().ToService());
        }

        public void Save()
        {
            var dao = new DirectoryNameParserDao();
            dao.CurrentConnection = DataAccessManager.WorkingDao.CurrentConnection;

            dao.DeleteAll();

            foreach (var item in Items.ToEntity())
            {
                dao.Insert(item);
            }
        }
    }
}
