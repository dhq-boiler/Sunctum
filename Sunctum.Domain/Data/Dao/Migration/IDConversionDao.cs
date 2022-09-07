

using Homura.Extensions;
using Sunctum.Domain.Data.Entity.Migration;
using System.Data;

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
                DomesticID = reader.SafeGetGuid("DomesticID", Table),
                ForeignID = reader.SafeGetGuid("ForeignID", Table),
            };
        }
    }
}
