using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProcessLayer.Entities
{
    public class InfractionContent
    {
        public long ID { get; set; }
        public long InfractionID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string File { get; set; }
        public string FilePath { get; set; }
        public bool? SaveOnly { get; set; } = false;
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