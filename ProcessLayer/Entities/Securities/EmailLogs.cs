using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class EmailLogs
    {
        public long ID { get; set; }
        public string Category { get; set; }
        public long? ItemID { get; set; }
        public string Email { get; set; }
        public string File { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Status { get; set; }
        public string Remarks { get; set; }
        public bool Resended { get; set; }
    }
}
