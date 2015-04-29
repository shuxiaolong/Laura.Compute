using System;
using System.Collections;
using Laura.Compute.Utils;

namespace Laura.Compute.Extend.OtherMethod
{
    [Serializable]
    [ComputeExpress(Express = "Min {A}", Keywords = new[] { "Min" }, Level = 1000000, ComputeType = typeof(MinComputeMethod))]
    public class MinComputeMethod : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            ArrayList arrayList = ArgumentsArray(0, expressSchema, objOrHash);
            if (arrayList == null || arrayList.Count <= 0) return null;

            bool isDateTime = arrayList[0] is DateTime;
            bool isDouble = arrayList[0] is Double;
            //bool isString = arrayList[0] is String;

            if (isDateTime)
            {
                DateTime minDateTime = Tools.ToDateTime(arrayList[0]);
                for (int i = 1; i < arrayList.Count; i++)
                {
                    DateTime itemValue = Tools.ToDateTime(arrayList[i]);
                    if (itemValue < minDateTime) minDateTime = itemValue;
                }
                return minDateTime;
            }
            else if (isDouble)
            {
                double minDouble = Tools.ToDouble(arrayList[0]);
                for (int i = 1; i < arrayList.Count; i++)
                {
                    double itemValue = Tools.ToDouble(arrayList[i]);
                    if (itemValue < minDouble) minDouble = itemValue;
                }
                return minDouble;
            }
            else
            {
                string minString = Tools.ToString(arrayList[0]);
                for (int i = 1; i < arrayList.Count; i++)
                {
                    string itemValue = Tools.ToString(arrayList[i]);
                    if (String.Compare(itemValue, minString, StringComparison.CurrentCulture) <= 0)
                        minString = itemValue;
                }
                return minString;
            }
        }


    }
}
