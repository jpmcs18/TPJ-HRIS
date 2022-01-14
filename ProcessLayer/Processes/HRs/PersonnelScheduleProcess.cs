using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.HRs
{
    public sealed class PersonnelScheduleProcess
    {
        public static readonly Lazy<PersonnelScheduleProcess> Instance = new Lazy<PersonnelScheduleProcess>(() => new PersonnelScheduleProcess());
        private PersonnelScheduleProcess() { }
        internal PersonnelSchedule Converter(DataRow dr)
        {
            var sched = new PersonnelSchedule
            {
                ID = dr["ID"].ToLong(),
                PersonnelID = dr["Personnel ID"].ToLong(),
                SundayScheduleID = dr["Sunday Schedule ID"].ToNullableInt(),
                MondayScheduleID = dr["Monday Schedule ID"].ToNullableInt(),
                TuesdayScheduleID = dr["Tuesday Schedule ID"].ToNullableInt(),
                WednesdayScheduleID = dr["Wednesday Schedule ID"].ToNullableInt(),
                ThursdayScheduleID = dr["Thursday Schedule ID"].ToNullableInt(),
                FridayScheduleID = dr["Friday Schedule ID"].ToNullableInt(),
                SaturdayScheduleID = dr["Saturday Schedule ID"].ToNullableInt(),
                EffectivityDate = dr["Effectivity Date"].ToNullableDateTime()
            };

            sched._SundaySchedule = ScheduleTypeProcess.Instance.Value.Get(sched.SundayScheduleID);
            sched._MondaySchedule = ScheduleTypeProcess.Instance.Value.Get(sched.MondayScheduleID);
            sched._TuesdaySchedule = ScheduleTypeProcess.Instance.Value.Get(sched.TuesdayScheduleID);
            sched._WednesdaySchedule = ScheduleTypeProcess.Instance.Value.Get(sched.WednesdayScheduleID);
            sched._ThursdaySchedule = ScheduleTypeProcess.Instance.Value.Get(sched.ThursdayScheduleID);
            sched._FridaySchedule = ScheduleTypeProcess.Instance.Value.Get(sched.FridayScheduleID);
            sched._SaturdaySchedule = ScheduleTypeProcess.Instance.Value.Get(sched.SaturdayScheduleID);

            return sched;
        }

        public PersonnelSchedule Get(long personnelScheduleId)
        {
            var parameters = new Dictionary<string, object> {
                {"@ID", personnelScheduleId}
            };

            using (var db = new DBTools())
            {
                using(var ds = db.ExecuteReader("[hr].[GetPersonnelSchedule]", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
        public List<PersonnelSchedule> GetList(long personnelId)
        {
            var parameters = new Dictionary<string, object> {
                {"@PersonnelID", personnelId}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[hr].[GetPersonnelSchedule]", parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public List<PersonnelSchedule> GetList(DateTime? startDate, DateTime? endDate)
        {
            var parameters = new Dictionary<string, object> {
                {"@StartDate", startDate},
                {"@EndDate", endDate}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[hr].[GetPersonnelSchedule]", parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public PersonnelSchedule CreateOrUpdate(PersonnelSchedule sched, int userid)
        {
            using (var db = new DBTools())
            {
                return CreateOrUpdate(db, sched, userid);
            }
        }

        public PersonnelSchedule CreateOrUpdate(DBTools db, PersonnelSchedule sched, int userid)
        {
            if (sched == null)
                return null;

            var parameters = new Dictionary<string, object> {
                {"@PersonnelID", sched.PersonnelID}
                , {"@SundayScheduleID", sched.SundayScheduleID}
                , {"@MondayScheduleID" ,sched.MondayScheduleID}
                , {"@TuesdayScheduleID", sched.TuesdayScheduleID}
                , {"@WednesdayScheduleID", sched.WednesdayScheduleID}
                , {"@ThursdayScheduleID ", sched.ThursdayScheduleID}
                , {"@FridayScheduleID", sched.FridayScheduleID}
                , {"@SaturdayScheduleID", sched.SaturdayScheduleID}
                , {"@EffectivityDate", sched.EffectivityDate}
                , {CredentialParameters.LogBy, userid}
            };

            var outParameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.BigInt, sched.ID }
            };

            db.ExecuteNonQuery("[hr].[CreateOrUpdatePersonnelSchedule]", ref outParameters, parameters);
            sched.ID = outParameters.Get("@ID").ToLong();
            
            return sched;
        }

        public void Delete(long id, int userid)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                {"@ID", id }
                , {"@Delete", true }
                , {CredentialParameters.LogBy, userid}
            };

                db.ExecuteNonQuery("[hr].[CreateOrUpdatePersonnelSchedule]", parameters);
            }

        }

    }

    public static class ContactNumberProcess
    {
        internal static ContactNumber Converter(DataRow dr)
        {
            return new ContactNumber {
                ID = dr["ID"].ToLong(),
                ContactNoTypeID = dr["Contact No Type ID"].ToInt(),
                PersonnelID = dr["Personnel ID"].ToLong(),
                Number = dr["Number"].ToString()
            };
        }

        public static ContactNumber Get(long id)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetContactNumber", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public static List<ContactNumber> GetList(long personnelId)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetContactNumber", new Dictionary<string, object> { { "@PersonnelID", personnelId } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public static ContactNumber CreateOrUpdate(ContactNumber contactNumber, int userid)
        {
            using (var db = new DBTools())
            {
                return CreateOrUpdate(db, contactNumber, userid);
            }
        }
        public static ContactNumber CreateOrUpdate(DBTools db, ContactNumber contactNumber, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", contactNumber.PersonnelID },
                { "@ContactNoTypeID", contactNumber.ContactNoTypeID },
                { "@Number", contactNumber.Number },
                { "@LogBy", userid }
            };
            var outparameters = new List<OutParameters> {
                { "@ID", SqlDbType.BigInt, contactNumber.ID }
            };

            db.ExecuteNonQuery("hr.CreateOrUpdateContactNumber", ref outparameters, parameters);

            contactNumber.ID = outparameters.Get("@ID").ToLong();

            return contactNumber;
        }
        public static void Delete(DBTools db, long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", id },
                { "@Delete", true },
                { "@LogBy", userid }
            };

                db.ExecuteNonQuery("hr.CreateOrUpdateContactNumber", parameters);
            
        }
        public static void Delete(long id, int userid)
        {
            using (var db = new DBTools())
            {
                Delete(db, id, userid);
            }
        }
    }
}
