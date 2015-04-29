using System;

namespace Laura.Compute.Extend.DateMethod
{
    [Serializable]
    [ComputeExpress(Express = "GETDATE()", Keywords = new[] { "GETDATE" }, Level = 1000000, ComputeType = typeof(DateNowComputeMethod))]
    public class DateNowComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            DateTime value = DateTime.Now;
            return value;
        }
    }
}
