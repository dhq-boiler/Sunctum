

using Homura.ORM;
using Homura.ORM.Mapping;
using System;

namespace Sunctum.Domain.Data.Entity.Migration
{
    public class IDConversion : EntityBaseObject
    {
        public IDConversion()
        { }

        public IDConversion(string tableName, Guid domesticID, Guid foreignID)
        {
            TableName = tableName;
            DomesticID = domesticID;
            ForeignID = foreignID;
        }

        [Column("TableName", "TEXT", 0)]
        public string TableName { get; set; }

        [Column("DomesticID", "NUMERIC", 1)]
        public Guid DomesticID { get; set; }

        [Column("ForeignID", "NUMERIC", 2)]
        public Guid ForeignID { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            var other = obj as IDConversion;
            return TableName.Equals(other.TableName)
                && DomesticID.Equals(other.DomesticID)
                && ForeignID.Equals(other.ForeignID);
        }

        public override int GetHashCode()
        {
            return TableName.GetHashCode()
                ^ DomesticID.GetHashCode()
                ^ ForeignID.GetHashCode();
        }
    }
}
