using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class MemoReplies
    {
        public ProcessLayer.Entities.MemoArchives ParentMemo { get; set; }
        public long PersonnelID { get; set; }
        public List<ProcessLayer.Entities.MemoArchives> Memos { get; set; } = new List<ProcessLayer.Entities.MemoArchives>();
    }
}