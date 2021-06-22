using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class WorkExperiences
    {
        public long PersonnelID { get; set; }
        public List<WorkExperience> WorkExperience { get; set; } = new List<WorkExperience>();
    }
}