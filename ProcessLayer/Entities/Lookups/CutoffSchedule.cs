using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class CutoffSchedule
    {
        public int ID { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "Invalid Month")]
        public byte Month { get; set; }
        [Required]
        [Range(1, 31, ErrorMessage = "Invalid Day")]
        public byte Day { get; set; }
    }
}
