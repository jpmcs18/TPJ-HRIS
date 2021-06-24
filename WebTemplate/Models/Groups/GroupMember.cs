using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Groups
{
    public class GroupMember
    {
        public PersonnelGroupMember PersonnelGroupMember { get; set; } = new PersonnelGroupMember();
    }
}