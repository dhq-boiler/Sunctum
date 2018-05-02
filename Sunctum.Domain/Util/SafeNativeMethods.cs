﻿

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Sunctum.Domain.Util
{
    /*
     * Natural Sort Order in C# http://stackoverflow.com/questions/248603/natural-sort-order-in-c-sharp
     * quizzer Michael Kniskern https://stackoverflow.com/users/26327/michael-kniskern
     * answerer Greg Beech https://stackoverflow.com/users/13552/greg-beech
     */

    internal static class SafeNativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
    }

    public sealed class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return SafeNativeMethods.StrCmpLogicalW(a, b);
        }
    }

    public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo a, FileInfo b)
        {
            return SafeNativeMethods.StrCmpLogicalW(a.Name, b.Name);
        }
    }
}
