

using System.Data;
using Sunctum.Domain.Data.Entity.Migration;
using Sunctum.Domain.Util;

namespace Sunctum.Domain.Data.Dao.Migration
{
    internal class IDConversionDao : SQLiteBaseDao<IDConversion>
    {
        public IDConversionDao() : base()
        { }

        protected override IDConversion ToEntity(IDataRecord reader)
        {
            return new IDConversion()
            {
                DomesticID = reader.SafeGetGuid("DomesticID"),
                ForeignID = reader.SafeGetGuid("ForeignID"),
            };
        }
    }
}
