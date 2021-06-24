using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class PersonnelDeductionProcess
    {
        internal static PersonnelDeduction Converter(DataRow dr)
        {
            return new PersonnelDeduction
            {
                ID = dr[PersonnelDeductionFields.ID].ToInt(),
                PersonnelID = dr[PersonnelDeductionFields.PersonnelID].ToNullableLong(),
                DeductionID = dr[PersonnelDeductionFields.DeductionID].ToNullableInt(),
                CurrencyID = dr[PersonnelDeductionFields.CurrencyID].ToNullableInt(),
                Amount = dr[PersonnelDeductionFields.Amount].ToNullableDecimal(),
                _Deduction = LookupProcess.GetDeduction(dr[PersonnelDeductionFields.DeductionID].ToNullableInt()),
                _Currency = LookupProcess.GetCurrency(dr[PersonnelDeductionFields.CurrencyID].ToNullableInt())
            };
        }
        internal static AssumedPersonnelDeduction APDConverter(DataRow dr)
        {
            return new AssumedPersonnelDeduction
            {
                Description = dr["Desc"].ToString(),
                PS = dr["PS"].ToNullableDecimal() ?? 0,
                ES = dr["ES"].ToNullableDecimal() ?? 0,
                EC = dr["EC"].ToNullableDecimal() ?? 0,
                Columns = dr["Col"].ToNullableInt() ?? 0
            };
        }
        public static List<AssumedPersonnelDeduction> GetAssumed(long personnelID)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetAssumedPersonnelDeduction", new Dictionary<string, object> { { "@PersonnelID", personnelID } }))
                {
                    return ds.GetList(APDConverter);
                }
            }
        }

        public static List<PersonnelDeduction> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelDeduction>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelDeductionParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelDeductionProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static PersonnelDeduction Get(long Id)
        {
            var eb = new PersonnelDeduction();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDeductionParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelDeductionProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public static PersonnelDeduction CreateOrUpdate(PersonnelDeduction personnelDeduction, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDeductionParameters.PersonnelID, personnelDeduction.PersonnelID },
                { PersonnelDeductionParameters.DeductionID, personnelDeduction.DeductionID },
                { PersonnelDeductionParameters.CurrencyID, personnelDeduction.CurrencyID },
                { PersonnelDeductionParameters.Amount, personnelDeduction.Amount },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelDeductionParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDeductionProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                personnelDeduction.ID = OutParameters.Get(PersonnelDeductionParameters.ID).ToLong();
                personnelDeduction._Deduction = LookupProcess.GetDeduction(personnelDeduction.DeductionID);
                personnelDeduction._Currency = LookupProcess.GetCurrency(personnelDeduction.CurrencyID);
            }

            return personnelDeduction;
        }

        public static PersonnelDeduction Create(PersonnelDeduction personnelDeduction, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDeductionParameters.PersonnelID, personnelDeduction.PersonnelID },
                { PersonnelDeductionParameters.DeductionID, personnelDeduction.DeductionID },
                { PersonnelDeductionParameters.CurrencyID, personnelDeduction.CurrencyID },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelDeductionParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDeductionProcedures.Create, ref OutParameters, Parameters);
                personnelDeduction.ID = OutParameters.Get(PersonnelDeductionParameters.ID).ToLong();
                personnelDeduction._Deduction = LookupProcess.GetDeduction(personnelDeduction.DeductionID);
                personnelDeduction._Currency = LookupProcess.GetCurrency(personnelDeduction.CurrencyID);
            }

            return personnelDeduction;
        }

        public static PersonnelDeduction Update(PersonnelDeduction personnelDeduction, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDeductionParameters.ID, personnelDeduction.ID },
                { PersonnelDeductionParameters.PersonnelID, personnelDeduction.PersonnelID },
                { PersonnelDeductionParameters.DeductionID, personnelDeduction.DeductionID },
                { PersonnelDeductionParameters.CurrencyID, personnelDeduction.CurrencyID },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDeductionProcedures.Update, Parameters);
                personnelDeduction._Deduction = LookupProcess.GetDeduction(personnelDeduction.DeductionID);
                personnelDeduction._Currency = LookupProcess.GetCurrency(personnelDeduction.CurrencyID);
            }
            return personnelDeduction;

        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDeductionFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDeductionProcedures.Delete, Parameters);
            }
        }
    }
}
