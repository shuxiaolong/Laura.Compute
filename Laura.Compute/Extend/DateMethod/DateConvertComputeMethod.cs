using System;
using Laura.Compute.Utils;

namespace Laura.Compute.Extend.DateMethod
{
    [Serializable]
    [ComputeExpress(Express = "CONVERTDATE {A}", Keywords = new[] { "CONVERTDATE" }, Level = 1000000, ComputeType = typeof(DateConvertComputeMethod))]
    public class DateConvertComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash);
            DateTime value = Tools.ToDateTime(arg1, DateTime.MinValue);
            return value;
        }
    }
}
