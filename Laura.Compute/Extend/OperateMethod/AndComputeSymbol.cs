using System;

namespace Laura.Compute.Extend.OperateMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} AND {A}", Keywords = new[] { "AND" }, Level = 525, ComputeType = typeof(AndComputeSymbol))]
    public class AndComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {               
            bool arg1 = ArgumentsBoolean(0, expressSchema, objOrHash);
            if (!arg1) return false;
            else
            {
                bool arg2 = ArgumentsBoolean(1, expressSchema, objOrHash);
                bool value = arg2;
                return value;
            }
        }
    }
}
