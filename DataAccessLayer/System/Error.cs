using DataLayer;
using System;

namespace DataAccessLayer.Security
{
    public class Error
    {
        public int? ErrorID { get; set; }

        public int? HResult { get; set; }

        public string Message { get; set; }

        public string InnerExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string Source { get; set; }

        public string PageName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public void Save()
        {
            using RPTAEntities entity = new();
            var err = new sys_Errors
            {
                HResult = this.HResult,
                Message = this.Message,
                InnerExceptionMessage = this.InnerExceptionMessage,
                StackTrace = this.StackTrace,
                Source = this.Source,
                CreatedDate = DateTime.Now
            };

            entity.sys_Errors.Add(err);

            entity.SaveChanges();

            this.ErrorID = err.ErrorID;
        }
    }
}
