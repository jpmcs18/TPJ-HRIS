using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Helpers
{
    public static class ExceptionExtension
    {
        public static string GetActualMessage(this Exception ex)
        {
            if (ex.InnerException != null)
                return ex.InnerException.GetActualMessage();
            else
                return ex.Message;
        }
    }
}
