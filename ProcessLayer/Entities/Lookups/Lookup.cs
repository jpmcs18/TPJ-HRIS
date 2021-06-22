using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class Lookup<T>
    {
        public T ID { get; set; }
        public string Description { get; set; }
    }
    public class Lookup
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }
    public class LeaveType : Lookup<byte>
    {
        public int? MaxAllowedDays { get; set; }
        public bool? BulkUse { get; set; }
        public int? DaysBeforeRequest { get; set; }
        public bool? HasDocumentNeeded { get; set; }
    }
}
