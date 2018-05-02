

using NUnit.Framework;
using Sunctum.UI.Core;
using System.Text.RegularExpressions;

namespace Sunctum.UI.Test
{
    [TestFixture]
    public class TreeGeneratorTest
    {
        [Test]
        public void Regex_Basic1()
        {
            string target = "abc";
            var rg = new Regex("^[a-zA-Z\\d]+?$");
            Assert.That(rg.IsMatch(target), Is.True);
        }

        [Test]
        public void Regex_Basic2()
        {
            string target = "-bc";
            var rg = new Regex("^[a-zA-Z\\d]+?$");
            Assert.That(rg.IsMatch(target), Is.False);
        }

        [Test]
        public void Regex_Basic3()
        {
            string target = "- bc:";
            var rg = new Regex(TreeGenerator.PATTERN_CANDIDATE);
            Assert.That(rg.IsMatch(target), Is.True);

            var mc = rg.Match(target);
            Assert.That(mc.Groups["indent"].Value, Is.EqualTo("- "));
            Assert.That(mc.Groups["array"].Value, Is.EqualTo("- "));
            Assert.That(mc.Groups["key"].Value, Is.EqualTo("bc"));
            Assert.That(mc.Groups["hash"].Value, Is.EqualTo(":"));
            Assert.That(mc.Groups["return"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["specify_indent"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["sign"].Value, Is.EqualTo(""));
        }

        [Test]
        public void PATTERN_Regex1()
        {
            string target = "- Image:";
            var rg = new Regex(TreeGenerator.PATTERN_CANDIDATE);
            Assert.That(rg.IsMatch(target), Is.True);

            var mc = rg.Match(target);
            Assert.That(mc.Groups["indent"].Value, Is.EqualTo("- "));
            Assert.That(mc.Groups["array"].Value, Is.EqualTo("- "));
            Assert.That(mc.Groups["key"].Value, Is.EqualTo("Image"));
            Assert.That(mc.Groups["hash"].Value, Is.EqualTo(":"));
            Assert.That(mc.Groups["return"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["specify_indent"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["sign"].Value, Is.EqualTo(""));
        }

        [Test]
        public void PATTERN_Regex2()
        {
            string target = "Message: |-";
            var rg = new Regex(TreeGenerator.PATTERN_CANDIDATE);
            Assert.That(rg.IsMatch(target), Is.True);

            var mc = rg.Match(target);
            Assert.That(mc.Groups["indent"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["array"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["key"].Value, Is.EqualTo("Message"));
            Assert.That(mc.Groups["hash"].Value, Is.EqualTo(":"));
            Assert.That(mc.Groups["return"].Value, Is.EqualTo("|"));
            Assert.That(mc.Groups["specify_indent"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["sign"].Value, Is.EqualTo("-"));
        }



        [Test]
        public void PATTERN_Regex3()
        {
            string target = "  UnescapedTitle: 201208-Karuizawa-53";
            var rg = new Regex(TreeGenerator.PATTERN_CANDIDATE);
            Assert.That(rg.IsMatch(target), Is.False);
        }

        [Test]
        public void PATTERN_VALUE_Regex()
        {
            string target = "  UnescapedTitle: 201208-Karuizawa-53";
            var rg = new Regex(TreeGenerator.PATTERN_VALUE);
            Assert.That(rg.IsMatch(target), Is.True);

            var mc = rg.Match(target);
            Assert.That(mc.Groups["indent"].Value, Is.EqualTo("  "));
            Assert.That(mc.Groups["key"].Value, Is.EqualTo("UnescapedTitle"));
            Assert.That(mc.Groups["hash"].Value, Is.EqualTo(":"));
            Assert.That(mc.Groups["return"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["ret2halfsp"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["specify_indent"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["save"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["remove"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["value"].Value, Is.EqualTo("201208-Karuizawa-53"));
        }

        [Test]
        public void PATTERN_VALUE_Regex2()
        {
            string target = "HResult: -2146233086";
            var rg = new Regex(TreeGenerator.PATTERN_VALUE);
            Assert.That(rg.IsMatch(target), Is.True);

            var mc = rg.Match(target);
            Assert.That(mc.Groups["indent"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["key"].Value, Is.EqualTo("HResult"));
            Assert.That(mc.Groups["hash"].Value, Is.EqualTo(":"));
            Assert.That(mc.Groups["return"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["ret2halfsp"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["specify_indent"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["save"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["remove"].Value, Is.EqualTo(""));
            Assert.That(mc.Groups["value"].Value, Is.EqualTo("-2146233086"));
        }



        [Test]
        public void PATTERN_MULTILINE_Regex1()
        {
            string target = "  インデックスは一覧の範囲内になければなりません。";
            var rg = new Regex(TreeGenerator.PATTERN_MULTILINE);
            Assert.That(rg.IsMatch(target), Is.True);

            var mc = rg.Match(target);
            Assert.That(mc.Groups["indent"].Value, Is.EqualTo("  "));
            Assert.That(mc.Groups["part_of_value"].Value, Is.EqualTo("インデックスは一覧の範囲内になければなりません。"));
        }

        [Test]
        public void PATTERN_MULTILINE_Regex2()
        {
            string target = "  パラメーター名:index";
            var rg = new Regex(TreeGenerator.PATTERN_MULTILINE);
            Assert.That(rg.IsMatch(target), Is.True);

            var mc = rg.Match(target);
            Assert.That(mc.Groups["indent"].Value, Is.EqualTo("  "));
            Assert.That(mc.Groups["part_of_value"].Value, Is.EqualTo("パラメーター名:index"));
        }

        [Test]
        public void ParseImage()
        {
            string yml = "- Image:\r\n    AbsoluteMasterPath: C:\\Users\\dhq_b_000\\Desktop\\TestField\\data/2018/03/18/314fac0c41ac429a815cad324e562278/201208-Karuizawa-53.JPG\r\n    RelativeMasterPath: data/2018/03/18/314fac0c41ac429a815cad324e562278/201208-Karuizawa-53.JPG\r\n    Thumbnail:\r\n      ImageID: 3787de41-566d-4f68-b3a2-2040b1d2c47a\r\n      AbsoluteMasterPath: C:\\Users\\dhq_b_000\\Desktop\\TestField\\cache\\3787de41566d4f68b3a22040b1d2c47a.JPG\r\n      RelativeMasterPath: cache\\3787de41566d4f68b3a22040b1d2c47a.JPG\r\n      ID: 3787de41-566d-4f68-b3a2-2040b1d2c47a\r\n    Path: C:\\Users\\dhq_b_000\\Desktop\\TestField\\cache\\3787de41566d4f68b3a22040b1d2c47a.JPG\r\n    MasterFileSize: 5887488\r\n    ThumbnailRecorded: true\r\n    ThumbnailLoaded: true\r\n    ThumbnailGenerated: true\r\n    Title: 201208-Karuizawa-53\r\n    UnescapedTitle: 201208-Karuizawa-53\r\n    ID: 3787de41-566d-4f68-b3a2-2040b1d2c47a\r\n  IsLoaded: true\r\n  UnescapedTitle: 201208-Karuizawa-53\r\n  BookID: 314fac0c-41ac-429a-815c-ad324e562278\r\n  ImageID: 3787de41-566d-4f68-b3a2-2040b1d2c47a\r\n  PageIndex: 1\r\n  Title: 201208-Karuizawa-53\r\n  ID: fcf1aa54-db49-4bb9-b9e3-5a373c6c8e1f\r\n";
            //- Image:
            //    AbsoluteMasterPath: C:\Users\dhq_b_000\Desktop\TestField\data/2018/03/18/314fac0c41ac429a815cad324e562278/201208-Karuizawa-53.JPG
            //    RelativeMasterPath: data/2018/03/18/314fac0c41ac429a815cad324e562278/201208-Karuizawa-53.JPG
            //    Thumbnail:
            //      ImageID: 3787de41-566d-4f68-b3a2-2040b1d2c47a
            //      AbsoluteMasterPath: C:\Users\dhq_b_000\Desktop\TestField\cache\3787de41566d4f68b3a22040b1d2c47a.JPG
            //      RelativeMasterPath: cache\3787de41566d4f68b3a22040b1d2c47a.JPG
            //      ID: 3787de41-566d-4f68-b3a2-2040b1d2c47a
            //    Path: C:\Users\dhq_b_000\Desktop\TestField\cache\3787de41566d4f68b3a22040b1d2c47a.JPG
            //    MasterFileSize: 5887488
            //    ThumbnailRecorded: true
            //    ThumbnailLoaded: true
            //    ThumbnailGenerated: true
            //    Title: 201208-Karuizawa-53
            //    UnescapedTitle: 201208-Karuizawa-53
            //    ID: 3787de41-566d-4f68-b3a2-2040b1d2c47a
            //  IsLoaded: true
            //  UnescapedTitle: 201208-Karuizawa-53
            //  BookID: 314fac0c-41ac-429a-815c-ad324e562278
            //  ImageID: 3787de41-566d-4f68-b3a2-2040b1d2c47a
            //  PageIndex: 1
            //  Title: 201208-Karuizawa-53
            //  ID: fcf1aa54-db49-4bb9-b9e3-5a373c6c8e1f
            //
            var actual = TreeGenerator.ParseYaml(yml);
            Assert.That(actual.Key, Is.EqualTo("ROOT"));
            Assert.That(actual.Children, Has.Count.EqualTo(1));

            actual = actual.Children[0];
            Assert.That(actual.Children, Has.Count.EqualTo(8));
            Assert.That(actual.Children[0], Has.Property("Key").EqualTo("Image"));

            var image = actual.Children[0];
            Assert.That(image.Children[0], Has.Property("Key").EqualTo("AbsoluteMasterPath").And.Property("Value").EqualTo(@"C:\Users\dhq_b_000\Desktop\TestField\data/2018/03/18/314fac0c41ac429a815cad324e562278/201208-Karuizawa-53.JPG"));
            Assert.That(image.Children[1], Has.Property("Key").EqualTo("RelativeMasterPath").And.Property("Value").EqualTo(@"data/2018/03/18/314fac0c41ac429a815cad324e562278/201208-Karuizawa-53.JPG"));
            Assert.That(image.Children[2], Has.Property("Key").EqualTo("Thumbnail"));
            Assert.That(image.Children[3], Has.Property("Key").EqualTo("Path").And.Property("Value").EqualTo(@"C:\Users\dhq_b_000\Desktop\TestField\cache\3787de41566d4f68b3a22040b1d2c47a.JPG"));
            Assert.That(image.Children[4], Has.Property("Key").EqualTo("MasterFileSize").And.Property("Value").EqualTo("5887488"));
            Assert.That(image.Children[5], Has.Property("Key").EqualTo("ThumbnailRecorded").And.Property("Value").EqualTo("true"));
            Assert.That(image.Children[6], Has.Property("Key").EqualTo("ThumbnailLoaded").And.Property("Value").EqualTo("true"));
            Assert.That(image.Children[7], Has.Property("Key").EqualTo("ThumbnailGenerated").And.Property("Value").EqualTo("true"));
            Assert.That(image.Children[8], Has.Property("Key").EqualTo("Title").And.Property("Value").EqualTo("201208-Karuizawa-53"));
            Assert.That(image.Children[9], Has.Property("Key").EqualTo("UnescapedTitle").And.Property("Value").EqualTo("201208-Karuizawa-53"));
            Assert.That(image.Children[10], Has.Property("Key").EqualTo("ID").And.Property("Value").EqualTo("3787de41-566d-4f68-b3a2-2040b1d2c47a"));

            Assert.That(actual.Children[1], Has.Property("Key").EqualTo("IsLoaded").And.Property("Value").EqualTo("true"));
            Assert.That(actual.Children[2], Has.Property("Key").EqualTo("UnescapedTitle").And.Property("Value").EqualTo("201208-Karuizawa-53"));
            Assert.That(actual.Children[3], Has.Property("Key").EqualTo("BookID").And.Property("Value").EqualTo("314fac0c-41ac-429a-815c-ad324e562278"));
            Assert.That(actual.Children[4], Has.Property("Key").EqualTo("ImageID").And.Property("Value").EqualTo("3787de41-566d-4f68-b3a2-2040b1d2c47a"));
            Assert.That(actual.Children[5], Has.Property("Key").EqualTo("PageIndex").And.Property("Value").EqualTo("1"));
            Assert.That(actual.Children[6], Has.Property("Key").EqualTo("Title").And.Property("Value").EqualTo("201208-Karuizawa-53"));
            Assert.That(actual.Children[7], Has.Property("Key").EqualTo("ID").And.Property("Value").EqualTo("fcf1aa54-db49-4bb9-b9e3-5a373c6c8e1f"));
        }

        [Test]
        public void ParseException()
        {
            string yml = "Message: |-\r\n  インデックスは一覧の範囲内になければなりません。\r\n  パラメーター名:index\r\nParamName: index\r\nData:\r\n  ? {}\r\n  : \r\nStackTrace: |2-\r\n     場所 System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)\r\n     場所 System.Collections.ObjectModel.Collection`1.Insert(Int32 index, T item)\r\n     場所 Sunctum.Domain.Logic.PageSorting.PageOrdering.OrderBackward(Page page, Book book) 場所 C:\\Git\\Sunctum\\Sunctum.Domain\\Logic\\PageSorting\\PageOrdering.cs:行 27\r\n     場所 Sunctum.ViewModel.Manager.LibraryManager.OrderBackward(Page page, Book book) 場所 C:\\Git\\Sunctum\\Sunctum\\ViewModel\\Manager\\LibraryManager.cs:行 469\r\n     場所 Sunctum.ViewModel.MainWindowViewModel.MovePageBackward(Page page) 場所 C:\\Git\\Sunctum\\Sunctum\\ViewModel\\MainWindowViewModel.cs:行 1298\r\n     場所 Sunctum.MainWindow.MoveBackword_Button_Click(Object sender, RoutedEventArgs e) 場所 C:\\Git\\Sunctum\\Sunctum\\MainWindow.xaml.cs:行 181\r\n     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)\r\n     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)\r\n     場所 System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)\r\n     場所 System.Windows.UIElement.RaiseEvent(RoutedEventArgs e)\r\n     場所 System.Windows.Controls.Primitives.ButtonBase.OnClick()\r\n     場所 System.Windows.Controls.Button.OnClick()\r\n     場所 System.Windows.Controls.Primitives.ButtonBase.OnMouseLeftButtonUp(MouseButtonEventArgs e)\r\n     場所 System.Windows.UIElement.OnMouseLeftButtonUpThunk(Object sender, MouseButtonEventArgs e)\r\n     場所 System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)\r\n     場所 System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)\r\n     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)\r\n     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)\r\n     場所 System.Windows.UIElement.ReRaiseEventAs(DependencyObject sender, RoutedEventArgs args, RoutedEvent newEvent)\r\n     場所 System.Windows.UIElement.OnMouseUpThunk(Object sender, MouseButtonEventArgs e)\r\n     場所 System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)\r\n     場所 System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)\r\n     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)\r\n     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)\r\n     場所 System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)\r\n     場所 System.Windows.UIElement.RaiseTrustedEvent(RoutedEventArgs args)\r\n     場所 System.Windows.UIElement.RaiseEvent(RoutedEventArgs args, Boolean trusted)\r\n     場所 System.Windows.Input.InputManager.ProcessStagingArea()\r\n     場所 System.Windows.Input.InputManager.ProcessInput(InputEventArgs input)\r\n     場所 System.Windows.Input.InputProviderSite.ReportInput(InputReport inputReport)\r\n     場所 System.Windows.Interop.HwndMouseInputProvider.ReportInput(IntPtr hwnd, InputMode mode, Int32 timestamp, RawMouseActions actions, Int32 x, Int32 y, Int32 wheel)\r\n     場所 System.Windows.Interop.HwndMouseInputProvider.FilterMessage(IntPtr hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam, Boolean& handled)\r\n     場所 System.Windows.Interop.HwndSource.InputFilterMessage(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)\r\n     場所 MS.Win32.HwndWrapper.WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)\r\n     場所 MS.Win32.HwndSubclass.DispatcherCallbackOperation(Object o)\r\n     場所 System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)\r\n     場所 System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)\r\nSource: mscorlib\r\nHResult: -2146233086\r\n";
            //Message: |-
            //  インデックスは一覧の範囲内になければなりません。
            //  パラメーター名: index
            //ParamName: index
            //StackTrace: |2-
            //     場所 System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
            //     場所 System.Collections.ObjectModel.Collection`1.Insert(Int32 index, T item)
            //     場所 Sunctum.Domain.Logic.PageSorting.PageOrdering.OrderBackward(Page page, Book book) 場所 C:\Git\Sunctum\Sunctum.Domain\Logic\PageSorting\PageOrdering.cs:行 27
            //     場所 Sunctum.ViewModel.Manager.LibraryManager.OrderBackward(Page page, Book book) 場所 C:\Git\Sunctum\Sunctum\ViewModel\Manager\LibraryManager.cs:行 469
            //     場所 Sunctum.ViewModel.MainWindowViewModel.MovePageBackward(Page page) 場所 C:\Git\Sunctum\Sunctum\ViewModel\MainWindowViewModel.cs:行 1298
            //     場所 Sunctum.MainWindow.MoveBackword_Button_Click(Object sender, RoutedEventArgs e) 場所 C:\Git\Sunctum\Sunctum\MainWindow.xaml.cs:行 181
            //     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)
            //     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)
            //     場所 System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)
            //     場所 System.Windows.UIElement.RaiseEvent(RoutedEventArgs e)
            //     場所 System.Windows.Controls.Primitives.ButtonBase.OnClick()
            //     場所 System.Windows.Controls.Button.OnClick()
            //     場所 System.Windows.Controls.Primitives.ButtonBase.OnMouseLeftButtonUp(MouseButtonEventArgs e)
            //     場所 System.Windows.UIElement.OnMouseLeftButtonUpThunk(Object sender, MouseButtonEventArgs e)
            //     場所 System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)
            //     場所 System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)
            //     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)
            //     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)
            //     場所 System.Windows.UIElement.ReRaiseEventAs(DependencyObject sender, RoutedEventArgs args, RoutedEvent newEvent)
            //     場所 System.Windows.UIElement.OnMouseUpThunk(Object sender, MouseButtonEventArgs e)
            //     場所 System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)
            //     場所 System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)
            //     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)
            //     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)
            //     場所 System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)
            //     場所 System.Windows.UIElement.RaiseTrustedEvent(RoutedEventArgs args)
            //     場所 System.Windows.UIElement.RaiseEvent(RoutedEventArgs args, Boolean trusted)
            //     場所 System.Windows.Input.InputManager.ProcessStagingArea()
            //     場所 System.Windows.Input.InputManager.ProcessInput(InputEventArgs input)
            //     場所 System.Windows.Input.InputProviderSite.ReportInput(InputReport inputReport)
            //     場所 System.Windows.Interop.HwndMouseInputProvider.ReportInput(IntPtr hwnd, InputMode mode, Int32 timestamp, RawMouseActions actions, Int32 x, Int32 y, Int32 wheel)
            //     場所 System.Windows.Interop.HwndMouseInputProvider.FilterMessage(IntPtr hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam, Boolean & handled)
            //     場所 System.Windows.Interop.HwndSource.InputFilterMessage(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean & handled)
            //     場所 MS.Win32.HwndWrapper.WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean & handled)
            //     場所 MS.Win32.HwndSubclass.DispatcherCallbackOperation(Object o)
            //     場所 System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
            //     場所 System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)
            //Source: mscorlib
            //HResult: -2146233086
            //
            var actual = TreeGenerator.ParseYaml(yml);
            Assert.That(actual.Key, Is.EqualTo("ROOT"));
            Assert.That(actual.Children, Has.Count.EqualTo(6));
            Assert.That(actual.Children[0], Has.Property("Key").EqualTo("Message").And.Property("Value").EqualTo("  インデックスは一覧の範囲内になければなりません。\r\n  パラメーター名:index"));
            Assert.That(actual.Children[1], Has.Property("Key").EqualTo("ParamName").And.Property("Value").EqualTo("index"));
            Assert.That(actual.Children[2], Has.Property("Key").EqualTo("Data"));
            Assert.That(actual.Children[3], Has.Property("Key").EqualTo("StackTrace").And.Property("Value").EqualTo("     場所 System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)\r\n     場所 System.Collections.ObjectModel.Collection`1.Insert(Int32 index, T item)\r\n     場所 Sunctum.Domain.Logic.PageSorting.PageOrdering.OrderBackward(Page page, Book book) 場所 C:\\Git\\Sunctum\\Sunctum.Domain\\Logic\\PageSorting\\PageOrdering.cs:行 27\r\n     場所 Sunctum.ViewModel.Manager.LibraryManager.OrderBackward(Page page, Book book) 場所 C:\\Git\\Sunctum\\Sunctum\\ViewModel\\Manager\\LibraryManager.cs:行 469\r\n     場所 Sunctum.ViewModel.MainWindowViewModel.MovePageBackward(Page page) 場所 C:\\Git\\Sunctum\\Sunctum\\ViewModel\\MainWindowViewModel.cs:行 1298\r\n     場所 Sunctum.MainWindow.MoveBackword_Button_Click(Object sender, RoutedEventArgs e) 場所 C:\\Git\\Sunctum\\Sunctum\\MainWindow.xaml.cs:行 181\r\n     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)\r\n     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)\r\n     場所 System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)\r\n     場所 System.Windows.UIElement.RaiseEvent(RoutedEventArgs e)\r\n     場所 System.Windows.Controls.Primitives.ButtonBase.OnClick()\r\n     場所 System.Windows.Controls.Button.OnClick()\r\n     場所 System.Windows.Controls.Primitives.ButtonBase.OnMouseLeftButtonUp(MouseButtonEventArgs e)\r\n     場所 System.Windows.UIElement.OnMouseLeftButtonUpThunk(Object sender, MouseButtonEventArgs e)\r\n     場所 System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)\r\n     場所 System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)\r\n     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)\r\n     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)\r\n     場所 System.Windows.UIElement.ReRaiseEventAs(DependencyObject sender, RoutedEventArgs args, RoutedEvent newEvent)\r\n     場所 System.Windows.UIElement.OnMouseUpThunk(Object sender, MouseButtonEventArgs e)\r\n     場所 System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)\r\n     場所 System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)\r\n     場所 System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)\r\n     場所 System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)\r\n     場所 System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)\r\n     場所 System.Windows.UIElement.RaiseTrustedEvent(RoutedEventArgs args)\r\n     場所 System.Windows.UIElement.RaiseEvent(RoutedEventArgs args, Boolean trusted)\r\n     場所 System.Windows.Input.InputManager.ProcessStagingArea()\r\n     場所 System.Windows.Input.InputManager.ProcessInput(InputEventArgs input)\r\n     場所 System.Windows.Input.InputProviderSite.ReportInput(InputReport inputReport)\r\n     場所 System.Windows.Interop.HwndMouseInputProvider.ReportInput(IntPtr hwnd, InputMode mode, Int32 timestamp, RawMouseActions actions, Int32 x, Int32 y, Int32 wheel)\r\n     場所 System.Windows.Interop.HwndMouseInputProvider.FilterMessage(IntPtr hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam, Boolean& handled)\r\n     場所 System.Windows.Interop.HwndSource.InputFilterMessage(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)\r\n     場所 MS.Win32.HwndWrapper.WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)\r\n     場所 MS.Win32.HwndSubclass.DispatcherCallbackOperation(Object o)\r\n     場所 System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)\r\n     場所 System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)"));
            Assert.That(actual.Children[4], Has.Property("Key").EqualTo("Source").And.Property("Value").EqualTo("mscorlib"));
            Assert.That(actual.Children[5], Has.Property("Key").EqualTo("HResult").And.Property("Value").EqualTo("-2146233086"));
        }
    }
}
