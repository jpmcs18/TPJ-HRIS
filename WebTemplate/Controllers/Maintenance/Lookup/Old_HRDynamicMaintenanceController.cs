using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Web.Mvc;

namespace WebTemplate.Controllers.Maintenance.Lookup
{

    public class Old_HRDynamicMaintenanceController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (!ViewBag.Page.View)
            {
                throw new Exception("Unauthorized Access");
            }

            Dictionary<string, object> tableDictionary = new Dictionary<string, object>();

            #region Table Dictionary Contents

            tableDictionary.Add("Compensation", "Compensation");
            tableDictionary.Add("Currency", "Currency");
            tableDictionary.Add("Deduction", "Deduction");
            tableDictionary.Add("Educational Level", "EducationalLevel");
            tableDictionary.Add("Employment Type", "EmploymentType");
            tableDictionary.Add("License Type", "LicenseType");
            tableDictionary.Add("Personnel Type", "PersonnelType");
            tableDictionary.Add("Position", "Position");
            tableDictionary.Add("Religion", "Religion");

            #endregion

            return ViewCustom(tableDictionary);
        }

        [HttpPost]
        public ActionResult Get(string target_table)
        {
            try
            {
                //Check Target Table
                if (string.IsNullOrEmpty(target_table))
                {
                    throw new Exception("Target table is invalid");
                }

                dynamic expando = new ExpandoObject();
                IDictionary<string, object> expandoModel = expando as IDictionary<string, object>;

                //Get Entity Type
                Type modelType = GetModelType(target_table, "ProcessLayer.Entities");
                //Get Entity Property List
                List<string> modelProperties = GetModelProperties(modelType);

                //Get Process Type
                Type modelProcessType = GetModelType($"{target_table}Process", "ProcessLayer.Processes");
                //Get Process Instance
                object modelProcessInstance = GetModelInstance(modelProcessType);
                //Invoke Process Method [GetList]
                object modelProcessObj = GetMethodInfo(modelProcessType, "GetList").Invoke(modelProcessInstance, null);
                //Get Entity Dictionary from the result of the Invoked Process Method
                List<Dictionary<string, Tuple<string, object>>> modelList = GetModelListDictionary(modelProcessObj);

                expandoModel.Add("Properties", modelProperties);
                expandoModel.Add("LookupList", modelList);

                return PartialViewCustom("_Search", expandoModel);
            }
            catch (Exception ex)
            {
                return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Management(string target_table, int? uid)
        {
            try
            {
                //Check Target Table
                if (string.IsNullOrEmpty(target_table))
                {
                    throw new Exception("Target table is invalid");
                }

                dynamic expando = new ExpandoObject();
                IDictionary<string, object> expandoModel = expando as IDictionary<string, object>;
                Dictionary<string, Tuple<string, object>> propertyModel = new Dictionary<string, Tuple<string, object>>();

                //Get Entity Type
                Type modelType = GetModelType(target_table, "ProcessLayer.Entities");
                //Get Entity Instance
                object modelInstance = GetModelInstance(modelType);

                //Get Process Type
                Type modelProcessType = GetModelType($"{target_table}Process", "ProcessLayer.Processes");
                //Get Process Instance
                object modelProcessInstance = GetModelInstance(modelProcessType);

                if (uid.HasValue)
                {
                    //Invoke Process Method [GetByID]
                    object modelObj = GetMethodInfo(modelProcessType, "GetByID").Invoke(modelProcessInstance, new object[] { uid });

                    if (modelObj == null)
                        throw new Exception("Selected Item not found");

                    //Get Entity Dictionary from the result of the Invoked Process Method
                    propertyModel = GetModelDictionary(modelObj);
                }
                else
                {
                    //Get Entity Dictionary of the Entity Instance
                    propertyModel = GetModelDictionary(modelInstance);
                }

                expandoModel.Add("SelectedTable", target_table);
                expandoModel.Add("Selected", propertyModel);

                return PartialViewCustom("_Management", expandoModel);
            }
            catch (Exception ex)
            {
                return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(string target_table, Dictionary<string, object> model)
        {
            try
            {
                //Check Target Table
                if (string.IsNullOrEmpty(target_table))
                {
                    throw new Exception("Target table is invalid");
                }

                if (model == null)
                {
                    throw new Exception("Values are not valid");
                }

                //Get Model Type
                Type modelType = GetModelType(target_table, "ProcessLayer.Entities");
                //Get Model Instance
                object modelInstance = GetModelInstance(modelType);
                //Set Model Instance Values using Dictionary
                SetModelValues(modelType, modelInstance, model);

                //Get Process Type
                Type modelProcessType = GetModelType($"{target_table}Process", "ProcessLayer.Processes");
                //Get Process Instance
                object modelProcessInstance = GetModelInstance(modelProcessType);
                //Set Method Parameters
                object[] methodParameters = new object[] { modelInstance, User.UserID };
                //Invoke Process Method [CreateOrUpdate]
                GetMethodInfo(modelProcessType, "CreateOrUpdate").Invoke(modelProcessInstance, methodParameters);

                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(string target_table, int? uid)
        {
            try
            {
                //Check Target Table
                if (string.IsNullOrEmpty(target_table))
                {
                    throw new Exception("Target table is invalid");
                }

                if (uid.HasValue)
                {
                    //Get Process Type
                    Type modelProcessType = GetModelType($"{target_table}Process", "ProcessLayer.Processes");
                    //Get Process Instance
                    object modelProcessInstance = GetModelInstance(modelProcessType);
                    //Set Method Parameters
                    object[] methodParameters = new object[] { uid, User.UserID };
                    //Invoke Process Method [Delete]
                    GetMethodInfo(modelProcessType, "Delete").Invoke(modelProcessInstance, methodParameters);
                }
                else
                {
                    throw new Exception("Selected Item is invalid");
                }

                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }
        
        #region Reflection Functions

        private object GetModelInstance(Type target_type)
        {
            try
            {
                object obj = Activator.CreateInstance(target_type);

                if (obj == null)
                    throw new Exception();

                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create model instance");
            }
        }

        private Type GetModelType(string target_name, string target_namespace)
        {
            try
            {
                Type type = null;

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    Type assemblyType = assembly.GetType($"{target_namespace}.{target_name}");
                    if (assemblyType != null)
                    {
                        type = assemblyType;
                        break;
                    }

                }

                if (type == null)
                    throw new Exception();

                return type;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model type");
            }
        }

        private PropertyInfo[] GetModelPropertyInfo(Type target_type)
        {
            try
            {
                PropertyInfo[] p = target_type.GetProperties();

                if (p == null)
                    throw new Exception();

                return p;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model properties");
            }
        }

        private PropertyInfo GetModelPropertyInfo(Type target_type, string target_property)
        {
            try
            {
                PropertyInfo p = target_type.GetProperty(target_property);

                if (p == null)
                    throw new Exception();

                return p;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model specified property");
            }
        }

        private MethodInfo[] GetMethodInfo(Type target_type)
        {
            try
            {
                MethodInfo[] m = target_type.GetMethods();

                if (m == null)
                    throw new Exception();

                return m;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model methods");
            }
        }

        private MethodInfo GetMethodInfo(Type target_type, string target_method)
        {
            try
            {
                MethodInfo m = target_type.GetMethod(target_method);

                if (m == null)
                    throw new Exception();

                return m;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model specified method");
            }
        }

        #endregion

        #region Reflection Helper Functions

        private List<string> GetModelProperties(Type model_type)
        {
            try
            {
                List<string> pL = new List<string>();
                foreach (PropertyInfo p in GetModelPropertyInfo(model_type))
                {
                    pL.Add(p.Name);
                }

                return pL;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model property list");
            }
        }

        private Dictionary<string, Tuple<string, object>> GetModelDictionary(object model_object)
        {
            try
            {
                // Property Name; Property Type; Property Value
                Dictionary<string, Tuple<string, object>> model = new Dictionary<string, Tuple<string, object>>();

                Type modelType = model_object.GetType();
                object modelInstance = GetModelInstance(modelType);
                PropertyInfo[] modelProperties = GetModelPropertyInfo(modelType);

                if (modelProperties != null)
                {
                    foreach (var modelPropertyItem in modelProperties)
                    {
                        model.Add(modelPropertyItem.Name, new Tuple<string, object>(modelPropertyItem.PropertyType.Name, modelPropertyItem.GetValue(model_object, null)));
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model");
            }
        }

        private List<Dictionary<string, Tuple<string, object>>> GetModelListDictionary(object model_list)
        {
            try
            {
                // Property Name; Property Type; Property Value
                List<Dictionary<string, Tuple<string, object>>> model = new List<Dictionary<string, Tuple<string, object>>>();

                IEnumerable e = model_list as IEnumerable;
                if (model_list != null)
                {
                    foreach (var modelListItem in e)
                    {
                        Type modelListItemType = modelListItem.GetType();
                        object modelListItemInstance = GetModelInstance(modelListItemType);
                        PropertyInfo[] modelListItemProperties = GetModelPropertyInfo(modelListItemType);

                        Dictionary<string, Tuple<string, object>> m = new Dictionary<string, Tuple<string, object>>();
                        if (modelListItemProperties != null)
                        {
                            foreach (var pItem in modelListItemProperties)
                            {
                                m.Add(pItem.Name, new Tuple<string, object>(pItem.PropertyType.Name, pItem.GetValue(modelListItem, null)));
                            }
                        }
                        model.Add(m);
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to fetch model list");
            }
        }

        private void SetModelValues(Type model_type, object model_instance, Dictionary<string, object> model)
        {
            try
            {
                PropertyInfo[] property_list = GetModelPropertyInfo(model_type);
                foreach (PropertyInfo property_list_item in property_list)
                {
                    if (model[property_list_item.Name] != null && !string.IsNullOrEmpty(((string[])model[property_list_item.Name])[0]))
                    {
                        //Catch Inconvertible values 	
                        string dictionaryValue = ((string[])model[property_list_item.Name])[0];
                        property_list_item.SetValue(model_instance, Convert.ChangeType(dictionaryValue, property_list_item.PropertyType));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}