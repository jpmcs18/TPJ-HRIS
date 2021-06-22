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
    }
}