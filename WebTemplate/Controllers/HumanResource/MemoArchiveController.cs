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
    public class MemoArchiveController : BaseController
    {
        private const string CONTENT = "Memo Archives";
        private const string FOLDER = "MemoFolder";
        private const string SAVE_LOCATION = "MemoSaveLocation";

        // GET: MemoArchive
        public ActionResult Index(M.Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.MemoArchives = MemoArchiveProcess.GetList(model.MemoTypeID, model.Personnel, model.Group, model.Filter, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;
            ViewBag.MemoNo = MemoArchiveProcess.GetMemoNumber();

            foreach (var m in model.MemoArchives)
            {
                long RecipientID = m._Persons.Any() ? m._Persons.FirstOrDefault().ID : 0;
                m._Replies = MemoArchiveProcess.GetPersonnelMemoReplies(RecipientID, m.ID);
                m.Replies_Count = m._Replies.Count();
                m.IsExistFile = !string.IsNullOrEmpty(m.File) && System.IO.File.Exists(GetFileLocation(m.ID));
                m.Files_Count = m._Replies.Where(r => !string.IsNullOrEmpty(r.File)).Count();
            }

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
        public ActionResult New()
        {
            ProcessLayer.Entities.MemoArchives model = new ProcessLayer.Entities.MemoArchives();
            return PartialViewCustom("_New", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult View(long MemoId)
        {
            ProcessLayer.Entities.MemoArchives model = MemoArchiveProcess.GetAll(MemoId, false);
            model.IsExistFile = !string.IsNullOrEmpty(model.File) && System.IO.File.Exists(GetFileLocation(model.ID));

            return PartialViewCustom("_View", model);
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

                return PartialViewCustom("_Personnel", model);
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
                M.Management model = new M.Management
                {
                    PersonnelGroups = PersonnelGroupProcess.GetList(filter),
                    PersonnelGroupSearchKey = filter
                };

                return PartialViewCustom("_Group", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Replies(long MemoId)
        {
            try
            {
                M.MemoReplies model = new M.MemoReplies
                {
                    ParentMemo = MemoArchiveProcess.Get(MemoId),
                    Personnels = PersonnelProcess.GetMemoPersonnel(MemoId),
                    MemoArchives = MemoArchiveProcess.GetAllReplies(MemoId)
                };

                if (model.ParentMemo != null)
                {
                    model.ParentMemo.IsExistFile = !string.IsNullOrEmpty(model.ParentMemo.File) && System.IO.File.Exists(GetFileLocation(model.ParentMemo.ID));

                    model.ParentMemo._Sender = PersonnelProcess.GetMemoPersonnel(MemoId).FirstOrDefault();
                    foreach (var m in model.MemoArchives)
                    {
                        m.IsExistFile = !string.IsNullOrEmpty(m.File) && System.IO.File.Exists(GetFileLocation(m.ID));
                        m._Sender = PersonnelProcess.GetMemoPersonnel(m.ID).FirstOrDefault();
                    }

                    ModelState.Clear();
                    return PartialViewCustom("_Replies", model);
                }
                else
                {
                    return Json(new { msg = false, res = "Memo achive not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ProcessLayer.Entities.MemoArchives model, HttpPostedFileBase fileBase, long? personnelId, int? personnelGroupId)
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
                {
                    model = MemoArchiveProcess.CreateOrUpdate(model, personnelId.Value, User.UserID);
                    model._Persons.Add(PersonnelProcess.Get(personnelId ?? 0, true));
                }
                else if (personnelGroupId != null && personnelGroupId > 0)
                {
                    model = MemoArchiveProcess.CreateOrUpdate(model, personnelGroupId.Value, User.UserID);
                    model._Groups.Add(PersonnelGroupProcess.Get(personnelGroupId ?? 0, true));
                }
                else
                {
                    return Json(new { msg = false, res = "No recipient(s) found. Not saved." });
                }

                if (model.File != null)
                {
                    model.File = Path.Combine(ConfigurationManager.AppSettings[FOLDER], model.File);
                }
                model.IsExistFile = !string.IsNullOrEmpty(model.File) && System.IO.File.Exists(GetFileLocation(model.ID));

                ModelState.Clear();
                return PartialViewCustom("_View", model);
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
                var memotype = LookupProcess.GetMemoType().Where(m => m.ID == memo.MemoTypeID).Select(m => m.Description).FirstOrDefault() ?? "(No Memo Type)";
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var file = "";
                if (System.IO.File.Exists(Path.Combine(directory, Path.GetFileName(memo.File))))
                    file = Path.Combine(directory, Path.GetFileName(memo.File));

                var credential = Web.GetMemoEmailCreadential();

                var subject = $"{memotype} {memo.MemoNo} {memo.Subject}" ?? "(No Subject)";

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
        public ActionResult Reply(long MemoId, long? personnelId, int? personnelGroupId, bool IsForward)
        {
            M.MemoReplies model = new M.MemoReplies
            {
                ParentMemo = MemoArchiveProcess.GetAll(MemoId, true)
            };
            model.ParentMemo.InReplyTo = MemoId;
            model.ParentMemo.IsForward = IsForward;

            if (personnelId != null && personnelId > 0)
                model.ParentMemo._Person = PersonnelProcess.Get(personnelId ?? 0, true);
            else if (personnelGroupId != null && personnelGroupId > 0)
                model.ParentMemo._Group = PersonnelGroupProcess.Get(personnelGroupId ?? 0, true);

            return PartialViewCustom("_Reply", model);
        }
    }
}