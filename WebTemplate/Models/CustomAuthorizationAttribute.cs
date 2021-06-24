using DataAccessLayer.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace WebTemplate.Models
{
    public class CustomAuthorizationAttribute : AuthorizeAttribute
    {
        public string LocalURL { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string localurl = LocalURL;
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

                if (cp == null)
                    return false;
                else
                {
                    return cp.IsAccessible(LocalURL);
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