using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.ViewModels
{
    public interface ISelectManager
    {
        Type ElementSelectedType { get; set; }
        ObservableCollection<object> SelectedItems { get; set; }

        ObservableCollection<T> GetCollection<T>() where T : EntityBaseObjectViewModel;
    }
}
