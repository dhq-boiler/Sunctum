using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Managers
{
    public interface ISelectManager<T> where T : EntityBaseObjectViewModel
    {
        Type SelectedType { get; }
        ObservableCollection<T> SelectedItems { get; set; }
    }
}
