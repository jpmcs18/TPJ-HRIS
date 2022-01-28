namespace ProcessLayer.Entities
{
    public class Sanction : Lookup<short> {
        public bool? WithDays { get; set; }
        public bool? WithDate { get; set; }
    }

}
