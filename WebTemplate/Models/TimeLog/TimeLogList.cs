using System.Collections.Generic;
namespace WebTemplate.Models.TimeLog
{
    public class TimeLogList
    {
        public List<ProcessLayer.Entities.TimeLog> TimeLogs { get; set; } = new List<ProcessLayer.Entities.TimeLog>();
    }
}