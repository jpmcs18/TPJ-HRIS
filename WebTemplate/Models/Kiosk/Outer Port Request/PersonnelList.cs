using System.Collections.Generic;

namespace WebTemplate.Models.Kiosk.Outer_Port_Request
{
    public class PersonnelList
    {
        public string key { get; set; }
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();
    }
}