using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Reports
{
    public sealed class ReportProcess
    {
        private static ReportProcess _instance;
        public static ReportProcess Instance
        {
            get { if (_instance == null) _instance = new ReportProcess(); return _instance; }
        }

        internal PersonnelReport Converter(DataRow dr)
        {
            return new PersonnelReport
            {
                FirstName = dr["First Name"].ToString(),
                MiddleName = dr["Middle Name"].ToString(),
                LastName = dr["Last Name"].ToString(),
                Amount = dr["Amount"].ToDecimal(),
                PS = dr["PS"].ToDecimal(),
                ES = dr["ES"].ToDecimal(),
                EC = dr["EC"].ToDecimal(),
                Col =  dr["Col"].ToInt()
            };
        }

        public List<PersonnelReport> GetList(ReportType tag, int year, int month)
        {
            List<PersonnelReport> list = null;

            using (var db = new DBTools())
            {
                var par = new Dictionary<string, object>
                {
                    { "@Tag", tag },
                    { "@Year", year },
                    { "@Month", month }
                };
                using (var ds = db.ExecuteReader("cnb.GetPersonnelAmount", par))
                {
                    list = ds.GetList(Converter);
                }
            }
            return list;
        }
    }
}
