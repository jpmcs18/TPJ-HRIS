using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Extension
{
    public static class ObjectIsNullExtension
    {
        public static T CheckIfNull<T>(this T t) where T : new()
        {
            if (t == null) return new T();
            else return t;
        }

        public static T CheckIfNull<T>(this Nullable<T> t) where T : struct
        {
            if (t == null) return default(T);
            else return t.Value;
        }

        public static string BoldMatch(string value, string search)
        {
            value = value.ToLower();

            if (String.IsNullOrEmpty(search))
            {
                return value;
            }

            var idx1 = value.IndexOf(search);
            var idx2 = value.Substring(value.IndexOf(search) + 1);
            if (idx1 == -1)
            {
                return value;
            }

            value = value.Remove(idx1, search.Length);
            return value.Insert(idx1, $"<b style='font-weight: 800; color: #009688;'>{search}</b>");
            //ret += value.ToLower().Replace(search.ToLower(), $"<b style='font-weight: 800;'>{search.ToLower()}</b>");
        }
    }
}