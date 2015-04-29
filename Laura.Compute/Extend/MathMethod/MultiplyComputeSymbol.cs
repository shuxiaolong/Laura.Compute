using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} * {A}", Keywords = new[] { "*" }, Level = 10000, ComputeType = typeof(MultiplyComputeSymbol))]
    public class MultiplyComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double arg2 = ArgumentsDouble(1, expressSchema, objOrHash);
            double value = arg1*arg2;
            return value;
        }
    }
}
