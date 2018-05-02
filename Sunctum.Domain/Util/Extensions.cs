
using Sunctum.Infrastructure.Data.Rdbms;
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

        public static Guid SafeGetGuid(this IDataRecord rdr, string columnName)
        {
            int index = rdr.CheckColumnExists(columnName);

            bool isNull = rdr.IsDBNull(index);

            return isNull ? Guid.Empty : rdr.GetGuid(index);
        }

        public static string SafeGetString(this IDataRecord rdr, string columnName)
        {
            int index = CheckColumnExists(rdr, columnName);

            bool isNull = rdr.IsDBNull(index);

            return isNull ? null : rdr.GetString(index);
        }

        public static int SafeGetInt(this IDataRecord rdr, string columnName)
        {
            int index = CheckColumnExists(rdr, columnName);

            bool isNull = rdr.IsDBNull(index);

            return isNull ? int.MinValue : rdr.GetInt32(index);
        }

        public static long? SafeNullableGetLong(this IDataRecord rdr, string columnName)
        {
            int index = CheckColumnExists(rdr, columnName);

            bool isNull = rdr.IsDBNull(index);

            return isNull ? null : (long?)rdr.GetInt64(index);
        }

        public static DateTime SafeGetDateTime(this IDataRecord rdr, string columnName)
        {
            int index = CheckColumnExists(rdr, columnName);

            return rdr.GetDateTime(index);
        }

        public static DateTime? SafeGetNullableDateTime(this IDataRecord rdr, string columnName)
        {
            int index = CheckColumnExists(rdr, columnName);

            bool isNull = rdr.IsDBNull(index);

            return isNull ? null : (DateTime?)rdr.GetDateTime(index);
        }

        public static int CheckColumnExists(this IDataRecord rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (index == -1)
            {
                throw new NotExistColumnException($"{columnName} ordinal is {index}");
            }

            return index;
        }

        public static bool IsDBNull(this IDataRecord rdr, string columnName)
        {
            return rdr.IsDBNull(rdr.GetOrdinal(columnName));
        }
    }
}
