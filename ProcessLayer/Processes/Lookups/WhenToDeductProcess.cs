using ProcessLayer.Entities;
using System.Collections.Generic;
using ProcessLayer.Processes.Lookups;
using System.Linq;
using System;

namespace ProcessLayer.Processes
{
    public sealed class WhenToDeductProcess : ILookupSourceProcess<WhenToDeduct>
    {
        public static readonly Lazy<WhenToDeductProcess> Instance = new Lazy<WhenToDeductProcess>(() => new WhenToDeductProcess());
        private WhenToDeductProcess() { }

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
