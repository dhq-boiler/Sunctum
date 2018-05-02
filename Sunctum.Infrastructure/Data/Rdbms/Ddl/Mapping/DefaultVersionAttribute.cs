

using System;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping
{
    /// <summary>
    /// Dao抽象クラスのデフォルトコンストラクタによってインスタンス化したサブクラスがアクセス対象とする，エンティティのバージョンを指定する
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultVersionAttribute : Attribute
    {
        public DefaultVersionAttribute(Type version)
        {
            Version = version;
        }

        public Type Version { get; set; }
    }
}
