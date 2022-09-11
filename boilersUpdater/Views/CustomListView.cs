using System.Collections.Specialized;
using System.Windows.Controls;

namespace boilersUpdater.Views
{
    public class CustomListView : ListView
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                int newItemCount = e.NewItems.Count;

                if (newItemCount > 0)
                    this.ScrollIntoView(e.NewItems[newItemCount - 1]);
            }
            base.OnItemsChanged(e);
        }
    }
}
