namespace ProcessLayer.Entities
{
    public class PersonnelGroupMember
    {
        public long ID { get; set; }
        public int? PersonnelGroupID { get; set; }
        public long? PersonnelID { get; set; }
        public bool Deleted { get; set; }

        public PersonnelGroup _PersonnelGroup { get; set; } = new PersonnelGroup();
        public Personnel _Personnel { get; set; } = new Personnel();
    }
}
