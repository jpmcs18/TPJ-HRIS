using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessLayer.Entities;
using ProcessLayer.Processes;

namespace ProcessLayer.UnitTest
{
    [TestClass]
    public class PersonnelProcessTest
    {
        [TestMethod]
        public void CreatePersonalInfo()
        {
            Personnel p = new Personnel()
            {
                EmployeeNo = PersonnelProcess.GetEmployeeNumber(),
                FirstName = "John Paul",
                MiddleName = "Ko",
                LastName = "Manlangit",
                Gender = true,
                DateHired = new DateTime(2017, 07, 17),
                BirthDate = new DateTime(1995, 12, 18),
                BirthPlace = "Manila",
                Address = "53 Galicia St. Bangkulasi Navotas City",
                CivilStatusID = 1,
                ReligionID = 1,
                SSS = "3460107333",
                TIN = "329282271",
                Philhealth = "20511694877",
                PAGIBIG = "121173065144",
                Email = "johnpaul.manlangit@outlook.com",
                EmergencyContactPerson = "Marjorie Cual",
                EmergencyContactNumber = "09169092337",
                EmergencyContactAddress = "53 Galicia St. Bangkulasi Navotas City"
            };

            var res = PersonnelProcess.CreatePersonalInfo(p, 1);

            Assert.IsNotNull(res.ID);
        }

        [TestMethod]
        public void UpdatePersonalInfo()
        {
            Personnel p = new Personnel()
            {
                ID = 11012,
                EmployeeNo = "2019-02-11012",
                FirstName = "John Paul",
                MiddleName = "Ko",
                LastName = "Manlangit",
                Gender = true,
                DateHired = new DateTime(2017, 07, 17),
                BirthDate = new DateTime(1995, 12, 18),
                BirthPlace = "Manila",
                Address = "53 Galicia St. Bangkulasi Navotas City",
                CivilStatusID = 1,
                ReligionID = 1,
                SSS = "3460107333",
                TIN = "329282271",
                Philhealth = "20511694877",
                PAGIBIG = "121173065144",
                Email = "johnpaul.manlangit@outlook.com",
                EmergencyContactPerson = "Marjorie Cual",
                EmergencyContactNumber = "09169092337",
                EmergencyContactAddress = "53 Galicia St. Bangkulasi Navotas City",
                SpouseFirstName = "Marjorie",
                SpouseMiddleName = "",
                SpouseLastName = "Cual"
            };

            PersonnelProcess.UpdatePersonalInfo(p, 1);
        }

        [TestMethod]
        public void CreateWorkExperience()
        {
            WorkExperience w = new WorkExperience
            {

                PersonnelID = 11012,
                Company = "Lee Systems Technology Ventures Inc.",
                EmploymentType = "Regular",
                Position = "Programmer",
                FromDate = new DateTime(2016, 4, 26),
                ToDate = new DateTime(2017, 7, 15)
            };

            var s = PersonnelProcess.CreateWorkExperience(w, 1);
        }


        [TestMethod]
        public void UpdateWorkExperience()
        {
            WorkExperience w = new WorkExperience
            {

                PersonnelID = 11012,
                Company = "Lee Systems Technology Ventures Inc.",
                EmploymentType = "Regular",
                Position = "Web Programmer",
                FromDate = new DateTime(2016, 4, 26),
                ToDate = new DateTime(2017, 7, 15)
            };

            PersonnelProcess.UpdateWorkExperience(w, 1);
        }

        [TestMethod]
        public void CreateEducationalBackground()
        {
            EducationalBackground e = new EducationalBackground
            {

                PersonnelID = 11012,
                EducationalLevelID = 3,
                SchoolName = "Navotas Polytechnic College",
                Course = "Bachelor of Science in Computer Science",
                FromYear = 2012,
                ToYear = 2016
            };

            var s = PersonnelProcess.CreateEducationalBackground(e, 1);
        }

        [TestMethod]
        public void UpdateEducationalBackground()
        {
            EducationalBackground e = new EducationalBackground
            {
                ID = 3,
                PersonnelID = 11012,
                EducationalLevelID = 1,
                SchoolName = "Towerville Elementary School",
                FromYear = 2002,
                ToYear = 2008
            };

            PersonnelProcess.UpdateEducationalBackground(e, 1);
        }
    }
}
