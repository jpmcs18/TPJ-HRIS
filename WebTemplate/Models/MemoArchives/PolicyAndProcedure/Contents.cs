using System.Collections.Generic;

namespace WebTemplate.Models.MemoArchives.PolicyAndProcedure
{
    public class Contents
    {
        public ProcessLayer.Entities.PolicyAndProcedure PolicyAndProcedure { get; set; } = new ProcessLayer.Entities.PolicyAndProcedure();
        public List<ProcessLayer.Entities.PersonnelPolicyAndProcedure> PersonnelPolicyAndProcedures { get; set; } = new List<ProcessLayer.Entities.PersonnelPolicyAndProcedure>();
    }
}