using DataAccessLayer.Security;
using System.Web.Mvc;
using System.Web.Security;
using WebTemplate.Models;

namespace WebTemplate.Controllers
{
    [AllowAnonymous]
    public class SecurityController : BaseController
    {
        public ActionResult Error(string rid)
        {
            ViewBag.ErrorID = rid;
            return ViewCustom();
        }

        public ActionResult ServerError(string rid)
        {
            ViewBag.ErrorID = rid;
            return ViewCustom();
        }

        public ActionResult NotFound()
        {
            return ViewCustom();
        }

        public ActionResult Unauthorized()
        {
            if (Request.IsAjaxRequest())
                return JavaScript(string.Format("window.location = '{0}'", Url.Action("Unauthorized")));

            return ViewCustom();
        }
        
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if(Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (Properties.Settings.Default.isDevMode)
            {
                CustomPrincipalSerialize principal = new CustomPrincipalSerialize
                {
                    UserID = 1, //superadmin
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailAddress = "Email@address.com"
                };
                SetCredentials(1);
                return RedirectToLocal(returnUrl);
            }

            if (Request.IsAjaxRequest())
                return JavaScript(string.Format("window.location = '{0}'", returnUrl));

            return ViewCustom(Request.Browser.IsMobileDevice ? "Login_Mobile" : "Login", new Login());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            if (Properties.Settings.Default.MobileVerification)
            {
                var wasauthenticated = model.IsAuthenticated ?? false;

                if (wasauthenticated)
                {
                    model.Username = Session["Username"]?.ToString();
                    model.Password = Session["Password"]?.ToString();
                }

                if (string.IsNullOrEmpty(model.Username))
                {
                    ModelState.AddModelError(nameof(model.Username), "Username field is required!");
                }
                else
                {
                    model.IsUserExists();

                    if (!model.IsValidUsername)
                    {
                        ModelState.AddModelError(nameof(model.Username), "Username does not exist!");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.Password))
                        {
                            ModelState.AddModelError(nameof(model.Password), "Password is Required!");
                        }
                        else
                        {
                            model._Login();

                            if (model.IsAuthenticated ?? false)
                            {
                                if (wasauthenticated)
                                {
                                    if (Request.Form["resend"] == "true")
                                    {
                                        var sms = new SMSVerification
                                        {
                                            UserID = model.UserID,
                                            CreatedBy = model.UserID
                                        };
                                        sms.CreateAndSend(true);
                                        ViewBag.resend = true;
                                    }
                                    else
                                    {
                                        var sms = new SMSVerification
                                        {
                                            UserID = model.UserID,
                                            Code = model.VerificationCode
                                        };
                                        if (sms.Verify(Properties.Settings.Default.VERIFICATIONCODE_EXPIRATION_MINUTES))
                                        {
                                            SetCredentials(model.UserID);
                                            model.LoginUpdate(model.UserID, Session.SessionID, Request.UserHostAddress, null, null);
                                            Session["Username"] = null;
                                            Session["Password"] = null;
                                            //returnUrl = returnUrl ?? "/";
                                            return RedirectToLocal(returnUrl);
                                        }
                                        else
                                        {
                                            ModelState.AddModelError(nameof(model.VerificationCode), "Incorrect Verification Code!");
                                        }
                                    }
                                }
                                else
                                {
                                    Session["Username"] = model.Username;
                                    Session["Password"] = model.Password;
                                    var sms = new SMSVerification
                                    {
                                        UserID = model.UserID,
                                        CreatedBy = model.UserID
                                    };
                                    sms.CreateAndSend(true);
                                }

                            }
                            else
                            if (model.UserStatusID == 2)
                            {
                                ModelState.AddModelError(nameof(model.Password), "Your account is inactive! Please contact administrator for assistance.");
                            }
                            else
                            if (model.UserStatusID == 3)
                            {
                                ModelState.AddModelError(nameof(model.Password), "Your account has been locked! Please contact administrator for assistance.");
                            }
                            else
                            {
                                ModelState.AddModelError(nameof(model.Password), "Incorrect Password!");
                            }
                        }
                    }
                }

            }
            else
            {
                if (string.IsNullOrEmpty(model.Username))
                {
                    ModelState.AddModelError(nameof(model.Username), "Username field is required!");
                }
                else
                {
                    model.IsUserExists();

                    if (!model.IsValidUsername)
                    {
                        ModelState.AddModelError(nameof(model.Username), "Username does not exist!");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.Password))
                        {
                            ModelState.AddModelError(nameof(model.Password), "Password is Required!");
                        }
                        else
                        {
                            model._Login();

                            if (model.IsAuthenticated ?? false)
                            {
                                SetCredentials(model.UserID);
                                model.LoginUpdate(model.UserID, Session.SessionID, Request.UserHostAddress, null, null);
                                //returnUrl = returnUrl ?? "/";
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            if (model.UserStatusID == 2)
                            {
                                ModelState.AddModelError(nameof(model.Password), "Your account is inactive! Please contact administrator for assistance.");
                            }
                            else
                            if (model.UserStatusID == 3)
                            {
                                ModelState.AddModelError(nameof(model.Password), "Your account has been locked! Please contact administrator for assistance.");
                            }
                            else
                            {
                                ModelState.AddModelError(nameof(model.Password), "Incorrect Password!");
                            }
                        }
                    }
                }
            }
            
            if (Request.IsAjaxRequest())
            {
                return PartialViewCustom(model);
            }
            
            return ViewCustom(Request.Browser.IsMobileDevice ? "Login_Mobile" : "Login", model);
        }
        
        public ActionResult Logout()
        {
            if (User != null)
            {
                var login = new Login();
                login.LogoutUpdate(User.UserID);
                FormsAuthentication.SignOut();
                RemoveCredential();
            }

            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                if (Request.IsAjaxRequest())
                    return JavaScript(string.Format("window.location = '{0}'", returnUrl));
                else
                    return Redirect(returnUrl);
            }
            else
            {
                if (Request.IsAjaxRequest())
                    return JavaScript(string.Format("window.location = '{0}'", Url.Action("Index", "Home")));
                else
                    return RedirectToAction("Index", "Home");
            }
        }
    }
}