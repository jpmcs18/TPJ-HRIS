using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProcessLayer.Helpers
{
    static class DataSetTableExtension
    {
        public static List<T> GetList<T>(this DataSet ds, Func<DataRow, T> function)
        {
            return ds.Tables.Count < 1 ? null : ds.Tables[0].AsEnumerable().Select(r => function(r)).ToList();
        }
        public static T Get<T>(this DataSet ds, Func<DataRow, T> function)
        {
            return ds.Tables.Count < 1 ? default : ds.Tables[0].AsEnumerable().Select(r => function(r)).FirstOrDefault();
        }
    }

    static class ListExtension
    {
        public static void Add(this List<Lookup> l, int id, string description)
        {
            l.Add(new Lookup { ID = id, Description = description });
        }
    }
}
