using ProcessLayer.Entities.Lookups;

namespace ProcessLayer.Entities
{
    public class Position : Lookup<int>
    {
        public string Abbreviation { get; set; }
        public string Display { get { return string.IsNullOrEmpty(Abbreviation) ? (string.IsNullOrEmpty(Description) ? null : Description) : Abbreviation; } }
        public string FullDisplay { get { return ID == 0 ? Description : string.Format("{0}{2}{1}", Abbreviation, Description, string.IsNullOrEmpty(Abbreviation) ? "" : (string.IsNullOrEmpty(Description) ? "" : " - ")); } }
        public bool? AllowApprove { get; set; } = false;
        public int? PersonnelTypeID { get; set; }
        public PersonnelType PersonnelType { get; set; }
    }
}
    