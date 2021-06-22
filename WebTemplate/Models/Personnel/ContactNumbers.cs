using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class ContactNumbers
    {
        public long PersonnelID { get; set; }
        public List<ContactNumber> ContactNumber { get; set; } = new List<ContactNumber>();
    }
}