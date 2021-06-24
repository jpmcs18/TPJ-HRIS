using DataAccessLayer.Security;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebTemplate.Models;

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
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            //if (authCookie != null)
            //{
            //    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //    JavaScriptSerializer serializer = new JavaScriptSerializer();
            //    string decUserdata = authTicket.UserData;
            //    CustomPrincipalSerialize serializeModel = serializer.Deserialize<CustomPrincipalSerialize>(decUserdata);

            //    CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
            //    newUser.UserID = serializeModel.UserID;
            //    newUser.FirstName = serializeModel.FirstName;
            //    newUser.LastName = serializeModel.LastName;
            //    newUser.Department = serializeModel.Department;
            //    newUser.Position = serializeModel.Position;
            //    newUser.ForcePasswordChange = serializeModel.ForcePasswordChange;

            //    HttpContext.Current.User = newUser;

            //    if (newUser.ForcePasswordChange == true 
            //        && !Request.Url.AbsolutePath.Contains("Maintenance/Systems/UserProfile")
            //        && !Request.Url.AbsolutePath.Contains("Security/Logout")
            //        && !Request.Url.AbsolutePath.Contains("Security/Login")
            //        && !Request.Url.AbsolutePath.Contains("Security/Error")
            //        && !Request.Url.AbsolutePath.Contains("Security/ServerError")
            //        && !Request.Url.AbsolutePath.Contains("Security/NotFound")
            //        && !Request.Url.AbsolutePath.Contains("Security/Unauthorized"))
            //    {
            //        Response.Redirect("~/Maintenance/Systems/UserProfile");
            //    }
            //}

            var ctx = HttpContext.Current;
            if(ctx.Session != null)
            {

                if (ctx.Session["Credentials"] is Credentials credentials)
                {
                    CustomPrincipal user = new CustomPrincipal(string.Format("{0}, {1} {2}", credentials.LastName, credentials.FirstName, credentials.MiddleName))
                    {
                        UserID = credentials.UserID,
                        FirstName = credentials.FirstName,
                        MiddleName = credentials.MiddleName,
                        LastName = credentials.LastName,
                        ForcePasswordChange = credentials.ForcePasswordChange,
                        EmailAddress = credentials.EmailAddress,
                        PersonnelID = credentials.PersonnelID
                    };

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

                if (err_code != 404 /*&& err_code != 500*/)
                {
                    var error = new Error
                    {
                        HResult = ex.HResult,
                        Message = ex.Message,
                        InnerExceptionMessage = ex.InnerException.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        CreatedDate = DateTime.Now
                    };
                    error.Save();
                    err_id = error.ID;
                }
                
                if (!Request.Url.AbsolutePath.StartsWith("~/Security/NotFound") && err_code == 404)
                    Response.Redirect("~/Security/NotFound");
                else
                if (!Request.Url.AbsolutePath.StartsWith("~/Security/ServerError") && err_code == 500)
                    Response.Redirect(String.Format("~/Security/ServerError?rid={0}", err_id));
                else
                if (!Request.Url.AbsolutePath.StartsWith("~/Security/Error"))
                    Response.Redirect(String.Format("~/Security/Error?rid={0}", err_id));

            }
            #pragma warning disable CS0168
            catch (Exception ex)
            {
            #pragma warning restore CS0168 
            }
        }
    }
}
