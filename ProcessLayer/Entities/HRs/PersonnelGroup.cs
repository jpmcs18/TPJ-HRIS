using System.Collections.Generic;

namespace ProcessLayer.Entities
{
    public class PersonnelGroup : Lookup
    {
        public List<PersonnelGroupMember> _PersonnelGroupMembers { get; set; } = new List<PersonnelGroupMember>();
    }
}
