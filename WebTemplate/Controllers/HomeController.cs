using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Linq;
using System.Web.Mvc;
using P = WebTemplate.Models.Personnel;

namespace WebTemplate.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index(P.Index model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialViewCustom("Index", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTab(int tabNo)
        {
            try
            {
                P.Index model = new();
                ModelState.Clear();
                if (tabNo == 1)
                {
                    model.BirthdayCelebrantsRecent = PersonnelProcess.GetBirthdayCelebrantsThisMonthRecent().ToList();
                    model.BirthdayCelebrantsToday = PersonnelProcess.GetBirthdayCelebrantsThisDay().ToList();
                    model.BirthdayCelebrantsUpcoming = PersonnelProcess.GetBirthdayCelebrantsThisMonthUpcoming().ToList();

                    return PartialView("~/Views/Home/_BirthdayCelebrants.cshtml", model);
                }
                if (tabNo == 2)
                {
                    model.ExpiringLicensesRecent = PersonnelLicenseProcess.GetExpiringLicensesThisMonthRecent().ToList();
                    model.ExpiringLicensesToday = PersonnelLicenseProcess.GetExpiringLicensesThisDay().ToList();
                    model.ExpiringLicensesUpcoming = PersonnelLicenseProcess.GetExpiringLicensesThisMonthUpcoming().ToList();

                    return PartialView("~/Views/Home/_ExpiringLicenses.cshtml", model);
                }
                else
                {
                    model.KioskNotifications = KioskNotificationProcess.Instance.GetList(User.UserID)?.Where(k => k.Count > 0)?.ToList();

                    return PartialView("~/Views/Home/_KioskNotifications.cshtml", model.KioskNotifications);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
    }
}