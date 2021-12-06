using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.HR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Helpers;
using WebTemplate.Models.MemoArchives.Infraction;

namespace WebTemplate.Controllers.MemoArchives
{
    public class InfractionController : BaseController
    {
        private const string CONTENT = "Infraction";
        private const string FOLDER = "MemoFolder";
        private const string SAVE_LOCATION = "MemoSaveLocation";

        // GET: MemoArchive
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Infractions = InfractionProcess.Instance.Value.Filter(model.Personnel, model.Date, model.StatusID, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_Search", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMemo()
        {
            Management model = new()
            {
                Infraction = new Infraction()
            };

            return PartialViewCustom("_Management", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPersonnel(string filter)
        {
            try
            {
                Management model = new Management
                {
                    Personnels = PersonnelProcess.GetList(filter),
                    PersonnelSearchKey = filter
                };
                if (model.Personnels != null)
                {
                    foreach (var personnel in model.Personnels)
                    {
                        personnel._Departments = PersonnelDepartmentProcess.GetList(personnel.ID);
                        personnel._Positions = PersonnelPositionProcess.GetList(personnel.ID);
                    }
                }

                return PartialViewCustom("_SearchPersonnel", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contents(long id)
        {
            try
            {
                Infraction model = InfractionProcess.Instance.Value.Get(id);

                if (model != null)
                {
                    return PartialViewCustom("_Replies", model);
                }
                else
                {
                    return Json(new { msg = false, res = "Infraction not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Infraction infraction, bool sendEmail, HttpPostedFileBase incidentReportFile, HttpPostedFileBase fileBase)
        {
            try
            {

                if (infraction.PersonnelID > 0)
                {
                    var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                    var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                    InfractionContent incidentReport = new()
                    {
                        SaveOnly = !sendEmail,
                        File = incidentReportFile.SaveFile(directory, $"{infraction.MemoNo}{DateTime.Now:MMddyyyyHHmmss}_0{Path.GetExtension(incidentReportFile.FileName)}")
                    };
                    InfractionContent content = new()
                    {
                        SaveOnly = !sendEmail,
                        File = fileBase.SaveFile(directory, $"{infraction.MemoNo}{DateTime.Now:MMddyyyyHHmmss}_1{Path.GetExtension(fileBase.FileName)}")
                    };

                    infraction.Content = new List<InfractionContent>
                    {
                        incidentReport,
                        content
                    };
                    infraction = InfractionProcess.Instance.Value.CreateOrUpdate(infraction, User.UserID);
                    infraction = InfractionProcess.Instance.Value.Get(infraction.ID);
                }
                else
                {
                    return Json(new { msg = false, res = "No recipient found. Not saved." });
                }
                ModelState.Clear();
                ViewBag.ContentId = string.Join(",", infraction.Content.Select(x => x.ID));
                return PartialViewCustom("_Replies", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResaveContent(long id, long infractionId, HttpPostedFileBase file)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var infraction = InfractionProcess.Instance.Value.Get(infractionId, true);
                string filePath = file.SaveFile(directory, $"{infraction.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(file.FileName)}");
                InfractionContentProcess.Instance.Value.UpdateFile(id, filePath, User.UserID);
                infraction = InfractionProcess.Instance.Value.Get(infractionId);
                ModelState.Clear();
                return PartialViewCustom("_Replies", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReSave(Infraction infraction)
        {
            try
            {
                if (infraction.ID > 0)
                {
                    infraction = InfractionProcess.Instance.Value.CreateOrUpdate(infraction, User.UserID);
                    infraction = InfractionProcess.Instance.Value.Get(infraction.ID);
                }
                else
                {
                    return Json(new { msg = false, res = "Invalid infraction. Not saved." });
                }
                ModelState.Clear();
                ViewBag.ContentId = string.Join(",", infraction.Content.Select(x => x.ID));
                return PartialViewCustom("_Replies", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFileContent(InfractionContent content, bool sendEmail, HttpPostedFileBase fileBase)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var infraction = InfractionProcess.Instance.Value.Get(content.InfractionID, true);
                content.File = fileBase.SaveFile(directory, $"{infraction.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(fileBase.FileName)}");
                content.SaveOnly = !sendEmail;
                content = InfractionContentProcess.Instance.Value.Create(content, User.UserID);
                infraction = InfractionProcess.Instance.Value.Get(content.InfractionID);

                ModelState.Clear();
                ViewBag.ContentId = infraction.Content.LastOrDefault()?.ID;
                return PartialViewCustom("_Replies", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleHearing(long id, DateTime hearingSchedule, bool sendEmail, bool resched = false)
        {
            try
            {

                if(resched)
                    InfractionProcess.Instance.Value.SetHearingStatus(id, 2, User.UserID, hearingSchedule);
                else
                    InfractionProcess.Instance.Value.ScheduleHearing(id, hearingSchedule, User.UserID);
                var content = new InfractionContent
                {
                    Message = $"Hearing {(resched ? "Reschedule" : "Schedule")} : {hearingSchedule:MMMM dd, yyyy}",
                    InfractionID = id,
                    SaveOnly = !sendEmail
                };

                content = InfractionContentProcess.Instance.Value.Create(content, User.UserID);

                var infraction = InfractionProcess.Instance.Value.Get(id);

                ModelState.Clear();
                ViewBag.ContentId = infraction.Content.LastOrDefault()?.ID;
                return PartialViewCustom("_Replies", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoneHearing(long id)
        {
            try
            {
                InfractionProcess.Instance.Value.SetHearingStatus(id, 1, User.UserID);
                var infraction = InfractionProcess.Instance.Value.Get(id);

                ModelState.Clear();
                return PartialViewCustom("_Replies", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CloseInfraction(long id, short sanction, DateTime? date, int? days)
        {
            try
            {
                InfractionProcess.Instance.Value.Close(id, sanction, date, days, User.UserID);
                var infraction = InfractionProcess.Instance.Value.Get(id);
                

                ModelState.Clear();
                return PartialViewCustom("_Replies", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            try
            {
                InfractionProcess.Instance.Value.Delete(id, User.UserID);
                return Json(new { msg = true, res = "Deleted" });

            }
            catch(Exception ex)
            {

                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(long id, long infractionId)
        {
            try
            {

                InfractionContentProcess.Instance.Value.Delete(id, User.UserID);
                var infraction = InfractionProcess.Instance.Value.Get(infractionId);


                ModelState.Clear();
                return PartialViewCustom("_Replies", infraction);

            }
            catch (Exception ex)
            {

                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmailToPersonnel(string contentId)
        {
            try
            {
                if(contentId == null)
                    return Json(new { msg = false, res = "Nothing to send." });

                if (!Web.HasInternetConnection())
                    return Json(new { msg = false, res = "No Internet Connection." });

                List<InfractionContent> infractionContents = InfractionContentProcess.Instance.Value.GetList(contentId);
                Infraction infraction = InfractionProcess.Instance.Value.Get(infractionContents.FirstOrDefault()?.InfractionID ?? 0, true);

                if (string.IsNullOrEmpty(infraction.Personnel.Email))
                    return Json(new { msg = false, res = infraction.Personnel.FullName + " has no email." });

                List<string> files = new List<string>();
                string appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                string directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;

                infractionContents.ForEach(content => {
                    if (System.IO.File.Exists(Path.Combine(directory, Path.GetFileName(content.File))))
                        files.Add(Path.Combine(directory, Path.GetFileName(content.File)));
                });


                var credential = Web.GetMemoEmailCreadential();

                var subject = $"Infraction {infraction.MemoNo} {infraction.Subject}" ?? "(No Subject)";
                EmailResult emailRet = null;
                if (infractionContents.Count > 1)
                {
                    emailRet = EmailUtil.SendEmail(credential, User.UserID, infraction.Personnel.Email, CONTENT, true, files, contentId, subject);
                }
                else
                {
                    var infractionContent = infractionContents.FirstOrDefault();
                    emailRet = EmailUtil.SendEmail(credential, User.UserID, infraction.Personnel.Email, CONTENT, infractionContent.ID, files?.FirstOrDefault(), infractionContent.Message, subject);
                }

                return Json(new { msg = emailRet.IsSuccess, res = emailRet.Message });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

    }
}