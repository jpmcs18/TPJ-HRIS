using DataAccessLayer.Security;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebTemplate.Models;
using ProcessLayer.Processes;
using System.Configuration;
using System.IO;

namespace WebTemplate
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new AuthorizationAttribute());
        }



        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            if(ctx.Session != null)
            {

                if (ctx.Session["Credentials"] is Credentials credentials)
                {
                    CustomPrincipal user = new(string.Format("{0}, {1} {2}", credentials.LastName, credentials.FirstName, credentials.MiddleName))
                    {
                        UserID = credentials.UserID,
                        FirstName = credentials.FirstName,
                        MiddleName = credentials.MiddleName,
                        LastName = credentials.LastName,
                        ForcePasswordChange = credentials.ForcePasswordChange,
                        EmailAddress = credentials.EmailAddress,
                        Username = credentials.UserName,
                        PersonnelID = credentials.PersonnelID,
                        Personnel = PersonnelProcess.Get(credentials.PersonnelID ?? 0)
                };
                    user.ImageFileExist = File.Exists(Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["ImageSaveLocation"]), user.Personnel.Image) ?? "");

                    HttpContext.Current.User = user;

                    if (user.ForcePasswordChange == true
                        && !Request.Url.AbsolutePath.Contains("Maintenance/Systems/UserProfile")
                        && !Request.Url.AbsolutePath.Contains("Security/Logout")
                        && !Request.Url.AbsolutePath.Contains("Security/Login")
                        && !Request.Url.AbsolutePath.Contains("Security/Error")
                        && !Request.Url.AbsolutePath.Contains("Security/ServerError")
                        && !Request.Url.AbsolutePath.Contains("Security/NotFound")
                        && !Request.Url.AbsolutePath.Contains("Security/Unauthorized"))
                    {
                        Response.Redirect("~/Maintenance/Systems/UserProfile");
                    }
                }
            }
        }

        protected void Application_Error()
        {
            try
            {
                string path = Server.MapPath("~/App_Data/logs/error_log.txt");
                Exception ex = Server.GetLastError();
                int err_code = 0;
                int err_id = 0;

                if (ex is HttpException httpex)
                    err_code = httpex?.GetHttpCode() ?? 0;

                if (err_code != 404)
                {
                    var error = new Error
                    {
                        HResult = ex.HResult,
                        Message = ex.Message,
                        InnerExceptionMessage = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        CreatedDate = DateTime.Now
                    };
                    error.Save();
                    err_id = error.ID;
                }

                if (!Request.Url.AbsolutePath.StartsWith("~/Security/NotFound") && err_code == 404)
                {
                    Response.Redirect("~/Security/NotFound");
                    return;
                }

                if (!Request.Url.AbsolutePath.StartsWith("~/Security/ServerError") && err_code == 500)
                {
                    Response.Redirect(String.Format("~/Security/ServerError?rid={0}", err_id));
                    return;
                }
                
                if (!Request.Url.AbsolutePath.StartsWith("~/Security/Error"))
                    Response.Redirect(String.Format("~/Security/Error?rid={0}", err_id));
            }
            catch{
            }
        }
    }
}
