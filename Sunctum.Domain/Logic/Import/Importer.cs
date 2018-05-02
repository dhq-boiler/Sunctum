

using Sunctum.Domain.Models.Managers;
using Sunctum.Infrastructure.Data.Rdbms;
using System.Collections.Generic;
using System.IO;

namespace Sunctum.Domain.Logic.Import
{
    public abstract class Importer
    {
        public int Count { get; protected set; }
        public int Processed { get; protected set; }
        public string Name { get; protected set; }
        public string Path { get; private set; }

        protected Importer()
        { }

        protected Importer(string path)
        {
            Path = path;
        }

        public static Importer CreateInstance(string objectPath)
        {
            if (File.Exists(objectPath))
                return new ImportPage(objectPath);
            else if (Directory.Exists(objectPath))
                return new ImportBook(objectPath);
            else
                throw new IOException($"{objectPath} doesn't exist.");
        }

        public static Importer CreateInstance(string[] objectPaths)
        {
            return new ImportIllust(objectPaths);
        }

        public abstract void Estimate();

        public abstract IEnumerable<System.Threading.Tasks.Task> GenerateTasks(ILibraryManager library, string copyTo, string entryName, DataOperationUnit dataOpUnit);
    }
}
