using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using DBUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessLayer.Computation.CnB;
using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Processes;
using ProcessLayer.Processes.CnB;
using WebTemplate.Helpers;

namespace HRTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test()
        {
            var sa = Math.Pow(10, 0);
        }

        [TestMethod]
        public void PangTestNgDate()
        {
            var dateString = "";

            var convertedDate = dateString.ToNullableDateTime();

            Assert.IsNotNull(convertedDate);
        }
        [TestMethod]
        public void GetName()
        {
            var type = PayrollSheet.A;
            var x = type.ToString();

            var y = 0;

            var z = y.ToString("#,###.##");

        }
        [TestMethod]
        public void YOS()
        {
            var start = new DateTime(1995, 12, 18);
            var end = DateTime.Now;

            var year = (end.Year - start.Year - 1) +
                (((end.Month > start.Month) ||
                ((end.Month == start.Month) && (end.Day >= start.Day))) ? 1 : 0);
        }
        [TestMethod]
        public void PayrollComputationTest()
        {
            var payroll = PayrollProcess.Instance.Value.GeneratePayroll(new ProcessLayer.Entities.CnB.PayrollPeriod
            {
                StartDate = new DateTime(2019, 9, 1),
                EndDate = new DateTime(2019, 9, 15),
                PayPeriod = "2019090120190915",
            }, ProcessLayer.Helpers.Enumerable.PayrollSheet.B, 1);

            Assert.IsTrue(payroll.Payrolls.Count > 0);
            //Assert
        }

        [TestMethod]
        public void ValidateExistingPayroll()
        {
            var res = PayrollProcess.Instance.Value.ValidatePayrollGeneration("2019010120190115", ProcessLayer.Helpers.Enumerable.PayrollSheet.B);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void TestScalar()
        {
            using (var db = new DBTools())
            {
                var res = db.ExecuteScalar("cnb.ValidatePayroll", new Dictionary<string, object> { { "@PayPeriod", "1321321321321" } });

                Assert.IsNotNull(res);
            }
        }

        [TestMethod]
        public void PositionTesting()
        {
            var timelog = TimeLogProcess.Get(23, new DateTime(2019, 9, 1), new DateTime(2019, 9, 15));
            var start = new DateTime(2019, 9, 2, 8, 0, 0);
            var end = new DateTime(2019, 9, 2, 17, 0, 0);

            var login = timelog.Where(x => (x.LoginDate >= start && x.LogoutDate <= end)
                                            || (start >= x.LoginDate && end <= x.LogoutDate)
                                            || (start <= x.LoginDate && end <= x.LogoutDate && end >= x.LoginDate)
                                            || (x.LoginDate <= start && x.LogoutDate <= end && x.LogoutDate >= end)).OrderBy(x => x.LoginDate).Select(x => x.LoginDate).FirstOrDefault();
            var logout = timelog.Where(x => (x.LoginDate >= start && x.LogoutDate <= end)
                                            || (start >= x.LoginDate && end <= x.LogoutDate)
                                            || (start <= x.LoginDate && end <= x.LogoutDate && end >= x.LoginDate)
                                            || (x.LoginDate <= start && x.LogoutDate <= end && x.LogoutDate >= end)).OrderByDescending(x => x.LoginDate).Select(x => x.LogoutDate).FirstOrDefault();

            Debug.WriteLine("sample");
        }

        [TestMethod]
        public void TestMethod1()
        {
            //var ot = new OTRequest { Approved = true, ApprovedHours = (decimal)1.5 };

        }

        [TestMethod]
        public void SendEmail()
        {
            using (var helper = new EmailHelper())
            {
                helper.Send();
            }
        }

        [TestMethod]
        public void CreateOrUpdateEmailCredential()
        {
            var pass = "St120n6312".Encrypt();
            var email = new EmailCredential {
                ID = 6,
                Owner = "Memo",
                Email = "sampleemailv2@gmail.com",
                Password = pass
            };

            var e = EmailCredentialProcess.CreateOrUpdate(email);
        }

        [TestMethod]
        public void GetEmailCredential()
        {
            var e = EmailCredentialProcess.Get(1);
            var pass = e.Password.Decrypt();
        }

        [TestMethod]
        public void TestConnection()
        {   
            var e = Web.HasInternetConnection();

        }

        [TestMethod]
        public void TestConnectionState()
        {
            var v = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        [TestMethod]
        public void sss()
        {
            List<Sample> lst = new List<Sample>();
            SetSelected setSelected = null;
            for(int i = 0; i < 10; i++)
            {
                var n = new Sample { num = i };
                setSelected += n.SetSelected;
                lst.Add(n);
            }

            setSelected?.Invoke(n => (n > 5 && n  < 8));
            var newList = lst.Where(x => x.selected).ToList();
            foreach (var nl in newList)
                setSelected -= nl.SetSelected;


            setSelected?.Invoke(n => (n > 1 && n < 5));

        }

        public delegate void SetSelected(Predicate<int> predicate);
    }

    public class Sample
    {
        public int num { get; set; }
        public bool selected { get; set; } = false;
        public void SetSelected(Predicate<int> predicate)
        {
            selected = predicate(num);
        }
    }
}
