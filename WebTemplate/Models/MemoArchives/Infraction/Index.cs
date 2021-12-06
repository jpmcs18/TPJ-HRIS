using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.MemoArchives.Infraction
{
    public class Index : BaseModel
    {
        public DateTime? Date { get; set; }
        public short? StatusID { get; set; }
        public string Personnel { get; set; }

        public List<ProcessLayer.Entities.Infraction> Infractions { get; set; } = new List<ProcessLayer.Entities.Infraction>();
    }
}