using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Groups
{
    public class GroupMembers
    {
        public PersonnelGroup PersonnelGroup { get; set; } = new PersonnelGroup();
        public List<PersonnelGroupMember> PersonnelGroupMembers { get; set; } = new List<PersonnelGroupMember>();
        public Personnels Personnels { get; set; } = new Personnels();

        public string MembersSearchKey { get; set; }
        public string PersonnelSearchKey { get; set; }
    }
}