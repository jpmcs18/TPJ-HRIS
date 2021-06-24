﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class PhilHealth
    {
        public short ID { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public decimal? EmployeeShare { get; set; }
        public decimal? EmployeePercentage { get; set; }
        public decimal? EmployerShare { get; set; }
        public decimal? EmployerPercentage { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
