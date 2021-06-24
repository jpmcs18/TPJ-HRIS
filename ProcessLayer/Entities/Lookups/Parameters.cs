namespace ProcessLayer.Entities
{
    public class Parameters
    {
        public string ID { get { return Description; } set { Description = value; } }
        public string Description { get; set; }
        public object Value { get; set; }
        public string Tag { get; set; }
        public string DisplayName { get; set; }
        public int Order { get; set; }
    }
}
