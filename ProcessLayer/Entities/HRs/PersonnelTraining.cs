using System;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelTraining : PersonnelBase
    {
        [DisplayName("Training Type")]
        public int? TrainingTypeID { get; set; }
        [DisplayName("Training Provider")]
        public string TrainingProvider { get; set; }
        public string Title { get; set; }
        [DisplayName("Training Date")]
        public DateTime? TrainingDate { get; set; }

        public Lookup _TrainingType { get; set; } = new Lookup();
    }
}
