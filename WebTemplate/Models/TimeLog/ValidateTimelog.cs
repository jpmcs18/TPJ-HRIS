using System.Collections.Generic;
namespace WebTemplate.Models.TimeLog
{
    public class ValidateTimelog
    {
        public string Path { get; set; }
        public List<string> PinNotRecognized { get; set; } = new List<string>();
        public List<string> InvalidTimelog { get; set; } = new List<string>();
        public List<ProcessLayer.Entities.TimeLog> Timelogs { get; set; } = new List<ProcessLayer.Entities.TimeLog>();
    }
}