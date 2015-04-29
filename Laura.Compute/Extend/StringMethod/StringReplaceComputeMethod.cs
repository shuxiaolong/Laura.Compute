using System;

namespace Laura.Compute.Extend.StringMethod
{
    [Serializable]
    [ComputeExpress(Express = "REPLACE {A}", Keywords = new[] { "REPLACE" }, Level = 1000000, ComputeType = typeof(StringReplaceComputeMethod))]
    public class StringReplaceComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash);
            string arg2 = ArgumentsString(1, expressSchema, objOrHash);
            string arg3 = ArgumentsString(2, expressSchema, objOrHash);
            string value = arg1.Replace(arg2, arg3);
            return value;
        }

        
    }
}
