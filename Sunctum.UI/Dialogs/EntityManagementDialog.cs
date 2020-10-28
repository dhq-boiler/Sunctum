


using Homura.ORM;
using Sunctum.Domain.Models;
using Sunctum.UI.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sunctum.UI.Dialogs
{
    public class EntityManagementDialog<E> : AbstractEntityManagementDialog where E : class, IId, IName, ICloneable
    {
        public EntityManagementDialogViewModel<E> EntityMngVM { get; set; }

        protected override void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            EntityMngVM.Add();
        }

        protected override void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void Integrate_Button_Click(object sender, RoutedEventArgs e)
        {
            EntityMngVM.Integrate();
        }

        protected override void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                EntityMngVM.Selected = (e.AddedItems[0] as E).Clone() as E;
            }
        }

        protected override void NewName_Text_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    EntityMngVM.Add();
                    break;
                default:
                    break;
            }
        }

        protected override void NewName_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            EntityMngVM.ButtonsStateUpdate_Add();
        }

        protected override void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            EntityMngVM.Remove();
        }

        protected override void Revert_Button_Click(object sender, RoutedEventArgs e)
        {
            EntityMngVM.Revert();
        }

        protected override void SelectedName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EntityMngVM.ButtonStateUpdate_UpdateRevertRemove();
        }

        protected override void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            EntityMngVM.Update();
        }

        protected override void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                default:
                    break;
            }
        }
    }
}
