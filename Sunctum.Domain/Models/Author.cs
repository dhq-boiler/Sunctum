

using Homura.ORM.Mapping;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(VersionOrigin))]
    public class Author : PkIdEntity
    {
        private string _Name;

        public Author()
        { }

        public Author(Guid guid, string name)
        {
            ID = guid;
            Name = name;
        }

        [Column("Name", "TEXT", 1), NotNull]
        [Since(typeof(VersionOrigin))]
        public string Name
        {
            [DebuggerStepThrough]
            get
            { return _Name; }
            set
            {
                SetProperty(ref _Name, value);
            }
        }
    }
}
