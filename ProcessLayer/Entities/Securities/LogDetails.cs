using System;

namespace ProcessLayer.Entities
{
    public class LogDetails
    {
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string _Creator { get; set; }
        public string _Modifier { get; set; }
    }
}