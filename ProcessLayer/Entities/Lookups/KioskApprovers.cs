namespace ProcessLayer.Entities
{
    public class KioskApprovers
    {
        public int ID { get; set; }
        public int? DepartmentID { get; set; }
        public long? ApproverID { get; set; }
        public byte? Sequence { get; set; }
        public bool Deleted { get; set; } = false;

        public Personnel _Personnel { get; set; }
    }
}
