using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities.Email
{
    public class Credential
    {
        public int ID { get; set; }
        public string Owner { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
