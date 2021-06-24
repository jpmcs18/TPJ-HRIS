using DataAccessLayer.Organization;
using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
namespace WebTemplate.Models.TimeLog
{
    public class PersonnelList {
        public string key { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();
    }
    public class DepartmentList
    {
        public List<ProcessLayer.Entities.Department> Departments { get; set; } = new List<ProcessLayer.Entities.Department>();
    }
}