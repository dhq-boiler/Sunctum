

using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    public class ChangePlan_Version_7 : ChangePlan<Version_7>
    {
        public ChangePlan_Version_7(VersioningMode mode) : base(mode)
        {
        }

        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_Author_Version_1(Mode);
                yield return new ChangePlan_Book_Version_3(Mode);
                yield return new ChangePlan_Page_Version_1(Mode);
                yield return new ChangePlan_Image_Version_3(Mode);
            }
        }
    }
}
