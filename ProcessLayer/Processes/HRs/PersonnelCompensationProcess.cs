using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System;

namespace ProcessLayer.Processes
{
    public sealed class PersonnelCompensationProcess
    {
        public static readonly Lazy<PersonnelCompensationProcess> Instance = new Lazy<PersonnelCompensationProcess>(() => new PersonnelCompensationProcess());
        private PersonnelCompensationProcess() { }
        internal PersonnelCompensation Converter(DataRow dr)
        {
            var pc = new PersonnelCompensation
            {
                ID = dr[PersonnelCompensationFields.ID].ToInt(),
                PersonnelID = dr[PersonnelCompensationFields.PersonnelID].ToNullableLong(),
                CompensationID = dr[PersonnelCompensationFields.CompensationID].ToNullableInt(),
                CurrencyID = dr[PersonnelCompensationFields.CurrencyID].ToNullableInt(),
                Amount = dr[PersonnelCompensationFields.Amount].ToNullableDecimal(),
                NewAmount = dr["New Amount"].ToNullableDecimal(),
                ApprovalDate = dr["Approval Date"].ToNullableDateTime(),
                DisapprovalDate = dr["Disapproval Date"].ToNullableDateTime(),
                Remarks = dr["Remarks"].ToString(),
                _Compensation = LookupProcess.GetCompensation(dr[PersonnelCompensationFields.CompensationID].ToNullableInt()),
                _Currency = LookupProcess.GetCurrency(dr[PersonnelCompensationFields.CurrencyID].ToNullableInt())
            };

            try { pc.FullName = dr["FullName"].ToString(); }
            catch { }

            return pc;
        }
        public List<PersonnelCompensation> GetByPersonnelID(long? PersonnelID = null)
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
        public PersonnelCompensation Get(long Id)
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
        public PersonnelCompensation CreateOrUpdate(PersonnelCompensation personnelCompensation, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelCompensationParameters.PersonnelID, personnelCompensation.PersonnelID },
                { PersonnelCompensationParameters.CompensationID, personnelCompensation.CompensationID },
                { PersonnelCompensationParameters.CurrencyID, personnelCompensation.CurrencyID },
                { PersonnelCompensationParameters.Amount, personnelCompensation.NewAmount },
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
        public List<PersonnelCompensation> FilterCompensationToApprove(string filter, int pageNumber, int gridCount, out int pageCount) 
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Filter", filter },
                    { "@PageNumber", pageNumber },
                    { "@GridCount", gridCount }
                };

                var outParameters = new List<OutParameters> { 
                    { "@PageCount", SqlDbType.Int },
                };

                using (var ds = db.ExecuteReader("[lookup].[FilterPersonnelCompensationToApprove]", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get("@PageCount").ToInt();
                    return ds.GetList(Converter);
                }
            }
        }
        public void Approve(long id, int userid)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@Approve", true },
                    { "@LogBy", userid }
                };

                db.ExecuteNonQuery("hr.ApproveOrDisapprovePersonnelCompensation", parameters);
            }
        }
        public void Disapprove(long id, string remarks, int userid)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@Approve", false },
                    { "@Remarks", remarks },
                    { "@LogBy", userid }
                };

                db.ExecuteNonQuery("hr.ApproveOrDisapprovePersonnelCompensation", parameters);
            }
        }
        public void Delete(long Id, int userid)
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
