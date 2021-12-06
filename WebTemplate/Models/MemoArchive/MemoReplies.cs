using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchive
{
    public class MemoReplies
    {
        public ProcessLayer.Entities.MemoArchives ParentMemo { get; set; }
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();
        public List<ProcessLayer.Entities.MemoArchives> MemoArchives { get; set; } = new List<ProcessLayer.Entities.MemoArchives>();
    }
}