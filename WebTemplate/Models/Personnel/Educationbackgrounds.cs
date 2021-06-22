using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class Educationbackgrounds
    {
        public long PersonnelID { get; set; }
        public List<EducationalBackground> EducationalBackground { get; set; } = new List<EducationalBackground>();
    }
}