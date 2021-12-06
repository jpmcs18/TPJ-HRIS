using System;
using System.Collections.Generic;

namespace ProcessLayer.Entities
{
    public class Infraction
    {
        public long ID { get; set; }
        public string MemoNo { get; set; }
        public DateTime? MemoDate { get; set; }
        public string Description { get; set; }
        public short StatusID { get; set; } 
        public DateTime? HearingSchedule { get; set; }
        public short? HearingStatusID { get; set; }
        public short? SanctionID { get; set; }
        public int? SanctionDays { get; set; }
        public DateTime? SanctionDate { get; set; }
        public long PersonnelID { get; set; }
        public string Subject { get; set; }
        public bool IsNew { get; set; }

        public Personnel Personnel { get; set; }
        public InfractionStatus Status { get; set; }
        public Sanction Sanction { get; set; }
        public HearingStatus HearingStatus { get; set; }
        public List<InfractionContent> Content { get; set; }
    }
}
