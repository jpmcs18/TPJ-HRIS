using System.Web.Mvc;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class RequestsApproverController : BaseController
    {
        // GET: Payroll
        public ActionResult Index(Models.RequestsApproval.Requests_Approver.Index model)
        {
            model.PageCount = 1;
            model.Page = 1;
            return ViewCustom("_RequestsApproverIndex", model);
        }
    }
}