using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchives.PolicyAndProcedure
{
    public class PolicyAndProcedures : BaseModel
    {
        public int Year { get; set; }
        public List<ProcessLayer.Entities.PolicyAndProcedure> PolicyAndProcedureList { get; set; } = new List<ProcessLayer.Entities.PolicyAndProcedure>();

    }
}