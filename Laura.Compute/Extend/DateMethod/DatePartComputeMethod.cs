using System;

namespace Laura.Compute.Extend.DateMethod
{
    [Serializable]
    [ComputeExpress(Express = "DATEPART {A}", Keywords = new[] { "DATEPART" }, Level = 1000000, ComputeType = typeof(DatePartComputeMethod))]
    public class DatePartComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash).Trim().ToUpper();
            DateTime arg2 = ArgumentsDate(1, expressSchema, objOrHash);

            double value = 0;
            //year | quarter | month | week | day | hour | minute | second | millisecond
            switch (arg1)
            {
                case "YY":
                case "YYYY":
                case "YEAR":
                    value = arg2.Year;
                    break;
                case "Y":
                case "YEARDAY":
                    value = arg2.DayOfYear;
                    break;
                case "M":
                case "MM":
                case "MONTH":
                    value = arg2.Month;
                    break;
                case "D":
                case "DD":
                case "DAY":
                case "DY":
                    value = arg2.Day;
                     break;
                case "W":
                case "WEEK":
                     DayOfWeek week = arg2.DayOfWeek;
                     switch(week)
                     {
                         case DayOfWeek.Sunday:value=1; break;
                         case DayOfWeek.Monday: value = 2; break;
                         case DayOfWeek.Tuesday: value = 3; break;
                         case DayOfWeek.Wednesday: value = 4; break;
                         case DayOfWeek.Thursday: value = 5; break;
                         case DayOfWeek.Friday: value = 6; break;
                         case DayOfWeek.Saturday: value = 7; break;
                     }
                     break;
                case "HH":
                case "HOUR":
                     value = arg2.Hour;
                    break;
                case "N":
                case "MI":
                case "MINUTE":
                    value = arg2.Minute;
                    break;
                case "S":
                case "SS":
                case "SECOND":
                    value = arg2.Second;
                    break;
                case "MS":
                case "MILLISECOND":
                    value = arg2.Millisecond;
                    break;
            }

            return value;
        }
    }
}
