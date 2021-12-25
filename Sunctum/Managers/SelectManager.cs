using Sunctum.Core.Extensions;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Managers
{
    public class SelectManager : ISelectManager
    {
        public Type ElementSelectedType { get; set; }

        private ObservableCollection<object> _selectedItems;

        public ObservableCollection<object> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
                ElementSelectedType = value.FirstOrEmpty().GetType();
            }
        }

        public ObservableCollection<T> GetCollection<T>() where T : EntityBaseObjectViewModel
        {
            return SelectedItems.Cast<T>().ToObservableCollection();
        }
    }
}
