using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchives.WrittenExplanation
{
    public class Contents
    {
        public ProcessLayer.Entities.WrittenExplanation WrittenExplanation { get; set; } = new ProcessLayer.Entities.WrittenExplanation();
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();
    }
}