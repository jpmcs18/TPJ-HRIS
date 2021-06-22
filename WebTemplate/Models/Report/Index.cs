using ProcessLayer.Entities;
using ProcessLayer.Helpers.Enumerable;
using System.Collections.Generic;

namespace WebTemplate.Models.Report
{
    public class Index
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public ReportType ReportType { get; set; }
        public List<PersonnelReport> Personnels { get; set; }
    }
}