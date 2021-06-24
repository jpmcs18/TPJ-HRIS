using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes
{
    public class LeaveTypeProcess
    {
        internal static LeaveType Converter(DataRow dr)
        {
            return new LeaveType
            {
                ID = dr["ID"].ToByte(),
                Description = dr["Description"].ToString(),
                MaxAllowedDays = dr["Max Allowed Days"].ToNullableInt(),
                BulkUse = dr["Bulk Use"].ToNullableBoolean(),
                DaysBeforeRequest = dr["Days Before Request"].ToNullableInt(),
                HasDocumentNeeded = dr["Has Document Needed"].ToNullableBoolean()
            };
        }

        public static List<LeaveType> GetLeaveType()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeaveType"))
                {
                    var l = ds.GetList(Converter);

                    return l;
                }
            }           
        }

        public static LeaveType GetLeaveType(byte? id)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeaveType", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public static List<LeaveType> GetLeaveTypesThatHasDocumentNeeded()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.[GetLeaveTypeThatNeedDocument]"))
                {
                    var l = ds.GetList(Converter);

                    return l;
                }
            }
        }

        public static List<LeaveType> GetLeavesWithCredits(long personnelID, short year)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeavesWithCredits", new Dictionary<string, object> { 
                    { "@PersonnelID", personnelID }, 
                    { "@Year", year } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }

    }
}
