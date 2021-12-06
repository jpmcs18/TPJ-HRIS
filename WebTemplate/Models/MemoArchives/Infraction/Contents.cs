using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchives.Infraction
{
    public class Contents
    {
        public ProcessLayer.Entities.Infraction Infraction { get; set; } = new ProcessLayer.Entities.Infraction();
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();
    }
}