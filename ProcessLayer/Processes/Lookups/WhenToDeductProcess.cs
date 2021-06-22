using ProcessLayer.Entities;
using System.Collections.Generic;
using ProcessLayer.Processes.Lookups;
using System.Linq;

namespace ProcessLayer.Processes
{
    public class WhenToDeductProcess : ILookupSourceProcess<WhenToDeduct>
    {
        private static WhenToDeductProcess _instance;
        public static WhenToDeductProcess Instance
        {
            get { if (_instance == null) _instance = new WhenToDeductProcess(); return _instance; }
        }

        public List<WhenToDeduct> WhenToDeducts { get; } = new List<WhenToDeduct> {   
                new WhenToDeduct{ ID= 1, Description = "1st Cutoff" },
                new WhenToDeduct{ ID= 2, Description = "2nd Cutoff" },
                new WhenToDeduct{ ID= 3, Description = "Both" }};
        public List<WhenToDeduct> GetList(bool hasDefault = false)
        {
            return WhenToDeducts;
        }
        public WhenToDeduct Get(byte id)
        {
            return WhenToDeducts.Where(x => x.ID == id).FirstOrDefault();
        }
    }
}
