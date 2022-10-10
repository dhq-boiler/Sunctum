﻿

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_EncryptImage_VersionOrigin : ChangePlan<EncryptImage, VersionOrigin>
    {
        public ChangePlan_EncryptImage_VersionOrigin(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("EncryptImage", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override void CreateTable(IConnection connection)
        {
            EncryptImageDao dao = new EncryptImageDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            EncryptImageDao dao = new EncryptImageDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
