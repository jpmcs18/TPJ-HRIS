namespace ProcessLayer.Entities
{
    public class Sanction : Lookup<short> {
        public bool? WithDays { get; set; } = false;
        public bool? WithDate { get; set; } = false;
    }

}
