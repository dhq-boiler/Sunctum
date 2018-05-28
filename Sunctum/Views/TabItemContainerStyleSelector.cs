

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Sunctum.Views
{
    [ContentProperty("Items")]
    public class TabItemContainerStyleSelector : StyleSelector
    {
        public List<TabItemTypedStyle> Items { get; set; }

        public TabItemContainerStyleSelector()
        {
            Items = new List<TabItemTypedStyle>();
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var styleData = Items.Find(s => item.GetType().Equals(s.DataType));
            if (styleData != null) return styleData.Style;

            return base.SelectStyle(item, container);
        }
    }

    [ContentProperty("Style")]
    public class TabItemTypedStyle
    {
        public Type DataType { get; set; }
        public Style Style { get; set; }
    }
}
