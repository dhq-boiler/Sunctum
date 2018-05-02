

using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration
{
    public class VersionChangeEventArgs : EventArgs
    {
        private readonly VersionOrigin _version;

        public VersionChangeEventArgs(VersionOrigin version)
        {
            _version = version;
        }

        public VersionOrigin Version { get { return _version; } }
    }
}
