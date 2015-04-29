using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "Tan {A}", Keywords = new[] { "Tan" }, Level = 1000000, ComputeType = typeof(TanComputeMethod))]
    public class TanComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double value = Math.Tan(arg1);
            return value;
        }

       
    }
}
