using System;

namespace Laura.Compute.Extend.DataMethod
{
    [Serializable]
    [ComputeExpress(Express = "NEWID()", Keywords = new[] { "NEWID" }, Level = 1000000, ComputeType = typeof(GuidNewComputeMethod))]
    public class GuidNewComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            Guid value = Guid.NewGuid();
            return value;
        }
    }
}
