using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public interface ILookupProcess<T>
    {
        List<T> Filter(string key, int page, int gridCount, out int pageCount);
        T Get(dynamic id);
        T CreateOrUpdate(string item, int user);
        void Delete(dynamic id, int user);
    }
    public interface ILookupProcess
    {
        List<Lookup> Filter(string table, string key, int page, int gridCount, out int pageCount);
        Lookup Get(string table, dynamic id);
        Lookup CreateOrUpdate(string table, string item, int user);
        void Delete(string table, dynamic id, int user);
    }
    public interface ILookupSourceProcess<T>
    {
        List<T> GetList(bool hasDefault = false);
    }
}
