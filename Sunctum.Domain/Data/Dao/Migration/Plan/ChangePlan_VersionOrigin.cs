﻿

using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_VersionOrigin : ChangePlanByVersion<VersionOrigin>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_Author_VersionOrigin();
                yield return new ChangePlan_Book_VersionOrigin();
                yield return new ChangePlan_ColorMap_VersionOrigin();
                yield return new ChangePlan_Image_VersionOrigin();
                yield return new ChangePlan_ImageTag_VersionOrigin();
                yield return new ChangePlan_Page_VersionOrigin();
                yield return new ChangePlan_Tag_VersionOrigin();
                yield return new ChangePlan_Thumbnail_VersionOrigin();
            }
        }
    }
}
