

using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System.Collections.Generic;
using static Sunctum.Infrastructure.Core.Delegate;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration
{
    public interface IVersionChangePlan : IModifiedCounter
    {
        VersionOrigin TargetVersion { get; }

        IEnumerable<IEntityVersionChangePlan> VersionChangePlanList { get; }

        void AddVersionChangePlan(IEntityVersionChangePlan plan);

        void RemoveVersionChangePlan(IEntityVersionChangePlan plan);

        void UpgradeToTargetVersion(IConnection connection);

        void DowngradeToTargetVersion(IConnection connection);

        event BeginToUpgradeToEventHandler BeginToUpgradeTo;
        event FinishedToUpgradeToEventHandler FinishedToUpgradeTo;
        event BeginToDowngradeToEventHandler BeginToDowngradeTo;
        event FinishedToDowngradeToEventHandler FinishedToDowngradeTo;
    }
}
