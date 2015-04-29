using System;
using System.Text.RegularExpressions;

namespace Laura.Compute.Extend.StringMethod
{
    [Serializable]
    [ComputeExpress(Express = "{A} LIKE {A}", Keywords = new[] { "LIKE" }, Level = 700, ComputeType = typeof(LikeEqualComputeSymbol))]
    public class LikeEqualComputeSymbol : ComputeBase
    {
        public override object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            string arg1 = ArgumentsString(0, expressSchema, objOrHash);
            string arg2 = ArgumentsString(1, expressSchema, objOrHash);
            string regexStr = arg2.Replace("%", "([\\s\\S]*)");

            //Regex regexLike = new Regex(regexStr);
            //Match resultMatch = regexLike.Match(arg1);
            //bool matchResult = resultMatch.Success;
            //if (matchResult)
            //    matchResult = arg1.Equals(resultMatch.Value, StringComparison.CurrentCultureIgnoreCase);
            //bool value = matchResult;

            bool value = Regex.IsMatch(arg1, regexStr, RegexOptions.IgnoreCase);
            return value;
        }
    }
}
