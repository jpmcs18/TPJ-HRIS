using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class Trainings
    {
        public long PersonnelID { get; set; }
        public List<PersonnelTraining> Training { get; set; } = new List<PersonnelTraining>();
    }
}