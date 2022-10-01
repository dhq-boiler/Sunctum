using Homura.Extensions;
using Homura.ORM;
using Sunctum.Domain.Models;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class KeyValueDao : Dao<KeyValue>
    {
        public KeyValueDao() : base()
        { }

        public KeyValueDao(Type entityVersionType) : base(entityVersionType)
        { }

        protected override KeyValue ToEntity(IDataRecord reader)
        {
            return new KeyValue()
            {
                Key = reader.SafeGetString("Key", Table),
                Value = reader.SafeGetString("Value", Table),
            };
        }
    }
}
