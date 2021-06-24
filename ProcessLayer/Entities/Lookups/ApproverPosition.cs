using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class ApproverPosition : LogDetails
    {
        public int ID { get; set; }
        [Required]
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public bool AllowApprove { get; set; } = false;
        public bool Deleted { get; set; } = false;
        public Position Position { get; set; }
    }
}
