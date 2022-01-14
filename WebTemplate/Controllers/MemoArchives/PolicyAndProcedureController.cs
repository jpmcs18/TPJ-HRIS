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
        public ActionResult PolicyAndProceduresContent(long papId)
        {
            try
            {
                var PolicyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Get(papId);
                ModelState.Clear();
                return PartialViewCustom("_PolicyAndProceduresContent", PolicyAndProcedure);
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
                    return Json(new { msg = false, res = "Nothing to send." });

                if (!Web.HasInternetConnection())
                    return Json(new { msg = false, res = "No Internet Connection." });

                PolicyAndProcedure policyAndProcedure = PolicyAndProcedureProcess.Instance.Value.Get(contentId ?? 0);
                
                List<string> files = new List<string>();
                string appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                string directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                for (int i = 0; i < (policyAndProcedure.Content?.Count ?? 0); i++)
                {
                    if (policyAndProcedure.Content[i].PersonnelID != null && string.IsNullOrEmpty(policyAndProcedure.Content[i].Personnel.Email))
                        return Json(new { msg = false, res = policyAndProcedure.Content[i].Personnel.FullName + " has no email." });

                    if (policyAndProcedure.Content[i].VesselID != null && string.IsNullOrEmpty(policyAndProcedure.Content[i].Vessel.Email))
                        return Json(new { msg = false, res = policyAndProcedure.Content[i].Vessel.Description + " has no email." });


                }

                string file = Path.Combine(directory, Path.GetFileName(policyAndProcedure.File));


                var credential = Web.GetMemoEmailCreadential();

                var subject = $"Memo No: {policyAndProcedure.MemoNo} {policyAndProcedure.Subject}" ?? "(No Subject)";

                EmailResult emailRet = null;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < policyAndProcedure.Content.Count; i++)
                {
                    string email = policyAndProcedure.Content[i].PersonnelID == null ? policyAndProcedure.Content[i].Vessel.Email : policyAndProcedure.Content[i].Personnel.Email;
                    emailRet = EmailUtil.SendEmail(credential, User.UserID, email, CONTENT, policyAndProcedure.Content[i].ID, file, policyAndProcedure.Description, subject);
                    if (!emailRet.IsSuccess)
                    {
                        sb.Append(emailRet.Message + "<br >");
                    }
                }

                return Json(new { msg = sb.Length == 0, res = sb.ToString() });
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