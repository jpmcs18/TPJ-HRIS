using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class Memos
    {
        public List<ProcessLayer.Entities.MemoArchives> MemoArchives { get; set; } = new List<ProcessLayer.Entities.MemoArchives>();
    }
}