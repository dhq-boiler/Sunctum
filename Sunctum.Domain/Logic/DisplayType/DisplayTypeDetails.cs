﻿

using Sunctum.Domain.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sunctum.Domain.Logic.DisplayType
{
    internal class DisplayTypeDetails : IDisplayType
    {
        public ViewBase Get()
        {
            var gridView = new GridView();
            gridView.AllowsColumnReorder = true;
            gridView.ColumnHeaderToolTip = "Book details";

            var gridViewColumn1 = new GridViewColumn();
            gridViewColumn1.Header = "著者";
            gridViewColumn1.DisplayMemberBinding = new Binding()
            {
                Path = new System.Windows.PropertyPath("Author.UnescapedName")
            };
            gridView.Columns.Add(gridViewColumn1);

            var gridViewColumn2 = new GridViewColumn();
            gridViewColumn2.Header = "タイトル";
            gridViewColumn2.DisplayMemberBinding = new Binding()
            {
                Path = new System.Windows.PropertyPath("UnescapedTitle")
            };
            gridView.Columns.Add(gridViewColumn2);

            var gridViewColumn3 = new GridViewColumn();
            gridViewColumn3.Header = "サイズ";
            gridViewColumn3.DisplayMemberBinding = new Binding()
            {
                Path = new System.Windows.PropertyPath("ByteSize"),
                Converter = new FileSizeFormatter()
            };
            gridView.Columns.Add(gridViewColumn3);

            var gridViewColumn4 = new GridViewColumn();
            gridViewColumn4.Header = "ページ数";
            gridViewColumn4.DisplayMemberBinding = new Binding()
            {
                Path = new System.Windows.PropertyPath("NumberOfPages.Value")
            };
            gridView.Columns.Add(gridViewColumn4);

            return gridView;
        }

        private class FileSizeFormatter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null) return null;
                return FileSize.ConvertFileSizeUnit((long)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}