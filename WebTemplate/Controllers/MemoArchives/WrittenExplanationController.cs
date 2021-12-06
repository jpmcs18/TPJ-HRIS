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
using WebTemplate.Models.MemoArchives.WrittenExplanation;

namespace WebTemplate.Controllers.MemoArchives
{
    public class WrittenExplanationController : BaseController
    {
        private const string CONTENT = "Written Explanation";
        private const string FOLDER = "MemoFolder";
        private const string SAVE_LOCATION = "MemoSaveLocation";

        // GET: MemoArchive
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.WrittenExplanations = WrittenExplanationProcess.Instance.Value.Filter(model.Personnel, model.Date, model.StatusID, model.Page, model.GridCount, out int PageCount).ToList();
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
                WrittenExplanation = new WrittenExplanation()
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
                WrittenExplanation model = WrittenExplanationProcess.Instance.Value.Get(id);

                if (model != null)
                {
                    return PartialViewCustom("_Replies", model);
                }
                else
                {
                    return Json(new { msg = false, res = "Written Explanation not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(WrittenExplanation writtenExplanation, bool sendEmail, HttpPostedFileBase incidentReportFile, HttpPostedFileBase fileBase)
        {
            try
            {
                if (writtenExplanation.PersonnelID > 0)
                {
                    var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                    var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                    WrittenExplanationContent incidentReport = new()
                    {
                        SaveOnly = !sendEmail,
                        File = incidentReportFile.SaveFile(directory, $"{writtenExplanation.MemoNo}{DateTime.Now:MMddyyyyHHmmss}_0{Path.GetExtension(incidentReportFile.FileName)}")
                    };
                    WrittenExplanationContent content = new()
                    {
                        SaveOnly = !sendEmail,
                        File = fileBase.SaveFile(directory, $"{writtenExplanation.MemoNo}{DateTime.Now:MMddyyyyHHmmss}_1{Path.GetExtension(fileBase.FileName)}")
                    };


                    writtenExplanation.Content = new List<WrittenExplanationContent>
                    {
                        incidentReport,
                        content
                    };
                    writtenExplanation = WrittenExplanationProcess.Instance.Value.CreateOrUpdate(writtenExplanation, User.UserID);
                    writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(writtenExplanation.ID);
                }
                else
                {
                    return Json(new { msg = false, res = "No recipient found. Not saved." });
                }
                ModelState.Clear();
                ViewBag.ContentId = writtenExplanation.Content.LastOrDefault()?.ID;
                return PartialViewCustom("_Replies", writtenExplanation);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFileContent(WrittenExplanationContent content, bool sendEmail, HttpPostedFileBase fileBase)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(content.WrittenExplanationID, true);

                content.File = fileBase.SaveFile(directory, $"{writtenExplanation.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(fileBase.FileName)}");
                content.SaveOnly = !sendEmail;
                content = WrittenExplanationContentProcess.Instance.Value.Create(content, User.UserID);
                writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(content.WrittenExplanationID);

                ModelState.Clear();
                ViewBag.ContentId = writtenExplanation.Content.LastOrDefault()?.ID;
                return PartialViewCustom("_Replies", writtenExplanation);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleConsultation(long id, DateTime consultationSchedule, bool sendEmail, bool resched = false)
        {
            try
            {

                if(resched)
                    WrittenExplanationProcess.Instance.Value.SetConsultationStatus(id, 2, User.UserID, consultationSchedule);
                else
                    WrittenExplanationProcess.Instance.Value.ScheduleConsultation(id, consultationSchedule, User.UserID);
                var content = new WrittenExplanationContent
                {
                    Message = $"Consultation {(resched ? "Reschedule" : "Schedule")} : {consultationSchedule:MMMM dd, yyyy}",
                    WrittenExplanationID = id,
                    SaveOnly = !sendEmail
                };

                content = WrittenExplanationContentProcess.Instance.Value.Create(content, User.UserID);

                var WrittenExplanation = WrittenExplanationProcess.Instance.Value.Get(id);

                ModelState.Clear();
                ViewBag.ContentId = WrittenExplanation.Content.LastOrDefault()?.ID;
                return PartialViewCustom("_Replies", WrittenExplanation);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoneConsultation(long id)
        {
            try
            {
                WrittenExplanationProcess.Instance.Value.SetConsultationStatus(id, 1, User.UserID);
                var WrittenExplanation = WrittenExplanationProcess.Instance.Value.Get(id);

                ModelState.Clear();
                return PartialViewCustom("_Replies", WrittenExplanation);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CloseWrittenExplanation(long id, short recommendation)
        {
            try
            {
                WrittenExplanationProcess.Instance.Value.Close(id, recommendation, User.UserID);
                var WrittenExplanation = WrittenExplanationProcess.Instance.Value.Get(id);
                

                ModelState.Clear();
                return PartialViewCustom("_Replies", WrittenExplanation);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResaveContent(long id, long writtenExplanationId, HttpPostedFileBase file)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(writtenExplanationId, true);
                string filePath = file.SaveFile(directory, $"{writtenExplanation.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(file.FileName)}");
                WrittenExplanationContentProcess.Instance.Value.UpdateFile(id, filePath, User.UserID);
                writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(writtenExplanationId);
                ModelState.Clear();
                return PartialViewCustom("_Replies", writtenExplanation);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReSave(WrittenExplanation writtenExplanation)
        {
            try
            {
                if (writtenExplanation.ID > 0)
                {
                    writtenExplanation = WrittenExplanationProcess.Instance.Value.CreateOrUpdate(writtenExplanation, User.UserID);
                    writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(writtenExplanation.ID);
                }
                else
                {
                    return Json(new { msg = false, res = "Invalid written explanation. Not saved." });
                }
                ModelState.Clear();
                ViewBag.ContentId = string.Join(",", writtenExplanation.Content.Select(x => x.ID));
                return PartialViewCustom("_Replies", writtenExplanation);
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
                WrittenExplanationProcess.Instance.Value.Delete(id, User.UserID);
                return Json(new { msg = true, res = "Deleted" });

            }
            catch (Exception ex)
            {

                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(long id, long writtenExplanationId)
        {
            try
            {

                WrittenExplanationContentProcess.Instance.Value.Delete(id, User.UserID);
                var writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(writtenExplanationId);


                ModelState.Clear();
                return PartialViewCustom("_Replies", writtenExplanation);

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
                if (contentId == null)
                    return Json(new { msg = false, res = "Nothing to send." });

                if (!Web.HasInternetConnection())
                    return Json(new { msg = false, res = "No Internet Connection." });

                List<WrittenExplanationContent> writtenExplanationContents = WrittenExplanationContentProcess.Instance.Value.GetList(contentId);
                WrittenExplanation writtenExplanation = WrittenExplanationProcess.Instance.Value.Get(writtenExplanationContents.FirstOrDefault()?.WrittenExplanationID ?? 0, true);

                if (string.IsNullOrEmpty(writtenExplanation.Personnel.Email))
                    return Json(new { msg = false, res = writtenExplanation.Personnel.FullName + " has no email." });

                List<string> files = new List<string>();
                string appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                string directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;

                writtenExplanationContents.ForEach(content => {
                    if (System.IO.File.Exists(Path.Combine(directory, Path.GetFileName(content.File))))
                        files.Add(Path.Combine(directory, Path.GetFileName(content.File)));
                });


                var credential = Web.GetMemoEmailCreadential();

                var subject = $"Written Explanation {writtenExplanation.MemoNo} {writtenExplanation.Subject}" ?? "(No Subject)";
                EmailResult emailRet = null;
                if (writtenExplanationContents.Count > 1)
                {
                    emailRet = EmailUtil.SendEmail(credential, User.UserID, writtenExplanation.Personnel.Email, CONTENT, true, files, contentId, subject);
                }
                else
                {
                    var infractionContent = writtenExplanationContents.FirstOrDefault();
                    emailRet = EmailUtil.SendEmail(credential, User.UserID, writtenExplanation.Personnel.Email, CONTENT, infractionContent.ID, files?.FirstOrDefault(), infractionContent.Message, subject);
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