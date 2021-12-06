using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.MemoArchives.PolicyAndProcedure
{
    public class Index : BaseModel
    {
        public List<int> Years { get; set; } = new List<int>();
    }
}