

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;

namespace Sunctum.Domain.Models
{
    [PrimaryKey("BookID", "Channel", "ValueOrder")]
    [Index("BookID", "Channel", "ValueOrder")]
    [DefaultVersion(typeof(VersionOrigin))]
    public class ColorMap : EntityBaseObject
    {
        public ColorMap()
        { }

        public ColorMap(Guid bookID, int channel, int valueOrder, int value, Guid imageID, int maxX, int maxY)
        {
            BookID = bookID;
            Channel = channel;
            ValueOrder = valueOrder;
            Value = value;
            ImageID = imageID;
            MaxX = maxX;
            MaxY = maxY;
        }

        [Column("BookID", "NUMERIC", 0)]
        [Since(typeof(VersionOrigin))]
        public Guid BookID { get; set; }

        [Column("Channel", "INTEGER", 1)]
        [Since(typeof(VersionOrigin))]
        public int Channel { get; set; }

        [Column("ValueOrder", "INTEGER", 2)]
        [Since(typeof(VersionOrigin))]
        public int ValueOrder { get; set; }

        [Column("Value", "INTEGER", 3)]
        [Since(typeof(VersionOrigin))]
        public int Value { get; set; }

        [Column("ImageID", "NUMERIC", 4), NotNull]
        [Since(typeof(VersionOrigin))]
        public Guid ImageID { get; set; }

        [Column("MaxX", "INTEGER", 5), NotNull]
        [Since(typeof(VersionOrigin))]
        public int MaxX { get; set; }

        [Column("MaxY", "INTEGER", 6), NotNull]
        [Since(typeof(VersionOrigin))]
        public int MaxY { get; set; }
    }
}
