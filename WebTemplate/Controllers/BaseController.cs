using DataAccessLayer.Security;
using DataAccessLayer.System;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebTemplate.Models;

namespace WebTemplate.Controllers
{

    public class BaseController : Controller
    {
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Page p = GetPage() ?? new Page();
            ViewBag.Page = p;
            base.OnActionExecuting(filterContext);
        }

        [NonAction]
        public Page GetPage()
        {
            string action = ControllerContext.RouteData.Values["action"]?.ToString();

            string url = string.Empty;
            if (Request.Url.AbsolutePath.EndsWith(action))
            {
                string[] parts = Request.Url.AbsolutePath.Split('/');
                for (int i = 0; i < parts.Length - 1; i++)
                    url += string.Format("{0}/", parts[i]);
            }
            else
                url = Request.Url.AbsolutePath;

            if (string.IsNullOrEmpty(Properties.Settings.Default.WebPrefix))
                return (Session["CredentialPages"] as CredentialPages)?.GetPage(url);
            else
            {
                url = Regex.Replace(url, Properties.Settings.Default.WebPrefix, "", RegexOptions.IgnoreCase);
                return (Session["CredentialPages"] as CredentialPages)?.GetPage(url);
            }
                
        }

        [NonAction]
        public Page GetPage(string url)
        {
            return (Session["CredentialPages"] as CredentialPages)?.GetPage(url);
        }

        [NonAction]
        public Credentials GetCredentials()
        {
            return Session["Credentials"] as Credentials;
        }

        [NonAction]
        public void SetCredentials()
        {
            if (Session["Credentials"] == null)
                Session["Credentials"] = new Credentials(this.User.UserID);

            if (Session["CredentialRoles"] == null)
                Session["CredentialRoles"] = new CredentialRoles(Session["Credentials"] as Credentials);

            if (Session["CredentialPages"] == null)
                Session["CredentialPages"] = new CredentialPages(Session["CredentialRoles"] as CredentialRoles, Properties.Settings.Default.ApplicationName);

            if (Session["NavMenu"] == null)
            {
                var navmenu = new NavMenu();
                Session["NavMenu"] = navmenu.NavMenusByRole(Session["CredentialPages"] as CredentialPages);
            }
        }

        [NonAction]
        public void SetCredentials(int userid)
        {
            Session["Credentials"] = new Credentials(userid);
            Session["CredentialRoles"] = new CredentialRoles(Session["Credentials"] as Credentials);
            Session["CredentialPages"] = new CredentialPages(Session["CredentialRoles"] as CredentialRoles, Properties.Settings.Default.ApplicationName);

            var navmenu = new NavMenu();
            Session["NavMenu"] = navmenu.NavMenusByRole(Session["CredentialPages"] as CredentialPages);
        }

        [NonAction]
        public void RemoveCredential()
        {
            Session["Credentials"] = null;
            Session["CredentialRoles"] = null;
            Session["CredentialPages"] = null;
            Session["NavMenu"] = null;
        }

        [NonAction]
        public ViewResult ViewCustom()
        {
            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath());
            return View(viewpath);
        }
        [NonAction]
        public ViewResult ViewCustom(string ViewName)
        {
            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath(ViewName));
            return View(viewpath);
        }
        [NonAction]
        public ViewResult ViewCustom(object model)
        {
            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath());
            return View(viewpath, model);
        }
        [NonAction]
        public ViewResult ViewCustom(string ViewName, object model)
        {
            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath(ViewName));
            return View(viewpath, model);
        }
        [NonAction]
        public PartialViewResult PartialViewCustom()
        {
            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath());
            return PartialView(viewpath);
        }
        [NonAction]
        public PartialViewResult PartialViewCustom(string ViewName)
        {
            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath(ViewName));
            return PartialView(viewpath);
        }
        [NonAction]
        public PartialViewResult PartialViewCustom(object model)
        {

            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath());
            return PartialView(viewpath, model);
        }
        [NonAction]
        public PartialViewResult PartialViewCustom(string ViewName, object model)
        {

            string viewpath = string.Format("~/Views{0}.cshtml", GetAbsoluteUrlPath(ViewName));
            return PartialView(viewpath, model);
        }

        [NonAction]
        public string GetAbsoluteUrlPath()
        {
            string action = ControllerContext.RouteData.Values["action"]?.ToString();
            string controller = ControllerContext.RouteData.Values["controller"]?.ToString();

            string url = Request.Url.AbsolutePath;
            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.WebPrefix))
            {
                url = Regex.Replace(url, string.Format("{0}/", Properties.Settings.Default.WebPrefix), "", RegexOptions.IgnoreCase);
            }

            if (url == "/")
                url = string.Format("/{0}/{1}", controller, action);

            if (!url.EndsWith(action))
            {
                url = string.Format("{0}/{1}", url, action);
            }

            return url;
        }
        [NonAction]
        public string GetAbsoluteUrlPath(string ViewName)
        {
            string controller = ControllerContext.RouteData.Values["controller"]?.ToString();
            string action = ControllerContext.RouteData.Values["action"]?.ToString();

            string url = Request.Url.AbsolutePath;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.WebPrefix))
            {
                url = Regex.Replace(url, string.Format("{0}/", Properties.Settings.Default.WebPrefix), "", RegexOptions.IgnoreCase);
            }

            if (url == "/")
                url = string.Format("/{0}/{1}", controller, ViewName);
            else
            { 
                if (url.EndsWith(action))
                {
                    string[] parts = url.Split('/');
                    string temp = string.Empty;
                    for (int i = 0; i < parts.Length - 1; i++)
                        temp += string.Format("{0}/", parts[i]);

                    url = temp.Remove(temp.Length - 1, 1);
                }

                url = string.Format("{0}/{1}", url, ViewName);
            }
            
            return url;
        }
        [NonAction]
        public string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}