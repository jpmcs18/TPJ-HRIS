using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebTemplate.Models.Groups;

namespace WebTemplate.Models.Searchable
{
    public class Search
    {
        public Personnels Personnels { get; set; } = new Personnels();
        public string Key { get; set; }
    }
}