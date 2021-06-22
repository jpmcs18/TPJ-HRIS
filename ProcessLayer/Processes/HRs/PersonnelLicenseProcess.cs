using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class PersonnelLicenseProcess
    {
        private static bool LicenseOnly { get; set; } = false;
        internal static PersonnelLicense Converter(DataRow dr)
        {
            var l = new PersonnelLicense
            {
                ID = dr[PersonnelLicenseFields.ID].ToInt(),
                PersonnelID = dr[PersonnelLicenseFields.PersonnelID].ToNullableLong(),
                ExpirationDate = dr[PersonnelLicenseFields.ExpirationDate].ToNullableDateTime(),
                LicenseNo = dr[PersonnelLicenseFields.LicenseNo].ToString(),
                LicenseTypeID = dr[PersonnelLicenseFields.LicenseTypeID].ToNullableInt()
            };

            l._LicenseType = LookupProcess.GetLicenseType(dr[PersonnelLicenseFields.LicenseTypeID].ToNullableInt());

            if (!LicenseOnly)
            {
                l._Personnel = PersonnelProcess.Get(l.PersonnelID ?? 0, true);
            }

            return l;
        }

        public static List<PersonnelLicense> GetByPersonnelID(long? PersonnelID = null, bool licenseOnly = false)
        {
            var eb = new List<PersonnelLicense>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelLicenseParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    LicenseOnly = licenseOnly;
                    using (var ds = db.ExecuteReader(PersonnelLicenseProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                    LicenseOnly = false;
                }
            }

            return eb;
        }

        public static PersonnelLicense Get(long Id, bool licenseOnly = false)
        {
            var eb = new PersonnelLicense();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLicenseParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelLicenseProcedures.Get, Parameters))
                {
                    LicenseOnly = licenseOnly;
                    eb = ds.Get(Converter);
                    LicenseOnly = false;
                }
            }

            return eb;
        }

        public static PersonnelLicense Create(PersonnelLicense personnelLicense, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLicenseParameters.PersonnelID, personnelLicense.PersonnelID },
                { PersonnelLicenseParameters.ExpirationDate, personnelLicense.ExpirationDate },
                { PersonnelLicenseParameters.LicenseNo, personnelLicense.LicenseNo },
                { PersonnelLicenseParameters.LicenseTypeID, personnelLicense.LicenseTypeID },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelLicenseParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLicenseProcedures.Create, ref OutParameters, Parameters);
                personnelLicense.ID = OutParameters.Get(PersonnelLicenseParameters.ID).ToLong();
                personnelLicense._LicenseType = LookupProcess.GetLicenseType(personnelLicense.LicenseTypeID);
            }

            return personnelLicense;
        }

        public static PersonnelLicense Update(PersonnelLicense personnelLicense, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLicenseParameters.ID, personnelLicense.ID },
                { PersonnelLicenseParameters.PersonnelID, personnelLicense.PersonnelID },
                { PersonnelLicenseParameters.ExpirationDate, personnelLicense.ExpirationDate },
                { PersonnelLicenseParameters.LicenseNo, personnelLicense.LicenseNo },
                { PersonnelLicenseParameters.LicenseTypeID, personnelLicense.LicenseTypeID },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLicenseProcedures.Update, Parameters);
                personnelLicense._LicenseType = LookupProcess.GetLicenseType(personnelLicense.LicenseTypeID);
            }
            return personnelLicense;

        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLicenseFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLicenseProcedures.Delete, Parameters);
            }
        }

        public static IEnumerable<PersonnelLicense> GetExpiringLicensesThisDay()
        {
            var lic = new List<PersonnelLicense>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetExpiringLicenseProcedures.GetExpiringLicensesThisDay))
                {
                    LicenseOnly = false;
                    lic = ds.GetList(Converter);
                }
            }

            return lic;
        }

        public static IEnumerable<PersonnelLicense> GetExpiringLicensesThisMonthRecent()
        {
            var lic = new List<PersonnelLicense>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetExpiringLicenseProcedures.GetExpiringLicensesThisMonthRecent))
                {
                    LicenseOnly = false;
                    lic = ds.GetList(Converter);
                }
            }

            return lic;
        }

        public static IEnumerable<PersonnelLicense> GetExpiringLicensesThisMonthUpcoming()
        {
            var lic = new List<PersonnelLicense>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetExpiringLicenseProcedures.GetExpiringLicensesThisMonthUpcoming))
                {
                    LicenseOnly = false;
                    lic = ds.GetList(Converter);
                }
            }

            return lic;
        }
    }
}
