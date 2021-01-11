

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sunctum.UI.Dialogs
{
    /// <summary>
    /// EncryptionStartingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EncryptionStartingDialog : Window
    {
        public string Password { get; set; }

        public EncryptionStartingDialog()
        {
            InitializeComponent();
            this.StartEncryption.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            string temp = "|NOMATCH|";
            List<PasswordBox> pb = new List<PasswordBox>(2) { };
            List<string> s = new List<string> { password.Password, passwordConfirm.Password };
            if (s[0] == s[1] && !string.IsNullOrWhiteSpace(s[0]))
            {
                temp = s[0];
                this.StartEncryption.IsEnabled = true;
            }
            else
            {
                this.StartEncryption.IsEnabled = false;
            }
            this.Password = temp;
        }
    }
}
