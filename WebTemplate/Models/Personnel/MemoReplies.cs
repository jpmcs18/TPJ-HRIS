using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class MemoReplies
    {
        public MemoArchives ParentMemo { get; set; }
        public long PersonnelID { get; set; }
        public List<MemoArchives> Memos { get; set; } = new List<MemoArchives>();
    }
}