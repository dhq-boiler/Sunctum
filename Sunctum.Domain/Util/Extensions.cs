
using Homura.ORM;
using System;
using System.Data;
using System.Diagnostics;

namespace Sunctum.Domain.Util
{
    public static class Extensions
    {
        public static string ReplaceAll(this string target, char[] v, char newValue)
        {
            string escaped = target;
            foreach (var c in v)
            {
                escaped = escaped.Replace(c, newValue);
            }
            return escaped;
        }

        public static void LogEllapsedTime(Action act, string actionName = null)
        {
            Stopwatch sw = new Stopwatch();
            BeginMeasuring(sw);

            act.Invoke();

            EndMesuring(actionName, sw);
        }

        [Conditional("DEBUG")]
        private static void BeginMeasuring(Stopwatch sw)
        {
            sw.Start();
        }

        [Conditional("DEBUG")]
        private static void EndMesuring(string actionName, Stopwatch sw)
        {
            sw.Stop();
            if (actionName != null)
            {
                Debug.Write($"{actionName}:");
            }
            Debug.WriteLine($"{sw.ElapsedMilliseconds}ms");
        }
    }
}
