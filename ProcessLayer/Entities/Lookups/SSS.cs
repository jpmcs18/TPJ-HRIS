using System;

namespace ProcessLayer.Entities
{
    public class SSS
    {
        public int ID { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public decimal? EmployeeShare { get; set; }
        public decimal? EmployerShare { get; set; }
        public decimal? EC { get; set; }
        public decimal? ProvPS { get; set; }
        public decimal? ProvES { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
