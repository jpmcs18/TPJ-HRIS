using System;

namespace ProcessLayer.Entities
{
    public class LeaveType : Lookup<int>
    {
        public int? MaxAllowedDays { get; set; }
        public bool? BulkUse { get; set; } = false;
        public int? DaysBeforeRequest { get; set; }
        public bool? HasDocumentNeeded { get; set; } = false;
        public bool? CNBNoteFirst { get; set; } = false;
        public bool? IsMidYear { get; set; } = false;
        public DateTime? DateStart { get; set; }
        public string Display { get { return $"{Description}{((BulkUse ?? false) ? " (Bulk Use)" : "")}"; } }
    }
}
