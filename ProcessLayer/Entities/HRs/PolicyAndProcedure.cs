using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;

namespace ProcessLayer.Entities
{
    public class PolicyAndProcedure
    {
        public long ID { get; set; }
        public string MemoNo { get; set; }
        public DateTime? MemoDate { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string File { get; set; }
        public string FilePath { get; set; }
        public bool? SaveOnly { get; set; }
        public bool IsFileSupported
        {
            get
            {
                return Web.IsFileSupported(File);
            }
        }

        public List<PersonnelPolicyAndProcedure> Content { get; set; }
        public long? PersonnelPolicyAndProcedureId { get; set; }
        public bool? Acknowledge { get; set; }
        public bool IsNew { get; set; }
    }
}
