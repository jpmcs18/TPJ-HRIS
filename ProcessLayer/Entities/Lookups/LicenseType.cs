namespace ProcessLayer.Entities
{
    public class LicenseType : Lookup<int>
    {
        public bool Perpetual { get; set; }
    }
}
