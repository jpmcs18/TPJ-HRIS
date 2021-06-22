using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models
{
    public class BaseModel
    {
        public int PageCount { get; set; }
        public int Page { get; set; }
        public string Filter { get; set; }
        public int GridCount { get; set; } = 10;
    }
}