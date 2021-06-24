using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace DataAccessLayer.Security
{
    public class Error
    {
        public int ID { get; set; }

        public int? HResult { get; set; }

        public string Message { get; set; }

        public string InnerExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string Source { get; set; }

        public DateTime? CreatedDate { get; set; }

        public void Save()
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var err = new DataLayer.Error
                {
                    HResult = this.HResult,
                    Message = this.Message,
                    InnerExceptionMessage = this.InnerExceptionMessage,
                    StackTrace = this.StackTrace,
                    Source = this.Source,
                    CreatedDate = DateTime.Now
                };

                entity.Errors.Add(err);

                entity.SaveChanges();

                this.ID = err.ID;
            }
        }
    }
}
