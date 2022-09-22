

using Homura.ORM;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sunctum.Domain.Logic.Import
{
    public abstract class Importer
    {
        public int Count { get; internal set; }
        public int Processed { get; protected set; }
        public string Name { get; protected set; }
        public string Path { get; private set; }
        public long Size { get; set; }
        public string FingerPrint { get; set; }

        protected Importer()
        { }

        protected Importer(string path)
        {
            Path = path;
        }

        public static Importer CreateInstance(string objectPath, ILibrary library)
        {
            if (File.Exists(objectPath))
                return new ImportPage(objectPath);
            else if (Directory.Exists(objectPath))
                return new ImportBook(objectPath, library);
            else
                throw new IOException($"{objectPath} doesn't exist.");
        }

        public static Importer CreateInstance(string[] objectPaths)
        {
            return new ImportIllust(objectPaths);
        }

        public abstract void Estimate();

        public abstract IEnumerable<System.Threading.Tasks.Task> GenerateTasks(ILibrary library, string copyTo, string entryName, DataOperationUnit dataOpUnit, Action<Importer, BookViewModel> progressUpdatingAction);
    }
}
