using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "Sin {A}", Keywords = new[] { "Sin" }, Level = 1000000, ComputeType = typeof(SinComputeMethod))]
    public class SinComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double value = Math.Sin(arg1);
            return value;
        }

       
    }
}
