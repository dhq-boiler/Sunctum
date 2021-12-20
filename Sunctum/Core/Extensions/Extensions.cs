using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sunctum.Core.Extensions
{
    public static class Extensions
    {
        /*
         * https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type
         */
        public static T GetChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static IEnumerable<T> EnumerateChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as IEnumerable<T>) ?? EnumerateChildOfType<T>(child);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        if (item != null)
                            yield return item;
                    }
                }
                var result2 = (child as T) ?? GetChildOfType<T>(child);
                if (result2 != null)
                    yield return result2;
            }
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(this DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static IEnumerable<T> GetCorrespondingViews<T>(this FrameworkElement parent, object dataContext, bool parentInclude = false)
            where T : FrameworkElement
        {
            if (parentInclude && parent.DataContext == dataContext)
            {
                if (parent is T)
                    yield return parent as T;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                var result = (child as IEnumerable<T>) ?? EnumerateChildOfType<T>(child);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        if (item != null && item.DataContext == dataContext)
                            yield return item;
                    }
                }
                var result2 = (child as T) ?? GetChildOfType<T>(child);
                if (result2 != null && result2.DataContext == dataContext)
                    yield return result2;
            }
        }

        public static IEnumerable<FrameworkElement> GetViewsHavingDataContext(this FrameworkElement parent, bool parentInclude = false)
        {
            if (parentInclude && !(parent.DataContext is null))
            {
                if (parent is FrameworkElement)
                    yield return parent as FrameworkElement;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var result = (child as IEnumerable<FrameworkElement>) ?? EnumerateChildOfType<FrameworkElement>(child);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        if (item != null && !(item.DataContext is null))
                            yield return item;
                    }
                }
                var result2 = (child as FrameworkElement) ?? GetChildOfType<FrameworkElement>(child);
                if (result2 != null && !(result2.DataContext is null))
                    yield return result2;
            }
        }

        public static T GetParentOfType<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            if (obj == null) return null;

            while (obj != null && !(obj is T))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            if (obj == null) return null;

            return (T)obj;
        }

        public static DependencyObject GetParentOfType(this DependencyObject obj, string name)
        {
            if (obj == null) return null;

            while (obj != null && (obj is FrameworkElement && !(obj as FrameworkElement).Name.Equals(name)))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            if (obj == null) return null;

            return obj;
        }

        public static double DpiXFactor(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                return source.CompositionTarget.TransformToDevice.M11;
            }
            else
            {
                return 1.0;
            }
        }

        public static double DpiYFactor(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                return source.CompositionTarget.TransformToDevice.M22;
            }
            else
            {
                return 1.0;
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new ObservableCollection<T>(source);
        }

        public static IEnumerable<DependencyObject> Children(this DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var count = VisualTreeHelper.GetChildrenCount(obj);
            if (count == 0)
                yield break;

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                yield return child;
            }
        }

        public static IEnumerable<DependencyObject> Descendants(this DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            foreach (var child in obj.Children())
            {
                yield return child;
                foreach (var grandChild in child.Descendants())
                    yield return grandChild;
            }
        }

        public static IEnumerable<T> Children<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            return obj.Children().OfType<T>();
        }

        //--- 特定の型の子孫要素を取得
        public static IEnumerable<T> Descendants<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            return obj.Descendants().OfType<T>();
        }

        //https://stackoverflow.com/questions/41608665/linq-recursive-parent-child
        public static IEnumerable<T2> SelectRecursive<T1, T2>(this IEnumerable<T1> source, Func<T2, IEnumerable<T2>> selector) where T1 : class where T2 : class
        {
            foreach (var parent in source)
            {
                yield return parent as T2;

                var children = selector(parent as T2);
                foreach (var child in SelectRecursive(children, selector))
                    yield return child;
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
    }
}
