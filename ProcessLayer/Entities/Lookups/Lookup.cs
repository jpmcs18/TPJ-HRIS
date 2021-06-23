using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class Lookup<T>
    {
        public T ID { get; set; }
        public string Description { get; set; }
    }
    public class Lookup
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }
}
