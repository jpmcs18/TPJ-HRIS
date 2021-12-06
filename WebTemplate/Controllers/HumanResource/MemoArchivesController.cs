using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Helpers;
using M = WebTemplate.Models.MemoArchive;

namespace WebTemplate.Controllers.HumanResource
{
    public class MemoArchivesController : BaseController
    {
        private const string CONTENT = "Memo Archives";
        private const string FOLDER = "MemoFolder";
        private const string SAVE_LOCATION = "MemoSaveLocation";

        #region Index...
        public ActionResult Index(M.Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.MemoArchives = MemoArchiveProcess.GetList(model.MemoTypeID, model.Personnel, model.Group, model.Filter, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;
            ViewBag.MemoNo = MemoArchiveProcess.GetMemoNumber();

            for (int i = 0; i < model.MemoArchives.Count(); i++)
            {
                model.MemoArchives[i].IsExistFile = !string.IsNullOrEmpty(model.MemoArchives[i].File) && System.IO.File.Exists(GetFileLocation(model.MemoArchives[i].ID));
            }

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_MemoArchiveSearch", model);
            }
            else
            {
                return ViewCustom("_MemoArchiveIndex", model);
            }
        }
        #endregion

        #region Memo Archive...  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMemo(long id)
        {
            try
            {
                M.Management model = new M.Management
                {
                    MemoArchive = MemoArchiveProcess.Get(id, true)
                };

                model.PersonnelGroups = model.MemoArchive._Groups;
                model.Personnels = model.MemoArchive._Persons;

                return PartialViewCustom("_MemoManagement", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPersonnel(string filter)
        {
            try
            {
                M.Management model = new M.Management
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
                return Json(new {msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchGroup(string filter)
        {
            try
            {
                M.Management model = new M.Management
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
        public ActionResult GetPersonnels(long ID)
        {
            try
            {
                M.MemoPersonnels model = new M.MemoPersonnels
                {
                    Personnels = PersonnelProcess.GetMemoPersonnel(ID)
                };

                ModelState.Clear();
                return PartialViewCustom("_MemoReplies", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetReplies(long MemoId, long PersonnelId)
        {            
            try
            {
                M.MemoReplies model = new M.MemoReplies
                {
                    ParentMemo = MemoArchiveProcess.Get(MemoId),
                    MemoArchives = MemoArchiveProcess.GetPersonnelMemoReplies(PersonnelId, MemoId)
                };

                foreach (var m in model.MemoArchives)
                {
                    m.IsExistFile = !string.IsNullOrEmpty(m.File) && System.IO.File.Exists(GetFileLocation(m.ID));
                }

                ModelState.Clear();
                return PartialViewCustom("_MemoDiscussion", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMemoArchive()
        {
            M.Management model = new()
            {
                MemoArchive = new ProcessLayer.Entities.MemoArchives() { InReplyTo = null, PersonnelReply = false }
            };

            return PartialViewCustom("_MemoManagement", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMemoArchiveReply(long MemoId)
        {
            try
            {
                ProcessLayer.Entities.MemoArchives model = new ProcessLayer.Entities.MemoArchives { InReplyTo = MemoId };
                ModelState.Clear();
                return PartialViewCustom("_MemoArchive", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelMemoArchiveReply(long MemoId)
        {
            try
            {
                ProcessLayer.Entities.MemoArchives model = new ProcessLayer.Entities.MemoArchives { InReplyTo = MemoId, PersonnelReply = true };
                ModelState.Clear();
                return PartialViewCustom("_MemoArchive", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelMemoArchive(long ID)
        {
            try
            {
                ProcessLayer.Entities.MemoArchives model = MemoArchiveProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_MemoArchive", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMemoArchive(ProcessLayer.Entities.MemoArchives model, HttpPostedFileBase fileBase, long? personnelId, int? personnelGroupId)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                if (fileBase != null && fileBase.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileBase.FileName);
                    var path = Path.Combine(directory, model.ID.ToString() + DateTime.Now.ToString("MMddyyyyHHmmss") + fileName);
                    var path1 = model.ID.ToString() + DateTime.Now.ToString("MMddyyyyHHmmss") + fileName;

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    fileBase.SaveAs(path);

                    model.File = path1;
                }
                else
                    model.File = String.IsNullOrEmpty(model.File) ? null : Path.GetFileName(model.File);

                if (personnelId != null && personnelId > 0)
                    model = MemoArchiveProcess.CreateOrUpdate(model, personnelId.Value, User.UserID);
                else if (personnelGroupId != null && personnelGroupId > 0)
                    model = MemoArchiveProcess.CreateOrUpdate(model, personnelGroupId.Value, User.UserID);

                if (model.File != null)
                {
                    model.File = Path.Combine(ConfigurationManager.AppSettings[FOLDER], model.File);
                }

                ModelState.Clear();
                if(String.IsNullOrEmpty(model.InReplyTo.ToString()))
                    return Json(new { msg = true, model.ID });
                else
                    return PartialViewCustom("_MemoReply", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPersonnelsCount(long memoId)
        {
            try
            {
                var Personnels = PersonnelProcess.GetMemoPersonnel(memoId);

                return Json(new { msg = true, res = Personnels.Select(x => x.ID).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        
        public string GetFileLocation(long memoid)
        {
            var memo = MemoArchiveProcess.GetAll(memoid, true);
            var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
            var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
            var file = Path.Combine(directory, Path.GetFileName(memo.File));

            return file;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmailToPersonnel(long personnelId, long memoid)
        {
            try
            {
                if (!Web.HasInternetConnection())
                    return Json(new { msg = false, res = "No Internet Connection." });

                var personnel = PersonnelProcess.Get(personnelId, true);
                if (string.IsNullOrEmpty(personnel.Email))
                    return Json(new { msg = false, res = personnel.FullName + " has no email." });

                var memo = MemoArchiveProcess.GetAll(memoid, true);
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var file = Path.Combine(directory, Path.GetFileName(memo.File));

                var credential = Web.GetMemoEmailCreadential();

                var subject = $"{memo.MemoNo} {memo.Subject}";

                var emailRet = EmailUtil.SendEmail(credential, User.UserID, personnel.Email, CONTENT, memoid, file, memo.Description, subject);

                return Json(new { msg = emailRet.IsSuccess, res = emailRet.Message, personnel = personnel.FullName });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMemoArchive(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    DeleteMemoArchiveSingle(id);
                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Memo not found." });
            }
            return Json(new { msg = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMultiple(string ids)
        {
            if (ids.Length > 0)
            {
                try
                {
                    ToApplyAction[] toApplyAction = new JavaScriptSerializer().Deserialize<ToApplyAction[]>(ids);
                    for (int i = 0; i < toApplyAction.Count(); i++)
                    {
                        DeleteMemoArchiveSingle(toApplyAction[i].ID);
                    }
                }
                catch (Exception e)
                {
                    return Json(new { msg = false, res = e.Message });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel not found." });
            }

            return Json(new { msg = true });
        }

        public void DeleteMemoArchiveSingle(long? id = null)
        {
            MemoArchiveProcess.Delete(id.Value, User.UserID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CloseMemo(long? id)
        {
            try
            {
                try
                {
                    MemoArchiveProcess.CloseMemo(id.Value, User.UserID);
                }
                catch (Exception e)
                {
                    return Json(new { msg = false, res = e.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }

            return Json(new { msg = true });
        }
        #endregion
    }   
}