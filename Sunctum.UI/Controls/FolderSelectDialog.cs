
/*
 * .NET Win 7-style folder select dialog
 * https://www.lyquidity.com/devblog/?p=136
 * Author Bill https://www.lyquidity.com/devblog/?author=1
 */

using Sunctum.UI.Core;
using System;
using System.Windows.Forms;

// ------------------------------------------------------------------
// Wraps System.Windows.Forms.OpenFileDialog to make it present
// a vista-style dialog.
// ------------------------------------------------------------------

namespace Sunctum.UI.Controls
{
    /// <summary>
    /// Wraps System.Windows.Forms.OpenFileDialog to make it present
    /// a vista-style dialog.
    /// </summary>
    [Obsolete]
    public class FolderSelectDialog
    {
        // Wrapped dialog
        private System.Windows.Forms.OpenFileDialog _ofd = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FolderSelectDialog()
        {
            _ofd = new System.Windows.Forms.OpenFileDialog();

            _ofd.Filter = "Folders|\n";
            _ofd.AddExtension = false;
            _ofd.CheckFileExists = false;
            _ofd.DereferenceLinks = true;
            _ofd.Multiselect = false;
        }

        #region Properties

        /// <summary>
        /// Gets/Sets the initial folder to be selected. A null value selects the current directory.
        /// </summary>
        public string InitialDirectory
        {
            get { return _ofd.InitialDirectory; }
            set { _ofd.InitialDirectory = value == null || value.Length == 0 ? Environment.CurrentDirectory : value; }
        }

        /// <summary>
        /// Gets/Sets the title to show in the dialog
        /// </summary>
        public string Title
        {
            get { return _ofd.Title; }
            set { _ofd.Title = value == null ? "Select a folder" : value; }
        }

        /// <summary>
        /// Gets the selected folder
        /// </summary>
        public string FileName
        {
            get { return _ofd.FileName; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the dialog
        /// </summary>
        /// <returns>True if the user presses OK else false</returns>
        public bool ShowDialog()
        {
            return ShowDialog(IntPtr.Zero);
        }

        /// <summary>
        /// Shows the dialog
        /// </summary>
        /// <param name="hWndOwner">Handle of the control to be parent</param>
        /// <returns>True if the user presses OK else false</returns>
        public bool ShowDialog(IntPtr hWndOwner)
        {
            bool flag = false;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                var r = new Reflector("System.Windows.Forms");

                uint num = 0;
                Type typeIFileDialog = r.GetType("FileDialogNative.IFileDialog");
                object dialog = r.Call(_ofd, "CreateVistaDialog");
                r.Call(_ofd, "OnBeforeVistaDialog", dialog);

                uint options = (uint)r.CallAs(typeof(System.Windows.Forms.FileDialog), _ofd, "GetOptions");
                options |= (uint)r.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
                r.CallAs(typeIFileDialog, dialog, "SetOptions", options);

                object pfde = r.New("FileDialog.VistaDialogEvents", _ofd);
                object[] parameters = new object[] { pfde, num };
                r.CallAs2(typeIFileDialog, dialog, "Advise", parameters);
                num = (uint)parameters[1];
                try
                {
                    int num2 = (int)r.CallAs(typeIFileDialog, dialog, "Show", hWndOwner);
                    flag = 0 == num2;
                }
                finally
                {
                    r.CallAs(typeIFileDialog, dialog, "Unadvise", num);
                    GC.KeepAlive(pfde);
                }
            }
            else
            {
                var fbd = new FolderBrowserDialog();
                fbd.Description = this.Title;
                fbd.SelectedPath = this.InitialDirectory;
                fbd.ShowNewFolderButton = false;
                if (fbd.ShowDialog(new WindowWrapper(hWndOwner)) != DialogResult.OK) return false;
                _ofd.FileName = fbd.SelectedPath;
                flag = true;
            }

            return flag;
        }

        #endregion
    }

    /// <summary>
    /// Creates IWin32Window around an IntPtr
    /// </summary>
    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handle">Handle to wrap</param>
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        /// <summary>
        /// Original ptr
        /// </summary>
        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }
}
