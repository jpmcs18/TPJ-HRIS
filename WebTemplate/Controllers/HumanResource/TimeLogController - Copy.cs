using LumenWorks.Framework.IO.Csv;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
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
                var model = new PersonnelList();
                model.Personnels = TimeLogProcess.GetPersonnels(key, startdate, enddate).ToList();
                ModelState.Clear();
                return PartialViewCustom("_Emp", model);
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
                var model = new TimeLogList
                {
                    TimeLogs = TimeLogProcess.Get(personnelid, startdate, enddate).ToList()
                };
                ModelState.Clear();
                return PartialViewCustom("_TimeLogs", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ByDepartment()
        {
            try
            {
                return PartialViewCustom("_SearchDepartment");
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ByPersonnel()
        {
            try
            {
                var model = new PersonnelList();
                return PartialViewCustom("_SearchPersonnel", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ByAll()
        {
            try
            {
                return PartialViewCustom("_SearchAll");
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
            catch(Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UploadTimelog(string path, bool proceedAllValid = true)
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
                    List<TimeLog> timelogs = TimeLogProcess.GetLast()?.ToList();
                    TimeLog lastTimeLog = new TimeLog();
                    var datas = System.IO.File.ReadAllLines(path);
                    Personnel personnel = new Personnel();
                    if (datas?.Any() ?? false)
                    {
                        for(int i = 0; i < datas.Length; i++)
                        {
                            var logs = datas[i].Split('\t');
                            if (!(logs?.Any() ?? false))
                            {
                                t.InvalidTimelog.Add($"{i + 1}");
                                continue;
                            }
                            if (logs.Length != 2)
                            {
                                t.InvalidTimelog.Add($"{i + 1}");
                                continue;
                            }
                            if (string.IsNullOrEmpty(logs[0]) || string.IsNullOrEmpty(logs[1]))
                            {
                                t.InvalidTimelog.Add($"{i + 1}");
                                continue;
                            }

                            var bioID = logs[0].ToInt();
                            var date = logs[1].ToNullableDateTime();
                            if (personnel?.BiometricsID != bioID)
                            {
                                if (lastTimeLog?.LoginDate != null)
                                    t.Timelogs.Add(lastTimeLog);

                                personnel = PersonnelProcess.GetByBiometricId(bioID);
                                lastTimeLog = t.Timelogs?.Where(x => x.PersonnelID == personnel?.ID).LastOrDefault();

                                if (lastTimeLog == null)
                                    lastTimeLog = timelogs.Where(x => x.PersonnelID == personnel?.ID && x.LoginDate?.Date < date?.Date).FirstOrDefault();
                            }


                            try
                            {
                                if (personnel?.ID == null)
                                    t.PinNotRecognized.Add($"{i + 1} ({logs[0]})");
                                else
                                {
                                    if (lastTimeLog != null)
                                    {
                                        if (lastTimeLog?.LogoutDate != null)
                                        {
                                            if (lastTimeLog != null)
                                                t.Timelogs.Add(lastTimeLog);

                                            lastTimeLog = new TimeLog
                                            {
                                                _Personnel = personnel,
                                                PersonnelID = personnel.ID,
                                                BiometricsID = bioID,
                                                LoginDate = date,
                                            };
                                        }
                                        else
                                        {
                                            lastTimeLog.LogoutDate = date;
                                        }
                                    }
                                    else
                                    {
                                        lastTimeLog = new TimeLog
                                        {
                                            _Personnel = personnel,
                                            PersonnelID = personnel.ID,
                                            BiometricsID = bioID,
                                            LoginDate = date,
                                        };
                                    }

                                }
                            }
                            catch
                            {
                                t.InvalidTimelog.Add($"{i + 1}");
                            }
                        }

                        if (lastTimeLog?.LoginDate != null)
                            t.Timelogs.Add(lastTimeLog);
                    }
                }
                catch(Exception ex) {
                   throw new Exception("Unable to read file.");
                }
            });

            return t;
        }
    }
}