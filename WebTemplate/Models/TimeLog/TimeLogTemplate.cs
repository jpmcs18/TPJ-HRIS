using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.TimeLog
{
    public class TimeLogTemplate
    {
        public TimeLogTemplate(int biometricsID, DateTime date)
        {
            BiometricsID = biometricsID;
            Date = date;
        }
        public int BiometricsID { get; set; }
        public DateTime Date { get; set; }
    }
}