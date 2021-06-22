using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Models.Groups;
using P = WebTemplate.Models.Groups;

namespace WebTemplate.Controllers.HumanResource
{
    public class GroupsController : BaseController
    {
        // GET: Groups
        #region Index...
        public ActionResult Index(P.Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.PersonnelGroups = PersonnelGroupProcess.GetList(model.Filter, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;
            ViewBag.MemoNo = MemoArchiveProcess.GetMemoNumber();

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_GroupsSearch", model);
            }
            else
            {
                return ViewCustom("_GroupsIndex", model);
            }
        }
        #endregion

        #region Group
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelGroup(int ID)
        {
            try
            {
                PersonnelGroup model = new PersonnelGroup { ID = ID };
                ModelState.Clear();
                return PartialViewCustom("_Group", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelGroup(int ID)
        {
            try
            {
                PersonnelGroup model = PersonnelGroupProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_Group", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelGroup(PersonnelGroup model)
        {
            try
            {
                model = PersonnelGroupProcess.CreateOrUpdate(model, User.UserID);
             
                ModelState.Clear();
                return PartialViewCustom("_Group", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelGroup(int? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    DeleteGroupSingle(id ?? 0);
                    return Json(new { msg = true, res = "Delete Success" });
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Group not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Group not found." });
            }
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
                        DeleteGroupSingle(toApplyAction[i].ID.ToInt());
                    }
                }
                catch (Exception e)
                {
                    return Json(new { msg = false, res = e.Message });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Group not found." });
            }

            return Json(new { msg = true });
        }

        public void DeleteGroupSingle(int? id = null)
        {
            PersonnelGroupProcess.Delete(id ?? 0, User.UserID);
        }
        #endregion

        #region Group Member...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetMembers(int PersonnelGroupID)
        {
            try
            {
                var model = new GroupMembers
                {
                    //model.Personnels.Personnel = PersonnelProcess.GetList("");
                    PersonnelGroup = PersonnelGroupProcess.Get(PersonnelGroupID),
                    PersonnelGroupMembers = PersonnelGroupMemberProcess.GetByGroup(PersonnelGroupID)
                };

                ModelState.Clear();
                return PartialViewCustom("_MembersSearch", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPersonnel(string key)
        {
            try
            {
                var model = new GroupMembers();
                model.Personnels.Personnel = PersonnelProcess.GetList(key, true);
                model.PersonnelSearchKey = key;

                ModelState.Clear();
                return PartialViewCustom("_SearchTable", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPersonnel(string filter, List<long> PersonnelId)
        {
            try
            {
                var model = new Personnels
                {
                    Personnel = PersonnelProcess.GetList(filter, true)
                };

                foreach (var personnel in model.Personnel)
                {
                    if (PersonnelId.Contains(personnel.ID))
                    {
                        model.Personnel.Remove(personnel);
                    }
                }

                ModelState.Clear();
                return PartialViewCustom("_MembersSearch", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewMember(int PersonnelGroupID)
        {
            try
            {
                PersonnelGroupMember model = new PersonnelGroupMember { PersonnelGroupID = PersonnelGroupID };

                ModelState.Clear();
                return PartialViewCustom("_Member", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelMember(long ID)
        {
            try
            {
                PersonnelGroupMember model = PersonnelGroupMemberProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_Member", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMember(PersonnelGroupMember model)
        {
            try
            {
                model = PersonnelGroupMemberProcess.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_Member", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMembers(List<PersonnelGroupMember> model)
        {
            try
            {
                GroupMembers models = new GroupMembers
                {
                    PersonnelGroupMembers = PersonnelGroupMemberProcess.CreateOrUpdate(model, User.UserID)
                };

                //model.OrderByDescending(m => m.Deleted);

                //foreach (var m in models.PersonnelGroupMembers)
                //{
                //    if (m.Deleted)
                //        models.PersonnelGroupMembers.Remove(m);
                //}

                models.PersonnelGroupMembers = models.PersonnelGroupMembers.Where(r => r.Deleted == false).ToList();

                ModelState.Clear();
                return PartialViewCustom("_MemberSearchTable", models);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMember(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelGroupMemberProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Group Member not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Group Member not found." });
            }
            return Json(new { msg = true });
        }
        #endregion
    }
}