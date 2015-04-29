using System;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} √ {A}", Keywords = new[] { "√" }, Level = 100000, ComputeType = typeof(SqrtComputeSymbol))]
    public class SqrtComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double arg2 = ArgumentsDouble(1, expressSchema, objOrHash);
            double value = Math.Pow(arg2, 1/arg1);
            return value;
        }
    }
}
