using System;
using Laura.Compute.Utils;

namespace Laura.Compute.Extend.MathMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} + {A}", Keywords = new[] { "+" }, Level = 1000, ComputeType = typeof(PlusComputeSymbol))]
    public class PlusComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            object argObj1 = ArgumentsObject(0, expressSchema, objOrHash);
            object argObj2 = ArgumentsObject(1, expressSchema, objOrHash);

            if (ArgumentsType(0) == ExpressType.String || ArgumentsType(1) == ExpressType.String || argObj1 is string || argObj2 is string)
            {
                string arg1 = Tools.ToString(argObj1);
                string arg2 = Tools.ToString(argObj2);
                string value = arg1 + arg2;
                return value;
            }
            else
            {
                double arg1 = Tools.ToDouble(argObj1);
                double arg2 = Tools.ToDouble(argObj2);
                double value = arg1 + arg2;
                return value;
            }
        }
    }
}
