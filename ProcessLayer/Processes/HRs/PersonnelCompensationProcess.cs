using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class PersonnelCompensationProcess
    {
        internal static PersonnelCompensation Converter(DataRow dr)
        {
            return new PersonnelCompensation
            {
                ID = dr[PersonnelCompensationFields.ID].ToInt(),
                PersonnelID = dr[PersonnelCompensationFields.PersonnelID].ToNullableLong(),
                CompensationID = dr[PersonnelCompensationFields.CompensationID].ToNullableInt(),
                CurrencyID = dr[PersonnelCompensationFields.CurrencyID].ToNullableInt(),
                Amount = dr[PersonnelCompensationFields.Amount].ToNullableDecimal(),
                _Compensation = LookupProcess.GetCompensation(dr[PersonnelCompensationFields.CompensationID].ToNullableInt()),
                _Currency = LookupProcess.GetCurrency(dr[PersonnelCompensationFields.CurrencyID].ToNullableInt())
            };
        }

        public static List<PersonnelCompensation> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelCompensation>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelCompensationParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelCompensationProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static PersonnelCompensation Get(long Id)
        {
            var eb = new PersonnelCompensation();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelCompensationParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelCompensationProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }


        public static PersonnelCompensation CreateOrUpdate(PersonnelCompensation personnelCompensation, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelCompensationParameters.PersonnelID, personnelCompensation.PersonnelID },
                { PersonnelCompensationParameters.CompensationID, personnelCompensation.CompensationID },
                { PersonnelCompensationParameters.CurrencyID, personnelCompensation.CurrencyID },
                { PersonnelCompensationParameters.Amount, personnelCompensation.Amount },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelCompensationParameters.ID, SqlDbType.BigInt, personnelCompensation.ID}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelCompensationProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                personnelCompensation.ID = OutParameters.Get(PersonnelCompensationParameters.ID).ToLong();

                personnelCompensation._Compensation = LookupProcess.GetCompensation(personnelCompensation.CompensationID);
                personnelCompensation._Currency = LookupProcess.GetCurrency(personnelCompensation.CurrencyID);
            }

            return personnelCompensation;
        }

        //public static PersonnelCompensation Create(PersonnelCompensation personnelCompensation, int userid)
        //{
        //    var Parameters = new Dictionary<string, object>
        //    {
        //        { PersonnelCompensationParameters.PersonnelID, personnelCompensation.PersonnelID },
        //        { PersonnelCompensationParameters.CompensationID, personnelCompensation.CompensationID },
        //        { PersonnelCompensationParameters.CurrencyID, personnelCompensation.CurrencyID },
        //        { PersonnelCompensationParameters.Amount, personnelCompensation.Amount },
        //        { CredentialParameters.LogBy, userid }
        //    };

        //    var OutParameters = new List<OutParameters>
        //    {
        //        { PersonnelCompensationParameters.ID, SqlDbType.BigInt}
        //    };

        //    using (var db = new DBTools())
        //    {
        //        db.ExecuteNonQuery(PersonnelCompensationProcedures.Create, ref OutParameters, Parameters);
        //        personnelCompensation.ID = OutParameters.Get(PersonnelCompensationParameters.ID).ToLong();

        //        personnelCompensation._Compensation = LookupProcess.GetCompensation(personnelCompensation.CompensationID);
        //        personnelCompensation._Currency = LookupProcess.GetCurrency(personnelCompensation.CurrencyID);
        //    }

        //    return personnelCompensation;
        //}

        //public static PersonnelCompensation Update(PersonnelCompensation personnelCompensation, int userid)
        //{
        //    var Parameters = new Dictionary<string, object>
        //    {
        //        { PersonnelCompensationParameters.ID, personnelCompensation.ID },
        //        { PersonnelCompensationParameters.PersonnelID, personnelCompensation.PersonnelID },
        //        { PersonnelCompensationParameters.CompensationID, personnelCompensation.CompensationID },
        //        { PersonnelCompensationParameters.CurrencyID, personnelCompensation.CurrencyID },
        //        { PersonnelCompensationParameters.Amount, personnelCompensation.Amount },
        //        { CredentialParameters.LogBy, userid }
        //    };

        //    using (var db = new DBTools())
        //    {
        //        db.ExecuteNonQuery(PersonnelCompensationProcedures.Update, Parameters);

        //        personnelCompensation._Compensation = LookupProcess.GetCompensation(personnelCompensation.CompensationID);
        //        personnelCompensation._Currency = LookupProcess.GetCurrency(personnelCompensation.CurrencyID);
        //    }

        //    return personnelCompensation;
        //}

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelCompensationFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelCompensationProcedures.Delete, Parameters);
            }
        }
    }
}
