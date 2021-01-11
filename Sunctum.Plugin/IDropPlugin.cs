

using System.Windows;

namespace Sunctum.Plugin
{
    public interface IDropPlugin
    {
        string AcceptableDataFormat { get; }

        void Execute(IDataObject dataObject);
    }
}
