using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchives.Infraction
{
    public class Management
    {
        public ProcessLayer.Entities.Infraction Infraction { get; set; } = new ProcessLayer.Entities.Infraction();
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();

        public string PersonnelSearchKey { get; set; }
    }
}