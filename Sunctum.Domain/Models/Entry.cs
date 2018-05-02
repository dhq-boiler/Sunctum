

using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    public abstract class Entry : PkIdEntity
    {
        protected Entry()
        { }

        protected Entry(Guid id, string title)
            : this()
        {
            ID = id;
            Title = title;
        }

        private string _Title;

        [Column("Title", "TEXT", 1), NotNull]
        [Since(typeof(VersionOrigin))]
        public string Title
        {
            [DebuggerStepThrough]
            get
            { return _Title; }
            set
            {
                SetProperty(ref _Title, value);
            }
        }
    }
}
