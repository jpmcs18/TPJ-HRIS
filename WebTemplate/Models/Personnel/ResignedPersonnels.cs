using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class ResignedPersonnels
    {
        public string Key { get; set; }
        public List<ProcessLayer.Entities.Personnel> ResignedPersonnel { get; set; }
    }
}