﻿

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Test.TestFixture.Migration;
using System;

namespace Sunctum.Infrastructure.Test.TestFixture.Entity
{
    public class AnotherEntity : EntityBaseObject
    {
        [Column("Id", "NUMERIC", 0), PrimaryKey, Index]
        public Guid Id { get; set; }

        [Column("Item1", "TEXT", 1)]
        public string Item1 { get; set; }

        [Column("Item2", "TEXT", 2)]
        [Since(typeof(VersionOrigin))]
        public string Item2 { get; set; }

        [Column("Item3", "TEXT", 3)]
        [Since(typeof(Version_1))]
        public string Item3 { get; set; }

        [Column("Item4", "TEXT", 3)]
        [Since(typeof(Version_2))]
        public string Item4 { get; set; }
    }
}
