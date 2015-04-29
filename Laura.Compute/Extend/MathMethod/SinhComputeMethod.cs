using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "Sinh {A}", Keywords = new[] { "Sinh" }, Level = 1000000, ComputeType = typeof(SinhComputeMethod))]
    public class SinhComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double value = Math.Sinh(arg1);
            return value;
        }

       
    }
}
