using DBUtilities;
using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace ProcessLayer.Helpers
{
    static class OutParameterListExtension
    {
        public static void Add(this List<OutParameters> parameters, string parameterName, SqlDbType type)
        {
            parameters.Add(new OutParameters { ParameterName = parameterName, Type = type });
        }
        public static void Add(this List<OutParameters> parameters, string parameterName, SqlDbType type, object value)
        {
            parameters.Add(new OutParameters { ParameterName = parameterName, Type = type, Value = value });
        }
        public static object Get(this List<OutParameters> parameters, string parameterName)
        {
            return parameters.Where(x => x.ParameterName == parameterName).Select(x => x.Value).First();
        }
    }
}
