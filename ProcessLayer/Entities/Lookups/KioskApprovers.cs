namespace ProcessLayer.Entities
{
    public class KioskApprovers
    {
        public int ID { get; set; }
        public int? DepartmentID { get; set; }
        public long? ApproverID { get; set; }
        public int? Sequence { get; set; }
        public bool Deleted { get; set; } = false;

        public Personnel _Personnel { get; set; }
    }
}
