using System;
using System.Text.RegularExpressions;

namespace Laura.Compute
{
    public class ComputeExpressAttribute : Attribute
    {
        private string symbol = string.Empty;
        private Regex regex;

        public string Express
        {
            get { return symbol; }
            set
            {
                symbol = value ?? string.Empty;
                if (!string.IsNullOrEmpty(symbol))
                {
                    string symbolTemp = StaticHelper.FormatRegex(symbol);
                    string regexExpres = symbolTemp.Replace("{A}", ExpressSchema.ExpressSchemaKeyStart + @"[\d]+" + ExpressSchema.ExpressSchemaKeyEnd);
                    regex = new Regex(regexExpres, RegexOptions.IgnoreCase);
                }
            }
        }
        public int Level { get; set; }
        public Regex Regex
        {
            get { return regex; }
        }
        public Type ComputeType { get; set; }

        /// <summary>
        /// 表达式 中 的 关键字
        /// </summary>
        public string[] Keywords { get; set; }

    }
}
