using System;
using Laura.Compute.Utils;

namespace Laura.Compute.Extend.DateMethod
{
    [Serializable]
    [ComputeExpress(Express = "DATEFORMAT {A}", Keywords = new[] { "DATEFORMAT" }, Level = 1000000, ComputeType = typeof(DateFormatComputeMethod))]
    public class DateFormatComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash).Trim();
            DateTime arg2 = Tools.ToDateTime(ArgumentsString(0, expressSchema, objOrHash), DateTime.MinValue);
            string value = string.IsNullOrEmpty(arg1) ? arg2.ToString("yyyy-MM-dd hh:mm:ss") : arg2.ToString(arg1);
            return value;
        }
    }
}
