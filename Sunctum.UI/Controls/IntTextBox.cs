

using System;
using System.Windows;
using System.Windows.Controls;

namespace Sunctum.UI.Controls
{
    public class IntTextBox : EasyEnterTextBox
    {
        private string _OldText = "";

        #region 依存プロパティ

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(int?),
            typeof(IntTextBox),
            new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnValueChanged)));

        #endregion //依存プロパティ

        #region 依存プロパティコールバック

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IntTextBox ctrl = d as IntTextBox;
            if (ctrl != null)
            {
                if (ctrl.Value.HasValue)
                {
                    ctrl.Text = ctrl.Value.Value.ToString();
                }
            }
        }

        #endregion //依存プロパティコールバック

        #region CLR プロパティ

        public int? Value
        {
            get { return (int?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion //CLR プロパティ

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            try
            {
                if (Text.Equals(""))
                {
                    if (Value.HasValue)
                    {
                        Value = null;
                    }
                }
                else
                {
                    int v = int.Parse(Text);
                    if (!Value.HasValue || Value.Value != v)
                    {
                        Value = v;
                    }
                }
                _OldText = Text;
            }
            catch (FormatException)
            {
                Text = _OldText;
            }
            catch (OverflowException)
            {
                Text = _OldText;
            }
        }
    }
}
