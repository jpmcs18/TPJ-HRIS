using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccessLayer.System;

namespace DataAccessLayer.Security
{
    public class CredentialPages
    {
        private List<Page> _Pages;
        public List<Page> Pages
        {
            get
            {
                if (this._Pages == null)
                {
                    return new List<Page>();
                }
                else
                {
                    return this._Pages;
                }
            }
            set
            {
                this._Pages = value;
            }
        }

        public Page GetPage(string URL)
        {
            Page page = this.Pages.Where(p => p.URL?.ToUpper() == URL?.ToUpper().Trim('/', '\\')).FirstOrDefault();
            return page;
        }

        public bool IsAccessible(string URL)
        {
            if (URL?.ToUpper().Trim('/', '\\') == "Home/Index".ToUpper() || URL?.ToUpper().Trim('/', '\\') == "")
                return true;
            else
                return this.Pages.Any(p => p.URL?.ToUpper() == URL?.ToUpper().Trim('/', '\\'));

        }

        public bool IsAccessible(string URL, string prefix)
        {
            URL = Regex.Replace(URL, prefix, "", RegexOptions.IgnoreCase);

            if (URL?.ToUpper().Trim('/', '\\') == "/Home/Index".ToUpper() || URL?.ToUpper().Trim('/', '\\') == "")
                return true;
            else
                return this.Pages.Any(p => p.URL?.ToUpper() == URL?.ToUpper().Trim('/', '\\'));

        }

        public CredentialPages(CredentialRoles cr, string ApplicationName)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var roleperms = from pa in entity.PageAccesses
                                join p in entity.Pages on pa.PageID equals p.ID
                                where (pa.Deleted == null || pa.Deleted == false)
                                && (p.Application == null || p.Application == ApplicationName)
                                && (p.Deleted == null || p.Deleted == false)
                                && cr.Roles.Contains(pa.RoleID)
                                group pa by new { p.ID, p.PageName, p.URL, p.Module, p.PageOrder, p.NavMenuId } into rp
                                select new Page
                                {
                                    ID = rp.Key.ID,
                                    PageName = rp.Key.PageName,
                                    URL = rp.Key.URL,
                                    Module = rp.Key.Module,
                                    PageOrder = rp.Key.PageOrder ?? 0,
                                    NavMenuId = rp.Key.NavMenuId ?? 0,
                                    View = rp.Max(r => r.EnableView == true ? 1 : 0).Equals(1),
                                    Insert = rp.Max(r => r.EnableInsert == true ? 1 : 0).Equals(1),
                                    Update = rp.Max(r => r.EnableUpdate == true ? 1 : 0).Equals(1),
                                    Delete = rp.Max(r => r.EnableDelete == true ? 1 : 0).Equals(1),
                                    Approve = rp.Max(r => r.AllAccess == true ? 1 : 0).Equals(1)
                                };

                this.Pages = roleperms.ToList();
            }
        }
    }
}
