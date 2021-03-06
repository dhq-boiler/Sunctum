﻿

using Homura.ORM.Migration;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Version_2 : ChangePlanByVersion<Version_2>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_DirectoryNameParser_VersionOrigin();
            }
        }
    }
}
