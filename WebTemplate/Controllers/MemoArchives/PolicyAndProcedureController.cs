using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.HR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Helpers;
using WebTemplate.Models.MemoArchives.PolicyAndProcedure;

namespace WebTemplate.Controllers.MemoArchives
{
    public class PolicyAndProcedureController : BaseController
    {
        private const string CONTENT = "Policy And Procedure";
        private const string FOLDER = "MemoFolder";
        private const string SAVE_LOCATION = "MemoSaveLocation";

        // GET: MemoArchive
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Years = PolicyAndProcedureProcess.Instance.Value.FilterYears(model.Page, model.GridCount, out int PageCount).ToList();
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
        public ActionResult PolicyAndProcedures(PolicyAndProcedures model)
        {
            try
            {
                model.Page = model.Page > 1 ? model.Page : 1;
                model.PolicyAndProcedureList = PolicyAndProcedureProcess.Instance.Value.Filter(model.Year, model.Page, model.GridCount, out int PageCount).ToList();
                model.PageCount = PageCount;
                ModelState.Clear();
                return PartialViewCustom("_PolicyAndProcedures", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonnelPolicyAndProcedures(long papId)
        {
            try
            {
                var PolicyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Get(papId);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelPolicyAndProcedures", PolicyAndProcedure);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMemo()
        {
            Management model = new()
            {
                PolicyAndProcedure = new PolicyAndProcedure()
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
        public ActionResult SearchGroup(string filter)
        {
            try
            {
                Management model = new Management
                {
                    PersonnelGroups = PersonnelGroupProcess.GetList(filter),
                    PersonnelGroupSearchKey = filter
                };

                return PartialViewCustom("_SearchGroup", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchVessel(string filter)
        {
            try
            {
                Management model = new()
                {
                    Vessels = VesselProcess.Instance.Value.Search(filter),
                    VesselSearchKey = filter
                };

                return PartialViewCustom("_SearchVessel", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(PolicyAndProcedure policyAndProcedure, bool sendEmail, long? personnelId, long? groupId, String vesselIds, HttpPostedFileBase fileBase)
        {
            try
            {
                List<int> newVesselIds = JsonConvert.DeserializeObject<List<int>>(vesselIds);
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                policyAndProcedure.SaveOnly = !sendEmail;
                policyAndProcedure.File = fileBase.SaveFile(directory, $"{policyAndProcedure.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(fileBase.FileName)}");

                policyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Create(policyAndProcedure, personnelId, groupId, newVesselIds, User.UserID);
                policyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Get(policyAndProcedure.ID);
             
                ModelState.Clear();
                ViewBag.ContentId = policyAndProcedure.ID;
                return PartialViewCustom("_PersonnelPolicyAndProcedures", policyAndProcedure);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmailToPersonnel(long? contentId)
        {
            try
            {
                if (contentId == null)
                    return Json(new { res = "Nothing to send." });

                if (!Web.HasInternetConnection())
                    return Json(new { res = "No Internet Connection." });

                PolicyAndProcedure pap = PolicyAndProcedureProcess.Instance.Value.Get(contentId ?? 0);

                //if (pap.Content.Count == 0)
                //    return Json(new { res = "Nothing to send." });

                StringBuilder sb = new();
                foreach(var content in pap.Content)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(content.Personnel.Email))
                        {
                            sb.AppendLine($"<br >- {content.Personnel.FullName} has no email.<br >");
                            continue;
                        }
                        //if (string.IsNullOrEmpty(content.Vessel.Email))
                        //{
                        //    sb.AppendLine($"<br >- {content.Vessel.Description} - {content.Vessel.Code} has no email.<br >");
                        //    continue;
                        //}

                        string appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                        string directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                        string file = "";
                        
                        if (System.IO.File.Exists(Path.Combine(directory, Path.GetFileName(pap.File))))
                            file = Path.Combine(directory, Path.GetFileName(pap.File));

                        var credential = Web.GetMemoEmailCreadential();

                        var subject = $"Policy And Procedure {pap.MemoNo} {pap.Subject}" ?? "(No Subject)";

                        var emailRet = EmailUtil.SendEmail(credential, User.UserID, content.Personnel.Email, CONTENT, content.ID, file, "", subject);
                        if(emailRet.IsSuccess)
                            sb.AppendLine($"- {content.Personnel.FullName} email send.<br >");
                        else
                            sb.AppendLine($"- Unable to send email for {content.Personnel.FullName}<br >");
                    }
                    catch
                    {
                        sb.AppendLine($"- Unable to send email for {content.Personnel.FullName}<br >");
                    }
                }
                return Json(new { res = sb.ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResaveContent(long id, HttpPostedFileBase file)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var policyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Get(id, true);
                string filePath = file.SaveFile(directory, $"{policyAndProcedure.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(file.FileName)}");
                PolicyAndProcedureProcess.Instance.Value.UpdateFile(id, filePath, User.UserID);
                policyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Get(id);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelPolicyAndProcedures", policyAndProcedure);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReSave(PolicyAndProcedure policyAndProcedure)
        {
            try
            {
                if (policyAndProcedure.ID > 0)
                {
                    PolicyAndProcedureProcess.Instance.Value.Update(policyAndProcedure, User.UserID);
                    policyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Get(policyAndProcedure.ID);
                }
                else
                {
                    return Json(new { msg = false, res = "Invalid policy and procedure. Not saved." });
                }
                return PartialViewCustom("_PersonnelPolicyAndProcedures", policyAndProcedure);
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
                PolicyAndProcedureProcess.Instance.Value.Delete(id, User.UserID);

                return Json(new { msg = true, res = "Deleted" });

            }
            catch (Exception ex)
            {

                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
    }
}