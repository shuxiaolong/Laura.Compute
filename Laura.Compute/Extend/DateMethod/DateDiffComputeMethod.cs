using System;

namespace Laura.Compute.Extend.DateMethod
{
    [Serializable]
    [ComputeExpress(Express = "DATEDIFF {A}", Keywords = new[] { "DATEDIFF" }, Level = 1000000, ComputeType = typeof(DateDiffComputeMethod))]
    public class DateDiffComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash).Trim().ToUpper();
            DateTime arg2 = ArgumentsDate(1, expressSchema, objOrHash);
            DateTime arg3 = ArgumentsDate(2, expressSchema, objOrHash);
            TimeSpan timeSpan = arg3 - arg2;

            double value = 0;
            //year | quarter | month | week | day | hour | minute | second | millisecond
            switch (arg1)
            {
                case "YY":
                case "YYYY":
                case "YEAR":
                    value = arg3.Year - arg2.Year;
                    break;
                case "M":
                case "MM":
                case "MONTH":
                    value = arg3.Month - arg2.Month;
                    break;
                case "D":
                case "DD":
                case "DAY":
                case "DY":
                    value = timeSpan.TotalDays;
                     break;
                case "HH":
                case "HOUR":
                    value = timeSpan.TotalHours;
                    break;
                case "N":
                case "MI":
                case "MINUTE":
                    value = timeSpan.TotalMinutes;
                    break;
                case "S":
                case "SS":
                case "SECOND":
                    value = timeSpan.TotalSeconds;
                    break;
                case "MS":
                case "MILLISECOND":
                    value = timeSpan.TotalMilliseconds;
                    break;
            }

            return value;
        }
    }
}
