using System;
using System.Collections.Generic;

namespace ProcessLayer.Entities
{
    public class WrittenExplanation
    {
        public long ID { get; set; }
        public string MemoNo { get; set; }
        public DateTime? MemoDate { get; set; }
        public string Description { get; set; }
        public short StatusID { get; set; }
        public DateTime? ConsultationSchedule { get; set; }
        public short? ConsultationStatusID { get; set; }
        public short? RecommendationID { get; set; }
        public long PersonnelID { get; set; }
        public string Subject { get; set; }
        public bool IsNew { get; set; }

        public Personnel Personnel { get; set; }
        public WrittenExplanationStatus Status { get; set; }
        public Recommendation Recommendation { get; set; }
        public ConsultationStatus ConsultationStatus { get; set; }
        public List<WrittenExplanationContent> Content { get; set; }
    }
}
