using System;

namespace ProcessLayer.Entities
{
    public class LeaveType : Lookup<int>
    {
        public int? MaxAllowedDays { get; set; }
        public bool? BulkUse { get; set; }
        public int? DaysBeforeRequest { get; set; }
        public bool? HasDocumentNeeded { get; set; }
        public bool? CNBNoteFirst { get; set; }
        public bool? IsMidYear { get; set; }
        public DateTime? DateStart { get; set; }
        public string Display { get { return $"{Description}{((BulkUse ?? false) ? " (Bulk Use)" : "")}"; } }
    }
}
