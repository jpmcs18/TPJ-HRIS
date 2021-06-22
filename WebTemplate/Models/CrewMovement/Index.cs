using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.CrewMovement
{
    public class Index : BaseModel
    {
        public string Name { get; set; }
        public List<ProcessLayer.Entities.Personnel> CrewList = new List<ProcessLayer.Entities.Personnel>();
    }
}