

using NUnit.Framework;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using Sunctum.Infrastructure.Data.Setup;
using Sunctum.Infrastructure.Test.TestFixture.Entity;
using Sunctum.Infrastructure.Test.TestFixture.Migration;
using Sunctum.Infrastructure.Test.TestFixture.Migration.Plan;
using System;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Test.UnitTest.Data.Setup
{
    [Category("Infrastructure")]
    [Category("UnitTest")]
    [TestFixture]
    public class UpgradeHelperTest
    {
        private static readonly Type s_upgradeHelperType = Type.GetType("Sunctum.Infrastructure.Data.Setup.UpgradeHelper, Sunctum.Infrastructure");

        [Test, TestCaseSource(typeof(UpgradeHelperTest), "TestCaseSource_EnumDefinedEntities")]
        public IEnumerable<Type> EnumDefinedEntities(Dictionary<EntityVersionKey, IEntityVersionChangePlan> planMap)
        {
            var method = s_upgradeHelperType.GetMethod("EnumDefinedEntities");
            return (IEnumerable<Type>)method.Invoke("STATIC", new object[] { planMap });
        }

        public static IEnumerable<TestCaseData> TestCaseSource_EnumDefinedEntities
        {
            get
            {
                var map0 = new Dictionary<EntityVersionKey, IEntityVersionChangePlan>();
                yield return new TestCaseData(map0).Returns(new Type[] { }).SetName("EnumDefinedEntities[0]");

                var map1 = new Dictionary<EntityVersionKey, IEntityVersionChangePlan>();
                map1.Add(new EntityVersionKey(typeof(Header), new VersionOrigin()), new HeaderChangePlan_VersionOrigin());
                map1.Add(new EntityVersionKey(typeof(Detail), new VersionOrigin()), new DetailChangePlan_VersionOrigin());
                yield return new TestCaseData(map1).Returns(new Type[] { typeof(Header), typeof(Detail) }).SetName("EnumDefinedEntities[1]");

                var map2 = new Dictionary<EntityVersionKey, IEntityVersionChangePlan>();
                map2.Add(new EntityVersionKey(typeof(Header), new VersionOrigin()), new HeaderChangePlan_VersionOrigin());
                map2.Add(new EntityVersionKey(typeof(Detail), new VersionOrigin()), new DetailChangePlan_VersionOrigin());
                map2.Add(new EntityVersionKey(typeof(Header), new Version_1()), new HeaderChangePlan_Version_1());
                map2.Add(new EntityVersionKey(typeof(Detail), new Version_1()), new DetailChangePlan_Version_1());
                map2.Add(new EntityVersionKey(typeof(Detail), new Version_2()), new DetailChangePlan_Version_2());
                yield return new TestCaseData(map2).Returns(new Type[] { typeof(Header), typeof(Detail) }).SetName("EnumDefinedEntities[2]");
            }
        }

        [Test, TestCaseSource(typeof(UpgradeHelperTest), "TestCaseSource_EnumDefinedVersions")]
        public IEnumerable<VersionOrigin> EnumDefinedVersions(Dictionary<EntityVersionKey, IEntityVersionChangePlan> planMap)
        {
            var method = s_upgradeHelperType.GetMethod("EnumDefinedVersions");
            return (IEnumerable<VersionOrigin>)method.Invoke("STATIC", new object[] { planMap });
        }

        public static IEnumerable<TestCaseData> TestCaseSource_EnumDefinedVersions
        {
            get
            {
                var map0 = new Dictionary<EntityVersionKey, IEntityVersionChangePlan>();
                yield return new TestCaseData(map0).Returns(new VersionOrigin[] { }).SetName("EnumDefinedVersions[0]");

                var map1 = new Dictionary<EntityVersionKey, IEntityVersionChangePlan>();
                map1.Add(new EntityVersionKey(typeof(Header), new VersionOrigin()), new HeaderChangePlan_VersionOrigin());
                map1.Add(new EntityVersionKey(typeof(Detail), new VersionOrigin()), new DetailChangePlan_VersionOrigin());
                yield return new TestCaseData(map1).Returns(new VersionOrigin[] { new VersionOrigin() }).SetName("EnumDefinedVersions[1]");

                var map2 = new Dictionary<EntityVersionKey, IEntityVersionChangePlan>();
                map2.Add(new EntityVersionKey(typeof(Header), new VersionOrigin()), new HeaderChangePlan_VersionOrigin());
                map2.Add(new EntityVersionKey(typeof(Detail), new VersionOrigin()), new DetailChangePlan_VersionOrigin());
                map2.Add(new EntityVersionKey(typeof(Header), new Version_1()), new HeaderChangePlan_Version_1());
                map2.Add(new EntityVersionKey(typeof(Detail), new Version_1()), new DetailChangePlan_Version_1());
                map2.Add(new EntityVersionKey(typeof(Detail), new Version_2()), new DetailChangePlan_Version_2());
                yield return new TestCaseData(map2).Returns(new VersionOrigin[] { new VersionOrigin(), new Version_1(), new Version_2() }).SetName("EnumDefinedVersions[2]");
            }
        }
    }
}
