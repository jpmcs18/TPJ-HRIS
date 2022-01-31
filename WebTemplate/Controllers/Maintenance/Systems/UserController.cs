using System;
using System.Web.Mvc;
using DataAccessLayer.System;
using DataAccessLayer.Security;
using WebTemplate.Models.Maintenance.Systems.User;
using DataAccessLayer.HR;
using ProcessLayer.Processes;

namespace WebTemplate.Controllers.Maintenance.Systems
{
    public class UserController : BaseController
    {
        public ActionResult Index(Index model)
        {
            var user = new User();

            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.FilterStatus = ViewBag.Page.Approve ? model.FilterStatus : 1;
            model.ItemList = user.GetList(model.PageNumber, model.GridCount, model.Filter,model.FilterStatus, out int count);
            model.ItemCount = count;

            if (Request.IsAjaxRequest())
            {
                return PartialViewCustom("_Search", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }

        public ActionResult Management(int? uid)
        {
            var model = new Management
            {
                MobileNumberPrefix = Properties.Settings.Default.PH_MOBILE_PREFIX
            };

            if (uid > 0)
            {
                model.User = new User(uid.Value);
            }

            if (model.User == null)
            {
                model.User = new User();
            }

            if (model.User.Personnel == null)
            {
                model.User.Personnel = new Personnel();
            }

            return PartialViewCustom("_Management", model);
        }

        public ActionResult AddRole(Management model)
        {
            model.User.AddRole(model.SelectedRoleID);
            return PartialViewCustom("_Roles", model);
        }
        
        public ActionResult RemoveRole(Management model)
        {
            model.User.RemoveRole(model.SelectedRoleID);
            return PartialViewCustom("_Roles", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Management model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new System.Collections.Generic.List<string>();
                    foreach (ModelState modelState in ModelState.Values)
                        foreach (ModelError error in modelState.Errors)
                            errors.Add(error.ErrorMessage);
                    throw new Exception("&middot;" + String.Join("</br>&middot;", errors));
                }

                if (model.User.IsExist())
                {
                    if (!ViewBag.Page.Update)
                    {
                        throw new Exception("Access Denied!");
                    }
                }
                else
                {
                    if (!ViewBag.Page.Insert)
                    {
                        throw new Exception("Access Denied!");
                    }
                }
                
                model.User.Save(GetCredentials());
                
                return Json(new { success = true, id = model.User.ID });
            }
            catch (Exception ex)
            {
                string error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                error = error.Replace("\'", "").Replace(Environment.NewLine, "");
                return Json(new { success = false, msg = error });
            }
        }

        public ActionResult SearchPersonnel(SearchPersonnel model)
        {
            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = PersonnelProcess.GetApprovedPersonnel(model.Filter, model.PageNumber, model.GridCount, out int count);
            model.ItemCount = count;

            return PartialViewCustom("_SearchPersonnel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? uid, int? did)
        {
            try
            {
                if (!ViewBag.Page.Delete)
                {
                    throw new Exception("Access Denied!");
                }

                if (uid > 0)
                {
                    var user = new User(uid.Value);
                    user.Delete();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                error = error.Replace("\'", "").Replace(Environment.NewLine, "");
                return Json(new { success = false, msg = error });
            }
        }

        public ActionResult SendCode(VerifyMobile model)
        {
            var sms = new SMSVerification
            {
                UserID = model.UserID,
                CreatedBy = User.UserID
            };
            sms.CreateAndSend(false);

            model.MobileNumberPrefix = Properties.Settings.Default.PH_MOBILE_PREFIX;
            model.MobileNumber = sms.MobileNumber;

            return PartialViewCustom("_VerifyMobile", model);
        }
        public ActionResult ResendCode(VerifyMobile model)
        {
            var sms = new SMSVerification
            {
                UserID = model.UserID,
                CreatedBy = User.UserID
            };
            sms.CreateAndSend(false);

            model.MobileNumberPrefix = Properties.Settings.Default.PH_MOBILE_PREFIX;
            model.MobileNumber = sms.MobileNumber;
            model.Resend = true;

            return PartialViewCustom("_VerifyMobile", model);
        }
        public ActionResult ConfirmCode(VerifyMobile model)
        {
            var sms = new SMSVerification
            {
                UserID = model.UserID,
                Code = model.Code
            };

            model.Verified = sms.Verify(Properties.Settings.Default.VERIFICATIONCODE_EXPIRATION_MINUTES);
            
            if (!model.Verified)
            {
                ModelState.AddModelError(nameof(model.Code), "Invalid Code");
            }
            model.MobileNumberPrefix = Properties.Settings.Default.PH_MOBILE_PREFIX;
            model.MobileNumber = model.MobileNumber;

            return PartialViewCustom("_VerifyMobile", model);
        }
    }
}