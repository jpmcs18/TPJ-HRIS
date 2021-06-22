using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System.IO;
using System.Configuration;
using System;

namespace ProcessLayer.Processes
{
    public static class PersonnelLegislationProcess
    {
        internal static PersonnelLegislation Converter(DataRow dr)
        {
            var l = new PersonnelLegislation
            {
                ID = dr[PersonnelLegislationFields.ID].ToInt(),
                PersonnelID = dr[PersonnelLegislationFields.PersonnelID].ToNullableLong(),
                File = dr[PersonnelLegislationFields.File].ToString(),
                LegislationDate = dr[PersonnelLegislationFields.LegislationDate].ToNullableDateTime(),
                LegislationStatusID = dr[PersonnelLegislationFields.LegislationStatusID].ToNullableInt(),
                StatusDate = dr[PersonnelLegislationFields.StatusDate].ToNullableDateTime(),
                Title = dr[PersonnelLegislationFields.Title].ToString(),
                _LegislationStatus = LookupProcess.GetLegislationStatus(dr[PersonnelLegislationFields.LegislationStatusID].ToNullableInt())
            };


            if (!String.IsNullOrEmpty(l.File))
            {
                l.File = Path.Combine(ConfigurationManager.AppSettings["LegislationFolder"], l.File);
            }
            return l;
        }

        public static List<PersonnelLegislation> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelLegislation>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelLegislationParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelLegislationProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static PersonnelLegislation Get(long Id)
        {
            var eb = new PersonnelLegislation();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLegislationParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelLegislationProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public static PersonnelLegislation Create(PersonnelLegislation personnelLegislation, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLegislationParameters.PersonnelID, personnelLegislation.PersonnelID },
                { PersonnelLegislationParameters.File, personnelLegislation.File },
                { PersonnelLegislationParameters.LegislationDate, personnelLegislation.LegislationDate },
                { PersonnelLegislationParameters.LegislationStatusID, personnelLegislation.LegislationStatusID },
                { PersonnelLegislationParameters.StatusDate, personnelLegislation.StatusDate },
                { PersonnelLegislationParameters.Title, personnelLegislation.Title },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelLegislationParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLegislationProcedures.Create, ref OutParameters, Parameters);
                personnelLegislation.ID = OutParameters.Get(PersonnelLegislationParameters.ID).ToLong();
                personnelLegislation._LegislationStatus = LookupProcess.GetLegislationStatus(personnelLegislation.LegislationStatusID);
            }

            return personnelLegislation;
        }

        public static PersonnelLegislation Update(PersonnelLegislation personnelLegislation, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLegislationParameters.ID, personnelLegislation.ID },
                { PersonnelLegislationParameters.PersonnelID, personnelLegislation.PersonnelID },
                { PersonnelLegislationParameters.File, personnelLegislation.File },
                { PersonnelLegislationParameters.LegislationDate, personnelLegislation.LegislationDate },
                { PersonnelLegislationParameters.LegislationStatusID, personnelLegislation.LegislationStatusID },
                { PersonnelLegislationParameters.StatusDate, personnelLegislation.StatusDate },
                { PersonnelLegislationParameters.Title, personnelLegislation.Title },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLegislationProcedures.Update, Parameters);
                personnelLegislation._LegislationStatus = LookupProcess.GetLegislationStatus(personnelLegislation.LegislationStatusID);
            }
            return personnelLegislation;
        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLegislationFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLegislationProcedures.Delete, Parameters);
            }
        }
    }
}
