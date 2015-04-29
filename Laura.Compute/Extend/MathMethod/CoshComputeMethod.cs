using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "Cosh {A}", Keywords = new[] { "Cosh" }, Level = 1000000, ComputeType = typeof(CoshComputeMethod))]
    public class CoshComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double value = Math.Cosh(arg1);
            return value;
        }

       
    }
}
