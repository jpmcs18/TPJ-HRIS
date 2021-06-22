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
    public static class PersonnelMemoProcess
    {
        internal static PersonnelMemo Converter(DataRow dr)
        {
            return new PersonnelMemo
            {
                ID = dr[PersonnelMemoFields.ID].ToInt(),
                PersonnelID = dr[PersonnelMemoFields.PersonnelID].ToNullableLong(),
                MemoID = dr[PersonnelMemoFields.MemoID].ToNullableLong()
            };
        }

        public static List<PersonnelMemo> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelMemo>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelMemoParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelMemoProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static PersonnelMemo Get(long Id)
        {
            var eb = new PersonnelMemo();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelMemoProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public static PersonnelMemo Create(PersonnelMemo personnelMemo, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.PersonnelID, personnelMemo.PersonnelID },
                { PersonnelMemoParameters.MemoID, personnelMemo.MemoID },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelMemoParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelMemoProcedures.Create, ref OutParameters, Parameters);
                personnelMemo.ID = OutParameters.Get(PersonnelMemoParameters.ID).ToLong();
            }

            return personnelMemo;
        }

        public static PersonnelMemo Update(PersonnelMemo personnelMemo, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.ID, personnelMemo.ID },
                { PersonnelMemoParameters.PersonnelID, personnelMemo.PersonnelID },
                { PersonnelMemoParameters.MemoID, personnelMemo.MemoID },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelMemoProcedures.Update, Parameters);
            }
            return personnelMemo;

        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelMemoProcedures.Delete, Parameters);
            }
        }
    }
}
