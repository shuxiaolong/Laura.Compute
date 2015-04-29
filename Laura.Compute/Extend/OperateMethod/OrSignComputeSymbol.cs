using System;

namespace Laura.Compute.Extend.OperateMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} || {A}", Keywords = new[] { "||" }, Level = 520, ComputeType = typeof(OrSignComputeSymbol))]
    public class OrSignComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            bool arg1 = ArgumentsBoolean(0, expressSchema, objOrHash);
            if (arg1) return true;
            else
            {
                bool arg2 = ArgumentsBoolean(1, expressSchema, objOrHash);
                bool value = arg2;
                return value;
            }
        }
    }
}
