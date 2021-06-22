using System;

namespace ProcessLayer.Entities
{
    public class ContactNumber : PersonnelBase
    {
        public int ContactNoTypeID { get; set; }
        public string Number { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}