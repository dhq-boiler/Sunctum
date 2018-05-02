

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Sunctum.UI.Core
{
    public static class Extensions
    {
        //http://tawamuredays.blog.fc2.com/blog-entry-82.html
        public static T FindAncestor<T>(this DependencyObject depObj) where T : class
        {
            var target = depObj;
            try
            {
                do
                {
                    target = VisualTreeHelper.GetParent(target);
                }
                while (target != null && !(target is T));

                return target as T;
            }
            finally
            {
                target = null;
                depObj = null;
            }
        }

        //http://tawamuredays.blog.fc2.com/blog-entry-82.html
        public static IEnumerable<T> FindChildren<T>(
                this DependencyObject depObj, bool descendant = true) where T : class
        {
            var count = VisualTreeHelper.GetChildrenCount(depObj);

            try
            {
                foreach (var idx in IntRange(0, count - 1))
                {
                    var child = VisualTreeHelper.GetChild(depObj, idx);

                    if (child != null)
                    {
                        if (child is T)
                        {
                            yield return child as T;
                        }

                        if (descendant)
                        {
                            count = VisualTreeHelper.GetChildrenCount(child);
                            if (count > 0)
                            {
                                foreach (var ch in child.FindChildren<T>(descendant))
                                {
                                    yield return ch;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                depObj = null;
            }
        }

        //http://stackoverflow.com/questions/7319952/how-to-get-listbox-itemspanel-in-code-behind
        public static T GetVisualChild<T>(this DependencyObject parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        //http://tawamuredays.blog.fc2.com/blog-entry-82.html
        public static IEnumerable<int> IntRange(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                yield return i;
            }
        }
    }
}
