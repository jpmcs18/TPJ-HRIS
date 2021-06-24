using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class PositionSalary
    {
        public int ID { get; set; }
        [Required]
        public int PositionID { get; set; }
        public double Salary { get; set; }
        public Position Position { get; set; }
    }
}
