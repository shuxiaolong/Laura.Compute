using System;
using System.Collections.Generic;
using System.Text;

namespace Laura.Compute.Test
{
    public class TestBaseExpress
    {
        public static void StartTest()
        {
            ExpressSchema expressSchema = ExpressSchema.Create("\"AABBCC\" LIKE \"%BB%\" AND 300>100 AND -0.000021323E+12 OR 34.543 AND True OR false AND 5697.000021323E+12 OR -45678.424123 AND [FName] IN (\"ShuXiaolong\",\"JiangXiaoya\")");
            Console.WriteLine(expressSchema);

            ExpressSchema inExpress = ExpressSchema.Create("23 IN (12,23,34)");
            object inValue = inExpress.Compute(null);
            Console.WriteLine(inValue);

            ExpressSchema maxExpress = ExpressSchema.Create("2 + Max (12,23,34)");  //,Max(2,4,100)
            object maxValue = maxExpress.Compute(null);
            Console.WriteLine(maxValue);

            ExpressSchema maxStringExpress = ExpressSchema.Create("Max (\"AAA\",\"BBB\",\"CCC\")");
            object maxStringValue = maxStringExpress.Compute(null);
            Console.WriteLine(maxStringValue);

            ExpressSchema minStringExpress = ExpressSchema.Create("Min (\"AAA\",\"BBB\",\"CCC\")");
            object minStringValue = minStringExpress.Compute(null);
            Console.WriteLine(minStringValue);

            ExpressSchema maxDateExpress = ExpressSchema.Create("Max (\"1989-11-27\",\"1990-04-11\",\"2013-11-28\")");
            object maxDateValue = maxDateExpress.Compute(null);
            Console.WriteLine(maxDateValue);

            ExpressSchema minDateExpress = ExpressSchema.Create("Min (\"1989-11-27\",\"1990-04-11\",\"2013-11-28\")");
            object minDateValue = minDateExpress.Compute(null);
            Console.WriteLine(minDateValue);

        }
    }
}
