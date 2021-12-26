using Sunctum.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfLibrary1
{
    public class Class2 : IDropPlugin
    {
        public string AcceptableDataFormat => string.Empty;

        public void Execute(IDataObject dataObject)
        {
        }
    }
}
