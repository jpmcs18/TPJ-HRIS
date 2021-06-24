﻿using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchive
{
    public class Management
    {
        public MemoArchives MemoArchive { get; set; } = new MemoArchives();
        public List<PersonnelGroup> PersonnelGroups { get; set; } = new List<PersonnelGroup>();
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();

        public string PersonnelGroupSearchKey { get; set; }
        public string PersonnelSearchKey { get; set; }
    }
}