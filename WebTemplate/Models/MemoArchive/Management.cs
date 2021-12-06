using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchive
{
    public class Management
    {
        public ProcessLayer.Entities.MemoArchives MemoArchive { get; set; } = new ProcessLayer.Entities.MemoArchives();
        public List<PersonnelGroup> PersonnelGroups { get; set; } = new List<PersonnelGroup>();
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();

        public string PersonnelGroupSearchKey { get; set; }
        public string PersonnelSearchKey { get; set; }
    }
}