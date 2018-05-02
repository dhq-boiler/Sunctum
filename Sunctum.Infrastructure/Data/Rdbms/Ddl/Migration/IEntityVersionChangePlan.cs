

using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration
{
    public interface IEntityVersionChangePlan : IModifiedCounter
    {
        VersionOrigin TargetVersion { get; set; }

        void UpgradeToTargetVersion(IConnection connection);

        void DowngradeToTargetVersion(IConnection connection);

        Type TargetEntityType { get; set; }

        void CreateTable(IConnection connection);

        void DropTable(IConnection connection);
    }
}
