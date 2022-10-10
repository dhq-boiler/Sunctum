

using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_VersionOrigin : ChangePlan<VersionOrigin>
    {
        public ChangePlan_VersionOrigin(VersioningMode mode) : base(mode)
        {
        }

        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_Author_VersionOrigin(Mode);
                yield return new ChangePlan_Book_VersionOrigin(Mode);
                yield return new ChangePlan_ColorMap_VersionOrigin(Mode);
                yield return new ChangePlan_Image_VersionOrigin(Mode);
                yield return new ChangePlan_ImageTag_VersionOrigin(Mode);
                yield return new ChangePlan_Page_VersionOrigin(Mode);
                yield return new ChangePlan_Tag_VersionOrigin(Mode);
                yield return new ChangePlan_Thumbnail_VersionOrigin(Mode);
            }
        }
    }
}
