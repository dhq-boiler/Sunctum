using simpleqb.Core;
using simpleqb.Dml.Syntaxes;
using System;
using System.Reflection;

namespace simpleqb.SQLite.Dml.Syntaxes
{
    internal class InsertOrReplaceSyntax : SyntaxBase, IInsertOrReplaceSyntax
    {
        public InsertOrReplaceSyntax()
        { }

        internal InsertOrReplaceSyntax(SyntaxBase syntax)
            : base(syntax)
        { }

        public IIntoSyntax Into
        {
            get
            {
                var assembly = Assembly.GetAssembly(typeof(ISyntaxBase));
                var type = assembly.GetType("simpleqb.Dml.Syntaxes.IntoSyntax");
                return (IIntoSyntax)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { this }, null);
            }
        }

        public override string Represent()
        {
            return "INSERT OR REPLACE";
        }
    }
}
