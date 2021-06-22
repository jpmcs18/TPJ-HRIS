namespace ProcessLayer.Entities
{
    public class Vessel
    {
        public short ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal? GrossTon { get; set; }
        public decimal? NetTon { get; set; }
        public decimal? HP { get; set; }

        public string Display { get; set; }
    }
}