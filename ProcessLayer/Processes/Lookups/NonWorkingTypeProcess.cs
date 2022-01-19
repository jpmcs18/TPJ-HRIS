using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class NonWorkingTypeProcess : ILookupSourceProcess<Lookup>
    {
        public static readonly NonWorkingTypeProcess Instance = new NonWorkingTypeProcess();
        private NonWorkingTypeProcess() { }
        private List<Lookup> List { get; } = new List<Lookup>() {
            new Lookup() {ID = 1, Description = "Regular Holiday"},
            new Lookup() {ID = 2, Description = "Special Holiday"},
            new Lookup() {ID = 3, Description = "Work Suspension"},
        };

        public Lookup Get(int id)
        {
            return List.Where(x => x.ID == id).FirstOrDefault();
        }
        public List<Lookup> GetList(bool hasDefault = false)
        {
            return List;
        }
    }

}
