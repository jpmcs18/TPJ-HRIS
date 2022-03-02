using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using P = WebTemplate.Models.Personnel;
using ProcessLayer.Helpers;
using System.IO;
using System.Configuration;
using ProcessLayer.Processes.Kiosk;

namespace WebTemplate.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index(P.Index model)
        {
            model.BirthdayCelebrantsRecent = PersonnelProcess.GetBirthdayCelebrantsThisMonthRecent().ToList();
            model.BirthdayCelebrantsToday = PersonnelProcess.GetBirthdayCelebrantsThisDay().ToList();
            model.BirthdayCelebrantsUpcoming = PersonnelProcess.GetBirthdayCelebrantsThisMonthUpcoming().ToList();

            model.ExpiringLicensesRecent = PersonnelLicenseProcess.GetExpiringLicensesThisMonthRecent().ToList();
            model.ExpiringLicensesToday = PersonnelLicenseProcess.GetExpiringLicensesThisDay().ToList();
            model.ExpiringLicensesUpcoming = PersonnelLicenseProcess.GetExpiringLicensesThisMonthUpcoming().ToList();

            model.KioskNotifications = KioskNotificationProcess.Instance.GetList(User.UserID)?.Where(c => c.Count > 0)?.ToList();

            if (Request.IsAjaxRequest())
            {
                return PartialViewCustom("Index", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }
    }
}