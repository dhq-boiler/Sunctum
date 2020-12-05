

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
    /// InputPassword.xaml の相互作用ロジック
    /// </summary>
    public partial class InputPasswordDialog : Window
    {
        public string Password { get { return this.password.Password; } }

        public InputPasswordDialog(string message)
        {
            InitializeComponent();
            this.Message.Content = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.OKButton.Focus();
                    break;
            }
        }
    }
}
