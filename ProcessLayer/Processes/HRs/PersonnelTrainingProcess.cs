using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class PersonnelTrainingProcess
    {
        internal static PersonnelTraining Converter(DataRow dr)
        {
            return new PersonnelTraining
            {
                ID = dr[PersonnelTrainingFields.ID].ToInt(),
                PersonnelID = dr[PersonnelTrainingFields.PersonnelID].ToNullableLong(),
                TrainingDate = dr[PersonnelTrainingFields.TrainingDate].ToNullableDateTime(),
                TrainingProvider = dr[PersonnelTrainingFields.TrainingProvider].ToString(),
                Title = dr[PersonnelTrainingFields.Title].ToString(),
                TrainingTypeID = dr[PersonnelTrainingFields.TrainingTypeID].ToNullableInt(),
                _TrainingType = LookupProcess.GetTrainingType(dr[PersonnelTrainingFields.TrainingTypeID].ToNullableInt())
            };
        }

        public static List<PersonnelTraining> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelTraining>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelTrainingParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelTrainingProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static PersonnelTraining Get(long Id)
        {
            var eb = new PersonnelTraining();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelTrainingParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelTrainingProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public static PersonnelTraining Create(PersonnelTraining Training, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelTrainingParameters.PersonnelID, Training.PersonnelID },
                { PersonnelTrainingParameters.Title, Training.Title },
                { PersonnelTrainingParameters.TrainingDate, Training.TrainingDate },
                { PersonnelTrainingParameters.TrainingProvider, Training.TrainingProvider },
                { PersonnelTrainingParameters.TrainingTypeID, Training.TrainingTypeID },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelTrainingParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelTrainingProcedures.Create, ref OutParameters, Parameters);
                Training.ID = OutParameters.Get(PersonnelTrainingParameters.ID).ToLong();
                Training._TrainingType = LookupProcess.GetTrainingType(Training.TrainingTypeID);
            }

            return Training;
        }

        public static PersonnelTraining Update(PersonnelTraining Training, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelTrainingParameters.ID, Training.ID },
                { PersonnelTrainingParameters.PersonnelID, Training.PersonnelID },
                { PersonnelTrainingParameters.Title, Training.Title },
                { PersonnelTrainingParameters.TrainingDate, Training.TrainingDate },
                { PersonnelTrainingParameters.TrainingProvider, Training.TrainingProvider },
                { PersonnelTrainingParameters.TrainingTypeID, Training.TrainingTypeID },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelTrainingProcedures.Update, Parameters);
                Training._TrainingType = LookupProcess.GetTrainingType(Training.TrainingTypeID);
            }
            return Training;

        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelTrainingFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelTrainingProcedures.Delete, Parameters);
            }
        }
    }
}
