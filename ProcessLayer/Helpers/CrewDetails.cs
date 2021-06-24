using ProcessLayer.Entities;
using System;

namespace ProcessLayer.Helpers
{
    public class CrewDetails
    {
        public CrewMovement Crew { get; set; } = new CrewMovement();
        public CrewMovement FromCrew { get; set; } = new CrewMovement();
        public CrewMovement ToCrew { get; set; } = new CrewMovement();
        public DateTime? Disembarked { get; set; }
        public string Reference { get; set; }
    }
}
