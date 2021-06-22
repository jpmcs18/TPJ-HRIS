using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.MemoArchive
{
    public class Index : BaseModel
    {
        public int? MemoTypeID { get; set; }
        public int? MemoStatusID { get; set; }
        public string Personnel { get; set; }
        public string Group { get; set; }
        public List<ProcessLayer.Entities.MemoArchives> MemoArchives { get; set; } = new List<ProcessLayer.Entities.MemoArchives>();
    }
}