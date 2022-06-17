using ProcessLayer.Entities;
using ProcessLayer.Entities.HR;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.CnB;
using ProcessLayer.Processes.HR;
using ProcessLayer.Processes.HRs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Models.Payroll;
using WebTemplate.Models.Personnel;
using P = WebTemplate.Models.Personnel;

namespace WebTemplate.Controllers.HumanResource
{ 
    public class PersonnelController : BaseController
    {
        #region Index...
        public ActionResult Index(P.Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Personnels = PersonnelProcess.GetList(model.Filter, model.EmploymentStatusID, model.DepartmentID, model.PersonnelTypeID, model.LocationID, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                //if (model.Personnels.Where(m => m.ID == User.PersonnelID).Any())
                //{
                //    Response.Cache.SetNoStore();
                //    Response.Cache.SetNoServerCaching();

                //    return ViewCustom("_PersonnelIndex", model);
                //}

                ModelState.Clear();
                return PartialViewCustom(Request.Browser.IsMobileDevice ? "" : "_PersonnelSearch", model);
            }

            return ViewCustom("_PersonnelIndex", model);
        }
        #endregion

        #region Personnel...
        public ActionResult PersonnelProfile(long? id)
        {
            Response.Cache.SetNoStore();
            Response.Cache.SetNoServerCaching();

            if (id.HasValue)
            {
                var model = new Management
                { 
                    Personnel = PersonnelProcess.Get(id.Value)
                };

                ModelState.Clear();
                ViewBag.ImageFileExist = model.Personnel?.Image == null ? false : System.IO.File.Exists(Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["ImageSaveLocation"]), model.Personnel.Image) ?? "");
                return PartialViewCustom("_PersonnelProfile", model);
            }
            else
            {
                return Json(new { msg = false, res = "Personnel not found." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePersonalInfo(Management model, HttpPostedFileBase fileBase)
        {
            try
            {

                if (fileBase != null && fileBase.ContentLength > 0)
                {
                    var extension = Path.GetExtension(fileBase.FileName);
                    var appSettingPath = ConfigurationManager.AppSettings["ImageSaveLocation"];
                    var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                    var path = Path.Combine(directory, (String.IsNullOrWhiteSpace(model.Personnel.EmployeeNo) ? ("noname" + model.Personnel.ID.ToString()) : model.Personnel.EmployeeNo.ToString()) + extension);
                    var path1 = (String.IsNullOrWhiteSpace(model.Personnel.EmployeeNo) ? ("noname" + model.Personnel.ID.ToString()) : model.Personnel.EmployeeNo.ToString()) + extension;

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    fileBase.SaveAs(path);

                    model.Personnel.ImagePath = path1;
                }
                else
                    model.Personnel.ImagePath = String.IsNullOrEmpty(model.Personnel.ImagePath) ? null : Path.GetFileName(model.Personnel.ImagePath);
                
                    model.Personnel = PersonnelProcess.CreateOrUpdatePersonalInfo(model.Personnel, User.UserID);

                Response.Cache.SetNoStore();
                Response.Cache.SetNoServerCaching();

                return Json(new { msg = true, res = model.Personnel.ID });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveEmployementInfo(P.Management model)
        {
            try
            {
                if (model.Personnel.ID > 0)
                {
                    model.Personnel = PersonnelProcess.UpdateEmploymentInfo(model.Personnel, User.UserID);
                    return Json(new { msg = true, res = model.Personnel.ID });
                }
                else
                {
                    return Json(new { msg = false, res = "Invalid Request." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnel(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    DeletePersonnelSingle(id);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel not found." });
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
                        DeletePersonnelSingle(toApplyAction[i].ID);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetPositionByDepartmentAndPersonnelType(int? departmentId, int? personnelTypeId)
        {
            try
            {
                var response = new PositionResponse
                {
                    Positions = PositionProcess.Instance.GetByDepartmentAndPersonnelType(departmentId, personnelTypeId).ToList()
                };

                return Json(new { msg = true, res = response });
            }
            catch (Exception e)
            {
                return Json(new { msg = false, res = e.Message });
            }
        }

        public void DeletePersonnelSingle(long? id = null)
        {
            var Personnel = PersonnelProcess.Get(id.Value);
            PersonnelProcess.Delete(id.Value, User.UserID);
            var appSettingPath = ConfigurationManager.AppSettings["ImageSaveLocation"];
            var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
            var path = Path.Combine(directory, Personnel.Image);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        #endregion

        #region Contact Number...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetContactNumbers(long PersonnelID)
        {
            try
            {
                ContactNumbers model = new ContactNumbers
                {
                    PersonnelID = PersonnelID,
                    ContactNumber = ContactNumberProcess.GetList(PersonnelID)
                };

                return PartialViewCustom("_PersonnelContactNumbers", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewContactNumber(long PersonnelID)
        {
            try
            {
                ContactNumbers model = new ContactNumbers { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelContact", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelContactNumber(long ID)
        {
            try
            {
                ContactNumber model = ContactNumberProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelContact", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelContactNumber(ContactNumber model)
        {
            try
            {
                model = ContactNumberProcess.CreateOrUpdate(model, User.UserID);
                
                ModelState.Clear();
                return PartialViewCustom("_PersonnelContact", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelContactNumber(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    ContactNumberProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Contact Number not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Contact Number not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Educational Background...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetEducationalBackgrounds(long PersonnelID)
        {
            try
            {
                Educationbackgrounds model = new Educationbackgrounds
                {
                    PersonnelID = PersonnelID,
                    EducationalBackground = EducationalBackgroundProcess.GetByPersonnelID(PersonnelID)
                };

                return PartialViewCustom("_PersonnelEducationalBackgrounds", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewEducationalBackground(long PersonnelID)
        {
            try
            {
                EducationalBackground model = new EducationalBackground { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelEducationalBackground", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelEducationalBackground(long ID)
        {
            try
            {
                EducationalBackground model = EducationalBackgroundProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelEducationalBackground", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEducationalBackground(EducationalBackground model)
        {
            try
            {
                if (model.ID == 0)
                {
                    model = EducationalBackgroundProcess.Create(model, User.UserID);
                }
                else
                {
                    model = EducationalBackgroundProcess.Update(model, User.UserID);
                }

                ModelState.Clear();
                return PartialViewCustom("_PersonnelEducationalBackground", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteEducationalBackground(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    EducationalBackgroundProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Educational Background not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Educational Background not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Work Experience...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetWorkExperiences(long PersonnelID)
        {
            try
            {
                WorkExperiences model = new WorkExperiences
                {
                    PersonnelID = PersonnelID,
                    WorkExperience = WorkExperienceProcess.GetByPersonnelID(PersonnelID)
                };
                ModelState.Clear();
                return PartialViewCustom("_PersonnelWorkExperiences", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewWorkExperience(long PersonnelID)
        {
            try
            {
                WorkExperience model = new WorkExperience { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelWorkExperience", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelWorkExperience(long ID)
        {
            try
            {
                WorkExperience model = WorkExperienceProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelWorkExperience", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveWorkExperience(WorkExperience model)
        {
            try
            {
                if (model.ID == 0)
                {
                    model = WorkExperienceProcess.Create(model, User.UserID);
                }
                else
                {
                    model = WorkExperienceProcess.Update(model, User.UserID);
                }
                ModelState.Clear();
                return PartialViewCustom("_PersonnelWorkExperience", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteWorkExperience(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    WorkExperienceProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Work Experience not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Work Experience not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Compensation And Deduction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCompensationsAndDeductions(long PersonnelID)
        {
            try
            {
                CompensationsAndDeductions model = new CompensationsAndDeductions
                {
                    PersonnelID = PersonnelID,
                    Compensation = PersonnelCompensationProcess.Instance.GetByPersonnelID(PersonnelID),
                    AssumedDeductions = PersonnelDeductionProcess.GetAssumed(PersonnelID)
                };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelCompensationsAndDeductions", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #region Compensation... 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelCompensation(long PersonnelID)
        {
            try
            {
                PersonnelCompensation model = new PersonnelCompensation { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelCompensation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelCompensation(long ID)
        {
            try
            {
                PersonnelCompensation model = PersonnelCompensationProcess.Instance.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelCompensation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelCompensation(PersonnelCompensation model)
        {
            try
            {
                model = PersonnelCompensationProcess.Instance.CreateOrUpdate(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_PersonnelCompensation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelCompensation(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelCompensationProcess.Instance.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Compensation not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Compensation not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Deduction...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelDeduction(long PersonnelID)
        {
            try
            {
                PersonnelDeduction model = new PersonnelDeduction { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelDeduction", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelDeduction(long ID)
        {
            try
            {
                PersonnelDeduction model = PersonnelDeductionProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelDeduction", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelDeduction(PersonnelDeduction model)
        {
            try
            {
                model = PersonnelDeductionProcess.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();

                return PartialViewCustom("_PersonnelDeduction", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelDeduction(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelDeductionProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Deduction not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Deduction not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #endregion

        #region Dependent...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelDependent(long PersonnelID)
        {
            try
            {
                PersonnelDependent model = new PersonnelDependent { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelDependent", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelDependent(long ID)
        {
            try
            {
                PersonnelDependent model = PersonnelDependentProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelDependent", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelDependent(PersonnelDependent model)
        {
            try
            {
                if (model.ID == 0)
                {
                    model = PersonnelDependentProcess.Create(model, User.UserID);
                }
                else
                {
                    model = PersonnelDependentProcess.Update(model, User.UserID);
                }
                ModelState.Clear();
                return PartialViewCustom("_PersonnelDependent", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelDependent(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelDependentProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Dependent not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Dependent not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Legislation...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelLegislation(long PersonnelID)
        {
            try
            {
                PersonnelLegislation model = new PersonnelLegislation { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelLegislation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelLegislation(long ID)
        {
            try
            {
                PersonnelLegislation model = PersonnelLegislationProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelLegislation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelLegislation(PersonnelLegislation model, HttpPostedFileBase fileBase)
        {
            try
            {
                if (fileBase != null && fileBase.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileBase.FileName);
                    var appSettingPath = ConfigurationManager.AppSettings["LegislationSaveLocation"];
                    var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                    var path = Path.Combine(directory, model.PersonnelID.ToString() + DateTime.Now.ToString("MMddyyyyHHmmss") + fileName);
                    var path1 = model.PersonnelID.ToString() + DateTime.Now.ToString("MMddyyyyHHmmss") + fileName;

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


                if (model.ID == 0)
                {
                    model = PersonnelLegislationProcess.Create(model, User.UserID);
                }
                else
                {
                    model = PersonnelLegislationProcess.Update(model, User.UserID);
                }

                if (model.File != null)
                {
                    model.File = Path.Combine(ConfigurationManager.AppSettings["LegislationFolder"], model.File);
                }

                ModelState.Clear();
                return PartialViewCustom("_PersonnelLegislation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelLegislation(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelLegislationProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Legislation not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Legislation not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region License...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelLicense(long PersonnelID)
        {
            try
            {
                PersonnelLicense model = new PersonnelLicense { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelLicense", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelLicense(long ID)
        {
            try
            {
                PersonnelLicense model = PersonnelLicenseProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelLicense", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelLicense(PersonnelLicense model)
        {
            try
            {
                if (model.ID == 0)
                {
                    model = PersonnelLicenseProcess.Create(model, User.UserID);
                }
                else
                {
                    model = PersonnelLicenseProcess.Update(model, User.UserID);
                }
                ModelState.Clear();
                return PartialViewCustom("_PersonnelLicense", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelLicense(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelLicenseProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel License not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel License not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Memo...
        private const string SAVE_LOCATION = "MemoSaveLocation";

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPolicyAndProcedures(long personnelId)
        {
            try
            {
                PolicyAndProcedures paps = new();
                paps.PolicyAndProcedureList = PolicyAndProcedureProcess.Instance.GetListByPersonnel(personnelId);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelPolicyAndProcedure", paps);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PolicyAndProcedureContents(long id)
        {
            try
            {
                PolicyAndProcedureContent model = new();
                model.Acknowledgement = PersonnelPolicyAndProcedureProcess.Instance.Get(id);
                model.PolicyAndProcedure = PolicyAndProcedureProcess.Instance.Get(model.Acknowledgement.PolicyAndProcedureID, true);

                if (model != null)
                {
                    return PartialViewCustom("_PersonnelPolicyAndProcedureContents", model);
                }
                else
                {
                    return Json(new { msg = false, res = "Policy And Procedure not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcknowledgePolicyAndProcedure(long id)
        {
            try
            {
                PersonnelPolicyAndProcedureProcess.Instance.Acknowledge(id, User.UserID);
                PolicyAndProcedureContent model = new();
                model.Acknowledgement = PersonnelPolicyAndProcedureProcess.Instance.Get(id);
                model.PolicyAndProcedure = PolicyAndProcedureProcess.Instance.Get(model.Acknowledgement.PolicyAndProcedureID, true);


                ModelState.Clear();
                return PartialViewCustom("_PersonnelPolicyAndProcedureContents", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetInfractions(long personnelId)
        {
            try
            {
                Infractions infractions = new();
                infractions.InfractionList = InfractionProcess.Instance.GetList(personnelId, true);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelInfractions", infractions);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfractionContents(long id)
        {
            try
            {
                Infraction model = InfractionProcess.Instance.Get(id);

                if (model != null)
                {
                    return PartialViewCustom("_PersonnelInfractionContents", model);
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
        public ActionResult SaveInfractionFileContent(InfractionContent content, HttpPostedFileBase fileBase)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var infraction = InfractionProcess.Instance.Get(content.InfractionID, true);
                content.FromPersonnel = true;
                content.File = fileBase.SaveFile(directory, $"{infraction.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(fileBase.FileName)}");

                content = InfractionContentProcess.Instance.Create(content, User.UserID);
                infraction = InfractionProcess.Instance.Get(content.InfractionID);

                ModelState.Clear();
                ViewBag.ContentId = infraction.Content.LastOrDefault()?.ID;
                return PartialViewCustom("_PersonnelInfractionContents", infraction);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetWrittenExplanations(long personnelId)
        {
            try
            {
                WrittenExplanations writtenExplanations = new();
                writtenExplanations.WrittenExplanationList = WrittenExplanationProcess.Instance.GetList(personnelId, true);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelWrittenExplanations", writtenExplanations);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WrittenExplanationContents(long id)
        {
            try
            {
                WrittenExplanation model = WrittenExplanationProcess.Instance.Get(id);

                if (model != null)
                {
                    return PartialViewCustom("_PersonnelWrittenExplanationContents", model);
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
        public ActionResult SaveWrittenExplanationFileContent(WrittenExplanationContent content, HttpPostedFileBase fileBase)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                var writtenExplanation = WrittenExplanationProcess.Instance.Get(content.WrittenExplanationID, true);
                content.FromPersonnel = true;
                content.File = fileBase.SaveFile(directory, $"{writtenExplanation.MemoNo}{DateTime.Now:MMddyyyyHHmmss}{Path.GetExtension(fileBase.FileName)}");

                content = WrittenExplanationContentProcess.Instance.Create(content, User.UserID);
                writtenExplanation = WrittenExplanationProcess.Instance.Get(content.WrittenExplanationID);

                ModelState.Clear();
                ViewBag.ContentId = writtenExplanation.Content.LastOrDefault()?.ID;
                return PartialViewCustom("_PersonnelWrittenExplanationContents", writtenExplanation);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelMemo()
        {
            try
            {
                ProcessLayer.Entities.MemoArchives model = new ProcessLayer.Entities.MemoArchives() { PersonnelReply = true };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelMemo", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelMemo(long PersonnelID, long ID)
        {
            try
            {
                ProcessLayer.Entities.MemoArchives model = MemoArchiveProcess.GetPersonnelMemo(PersonnelID, ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelMemo", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelMemo(ProcessLayer.Entities.MemoArchives model, HttpPostedFileBase fileBase, long personnelId)
        {
            try
            {
                if (fileBase != null && fileBase.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(fileBase.FileName);
                    var appSettingPath = ConfigurationManager.AppSettings["MemoSaveLocation"];
                    var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
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

                model = MemoArchiveProcess.CreateOrUpdate(model, personnelId, User.UserID);

                if (model.File != null)
                {
                    model.File = Path.Combine(ConfigurationManager.AppSettings["MemoFolder"], model.File);
                }

                ModelState.Clear();
                return PartialViewCustom("_PersonnelMemoReply", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetMemoArchives(long personnelId, int? memotypeid)
        {
            try
            {
                P.Memos model = new P.Memos
                {
                    MemoArchives = MemoArchiveProcess.GetPersonnelMemos(personnelId, memotypeid)
                };

                ModelState.Clear();
                var ViewName = "_Personnel" + (memotypeid == 1 ? "Memos" : "Legislations");

                return PartialViewCustom(ViewName, model);
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
                Models.MemoArchive.MemoPersonnels model = new Models.MemoArchive.MemoPersonnels
                {
                    Personnels = PersonnelProcess.GetMemoPersonnel(ID)
                };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelMemoReplies", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetMemoReplies(long MemoId, long PersonnelId)
        {
            try
            {
                MemoReplies model = new MemoReplies
                {
                    ParentMemo = MemoArchiveProcess.Get(MemoId),
                    PersonnelID = PersonnelId,
                    Memos = MemoArchiveProcess.GetPersonnelMemoReplies(PersonnelId, MemoId)
                };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelMemoDiscussion", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelMemo(long? id, long personnelId)
        {
            if (id.HasValue)
            {
                try
                {
                    MemoArchiveProcess.Delete(id.Value, personnelId, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Memo not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Memo not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Vaccine...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelVaccine(long PersonnelID)
        {
            try
            {
                PersonnelVaccine model = new PersonnelVaccine { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelVaccine", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelVaccine(long ID)
        {
            try
            {
                PersonnelVaccine model = PersonnelVaccineProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelVaccine", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelVaccine(PersonnelVaccine model)
        {
            try
            {
                if (model.ID == 0)
                {
                    model = PersonnelVaccineProcess.Create(model, User.UserID);
                }
                else
                {
                    model = PersonnelVaccineProcess.Update(model, User.UserID);
                }
                ModelState.Clear();
                return PartialViewCustom("_PersonnelVaccine", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelVaccine(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelVaccineProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Vaccine not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Vaccine not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Training...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTrainings(long PersonnelID)
        {
            try
            {
                Trainings model = new Trainings
                {
                    PersonnelID = PersonnelID,
                    Training = PersonnelTrainingProcess.GetByPersonnelID(PersonnelID)
                };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelTrainings", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewTraining(long PersonnelID)
        {
            try
            {
                PersonnelTraining model = new PersonnelTraining { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelTraining", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelTraining(long ID)
        {
            try
            {
                PersonnelTraining model = PersonnelTrainingProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelTraining", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveTraining(PersonnelTraining model)
        {
            try
            {
                if (model.ID == 0)
                {
                    model = PersonnelTrainingProcess.Create(model, User.UserID);
                }
                else
                {
                    model = PersonnelTrainingProcess.Update(model, User.UserID);
                }
                ModelState.Clear();
                return PartialViewCustom("_PersonnelTraining", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteTraining(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelTrainingProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Training not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Training not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

        #region Department...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelDepartment(long PersonnelID)
        {
            try
            {
                PersonnelDepartment model = new PersonnelDepartment { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelDepartment", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelDepartment(long ID)
        {
            try
            {
                PersonnelDepartment model = PersonnelDepartmentProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelDepartment", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelDepartment(PersonnelDepartment model)
        {
            try
            {
                model = PersonnelDepartmentProcess.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelDepartment", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #endregion

        #region Assigned Location...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelAssignedLocation(long PersonnelID)
        {
            try
            {
                PersonnelAssignedLocation model = new PersonnelAssignedLocation { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelAssignedLocation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelAssignedLocation(long ID)
        {
            try
            {
                PersonnelAssignedLocation model = PersonnelAssignedLocationProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelAssignedLocation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelAssignedLocation(PersonnelAssignedLocation model)
        {
            try
            {
                model = PersonnelAssignedLocationProcess.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelAssignedLocation", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #endregion

        #region Employment Type...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelEmploymentType(long PersonnelID)
        {
            try
            {
                PersonnelEmploymentType model = new PersonnelEmploymentType { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelEmploymentType", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelEmploymentType(long ID)
        {
            try
            {
                PersonnelEmploymentType model = PersonnelEmploymentTypeProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelEmploymentType", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelEmploymentType(PersonnelEmploymentType model)
        {
            try
            {
                model = PersonnelEmploymentTypeProcess.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelEmploymentType", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #endregion

        #region Position...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelPosition(long PersonnelID)
        {
            try
            {
                PersonnelPosition model = new PersonnelPosition { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelPosition", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelPosition(long ID)
        {
            try
            {
                PersonnelPosition model = PersonnelPositionProcess.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelPosition", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelPosition(PersonnelPosition model)
        {
            try
            {
                model = PersonnelPositionProcess.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelPosition", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #endregion

        #region Leave Credits...
        public ActionResult GetLeaveCredits(long PersonnelID)
        {
            try
            {
                List<PersonnelLeaveCredit> leaveCredits = PersonnelLeaveCreditProcess.Instance.GetByPersonnelID(PersonnelID);
                PersonnelLeaveCredits model = new PersonnelLeaveCredits
                {
                    PersonnelID = PersonnelID,
                    PersonnelLeaveCreditDate = leaveCredits.Where(x => x._LeaveType.IsMidYear ?? false).ToList(),
                    PersonnelLeaveCreditYear = leaveCredits.Where(x => !(x._LeaveType.IsMidYear ?? false)).ToList()
                };

                return PartialViewCustom("_PersonnelLeaveCredits", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelLeaveCredits(long PersonnelID)
        {
            try
            {
                PersonnelLeaveCredit model = new PersonnelLeaveCredit { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelLeaveCredit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelLeaveCredits(long ID)
        {
            try
            {
                PersonnelLeaveCredit model = PersonnelLeaveCreditProcess.Instance.Get(ID);
                ModelState.Clear();

                if (model._LeaveType.IsMidYear ?? false)
                    return PartialViewCustom("_PersonnelLeaveCreditDate", model);
                else
                    return PartialViewCustom("_PersonnelLeaveCreditYear", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelLeaveCredits(PersonnelLeaveCredit model)
        {
            try
            {
                model = PersonnelLeaveCreditProcess.Instance.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();

                if (model._LeaveType.IsMidYear ?? false)
                    return PartialViewCustom("_PersonnelLeaveCreditDate", model);
                else
                    return PartialViewCustom("_PersonnelLeaveCreditYear", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Delete Loan
        public JsonResult DeletePersonnelLeaveCredits(PersonnelLeaveCredit model)
        {
            try
            {
                PersonnelLeaveCreditProcess.Instance.Delete(model.ID, User.UserID);
                return Json(new { msg = true });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #endregion

        #region Loans...
        public ActionResult GetLoans(long PersonnelID)
        {
            try
            {
                PersonnelLoans model = new PersonnelLoans
                {
                    PersonnelID = PersonnelID,
                    PersonnelLoan = PersonnelLoanProcess.Instance.GetList(PersonnelID)
                };

                return PartialViewCustom("_PersonnelLoans", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        //List of deducted loan amount
        public ActionResult GetLoanDeductions(long PersonnelLoanID)
        {
            try
            {
                PersonnelLoanDeductions model = new PersonnelLoanDeductions
                {
                    PersonnelLoanId = PersonnelLoanID,
                    LoanDeductions = PayrollProcess.Instance.GetPersonnelLoanDeductions(PersonnelLoanID)
                };

                return PartialViewCustom("_PersonnelLoanDetails", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Creating new loan
        public ActionResult NewPersonnelLoan(long PersonnelID)
        {
            try
            {
                PersonnelLoan model = new PersonnelLoan { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelLoan", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Undo Loan
        public ActionResult CancelPersonnelLoan(long ID)
        {
            try
            {
                PersonnelLoan model = PersonnelLoanProcess.Instance.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelLoan", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Save Loan
        public ActionResult SavePersonnelLoan(PersonnelLoan model)
        {
            try
            {
                model = PersonnelLoanProcess.Instance.CreateOrUpdate(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_PersonnelLoan", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Delete Loan
        public JsonResult DeletePersonnelLoan(PersonnelLoan model)
        {
            try
            {
                PersonnelLoanProcess.Instance.Delete(model, User.UserID);
                return Json(new { msg = true });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #endregion

        #region Additional Loan...
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Get All Vale
        public ActionResult GetAdditionalLoan(long PersonnelID)
        {
            try
            {
                AdditionalLoans model = new AdditionalLoans
                {
                    PersonnelLoans = PersonnelLoanProcess.Instance.GetList(PersonnelID),
                    AdditionalLoanForApproval = AdditionalLoanProcess.Instance.GetList(PersonnelID)
                };

                return PartialViewCustom("_AdditionalLoans", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Merge vale to existing vale
        public ActionResult MergeAdditionalToExistingLoan(MergeAdditional merge)
        {
            try
            {
                PersonnelLoanProcess.Instance.MergeAdditionalToExistingLoan(merge.PersonnelLoanID, merge.AdditionalLoanID, merge.Amount, User.UserID);
                AdditionalLoans model = new AdditionalLoans
                {
                    PersonnelLoans = PersonnelLoanProcess.Instance.GetList(merge.PersonnelID),
                    AdditionalLoanForApproval = AdditionalLoanProcess.Instance.GetList(merge.PersonnelID)
                };

                return PartialViewCustom("_AdditionalLoans", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Add vale as new loan
        public ActionResult AddAdditionalLoan(long AdditionalLoanID)
        {
            try
            {
                var al = AdditionalLoanProcess.Instance.Get(AdditionalLoanID);
                if (al != null)
                {

                    PersonnelLoanModel model = new PersonnelLoanModel
                    {
                        PersonnelLoan = new PersonnelLoan
                        {
                            PersonnelID = al.PersonnelID,
                            Amount = al.Amount,
                            LoanID = al.LoanID,
                            Remarks = al.Remarks,
                            PayrollDeductible = true
                        },
                        AdditionalLoanID = al.ID
                    };

                    return PartialViewCustom("_PersonnelLoanAdditional", model);
                }
                else
                    return Json(new { msg = false, res = "Unable to process request." });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Save vale
        public ActionResult SaveAdditionalLoanToPersonnelLoan(PersonnelLoanModel loan)
        {
            try
            {
                PersonnelLoanProcess.Instance.InsertAdditionalLoanToPersonnelLoan(loan.AdditionalLoanID ?? 0, loan.PersonnelLoan, User.UserID);
                AdditionalLoans model = new AdditionalLoans
                {
                    PersonnelLoans = PersonnelLoanProcess.Instance.GetList(loan.PersonnelLoan.PersonnelID ?? 0),
                    AdditionalLoanForApproval = AdditionalLoanProcess.Instance.GetList(loan.PersonnelLoan.PersonnelID ?? 0)
                };

                return PartialViewCustom("_AdditionalLoans", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
        #endregion

        #region Schedule...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPersonnelSchedule(long PersonnelID)
        {
            try
            {
                PersonnelSchedule model = new PersonnelSchedule { PersonnelID = PersonnelID };

                ModelState.Clear();
                return PartialViewCustom("_PersonnelSchedule", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPersonnelSchedule(long ID)
        {
            try
            {
                PersonnelSchedule model = PersonnelScheduleProcess.Instance.Get(ID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelSchedule", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePersonnelSchedule(PersonnelSchedule model)
        {
            try
            {
                if ((model.SundayScheduleID ?? model.MondayScheduleID ?? model.TuesdayScheduleID ?? model.WednesdayScheduleID ?? model.ThursdayScheduleID ?? model.FridayScheduleID ?? model.SaturdayScheduleID ?? 0) == 0)
                    return Json(new { msg = false, res = "Atleast one day schedule is required." });

                if (model.EffectivityDate == null)
                    return Json(new { msg = false, res = "Effectivity Date is required." });

                model = PersonnelScheduleProcess.Instance.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_PersonnelSchedule", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePersonnelSchedule(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelScheduleProcess.Instance.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Personnel Schedule not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Schedule not found." });
            }
            return Json(new { msg = true });
        }
        #endregion

    }
}