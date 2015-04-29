using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using Laura.Compute.Utils;

namespace Laura.Compute
{
    internal static class StaticHelper
    {
        /// <summary>
        /// 格式化一个正则表达式
        /// </summary>
        internal static string FormatRegex(string str)
        {
            return str.Replace("+", "\\+").Replace("*", "\\*").Replace("?", "\\?").Replace("(", "\\(").Replace(")", "\\)").Replace("[", "\\[").Replace("]", "\\]").Replace("^", "\\^").Replace("|", "\\|").Replace("&", "\\&").Replace("$", "\\$").Replace(" ", @"\s*").Replace(@"\(\)", @"\s*\(\s*\)\s*");
        }

        internal static string FormatString(string input)
        {
            input = (input ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(input)) return string.Empty;

            if ((input.StartsWith("\"") && input.EndsWith("\""))
                || (input.StartsWith("\'") && input.EndsWith("\'")))
                input = input.Substring(1, input.Length - 2);

            return input;
        }

        internal static string FormatArray(string input)
        {
            input = (input ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(input)) return string.Empty;

            if ((input.StartsWith("(") && input.EndsWith(")"))
                || (input.StartsWith("[") && input.EndsWith("]")))
                input = input.Substring(1, input.Length - 2);

            return input;
        }

        internal static string FormatBracket(string input)
        {
            input = (input ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(input)) return string.Empty;

            if (input.StartsWith("(") && input.EndsWith(")"))
                input = input.Substring(1, input.Length - 2);

            return input;
        }


        #region  所 有 计 算 器 插 件

        private static readonly object listComputerLocker = new object();
        private static readonly List<ComputeExpressAttribute> listComputeAttribute = new List<ComputeExpressAttribute>();
        internal static List<ComputeExpressAttribute> ListComputeAttribute
        {
            get { return listComputeAttribute; }
            set
            {
                if (listComputeAttribute != value)
                {
                    listComputeAttribute.Clear();
                    if (value != null)
                        listComputeAttribute.AddRange(value);
                }
            }
        }

        /// <summary>
        /// 本函数 会 扫描所有程序集 以需找插件，效率极低：速度约为 200-700毫秒，使用本函数 务必做好 缓存机制
        /// </summary>
        internal static List<ComputeExpressAttribute> InitAllComputeAttributes()
        {
            List<ComputeExpressAttribute> listCoputeAttrbute = new List<ComputeExpressAttribute>();

            Dictionary<Type, ComputeExpressAttribute> allTypeAttributes = AssemblyHelper.GetAttributes<ComputeExpressAttribute>();
            if(allTypeAttributes!=null)
            {
                foreach(KeyValuePair<Type, ComputeExpressAttribute> pair in allTypeAttributes)
                {
                    try
                    {
                        //Type type = pair.Key;
                        ComputeExpressAttribute attribute = pair.Value;
                        if (/*type != null && */attribute != null)
                        {
                            try { listCoputeAttrbute.Add(attribute); }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }
                }
            }

            return listCoputeAttrbute;
        }

        #endregion



        internal static object GetValue(ExpressSchema expressSchema, object objOrHash, string argumentExpress)
        {
            if (objOrHash == null) return null;
            if (StringExtend.IsNullOrWhiteSpace(argumentExpress)) return objOrHash;

            //去掉关键字
            argumentExpress = argumentExpress.Trim();
            if (argumentExpress.StartsWith(ExpressSchema.CurrentObjectKeyword, StringComparison.CurrentCultureIgnoreCase))
                argumentExpress = argumentExpress.Substring(ExpressSchema.CurrentObjectKeyword.Length).Trim().Trim('.');

            if (StringExtend.IsNullOrWhiteSpace(argumentExpress)) return objOrHash;

            object propertyValue = objOrHash;

            //拆分属性
            MatchCollection matches = ExpressSchema.regexArgumentProperty.Matches(argumentExpress);
            if (/*matches!=null*/matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (propertyValue != null)
                    {
                        string propertyName = match.Value.Trim();
                        bool isArgument = ExpressSchema.regexArgument.IsMatch(propertyName);
                        if (isArgument)
                        {
                            ExpressSlice argumentSlice = expressSchema.Arguments[propertyName];
                            propertyName = argumentSlice.Compute(expressSchema, objOrHash).ToString();
                        }

                        propertyValue = ComputeHelper.ComputePropertyValue(propertyValue, propertyName);
                    }
                }
            }

            return propertyValue;
        }


        static StaticHelper()
        {
            lock(listComputerLocker)
            {
                if (ListComputeAttribute.Count <= 0)
                {
                    ListComputeAttribute = InitAllComputeAttributes();
                }
            }
        }


    }
}
