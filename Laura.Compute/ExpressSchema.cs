using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Laura.Compute.Utils;

namespace Laura.Compute
{
    /// <summary>
    /// 表达式结构 对象：任何一个需要计算的 表达式 都会 被转换成 这个对象
    /// </summary>
    [Serializable]
    public class ExpressSchema
    {
        private ExpressSchema() { }
        //Max(12,23,34,"","")
        private string sourceExpress = string.Empty;
        private readonly Dictionary<string, ExpressSlice> arguments = new Dictionary<string, ExpressSlice>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 表达式 的 原始 字符串
        /// </summary>
        public string SourceExpress
        {
            get { return sourceExpress; }
            internal set { sourceExpress = (value ?? string.Empty).Trim(); }
        }

        /// <summary>
        /// 当前表达式的 主片段
        /// </summary>
        public ExpressSlice MainSlice { get; internal set; }

        /// <summary>
        /// 该表达式的 所有参数 列表
        /// </summary>
        public Dictionary<string, ExpressSlice> Arguments
        {
            get { return arguments; }
            internal set
            {
                if (arguments != value)
                {
                    arguments.Clear();
                    if (value != null && value.Count > 0)
                        foreach (string key in value.Keys)
                            arguments.Add(key, value[key]);
                }
            }
        }

        /// <summary>
        /// 计算表达式的结果
        /// </summary>
        public object Compute(object objOrHash)
        {
            ExpressSlice mainSlice = MainSlice;
            if (mainSlice != null)
            {
                object result = mainSlice.Computer == null
                                    ? mainSlice.MetaValue
                                    : mainSlice.Computer.Compute(this, objOrHash);
                return result;
            }

            return null;
        }


        #region  静 态 函 数

        #region  正 则 表 达 式

        internal static readonly Regex regexString = new Regex(@"(""((\\"")|[^\""])*"")|('((\\')|[^\'])*')", RegexOptions.IgnoreCase);
        internal static readonly Regex regexDouble = new Regex(@"(?<=[^\w]|(^))(-){0,1}\d+(\.){0,1}\d*(((E\+)|(E\-))\d+){0,1}", RegexOptions.IgnoreCase);
        internal static readonly Regex regexBoolean = new Regex(@"(False)|(True)", RegexOptions.IgnoreCase);
        internal static readonly Regex regexSingleArgument = new Regex(@"(?<=(^)|[^\.\w$\]])" + CurrentObjectKeyword + @"(?=[^\.\w$\[])", RegexOptions.IgnoreCase);                                                                                                                                                      //{Current} 是一个元数据 没有属性
        internal static readonly Regex regexIndexArgument = new Regex(@"(?<=(^)|[^\.\w$\]])" + CurrentObjectKeyword + @"\[" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\]((.\[($|\w)+\])|(\[" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\]))*", RegexOptions.IgnoreCase);               //{Current} 是一个数组或Hash 有索引器
        internal static readonly Regex regexFullArgument = new Regex(@"(?<=(^)|[^\.\w$\]])" + CurrentObjectKeyword + @"((.\[($|\w)+\])|(\[" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\]))*", RegexOptions.IgnoreCase);                                                                                 //{Current} 是一个普通对象 表达式为全表达式：{Current}后面直接是 属性
        internal static readonly Regex regexSimpleArgument = 
            new Regex(@"(?<=(^)|[^\.\w$\]])\[($|\w)+\]((\[($|\w)+\])|(\[" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\]))*", RegexOptions.IgnoreCase);                                                                                                 //{Current} 是一个普通对象 表达式为省略表达式：表达式中 没有 {Current}——直接 使用属性(最人性化 的 表达式)
            //new Regex(@"(?<=(^)|[^\.\w$\]])\[($|\w)+\]((.\[($|\w)+\])|(\[" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\]))*", RegexOptions.IgnoreCase);                                                                                                 //{Current} 是一个普通对象 表达式为省略表达式：表达式中 没有 {Current}——直接 使用属性(最人性化 的 表达式)
        internal static readonly Regex regexArray = new Regex(@"\(\s*" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\s*,(\s*" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\s*(,|(?=\)))\s*)+\)", RegexOptions.IgnoreCase);
        internal static readonly Regex regexReplaceBracket = new Regex(@"\(\s*" + ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd + @"\s*\)", RegexOptions.IgnoreCase);
        internal static readonly Regex regexExpressBracket = new Regex(@"\([^\(\)]+\)", RegexOptions.IgnoreCase);
        internal static readonly Regex regexArgument = new Regex(ExpressSchemaKeyStart + @"\d+" + ExpressSchemaKeyEnd, RegexOptions.IgnoreCase);
        internal static readonly Regex regexArgumentProperty = new Regex(@"(?<=\[)[^\]]+(?=\])");
        internal static readonly Regex regexPi = new Regex(@"π");

        #endregion

        #region  常 量 值

        internal const string ExpressSchemaKeyStart = "{Express_";
        internal const string ExpressSchemaKeyEnd = "}";
        internal const string CurrentObjectKeyword = "this";     //"{Current}";  //改为使用 this 关键字

        #endregion


        /// <summary>
        /// 通过表达式 初始化一个 ExpressSchema 对象
        /// </summary>
        public static ExpressSchema Create(string express)
        {
            if (StringExtend.IsNullOrWhiteSpace(express)) return null;

            //提前获取 可能 需要 的计算器
            IEnumerable<ComputeExpressAttribute> listComputeAttribute = FindComputeAttributes(express);

            ExpressSchema expressSchema = new ExpressSchema();
            IgnoreDictionary<ExpressSlice> listExpressSchema = new IgnoreDictionary<ExpressSlice>();

            //匹配 元数据
            string noPiExpress = MatchExpressPi(express, listExpressSchema, listComputeAttribute);
            string noStringExpress = MatchExpressString(noPiExpress, listExpressSchema, listComputeAttribute);
            string noDoubleExpress = MatchExpressDouble(noStringExpress, listExpressSchema, listComputeAttribute);
            string noBooleanExpress = MatchExpressBoolean(noDoubleExpress, listExpressSchema, listComputeAttribute);
            string noSingleArgumentExpress = MatchExpressSingleArgument(noBooleanExpress, listExpressSchema, listComputeAttribute);
            string noIndexArgumentExpress = MatchExpressIndexArgument(noSingleArgumentExpress, listExpressSchema, listComputeAttribute);
            string noFullArgumentExpress = MatchExpressFullArgument(noIndexArgumentExpress, listExpressSchema, listComputeAttribute);
            string noSimpleArgumentExpress = MatchExpressSimpleArgument(noFullArgumentExpress, listExpressSchema, listComputeAttribute);

            //匹配 计算器表达式
            string singleExpress = MatchExpressComputer(noSimpleArgumentExpress, listExpressSchema, listComputeAttribute);
            singleExpress = singleExpress.Trim();

            ExpressSlice mainSlice = listExpressSchema[singleExpress];
            if (mainSlice == null)
                throw new Exception(string.Format("Unknown Expression-Slice \"{0}\"", singleExpress));

            expressSchema.SourceExpress = express;
            expressSchema.MainSlice = mainSlice;
            expressSchema.Arguments = listExpressSchema;
            return expressSchema;
        }


        #region  匹 配 元 数 据

        private static string MatchExpressPi(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexPi.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.Double;
                expressSlice.Express = matchValue;
                expressSlice.MetaValue = Math.PI;
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });
            return replaceExpress;
        }
        private static string MatchExpressString(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexString.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.String;
                expressSlice.Express = StaticHelper.FormatString(matchValue);
                expressSlice.MetaValue = expressSlice.Express;
                listExpressSchema.Add(replaceKey, expressSlice);

                return replaceKey;
            });
            return replaceExpress;
        }
        private static string MatchExpressDouble(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexDouble.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.Double;
                expressSlice.Express = matchValue;
                expressSlice.MetaValue = Tools.ToDouble(matchValue, 0);
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });
            return replaceExpress;
        }
        private static string MatchExpressBoolean(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexBoolean.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.Boolean;
                expressSlice.Express = matchValue;
                expressSlice.MetaValue = Tools.ToBoolean(matchValue, false);
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });
            return replaceExpress;
        }
        private static string MatchExpressSingleArgument(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexSingleArgument.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.SingleArgument;
                expressSlice.Express = matchValue;
                expressSlice.Computer = new PropertyComputer {ExpressSlice = expressSlice};
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });
            return replaceExpress;
        }
        private static string MatchExpressIndexArgument(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexIndexArgument.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.IndexArgument;
                expressSlice.Express = matchValue;
                expressSlice.Computer = new PropertyComputer { ExpressSlice = expressSlice };
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });
            return replaceExpress;
        }
        private static string MatchExpressFullArgument(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexFullArgument.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.FullArgument;
                expressSlice.Express = matchValue;
                expressSlice.Computer = new PropertyComputer { ExpressSlice = expressSlice };
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });
            return replaceExpress;
        }
        private static string MatchExpressSimpleArgument(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            string replaceExpress = regexSimpleArgument.Replace(express, match =>
            {
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.SimpleArgument;
                expressSlice.Express = matchValue;
                expressSlice.Computer = new PropertyComputer { ExpressSlice = expressSlice };
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });
            return replaceExpress;
        }

        #endregion

        #region  匹 配 运 算 符

        /// <summary>
        /// 通过表达式 先过滤出 可能使用的 计算器
        /// </summary>
        private static IEnumerable<ComputeExpressAttribute> FindComputeAttributes(string express)
        {
            if (StringExtend.IsNullOrWhiteSpace(express)) return null;

            List<ComputeExpressAttribute> listReultComputeAttribute = new List<ComputeExpressAttribute>();

            List<ComputeExpressAttribute> listComputeAttribute = StaticHelper.ListComputeAttribute;
            if (listComputeAttribute != null)
            {
                foreach (ComputeExpressAttribute attribute in listComputeAttribute)
                {
                    try
                    {
                        if (attribute != null && attribute.Keywords != null && attribute.Keywords.Length > 0)
                        {
                            bool match = true;
                            foreach (string keyword in attribute.Keywords)
                            {
                                int index = express.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase);
                                if (index < 0) match = false;
                            }
                            if (match)
                                listReultComputeAttribute.Add(attribute);
                        }
                    }
                    catch(Exception) { }
                }
            }

            listReultComputeAttribute.Sort((x, y) => y.Level - x.Level);
            return listReultComputeAttribute;
        }

        private static string MatchExpressComputer(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute)
        {
            //先匹配 数组
            bool matchExpressBracket;
            string replaceExpress = MatchArrayOrExpressBracket(express, listExpressSchema, listComputeAttribute, out matchExpressBracket);
            
            bool matchAnyComputer = false;
            if (listComputeAttribute != null)
            {
                //先判定运算的优先级
                Match priorMatch = null;
                ComputeExpressAttribute priorComputeAttribute = null;                //对于运算的优先级，平级时 前后运算
                foreach (ComputeExpressAttribute computeAttribute in listComputeAttribute)
                {
                    if (priorMatch != null && /*priorComputeAttribute != null && */computeAttribute.Level < priorComputeAttribute.Level) continue;
                    Match match = computeAttribute.Regex.Match(replaceExpress);
                    if (match.Success)
                    {
                        if ((priorMatch == null/* && priorComputeAttribute == null*/)                                           //如果还没有确定优先级，则当前优先
                            || (computeAttribute.Level > priorComputeAttribute.Level)                                           //如果等级高，则优先
                            || (computeAttribute.Level == priorComputeAttribute.Level && match.Index < priorMatch.Index))       //如果等级一致，则前面的优先
                        {
                            priorMatch = match;
                            priorComputeAttribute = computeAttribute;
                        }
                    }
                }

                //再来对判定优先的 匹配式 进行分析
                if (priorComputeAttribute != null && /*priorMatch!=null && */priorMatch.Success)
                {
                    matchAnyComputer = true;
                    Type computerType = priorComputeAttribute.ComputeType;
                    string matchValue = priorMatch.Value;
                    string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;


                    //初始化 计算器 对象
                    ICompute computer = Activator.CreateInstance(computerType) as ICompute;
                    if (computer == null) throw new Exception(string.Format("Compute-Type \"{0}\" Is Not ICompute", Tools.ToTypeName(computerType)));

                    //捕获 计算器 参数
                    List<ExpressSlice> listArgument = MatchArguments(matchValue, listExpressSchema);
                    computer.Arguments = listArgument.ToArray();

                    //合并为 当前 表达式片段对象
                    ExpressSlice expressSlice = new ExpressSlice();
                    expressSlice.ExpressType = ExpressType.Computer;
                    expressSlice.Express = matchValue;
                    computer.ExpressSlice = expressSlice;
                    expressSlice.Computer = computer;

                    listExpressSchema.Add(replaceKey, expressSlice);

                    //置换 匹配到的 表达式
                    replaceExpress = replaceExpress.Substring(0, priorMatch.Index) + (" " + replaceKey + " ") + replaceExpress.Substring(priorMatch.Index + priorMatch.Length);
                }
            }

            //如果匹配到 数组 或者 计算器 则 再次递归 【警告：数组是否匹配到也是 递归条件】
            if (matchExpressBracket || matchAnyComputer)
                replaceExpress = MatchExpressComputer(replaceExpress, listExpressSchema, listComputeAttribute);

            return replaceExpress;
        }
        private static string MatchExpressArray(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute, out bool matchAnyone)
        {
            bool matchAnyoneTemp = false;
            string replaceExpress = regexArray.Replace(express, match =>
            {
                matchAnyoneTemp = true;
                string matchValue = match.Value;
                string replaceKey = ExpressSchemaKeyStart + listExpressSchema.Count.ToString() + ExpressSchemaKeyEnd;

                ExpressSlice expressSlice = new ExpressSlice();
                expressSlice.ExpressType = ExpressType.ArrayList;
                expressSlice.Express = StaticHelper.FormatArray(matchValue);

                //捕获 数组 参数
                List<ExpressSlice> listArgument = MatchArguments(expressSlice.Express, listExpressSchema);
                expressSlice.Computer = new ArrayListComputer { ExpressSlice = expressSlice, Arguments = listArgument.ToArray() };
                listExpressSchema.Add(replaceKey, expressSlice);

                return " " + replaceKey + " ";
            });

            matchAnyone = matchAnyoneTemp;
            return replaceExpress;
        }
        private static string ReplaceSingleBracket(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute, out bool matchAnyone)
        {
            bool matchAnyoneTemp = false;
            string replaceExpress = regexReplaceBracket.Replace(express, match =>
            {
                matchAnyoneTemp = true;
                string matchValue = match.Value;
                string replaceString = StaticHelper.FormatBracket(matchValue);
                return replaceString;
            });

            matchAnyone = matchAnyoneTemp;
            return replaceExpress;
        }
        private static string MatchArrayOrExpressBracket(string express, IgnoreDictionary<ExpressSlice> listExpressSchema, IEnumerable<ComputeExpressAttribute> listComputeAttribute, out bool matchAnyone)
        {
            bool matchAnySingleBracket;
            bool matchAnyArray;
            bool matchAnyoneTemp = false;

            string replaceExpress = ReplaceSingleBracket(express, listExpressSchema, listComputeAttribute, out matchAnySingleBracket);

            replaceExpress = MatchExpressArray(replaceExpress, listExpressSchema, listComputeAttribute, out matchAnyArray);
            if (!matchAnyArray)
            {                
                replaceExpress = regexExpressBracket.Replace(replaceExpress, match =>
                {
                    matchAnyoneTemp = true;
                    string matchValue = match.Value;
                    string replaceString = StaticHelper.FormatBracket(matchValue);
                    if (StringExtend.IsNullOrWhiteSpace(replaceString)) return "()";  //如果 是 纯 括号，则不匹配

                    //匹配 计算器表达式
                    string singleExpress = MatchExpressComputer(replaceString, listExpressSchema, listComputeAttribute);
                    singleExpress = singleExpress.Trim();
                    return singleExpress;
                });
            }
            matchAnyone = matchAnySingleBracket || matchAnyoneTemp || matchAnyArray;
            return replaceExpress;
        }

        
        ////将一个参数的数组 拆为 当前数组
        //private static List<ExpressSlice> FormatArrayArgument(IgnoreDictionary<ExpressSlice> listExpressSchema, List<ExpressSlice> listArgument)
        //{
        //    if (listArgument == null) return null;
        //    if (listArgument.Count == 1)    //数组数目为一 则继续分析 这个数组 内部结构
        //    {
        //        ExpressSlice itemExpressSlice = listArgument[0];
        //        if (itemExpressSlice.ExpressType == ExpressType.ArrayList)
        //        {
        //            listArgument = new List<ExpressSlice>(itemExpressSlice.Computer.Arguments);
        //            listArgument = FormatArrayArgument(listExpressSchema, listArgument);
        //        }
        //    }

        //    return listArgument;
        //}
        private static List<ExpressSlice> MatchArguments(string express, IgnoreDictionary<ExpressSlice> listExpressSchema)
        {
            List<ExpressSlice> listArgument = new List<ExpressSlice>();
            MatchCollection matchArguments = regexArgument.Matches(express);
            if ( /*matchArguments!=null && */matchArguments.Count > 0)
                foreach (Match matchArgument in matchArguments)
                {
                    if (matchArgument != null && matchArgument.Success)
                    {
                        string argumentExpress = matchArgument.Value.Trim();
                        ExpressSlice argumentSlice = listExpressSchema[argumentExpress];
                        listArgument.Add(argumentSlice);
                    }
                }
            //listArgument = FormatArrayArgument(listExpressSchema, listArgument);
            return listArgument;
        }

        #endregion

        #endregion

    }
}
