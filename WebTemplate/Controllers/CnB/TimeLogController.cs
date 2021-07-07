using LumenWorks.Framework.IO.Csv;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Lookups;
using ReportLayer.Helpers;
using ReportLayer.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models.Groups;
using WebTemplate.Models.TimeLog;

namespace WebTemplate.Controllers.HumanResource
{
    public class TimeLogController : BaseController
    {
        private const string SAVE_LOCATION = "TimelogFileSaveLocation";
        // GET: TimeLog
        public ActionResult Index()
        {
            return ViewCustom("_TimeLogIndex");
        }

        [HttpPost]
        public ActionResult GetPersonnels(string key, DateTime? startdate, DateTime? enddate)
        {
            try
            {
                var model = new PersonnelList
                {
                    Personnels = TimeLogProcess.GetPersonnels(key, startdate, enddate).ToList()
                };
                ModelState.Clear();
                return PartialViewCustom("_Employees", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        public ActionResult GetTimeLogs(long personnelid, DateTime? startdate, DateTime? enddate)
        {
            try
            {
                var model = new TimeLogList();
                model.TimeLogs = TimeLogProcess.Get(personnelid, startdate, enddate);
                ModelState.Clear();
                return PartialViewCustom("_TimeLogs", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ValidateTimelogFile(HttpPostedFileBase fileBase)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;
                if (fileBase != null && fileBase.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileBase.FileName);
                    var path = Path.Combine(directory, User.UserID + DateTime.Now.ToString("MMddyyyyHHmmss") + fileName);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);


                    fileBase.SaveAs(path);
                    ValidateTimelog t = await Converter(path);


                    ModelState.Clear();
                    return PartialViewCustom("_ValidateTimeLogs", t);
                }
                else
                {
                    return Json(new { msg = false, res = "Unable to validate." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UploadTimelog(string path)
        {
            try
            {
                ValidateTimelog t = await Converter(path);
                TimeLogProcess.ImportTimelog(t.Timelogs, User.UserID);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                return Json(new { msg = true, res = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTimesheet(long? personnelId, int? departmentId, DateTime start, DateTime end)
        {
            try
            {
                using (var timesheet = new PrintTimeSheet(Server.MapPath(PrintTimeSheetHelper.Instance.Template)))
                {
                    timesheet.PersonnelID = personnelId;
                    timesheet.DepartmentID = departmentId;
                    timesheet.Start = start;
                    timesheet.End = end;
                    timesheet.GenerateReport();
                    ViewBag.Content = timesheet.SaveToPDF();
                }

                return View("~/Views/PrintingView.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.Content = null;
                ViewBag.Error = ex.GetActualMessage();
                return View("~/Views/PrintingView.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPersonnel(string key)
        {
            try
            {
                var model = new PersonnelList();
                model.Personnels = PersonnelProcess.GetList(key);
                ModelState.Clear();
                return PartialViewCustom("_Employees", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDepartment(string key)
        {
            try
            {
                var model = new DepartmentList();
                model.Departments = DepartmentProcess.Instance.Search(key);
                ModelState.Clear();
                return PartialViewCustom("_TimeLogs", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [NonAction]
        public async Task<ValidateTimelog> Converter(string path)
        {
            ValidateTimelog t = new ValidateTimelog
            {
                Path = path
            };
            await Task.Run(() =>
            {
                try
                {
                    List<TimeLog> timelogs = TimeLogProcess.GetLast()?.OrderBy(x => x.LoginDate)?.ToList();
                    List<TimeLogTemplate> template = new List<TimeLogTemplate>();
                    var datas = System.IO.File.ReadAllLines(path);
                    if (datas?.Any() ?? false)
                    {
                        //Organize file data
                        for (int i = 0; i < datas.Length; i++)
                        {
                            if (string.IsNullOrEmpty(datas[i])) continue;

                            var logs = datas[i].Split('\t');
                            //validate timelog
                            if (!(logs?.Any() ?? false) || 
                                    logs.Length != 2 || 
                                    string.IsNullOrEmpty(logs[0]) || 
                                    string.IsNullOrEmpty(logs[1]) ||
                                    !int.TryParse(logs[0], out int bioId) ||
                                    !DateTime.TryParse(logs[1], out DateTime datetime))
                            {
                                t.InvalidTimelog.Add($"{i + 1} {datas[i]}");
                                continue;
                            }
                            template.Add(new TimeLogTemplate(bioId, datetime));
                        }

                        //group data by biometric id
                        var groups = template.GroupBy(x => x.BiometricsID).ToList();

                        //iterate timelog that is grouped by biometric id
                        foreach (var group in groups)
                        {
                            Personnel personnel = PersonnelProcess.GetByBiometricId(group.Key);
                            //validate if biometric id is exist in personnel
                            if ((personnel?.ID ?? 0) == 0)
                            {
                                t.PinNotRecognized.Add(group.Key.ToString());
                                continue;
                            }

                            //getting the last timelog of employee
                            TimeLog lastTimeLog = t.Timelogs?.Where(x => x.PersonnelID == personnel?.ID).LastOrDefault();
                            List<TimeLogTemplate> lsts = group.OrderBy(x => x.Date).ToList();
                            
                            //initialize timelog
                            TimeLog timeLog = new TimeLog()
                            {
                                PersonnelID = personnel.ID,
                                _Personnel = personnel,
                                BiometricsID = personnel.BiometricsID
                            };

                            //remove all timelog that is already inserted
                            if ((lastTimeLog?.ID ?? 0) > 0)
                            {
                                lsts = lsts.Where(x => (lastTimeLog.LogoutDate.HasValue && lastTimeLog.LogoutDate?.Date < x.Date) && lastTimeLog.LoginDate?.Date < x.Date).ToList();
                                if (!lastTimeLog.LogoutDate.HasValue)
                                    timeLog = lastTimeLog;
                            }

                            DateTime? start = lsts.FirstOrDefault()?.Date;
                            DateTime? end = lsts?.LastOrDefault()?.Date;

                            DateTime validationEnd = DateTime.Now;

                            DateTime? startvalidate = timeLog.LoginDate.HasValue ? timeLog.LoginDate : start;

                            if (startvalidate.HasValue)
                            {
                                var schedule = GlobalHelper.GetSchedule(personnel._Schedules, startvalidate.Value.Date);
                                if (schedule == null || (schedule?.TimeIn == null || schedule?.TimeOut == null) || schedule?.TimeIn < schedule?.TimeOut)
                                    validationEnd = new DateTime(startvalidate.Value.Year, startvalidate.Value.Month, startvalidate.Value.Day, 4, 59, 59).AddDays(1);
                                else if (schedule?.TimeIn > schedule?.TimeOut)
                                    validationEnd = new DateTime(startvalidate.Value.Year, startvalidate.Value.Month, startvalidate.Value.Day, 11, 59, 59).AddDays(1);
                            }


                            if (start == null)
                                continue;

                            foreach (var lst in lsts)
                            {
                                if (timeLog.LoginDate.HasValue && lst.Date > validationEnd)
                                {
                                    t.Timelogs.Add(timeLog);
                                    timeLog = new TimeLog
                                    {
                                        PersonnelID = personnel.ID,
                                        _Personnel = personnel,
                                        BiometricsID = personnel.BiometricsID
                                    };
                                }

                                if (!timeLog.LoginDate.HasValue)
                                {
                                    timeLog.LoginDate = lst.Date;
                                    var schedule = GlobalHelper.GetSchedule(personnel._Schedules, timeLog.LoginDate.Value.Date);
                                    if (schedule == null || (schedule?.TimeIn == null || schedule?.TimeOut == null) || schedule?.TimeIn < schedule?.TimeOut)
                                        validationEnd = new DateTime(timeLog.LoginDate.Value.Year, timeLog.LoginDate.Value.Month, timeLog.LoginDate.Value.Day, 4, 59, 59).AddDays(1);
                                    else if (schedule?.TimeIn > schedule?.TimeOut)
                                        validationEnd = new DateTime(timeLog.LoginDate.Value.Year, timeLog.LoginDate.Value.Month, timeLog.LoginDate.Value.Day, 11, 59, 59).AddDays(1);
                                    continue;
                                }

                                timeLog.LogoutDate = lst.Date;
                            }

                            if (timeLog?.PersonnelID != null)
                                t.Timelogs.Add(timeLog);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return t;
        }
    }
}