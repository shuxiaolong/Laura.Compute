using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} - {A}", Keywords = new[] { "-" }, Level = 1000, ComputeType = typeof(MinusComputeSymbol))]
    public class MinusComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double arg2 = ArgumentsDouble(1, expressSchema, objOrHash);
            double value = arg1 - arg2;
            return value;
        }
    }
}
