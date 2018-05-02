﻿using simpleqb.Iso.Dml.Syntaxes;
using simpleqb.Vendor.SQLite.Dml.Syntaxes;

namespace simpleqb.Vendor.SQLite.Dml
{
    public class InsertOrReplace : IInsertOrReplaceSyntax
    {
        private InsertOrReplaceSyntax syntax = new InsertOrReplaceSyntax();

        public InsertOrReplace()
        { }

        public IIntoSyntax Into { get { return syntax.Into; } }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    syntax.Dispose();
                    syntax = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
