using System;

namespace Laura.Compute.Extend.OperateMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} == {A}", Keywords = new[] { "==" }, Level = 605, ComputeType = typeof(EqualComputeSymbol))]
    public class EqualComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash);
            string arg2 = ArgumentsString(1, expressSchema, objOrHash);
            bool value = arg1 == arg2;
            return value;
        }
    }
}
