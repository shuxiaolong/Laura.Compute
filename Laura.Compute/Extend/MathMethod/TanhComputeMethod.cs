using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "Tanh {A}", Keywords = new[] { "Tanh" }, Level = 1000000, ComputeType = typeof(TanhComputeMethod))]
    public class TanhComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double value = Math.Tanh(arg1);
            return value;
        }

       
    }
}
