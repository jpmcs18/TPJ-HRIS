using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class TimeLog
    {
        public long ID { get; set; }
        public long PersonnelID { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        
        public int BiometricsID { get; set; }
        public Personnel _Personnel { get; set; } = new Personnel();
    }
}
