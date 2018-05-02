

using Sunctum.Infrastructure.Data.Rdbms;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Test.TestFixture
{
    internal class DummyImageTagTable : DummyAbstractTable
    {
        public override IEnumerable<IColumn> Columns
        {
            get
            {
                List<IColumn> list = new List<IColumn>();
                list.Add(new Column("ImageID", "datatype", null, 0, null));
                list.Add(new Column("TagID", "datatype", null, 1, null));
                return list;
            }
        }

        public override string Name
        {
            get
            {
                return "ImageTag";
            }
        }
    }
}
