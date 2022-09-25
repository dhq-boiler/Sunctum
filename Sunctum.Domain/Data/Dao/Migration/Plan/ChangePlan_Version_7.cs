

using Homura.ORM.Migration;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Version_7 : ChangePlanByVersion<Version_7>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_Author_Version_1();
                yield return new ChangePlan_Book_Version_3();
                yield return new ChangePlan_Page_Version_1();
                yield return new ChangePlan_Image_Version_3();
            }
        }
    }
}
