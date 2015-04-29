using System;

namespace Laura.Compute.Extend.StringMethod
{
    [Serializable]
    [ComputeExpress(Express = "LEN {A}", Keywords = new[] { "LEN" }, Level = 1000000, ComputeType = typeof(StringLengthComputeMethod))]
    public class StringLengthComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash);
            double value = arg1.Length;
            return value;
        }
    }
}
