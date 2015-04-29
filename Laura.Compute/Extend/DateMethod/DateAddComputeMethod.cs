using System;

namespace Laura.Compute.Extend.DateMethod
{
    [Serializable]
    [ComputeExpress(Express = "DATEADD {A}", Keywords = new[] { "DATEADD" }, Level = 1000000, ComputeType = typeof(DateAddComputeMethod))]
    public class DateAddComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash).Trim().ToUpper();
            int arg2 = Convert.ToInt32(ArgumentsDouble(1, expressSchema, objOrHash));
            DateTime arg3 = ArgumentsDate(2, expressSchema, objOrHash);

            DateTime value = DateTime.MinValue;
            //year | quarter | month | week | day | hour | minute | second | millisecond
            switch (arg1)
            {
                case "YY":
                case "YYYY":
                case "YEAR":
                    value = arg3.AddYears(arg2);
                    break;
                //case "Y":
                //case "YEARDAY":
                //    value = arg3;
                //    break;
                case "M":
                case "MM":
                case "MONTH":
                    value = arg3.AddMonths(arg2);
                    break;
                case "D":
                case "DD":
                case "DAY":
                case "DY":
                    value = arg3.AddDays(arg2);
                     break;
                //case "W":
                //case "WEEK":
                //     value = arg3;
                //     break;
                case "HH":
                case "HOUR":
                     value = arg3.AddHours(arg2);
                    break;
                case "N":
                case "MI":
                case "MINUTE":
                    value = arg3.AddMinutes(arg2);
                    break;
                case "S":
                case "SS":
                case "SECOND":
                    value = arg3.AddSeconds(arg2);
                    break;
                case "MS":
                case "MILLISECOND":
                    value = arg3.AddMilliseconds(arg2);
                    break;
            }

            return value;
        }
    }
}
