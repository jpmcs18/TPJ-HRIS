using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using i = WebTemplate.Models.Maintenance.Lookup.Position;
namespace WebTemplate.Controllers.Maintenance.Lookup
{
    public class PositionController : BaseController
    {
        public ActionResult Index(i.Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Positions = PositionProcess.Instance.GetList(model.Filter, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;
            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_Search", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Get(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var model = PositionProcess.Instance.Get(id);

                    ModelState.Clear();
                    return PartialViewCustom("_Position", model);
                }
                else
                    throw new Exception("Unable to get position");
            }
            catch(Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Position position)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    position = PositionProcess.Instance.SavePosition(position, User.UserID);

                    ModelState.Clear();
                    return PartialViewCustom("_Position", position);
                }
                else
                    throw new Exception("Unable to save position.");
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    PositionProcess.Instance.Delete(id.Value, User.UserID);

                    ModelState.Clear(); 
                    return Json(new { msg = true });
                }
                else
                    throw new Exception("Unable to delete position.");
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
    }
}
