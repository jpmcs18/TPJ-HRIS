using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.DynamicLookup
{
    public class Index : BaseModel
    {
        public string Key { get; set; }
        public Lookups? LookupName { get; set; }
        public dynamic DynamicList { get; set; }
        public bool IsDefault { get; set; }
    }
}