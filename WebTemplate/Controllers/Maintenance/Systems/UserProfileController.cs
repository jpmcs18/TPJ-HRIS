using System;
using System.Web.Mvc;
using DataAccessLayer.System;
using WebTemplate.Models.Maintenance.Systems.UserProfile;

namespace WebTemplate.Controllers.Maintenance.Systems
{
    public class UserProfileController : BaseController
    {
        // GET: UserProfile
        public ActionResult Index()
        {
            var model = new Index
            {
                User = new User(User.UserID)
            };

            return ViewCustom("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Index model)
        {
            try
            {
                //if (String.IsNullOrWhiteSpace(model.CurrentPassword))
                //{
                //    throw new Exception("Please enter your current password.");
                //}

                if (String.IsNullOrWhiteSpace(model.NewPassword))
                {
                    throw new Exception("Please enter your new password.");
                }
                
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    throw new Exception("New Password and Confirm Password does not match");
                }

                if (model.NewPassword.Length < Properties.Settings.Default.MaxPassLength)
                {
                    throw new Exception(String.Format("Password must be at least {0} letters long.", Properties.Settings.Default.MaxPassLength));
                }

                var user = new User();
                user.ChangePassword(User.UserID, model.CurrentPassword, model.NewPassword);
                SetCredentials(User.UserID);
                TempData["NewRecord"] = User.UserID;
                return Json(new { success = true, id = User.UserID });
            }
            catch (Exception ex)
            {
                string error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                error = error.Replace("\'", "").Replace(Environment.NewLine, "");
                return Json(new { success = false, msg = error });
            }
        }
    }
}