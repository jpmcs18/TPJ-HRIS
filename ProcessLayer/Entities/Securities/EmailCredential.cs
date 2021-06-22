using System.Security;

namespace ProcessLayer.Entities
{
    public class EmailCredential
    {
        public int ID { get; set; }
        public string Owner { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
    }
}
