using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ProcessLayer.Processes.Lookups
{
    public class NonWorkingTypeProcess : ILookupSourceProcess<Lookup>
    {

        private static NonWorkingTypeProcess _instance;
        public static NonWorkingTypeProcess Instance
        {
            get { if (_instance == null) _instance = new NonWorkingTypeProcess(); return _instance; }
        }

        private List<Lookup> List { get; } = new List<Lookup>() {
            new Lookup() {ID = 1, Description = "Regular Holiday"},
            new Lookup() {ID = 2, Description = "Special Holiday"},
            new Lookup() {ID = 3, Description = "Work Suspension"},
        };

        public Lookup Get(byte id)
        {
            return List.Where(x => x.ID == id).FirstOrDefault();
        }
        public List<Lookup> GetList(bool hasDefault = false)
        {
            return List;
        }
    }

}
