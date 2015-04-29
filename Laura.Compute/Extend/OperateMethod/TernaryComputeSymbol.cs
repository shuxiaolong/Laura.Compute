using System;

namespace Laura.Compute.Extend.OperateMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} ? {A} : {A}", Keywords = new[] { "?", ":" }, Level = 100, ComputeType = typeof(TernaryComputeSymbol))]
    public class TernaryComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            bool arg1 = ArgumentsBoolean(0, expressSchema, objOrHash);
            //if (Arguments[1].Type != Arguments[2].Type)
            //    throw new Exception("TernaryComputeSymbol Type Is Not Same!");

            string value = arg1 ? ArgumentsString(1, expressSchema, objOrHash) : ArgumentsString(2, expressSchema, objOrHash);
            return value;  //Arguments[1].ExpressType
        }
    }
}
