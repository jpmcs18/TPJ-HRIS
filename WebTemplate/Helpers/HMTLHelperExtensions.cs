using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WebTemplate
{
    public static class HttpPostFileBaseExtension
    {
        public static string SaveFile(this HttpPostedFileBase file, string directory, string fileName)
        {
            if (file == null || file.ContentLength <= 0)
            {
                return null;
            }

            string path = Path.Combine(directory, fileName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (File.Exists(path))
                File.Delete(path);

            if (File.Exists(path))
                File.Delete(path);

            file.SaveAs(path);

            return fileName;
        }
    }
    public static class HMTLHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null, string link = null)
        {
            
            if (String.IsNullOrEmpty(cssClass)) 
                cssClass = "active";

            if (link != null)
            {
                string url = HttpContext.Current.Request.Url.AbsolutePath;
                if (url.StartsWith(link))
                    return cssClass;
            }

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

	}
}
