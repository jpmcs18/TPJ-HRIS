using System;

namespace ProcessLayer.Entities
{
    public class PersonnelHistoryBase : PersonnelBase
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
