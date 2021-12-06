using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchives.WrittenExplanation
{
    public class Management
    {
        public ProcessLayer.Entities.WrittenExplanation WrittenExplanation { get; set; } = new ProcessLayer.Entities.WrittenExplanation();
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();

        public string PersonnelSearchKey { get; set; }
    }
}