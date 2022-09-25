

using Homura.ORM.Mapping;
using Sunctum.Domain.Models.Conversion;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(Version_1))]
    public class Author : PkIdEntity
    {
        private string _Name;
        private bool _NameIsEncrypted;

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

        [Column("NameIsEncrypted", "INTEGER", 2, Homura.ORM.HandlingDefaultValue.AsValue, false), NotNull]
        [Since(typeof(Version_1))]
        public bool NameIsEncrypted
        {
            [DebuggerStepThrough]
            get
            { return _NameIsEncrypted; }
            set
            {
                SetProperty(ref _NameIsEncrypted, value);
            }
        }
    }
}
