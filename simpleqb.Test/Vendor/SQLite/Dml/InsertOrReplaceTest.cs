using NUnit.Framework;
using simpleqb.Vendor.SQLite.Dml;
using System.Collections.Generic;

namespace simpleqb.Test.Vendor.SQLite.Dml
{
    [TestFixture]
    public class InsertOrReplaceTest
    {
        [Category("simpleqb QueryBuilder")]
        public class BasicTest
        {

            [Test]
            public void InsertOrReplace_Into_Table_Values_Value1()
            {
                using (var query = new InsertOrReplace().Into.Table("Table").Values.Value("Value1"))
                {
                    Assert.That(query.ToSql(), Is.EqualTo("INSERT OR REPLACE INTO Table VALUES (@val_0)"));
                    Assert.That(query, Has.Property("Parameters").One.EqualTo(new KeyValuePair<string, object>("@val_0", "Value1")));
                }
            }
        }
    }
}
