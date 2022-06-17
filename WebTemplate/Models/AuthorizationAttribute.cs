using DataAccessLayer.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebTemplate.Models
{
    public class AuthorizationAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorize = base.AuthorizeCore(httpContext);

            if (!isAuthorize)
                return false;
            else
            {
                HttpContext ctx = HttpContext.Current;

                
                // check  sessions here

                //if (ctx.Session["Credentials"] == null)
                //{
                //    CustomPrincipal user = ctx.User as CustomPrincipal;
                //    if (user != null && user.UserID > 0)
                //    {
                //        ctx.Session["Credentials"] = Security.Credentials(user.UserID);
                //        ctx.Session["CredentialRoles"] = Security.CredentialRoles(ctx.Session["Credentials"] as Credentials);
                //        ctx.Session["CredentialPages"] = Security.CredentialPages(ctx.Session["CredentialRoles"] as CredentialRoles);
                //        ctx.Session["NavMenu"] = Page.NavMenusByRole(ctx.Session["CredentialPages"] as CredentialPages);
                //    }
                //}

                CredentialPages cp = ctx.Session["CredentialPages"] as CredentialPages;
                ctx.Cache.Remove("__AppStartPage__~/_appstart.cshtml");
                if (Properties.Settings.Default.isDevMode)
                    return true;

                if (cp == null)
                    return false;
                else
                {
                    string action = RouteTable.Routes.GetRouteData(new HttpContextWrapper(ctx)).Values["Action"]?.ToString();
                    string url = string.Empty;
                    if (ctx.Request.Url.AbsolutePath.EndsWith(action))
                    {
                        string[] parts = ctx.Request.Url.AbsolutePath.Split('/');
                        for (int i = 0; i < parts.Length - 1; i++)
                            url += string.Format("{0}/", parts[i]);
                    }
                    else
                        url = ctx.Request.Url.AbsolutePath;

                    return string.IsNullOrEmpty(Properties.Settings.Default.WebPrefix) ? cp.IsAccessible(url) : cp.IsAccessible(url, Properties.Settings.Default.WebPrefix);
                }
                
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
                filterContext.Result = new HttpUnauthorizedResult();
            else
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Security", action = "Unauthorized" }));
        }
    }
}