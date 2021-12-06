using ProcessLayer.Helpers;
using System;

namespace ProcessLayer.Entities
{
    public class WrittenExplanationContent
    {
        public long ID { get; set; }
        public long WrittenExplanationID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string File { get; set; }
        public string FilePath { get; set; }
        public bool? SaveOnly { get; set; }
        public bool FromPersonnel { get; set; }
        public bool IsFileSupported
        {
            get
            {
                return Web.IsFileSupported(File);
            }
        }
        public DateTime? Date { get; set; }
    }
}
