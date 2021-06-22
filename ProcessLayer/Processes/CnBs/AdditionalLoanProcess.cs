using DBUtilities;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.CnB
{
    public class AdditionalLoanProcess
    {
        private static AdditionalLoanProcess _instance;

        public static AdditionalLoanProcess Instance
        {
            get { if (_instance == null) _instance = new AdditionalLoanProcess(); return _instance; }
        }

        internal  AdditionalLoanForApproval Converter(DataRow dr)
        {
            var al = new AdditionalLoanForApproval
            {
                ID = dr["ID"].ToLong(),
                Amount = dr["Amount"].ToDecimal(),
                LoanID = dr["Loan ID"].ToByte(),
                PersonnelID = dr["Personnel ID"].ToLong(),
                Remarks = dr["Remarks"].ToString()
            };

            al._Loan = LookupProcess.GetLoan(al.LoanID);

            return al;
        }

        public List<AdditionalLoanForApproval> GetList(long PersonnelID)
        {
            var al = new List<AdditionalLoanForApproval>();
            var parameters = new Dictionary<string, object>
            {
                { "@PersonnelID", PersonnelID}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetAdditionalLoanForApproval", parameters))
                {
                    al = ds.GetList(Converter);
                }
            }
            return al;
        }

        public AdditionalLoanForApproval Get(long ID)
        {
            var al = new AdditionalLoanForApproval();
            var parameters = new Dictionary<string, object>
            {
                { "@ID", ID}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetAdditionalLoanForApproval", parameters))
                {
                    al = ds.Get(Converter);
                }
            }
            return al;
        }

        public bool HasAdditionalLoan(long PersonnelID)
        {
            var ret = false;
            var parameters = new Dictionary<string, object>
            {
                { "@PersonnelID", PersonnelID}
            };

            using (var db = new DBTools())
            {
                var result = db.ExecuteScalar("cnb.HasAdditionalLoanForApproval", parameters);
                ret = (result.ToNullableInt() ?? 0) > 0;
            }
            return ret;
        }
    }
}
