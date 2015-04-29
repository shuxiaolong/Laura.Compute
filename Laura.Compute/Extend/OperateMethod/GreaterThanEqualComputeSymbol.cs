using System;

namespace Laura.Compute.Extend.OperateMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} >= {A}", Keywords = new[] { ">=" }, Level = 680, ComputeType = typeof(GreaterThanEqualComputeSymbol))]
    public class GreaterThanEqualComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            double arg1 = ArgumentsDouble(0, expressSchema, objOrHash);
            double arg2 = ArgumentsDouble(1, expressSchema, objOrHash);
            bool value = arg1 >= arg2;
            return value;
        }
    }
}
