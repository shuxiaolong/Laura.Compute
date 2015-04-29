using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "Cos {A}", Keywords = new[] { "Cos" }, Level = 1000000, ComputeType = typeof(CosComputeMethod))]
    public class CosComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double value = Math.Cos(arg1);
            return value;
        }

       
    }
}
