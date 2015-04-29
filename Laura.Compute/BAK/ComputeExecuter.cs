using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Laura.Compute.Utils;

namespace Laura.Compute
{
    /// <summary>
    /// 集合内存计算的执行者：该对象为即释放对象：请使用尽量使用 using 语法 或 显示调用 Dispose() 函数；
    /// </summary>
    [Serializable]
    public class ComputeExecuter : IDisposable
    {
        public ComputeExecuter(string expres, bool withObjParam, Type objType, string propertyFormat, bool resultBoolean, bool defultBoolean, bool enableCache)
        {
            EnableCache = enableCache;
            Expres = expres;
            WithObjParam = withObjParam;
            //ObjType = objType;
            PropertyNames = GetPropertyNames(objType);
            PropertyFormat = propertyFormat;
            FieldFormat = StringExtend.IsNullOrWhiteSpace(propertyFormat) ? null : new FieldFormat(PropertyFormat);
            ResultBoolean = resultBoolean;
            DefaultBoolean = defultBoolean;
            SchemaExpres = AnalyseSchema();

            MethodAttributes = AnalyseMethod(SchemaExpres, listBracket);
            string methodRegexExpres = ComputeMethodManage.GetMethodsRegex(MethodAttributes);
            if (!StringExtend.IsNullOrWhiteSpace(methodRegexExpres))
                MethodRegex = new Regex(methodRegexExpres);
            WillComputeMethod = MethodAttributes.Count >= 1;
        }
        public ComputeExecuter(string expres, bool withObjParam, Type objType, FieldFormat fieldFormat, bool resultBoolean, bool defultBoolean, bool enableCache)
        {
            //if (fieldFormat == null || !fieldFormat.IsValid)
            //    throw new Exception("ComputeExecuter Must Create By Valid FieldFormat!");

            EnableCache = enableCache;
            Expres = expres;
            WithObjParam = withObjParam;
            //ObjType = objType;
            PropertyNames = GetPropertyNames(objType);
            if (fieldFormat != null && fieldFormat.IsValid)
            {
                PropertyFormat = fieldFormat.FormatExpres;
                FieldFormat = fieldFormat;
            }
            else
            {
                PropertyFormat = string.Empty;
                FieldFormat = null;
            }
            ResultBoolean = resultBoolean;
            DefaultBoolean = defultBoolean;
            SchemaExpres = AnalyseSchema();

            MethodAttributes = AnalyseMethod(SchemaExpres, listBracket);
            string methodRegexExpres = ComputeMethodManage.GetMethodsRegex(MethodAttributes);
            if (!StringExtend.IsNullOrWhiteSpace(methodRegexExpres))
                MethodRegex = new Regex(methodRegexExpres);
            WillComputeMethod = MethodAttributes.Count >= 1;
        }
        public ComputeExecuter(string expres, bool withObjParam, IList<string> propertyNames, string propertyFormat, bool resultBoolean, bool defultBoolean, bool enableCache)
        {
            EnableCache = enableCache;
            Expres = expres;
            WithObjParam = withObjParam;
            PropertyNames = propertyNames;
            PropertyFormat = propertyFormat;
            FieldFormat = StringExtend.IsNullOrWhiteSpace(propertyFormat) ? null : new FieldFormat(PropertyFormat);
            ResultBoolean = resultBoolean;
            DefaultBoolean = defultBoolean;
            SchemaExpres = AnalyseSchema();

            MethodAttributes = AnalyseMethod(SchemaExpres, listBracket);
            string methodRegexExpres = ComputeMethodManage.GetMethodsRegex(MethodAttributes);
            if (!StringExtend.IsNullOrWhiteSpace(methodRegexExpres))
                MethodRegex = new Regex(methodRegexExpres);
            WillComputeMethod = MethodAttributes.Count >= 1;
        }
        public ComputeExecuter(string expres, bool withObjParam, IList<string> propertyNames, FieldFormat fieldFormat, bool resultBoolean, bool defultBoolean, bool enableCache)
        {
            //if (fieldFormat == null || !fieldFormat.IsValid)
            //    throw new Exception("ComputeExecuter Must Create By Valid FieldFormat!");

            EnableCache = enableCache;
            Expres = expres;
            WithObjParam = withObjParam;
            PropertyNames = propertyNames;
            if (fieldFormat != null && fieldFormat.IsValid)
            {
                PropertyFormat = fieldFormat.FormatExpres;
                FieldFormat = fieldFormat;
            }
            else
            {
                PropertyFormat = string.Empty;
                FieldFormat = null;
            }
            ResultBoolean = resultBoolean;
            DefaultBoolean = defultBoolean;
            SchemaExpres = AnalyseSchema();

            MethodAttributes = AnalyseMethod(SchemaExpres, listBracket);
            string methodRegexExpres = ComputeMethodManage.GetMethodsRegex(MethodAttributes);
            if (!StringExtend.IsNullOrWhiteSpace(methodRegexExpres))
                MethodRegex = new Regex(methodRegexExpres);
            WillComputeMethod = MethodAttributes.Count >= 1;
        }

        public ComputeValue Compute()
        {
            try
            {
                ComputeValue computeValue = null;
                if (EnableCache && cacheComputeValue != null)
                    computeValue = new ComputeValue(cacheComputeValue.ArgTemp, cacheComputeValue.Value, cacheComputeValue.Type);

                if (computeValue == null)
                {
                    //任何一种起步都可以
                    computeValue = ComputeMethod(SchemaExpres, listBracket, listArg); //起步就开始计算 函数表达式
                    //computeValue = ComputeBracket(expresBracketTemp, listBracket, listArg);             //起步就开始计算 括号表达式
                    if (EnableCache && computeValue != null)
                        cacheComputeValue = new ComputeValue(computeValue.ArgTemp, computeValue.Value, computeValue.Type);
                }

                return computeValue;
            }
            catch(Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }
        public ComputeValue Compute(IgnoreHashtable parameters)
        {
            try
            {
                InitParameterComputeValues(parameters); //初始化 需要 对象才能完善的 ComputeValue

                //任何一种起步都可以
                ComputeValue computeValue = ComputeMethod(SchemaExpres, listBracket, listArg);
                //ComputeValue computeValue = ComputeBracket(expresBracketTemp, listBracket, listArg);             //起步就开始计算 括号表达式
                return computeValue;
            }
            catch (Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }
        public ComputeValue Compute(object obj)
        {
            if (!WithObjParam) return Compute();                            //当前表达式不允许计算 对象，直接转为 普通计算
            try
            {
                ComputeValue computeValue = null;
                if (EnableCache)
                {
                    ComputeValue cacheValue = GetCacheValues(obj);
                    if (cacheValue != null)
                        computeValue = new ComputeValue(cacheValue.ArgTemp, cacheValue.Value, cacheValue.Type);
                }

                if (computeValue == null)
                {
                    InitPropertyComputeValues(obj); //初始化 需要 对象才能完善的 ComputeValue
                    //任何一种起步都可以
                    computeValue = ComputeMethod(SchemaExpres, listBracket, listArg); //起步就开始计算 函数表达式
                    //computeValue = ComputeBracket(expresBracketTemp, listBracket, listArg);             //起步就开始计算 括号表达式

                    if (EnableCache && computeValue != null)
                    {
                        ComputeValue cacheValue = new ComputeValue(computeValue.ArgTemp, computeValue.Value, computeValue.Type);
                        cacheValues.Add(obj, cacheValue);
                    }
                }

                return computeValue;
            }
            catch (Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }
        public ComputeValue Compute(object obj, IgnoreHashtable parameters)
        {
            if (!WithObjParam) return Compute();                            //当前表达式不允许计算 对象，直接转为 普通计算
            try
            {
                InitPropertyComputeValues(obj); //初始化 需要 对象才能完善的 ComputeValue
                InitParameterComputeValues(parameters); //初始化 需要 对象才能完善的 ComputeValue

                //任何一种起步都可以
                ComputeValue computeValue = ComputeMethod(SchemaExpres, listBracket, listArg);
                //ComputeValue computeValue = ComputeBracket(expresBracketTemp, listBracket, listArg);             //起步就开始计算 括号表达式

                return computeValue;
            }
            catch (Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }

        public bool ComputeBoolean()
        {
            try
            {
                bool? computeValue = null;
                if (EnableCache && cacheBooleanComputeValue != null)
                    computeValue = cacheBooleanComputeValue;

                if (computeValue == null)
                {
                    computeValue = ComputeAndOr(SchemaExpres, listBracket, listArg);
                    if (EnableCache && computeValue != null)
                        cacheBooleanComputeValue = computeValue;
                }

                return computeValue == null ? DefaultBoolean : computeValue.Value;
            }
            catch (Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }
        public bool ComputeBoolean(IgnoreHashtable parameters)
        {
            try
            {
                InitParameterComputeValues(parameters); //初始化 需要 对象才能完善的 ComputeValue

                bool? computeValue = ComputeAndOr(SchemaExpres, listBracket, listArg);
                return computeValue == null ? DefaultBoolean : computeValue.Value;
            }
            catch (Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }
        public bool ComputeBoolean(object obj)
        {
            if (!WithObjParam) return ComputeBoolean();                            //当前表达式不允许计算 对象，直接转为 普通计算
            try
            {
                bool? computeValue = null;
                if (EnableCache)
                {
                    bool? cacheValue = GetCacheBooleanValues(obj);
                    if (cacheValue != null/* && cacheValue.HasValue*/)
                        computeValue = cacheValue;
                }

                if (computeValue == null)
                {
                    InitPropertyComputeValues(obj); //初始化 需要 对象才能完善的 ComputeValue
                    computeValue = ComputeAndOr(SchemaExpres, listBracket, listArg);

                    if (EnableCache && computeValue != null)
                    {
                        bool cacheValue = computeValue.Value;
                        cacheBooleanValues.Add(obj, cacheValue);
                    }
                }

                return computeValue == null ? DefaultBoolean : computeValue.Value;
            }
            catch (Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }
        public bool ComputeBoolean(object obj, IgnoreHashtable parameters)
        {
            if (!WithObjParam) return ComputeBoolean(); //当前表达式不允许计算 对象，直接转为 普通计算
            try
            {
                InitPropertyComputeValues(obj); //初始化 需要 对象才能完善的 ComputeValue
                InitParameterComputeValues(parameters); //初始化 需要 对象才能完善的 ComputeValue
                bool? computeValue = ComputeAndOr(SchemaExpres, listBracket, listArg);

                return computeValue == null ? DefaultBoolean : computeValue.Value;
            }
            catch (Exception exp)
            {
                string logMsg = string.Format("未能正确计算表达式:\r\n{0}", exp);
                Tools.ErrorLog(logMsg, "Logs/Laura.Compute/ErrorLog/");
                throw new Exception(logMsg, exp);
            }
        }

        #region  表 达 式 属 性

        private string expres = string.Empty;
        private string propertyFormat = string.Empty;
        private string schemaExpres = string.Empty;
        private FieldFormat FieldFormat { get; set; }
        private readonly List<ComputeMethodAttribute> methodAttributes = new List<ComputeMethodAttribute>();
        private readonly ComputeValueCollection listArg = new ComputeValueCollection();
        private readonly BracketExpressionCollection listBracket = new BracketExpressionCollection();

        public string Expres
        {
            get { return expres; }
            private set { expres = value ?? string.Empty; }
        }
        public bool WillComputeMethod { get; private set; }                 //解析出该表达式是否需要计算函数表达式
        public Regex MethodRegex { get; private set; }                      //如果要计算函数表达式，则需要解析哪几个函数的正则匹配式
        public List<ComputeMethodAttribute> MethodAttributes
        {
            get { return methodAttributes; }
            private set
            {
                methodAttributes.Clear();
                if (value != null && value.Count >= 1)
                    methodAttributes.AddRange(value);
            }
        }              //如果需要计算函数表达式，则需要计算那几个函数的特性信息
        public bool WithObjParam { get; private set; }                      //是否具备计算 对象的能力
        public bool ResultBoolean { get; private set; }                     //表达式是否只计算 bool 值，即：仅 ComputeBoolean 有效
        public bool DefaultBoolean { get; private set; }                    //当 Boolean 无法计算出 bool 值时，默认返回的 bool 结果：该情况理论上不存在
        public string PropertyFormat
        {
            get { return propertyFormat; }
            private set { propertyFormat = value ?? string.Empty; }
        }                                     //如果该表达式需要计算 对象，则该属性标识 表达式中 对象属性的 格式
        //public Type ObjType { get; private set; }                           //如果该表达式需要计算 对象，则必须指定对象的类型
        private string SchemaExpres
        {
            get { return schemaExpres; }
            set { schemaExpres = value ?? string.Empty; }
        }                                      //当前表达式 被解析之后 的结构表达式：这时真正进行计算的表达式；
        public bool EnableCache { get; set; }                               //对于 同一对象的计算结果进行保存
        public IList<string> PropertyNames { get; set; }

        #endregion


        #region  分 析 表 达 式 的 结 构

        private static AndOrExpres AnalyseBooleanSchema(string expression, BracketExpressionCollection brackets)
        {
            string surceSchemaExpres = expression;
            //surceSchemaExpres = surceSchemaExpres.Replace("||", "OR").Replace("&&", "AND");
            //surceSchemaExpres = regexAnd.Replace(surceSchemaExpres, "AND");           //将小写 AND 转换成大写 AND
            //surceSchemaExpres = regexOr.Replace(surceSchemaExpres, "OR");             //将小写 OR 转换成大写 OR

            string andOrExpression = surceSchemaExpres.Trim();
            if (regexOnlyBracket.IsMatch(andOrExpression))
            {
                BracketExpression bracket = brackets[andOrExpression];
                if (bracket != null)            //一般不可能为空
                {
                    andOrExpression = bracket.Expression.Trim();
                    if (!andOrExpression.StartsWith("(") || !andOrExpression.EndsWith(")"))
                        return null;            //对于括号表达式，明显不可以直接 AND OR 运算，则返回 Null：让程序进行复杂运算
                    else
                        andOrExpression = andOrExpression.TrimStart('(').TrimEnd(')');
                }
            }

            if (regexOr.IsMatch(andOrExpression))
            {
                List<string> listExpres = new List<string>();
                andOrExpression = regexOr.Replace(andOrExpression, "_Sys_OR_");
                #if (!WindowsCE && !PocketPC)
                string[] orExpreses = andOrExpression.Split(new[] { "_Sys_OR_" }, StringSplitOptions.RemoveEmptyEntries);
                #endif
                #if (WindowsCE || PocketPC)
                string[] orExpreses = StringExtend.Split(andOrExpression, "_Sys_OR_", StringSplitOptions.RemoveEmptyEntries);
                #endif
                if ( /*andExpreses != null && */orExpreses.Length >= 1)
                    listExpres.AddRange(orExpreses);

                AndOrExpres orExpres = new AndOrExpres {BooleanSchemaExpres = listExpres, ExpresType = AndOrType.Or};
                return orExpres;
            }
            else if (regexAnd.IsMatch(andOrExpression))
            {
                List<string> listExpres = new List<string>();
                andOrExpression = regexAnd.Replace(andOrExpression, "_Sys_AND_");
                #if (!WindowsCE && !PocketPC)
                string[] andExpreses = andOrExpression.Split(new[] { "_Sys_AND_" }, StringSplitOptions.RemoveEmptyEntries);
                #endif
                #if (WindowsCE || PocketPC)
                string[] andExpreses = StringExtend.Split(andOrExpression, "_Sys_AND_", StringSplitOptions.RemoveEmptyEntries);
                #endif
                if ( /*andExpreses != null && */andExpreses.Length >= 1)
                    listExpres.AddRange(andExpreses);

                AndOrExpres orExpres = new AndOrExpres { BooleanSchemaExpres = listExpres, ExpresType = AndOrType.And };
                return orExpres;
            }
            return null;
        }
        private string AnalyseSchema()
        {
            //分析当前表达式的结构
            if (!WithObjParam)
            {
                string expresParamTemp = AnalyseParameter(Expres);
                string expresArgTemp = AnalyseArgs(expresParamTemp);
                string expresBracketTemp = AnalyseBracket(expresArgTemp);
                return expresBracketTemp;
            }
            else
            {
                string expresParamTemp = AnalyseParameter(Expres);
                string expresObjectTemp = AnalyseProperty(expresParamTemp);
                string expresArgTemp = AnalyseArgs(expresObjectTemp);
                string expresBracketTemp = AnalyseBracket(expresArgTemp);
                return expresBracketTemp;
            }
        }
        private string AnalyseProperty(string tempExpres)
        {
            //分析表达式中的属性
            if (!WithObjParam) return tempExpres;

            #region  已经提前解析出了属性结构
            if (PropertyNames != null && PropertyNames.Count >= 1)
            {
                foreach (string propertyName in PropertyNames)
                {
                    List<CaptionValue<PropertyComputeValue>> captionValues = GetStandardPropertyValue(tempExpres,/* obj,*/ propertyName);
                    if (captionValues != null && captionValues.Count > 0)
                        foreach (CaptionValue<PropertyComputeValue> captionValue in captionValues)
                        {
                            string argName = "{Sys_" + listArg.Count + "}";
                            tempExpres = tempExpres.Replace(captionValue.Caption, " " + argName + " ");
                            PropertyComputeValue typeValue = captionValue.Value;
                            typeValue.ArgTemp = argName;
                            listArg.Add(typeValue);
                        }
                }
            }
            #endregion



            return tempExpres;
        }
        private string AnalyseParameter(string tempExpres)
        {
            //分析表达式中的参数
            if (StringExtend.IsNullOrWhiteSpace(tempExpres)) return tempExpres;

            tempExpres = regexParam.Replace(tempExpres, match =>
                                               {
                                                   string argName = "{Sys_" + listArg.Count + "}";
                                                   ParameterComputeValue typeValue = new ParameterComputeValue { ParamName = match.Value.Trim(), ArgTemp = argName };
                                                   typeValue.ArgTemp = argName;
                                                   listArg.Add(typeValue);
                                                   return " " + argName + " ";
                                               });

            return tempExpres;
        }
        private string AnalyseArgs(string tempExpres)
        {
            //分析表达式中的参数
            //MatchCollection matchArgs = regexArg.Matches(expression);
            //string expresArgTemp = regexArg.Replace(expression, match =>
            //                                                        {
            //                                                            string matchValue = match.Value;
            //                                                            if (matchValue.Trim().StartsWith("{Sys_", StringComparison.CurrentCultureIgnoreCase))
            //                                                                return matchValue;

            //                                                            string argName = "{Sys_" + listArg.Count + "}";
            //                                                            ComputeValue argValue = new ComputeValue(argName, match.Value);
            //                                                            listArg.Add(argValue);
            //                                                            return " " + argName + " ";
            //                                                        });

            //分析表达式中的 字符串
            string expresArgTemp = regexString.Replace(tempExpres, match =>
            {
                string argName = "{Sys_" + listArg.Count + "}";
                ComputeValue argValue = new ComputeValue(argName, match.Value, typeof(string));
                listArg.Add(argValue);
                return " " + argName + " ";
            });

            //分析表达式中的 数字
            expresArgTemp = regexDouble.Replace(expresArgTemp, match =>
            {
                string matchValue = match.Value;
                if (matchValue.Trim().StartsWith("{Sys_", StringComparison.CurrentCultureIgnoreCase))
                    return matchValue;

                string argName = "{Sys_" + listArg.Count + "}";
                ComputeValue argValue = new ComputeValue(argName, Convert.ToDouble(matchValue).ToString(), typeof(double));
                listArg.Add(argValue);
                return " " + argName + " ";
            });

            //分析表达式中的 布尔值
            expresArgTemp = regexBool.Replace(expresArgTemp, match =>
            {
                string argName = "{Sys_" + listArg.Count + "}";
                ComputeValue argValue = new ComputeValue(argName, Convert.ToBoolean(match.Value).ToString(), typeof(bool));
                listArg.Add(argValue);
                return " " + argName + " ";
            });

            return expresArgTemp;
        }
        private string AnalyseBracket(string tempExpres)
        {
            //分析表达式中的括号
            MatchCollection matchBrackets;

            string expresBracketTemp = tempExpres;
            do
            {
                matchBrackets = regexBracket.Matches(expresBracketTemp);
                expresBracketTemp = regexBracket.Replace(expresBracketTemp, match =>
                {
                    string bracketName = "{Sys_Bracket_" + listBracket.Count + "}";
                    BracketExpression bracketExpression = new BracketExpression { Expression = match.Value, BracketTemp = bracketName };
                    listBracket.Add(bracketExpression);
                    return " " + bracketName + " ";
                });
            } while (/*matchBrackets != null && */matchBrackets.Count > 0);

            return expresBracketTemp;
        }
        private static List<ComputeMethodAttribute> AnalyseMethod(string lastExpres, IEnumerable<BracketExpression> brackets)
        {
            List<ComputeMethodAttribute> listUsedMethod = new List<ComputeMethodAttribute>();

            List<ComputeMethodAttribute> listMethodAttribute = ComputeMethodManage.Methods;
            foreach(ComputeMethodAttribute methodAttribute in listMethodAttribute)
            {
                bool isMatch = methodAttribute.Regex.IsMatch(lastExpres);
                if(!isMatch)
                {
                    foreach(BracketExpression bracketExpres in brackets)
                    {
                        isMatch = methodAttribute.Regex.IsMatch(bracketExpres.Expression);
                        if (isMatch) break;
                    }
                }

                if (isMatch)
                    listUsedMethod.Add(methodAttribute);
            }
            return listUsedMethod;
        }
        private void InitPropertyComputeValues(object obj)
        {
            if (obj == null) return;

            foreach(ComputeValue computeValue in listArg)
            {
                PropertyComputeValue propertyComputeValue = computeValue as PropertyComputeValue;
                if (propertyComputeValue != null)
                    propertyComputeValue.InitValue(obj);
            }
        }
        private void InitParameterComputeValues(IgnoreHashtable parameters)
        {
            if (parameters == null || parameters.Count <= 0) return;

            foreach (ComputeValue computeValue in listArg)
            {
                ParameterComputeValue parameterComputeValue = computeValue as ParameterComputeValue;
                if (parameterComputeValue != null)
                    parameterComputeValue.InitValue(parameters);
            }
        }

        private List<CaptionValue<PropertyComputeValue>> GetStandardPropertyValue(string expression, string propertyName)
        {
            List<CaptionValue<PropertyComputeValue>> list = new List<CaptionValue<PropertyComputeValue>>();

            if (FieldFormat==null || !FieldFormat.IsValid || StringExtend.IsNullOrWhiteSpace(FieldFormat.FormatExpres))
            {
                Regex regPropertyLink = new Regex(propertyName + "(.[a-zA-Z$_]+[a-zA-Z0-9$_]*)*", RegexOptions.IgnoreCase);
                MatchCollection matches = regPropertyLink.Matches(expression);
                if (/*matches != null && */matches.Count > 0)
                    foreach (Match match in matches)
                    {
                        string propertyLink = match.Value.Trim();
                        PropertyComputeValue computeValue = new PropertyComputeValue(propertyLink);
                        CaptionValue<PropertyComputeValue> captionValue = new CaptionValue<PropertyComputeValue>(propertyLink, computeValue);
                        list.Add(captionValue);
                    }
            }
            else
            {
                if (/*FieldFormat != null && */FieldFormat.IsValid)
                {
                    string start = FieldFormat.StartExpres;
                    string end = FieldFormat.EndExpres;
                    string regStart = Tools.FormatRegex(start);
                    string regEnd = Tools.FormatRegex(end);

                    Regex regPropertyLink = new Regex(regStart + propertyName + regEnd + "(." + regStart + "[a-zA-Z$_]+[a-zA-Z0-9$_]*" + regEnd + ")*", RegexOptions.IgnoreCase);
                    MatchCollection matches = regPropertyLink.Matches(expression);
                    if (/*matches != null && */matches.Count > 0)
                        foreach (Match match in matches)
                        {
                            string matchLink = match.Value.Trim();
                            string propertyLink = matchLink.Replace("." + start, ".").Replace(end + ".", ".").Trim();
                            if (propertyLink.StartsWith(start)) propertyLink = propertyLink.Substring(start.Length);
                            if (propertyLink.EndsWith(end)) propertyLink = propertyLink.Substring(0, propertyLink.Length - end.Length);
                            PropertyComputeValue computeValue = new PropertyComputeValue(propertyLink);
                            CaptionValue<PropertyComputeValue> captionValue = new CaptionValue<PropertyComputeValue>(matchLink, computeValue);
                            list.Add(captionValue);
                        }
                }
            }

            return list;
        }



        #endregion


        #region  正 则 表 达 式

        //(\"((\\\")|[^\"])+\")|(((?<=(^|\())[\-]){0,1}((e\+)|(E\+)|(e\-)|(E\-)|[0-9\.])+)|(False)|(True)
        //MAbs{Sys_Bracket_[0-9]+}
        //\([^\(\)]+\)
        private static readonly Regex regexBracket = new Regex("\\([^\\(\\)]+\\)");
        //private static readonly Regex regexArg = new Regex("(\\\"((\\\\\\\")|[^\\\"])*\\\")|(((?<=(^|\\())[\\-]){0,1}((e\\+)|(E\\+)|(e\\-)|(E\\-)|[0-9\\.])+)|(False)|(True)|({Sys_[0-9]+})|({Sys_Bracket_[0-9]+})");
        private static readonly Regex regexString = new Regex(@"(""((\\"")|[^\""])*"")|('((\\')|[^\'])*')");
        private static readonly Regex regexDouble = new Regex("(((?<=(^|\\())[\\-]){0,1}((E\\+)|(E\\-)|[0-9\\.])+)|({Sys_[0-9]+})|({Sys_Bracket_[0-9]+})", RegexOptions.IgnoreCase);
        private static readonly Regex regexBool = new Regex("(False)|(True)");

        private static readonly Regex regexAnd = new Regex("(?<=[^$0-9A-Za-z\\|\\&])((AND)|(\\&\\&))(?=[^$0-9A-Za-z\\|\\&])", RegexOptions.IgnoreCase);
        private static readonly Regex regexOr = new Regex("(?<=[^$0-9A-Za-z\\|\\&])((OR)|(\\|\\|))(?=[^$0-9A-Za-z\\|\\&])", RegexOptions.IgnoreCase);
        private static readonly Regex regexBracketTemp = new Regex("{Sys_Bracket_[0-9]+}");
        private static readonly Regex regexSpace = new Regex("[ 　\\t]+");
        private static readonly Regex regexArgTemp = new Regex("{Sys_[0-9]+}");
        private static readonly Regex regexOnlyBracket = new Regex("^{Sys_Bracket_[0-9]+}$");
        private static readonly Regex regexOnlyArg = new Regex("^{Sys_[0-9]+}$");
        private static readonly Regex regexParam = new Regex("@[A-Za-z$_][A-Za-z0-9$_]*");

        #endregion


        #region  计 算 表 达 式 的 私 有 函 数

        private ComputeValue ComputeBracket(string bracketExpression, BracketExpressionCollection brackets, ComputeValueCollection args)
        {            
            //计算有括号的表达式
            bracketExpression = regexSpace.Replace(bracketExpression, " ");

            //计算函数式
            if (WillComputeMethod)
            {
                Regex methodsRegex = MethodRegex; //ComputeMethodManage.MethodsRegex;
                bracketExpression = methodsRegex.Replace(bracketExpression, matchMethod =>
                {
                    if (!matchMethod.Success) throw new Exception("未能匹配到 函数式!");
                    string methodExpression = matchMethod.Value;
                    ComputeValue result = ComputeMethod(methodExpression, brackets, args);
                    ComputeValue methodValue = new ComputeValue("{Sys_" + args.Count + "}", result.Value, result.Type);
                    args.Add(methodValue);
                    return methodValue.ArgTemp;
                });
            }

            //计算括号式
            do
            {
                string newBracketExpression = regexBracketTemp.Replace(bracketExpression, matchBracket =>
                {
                    if (!matchBracket.Success) throw new Exception("未能匹配到 括号表达式!");
                    BracketExpression childBracket = brackets[matchBracket.Value];
                    ComputeValue result = ComputeBracket(childBracket.Expression, brackets, args);
                    ComputeValue bracketValue = new ComputeValue("{Sys_" + args.Count + "}", result.Value, result.Type);
                    args.Add(bracketValue);
                    return bracketValue.ArgTemp;
                });

                if (newBracketExpression == bracketExpression) break;           //新字符串没有变化，则表示没有匹配到内部小括号
                else bracketExpression = newBracketExpression;
            } while (true);

            return ComputeSimpleness(bracketExpression, args);
        }
        private static ComputeValue ComputeSimpleness(string simplenessExpression, ComputeValueCollection args)
        {
            //计算没有小括号的表达式
            List<ComputeSymbolAttribute> listSymbol = ComputeSymbolManage.Symbols;
            simplenessExpression = regexSpace.Replace(simplenessExpression, " ");

            Match priorMatch = null;
            ComputeSymbolAttribute priorSymbol = null;                //对于运算的优先级，平级时 前后运算
            foreach (ComputeSymbolAttribute symbol in listSymbol)
            {
                if (priorMatch != null && /*priorSymbol != null && */symbol.Level < priorSymbol.Level) continue;
                Match match = symbol.Regex.Match(simplenessExpression);
                if (match.Success)
                {
                    if ((priorMatch == null/* && priorSymbol == null*/)                                     //如果还没有确定优先级，则当前优先
                        || (symbol.Level > priorSymbol.Level)                                           //如果等级高，则优先
                        || (symbol.Level == priorSymbol.Level && match.Index < priorMatch.Index))       //如果等级一致，则前面的优先
                    {
                        priorMatch = match;
                        priorSymbol = symbol;
                    }
                }
            }

            if (priorMatch != null/* && priorSymbol != null*/)                                              //如果确定了优先级，即表示还需继续运算
            {
                List<ComputeValue> symbolArgs = new List<ComputeValue>();
                string priorMatchValue = priorMatch.Value;
                MatchCollection matchIndexs = regexArgTemp.Matches(priorMatchValue);
                foreach (Match matchIndex in matchIndexs)
                {
                    string argTemp = matchIndex.Value;
                    ComputeValue arg = args[argTemp];
                    symbolArgs.Add(arg);
                }
                IComputeSymbol calculateSymbol = (IComputeSymbol)Activator.CreateInstance(priorSymbol.SymbolType);
                calculateSymbol.Arguments = symbolArgs.ToArray();
                ComputeValue symbolValue = calculateSymbol.Compute();
                symbolValue.ArgTemp = "{Sys_" + args.Count + "}";
                args.Add(symbolValue);

                string newExpresTemp = simplenessExpression.Replace(priorMatchValue, symbolValue.ArgTemp);        //无论参数，括号表达式，函数式，因为有 索引的存在，所以不会出现多次替换
                return ComputeSimpleness(newExpresTemp, args);
            }
            else                                                                                        //如果没有确定优先级，即表示表达式已经是最后结果
            {
                Match matchResultIndex = regexArgTemp.Match(simplenessExpression);
                return args[matchResultIndex.Value];
            }
        }
        private ComputeValue ComputeMethod(string bracketExpression, BracketExpressionCollection brackets, ComputeValueCollection args)
        {
            //计算函数表达式：将函数表达式 计算并替换成参数：然后调用 括号式 进行运算；
            if (WillComputeMethod)
            {
                List<ComputeMethodAttribute> listMethod = MethodAttributes; //ComputeMethodManage.Methods;
                bracketExpression = regexSpace.Replace(bracketExpression, " ");

                foreach (ComputeMethodAttribute method in listMethod)
                {
                    Match match = method.Regex.Match(bracketExpression);
                    if (match.Success)
                    {
                        string methodMatchValue = match.Value;
                        string bracketTemp = regexBracketTemp.Match(methodMatchValue).Value;
                        BracketExpression methodBracket = brackets[bracketTemp];
                        ComputeValueCollection methodArgs = AnalysisSingleBracket(methodBracket, brackets, args);

                        IComputeMethod calculateMethod = (IComputeMethod) Activator.CreateInstance(method.MethodType);
                        calculateMethod.Arguments = methodArgs.ToArray();
                        ComputeValue result = calculateMethod.Compute();
                        ComputeValue methodValue = new ComputeValue(string.Empty, result.Value, result.Type);
                        methodValue.ArgTemp = "{Sys_" + args.Count + "}";
                        args.Add(methodValue);

                        bracketExpression = bracketExpression.Replace(methodMatchValue, methodValue.ArgTemp);
                            //无论参数，括号表达式，函数式，因为有 索引的存在，所以不会出现多次替换
                    }
                }
            }
            return ComputeBracket(bracketExpression, brackets, args);
        }
        private ComputeValueCollection AnalysisSingleBracket(BracketExpression methodBracket, BracketExpressionCollection brackets, ComputeValueCollection args)
        {
            //从一组小括号中，返回多个参数
            ComputeValueCollection list = new ComputeValueCollection();
            string argsExpression = methodBracket.Expression;

            //计算子函数表达式
            if (WillComputeMethod)
            {
                Regex methodsRegex = MethodRegex; //ComputeMethodManage.MethodsRegex;
                argsExpression = methodsRegex.Replace(argsExpression, match =>
                {
                    string childMethodBracket = match.Value;
                    ComputeValue result = ComputeMethod(childMethodBracket, brackets, args);
                    ComputeValue childValue = new ComputeValue("{Sys_" + args.Count + "}", result.Value, result.Type);
                    args.Add(childValue);
                    return childValue.ArgTemp;
                });
            }

            //计算小括号表达式
            argsExpression = regexBracketTemp.Replace(argsExpression, match =>
            {
                string bracketExpression = match.Value;
                ComputeValue result = ComputeBracket(bracketExpression, brackets, args);
                ComputeValue bracketValue = new ComputeValue("{Sys_" + args.Count + "}", result.Value, result.Type);
                args.Add(bracketValue);
                return bracketValue.ArgTemp;
            });

            //至此，argsExpression 不会再包括任何 小括号表达式，不包括任何函数——但可能有简单表达式

            #if (!WindowsCE && !PocketPC)
            string[] argExpres = argsExpression.Split(new[] { "," }, StringSplitOptions.None);
            #endif
            #if (WindowsCE || PocketPC)
            string[] argExpres = StringExtend.Split(argsExpression, ",", StringSplitOptions.None);
            #endif
            if (/*argExpres != null && */argExpres.Length > 0)
                foreach (string simpExpres in argExpres)
                {
                    //ComputeValue expresValue = ComputeBracket(simpExpres, listBracket, listArg);          //这种情况不存在
                    ComputeValue expresValue = ComputeSimpleness(simpExpres, args);
                    list.Add(expresValue);
                }

            return list;
        }

        private bool? ComputeAndOr(string expressions, BracketExpressionCollection brackets, ComputeValueCollection args)
        {
            expressions = expressions.Trim();
            AndOrExpres andOrExpres = AnalyseBooleanSchema(expressions, brackets);
            if (andOrExpres != null)
            {
                bool? computeValue = andOrExpres.ExpresType == AndOrType.Or
                                         ? ComputeOr(andOrExpres.BooleanSchemaExpres, brackets, args)
                                         : ComputeAnd(andOrExpres.BooleanSchemaExpres, brackets, args);
                return computeValue;
            }
            else
            {
                //先进性直接取值的尝试，无法直接取值的，再进行 复杂运算取值
                if (regexOnlyArg.IsMatch(expressions))
                {
                    ComputeValue arg = args[expressions];
                    if (arg != null)
                        return Convert.ToBoolean(arg.Value);
                }
                
                ComputeValue computeValue = ComputeMethod(expressions, brackets, args);
                return Convert.ToBoolean(computeValue.Value);
            }
        }
        private bool? ComputeAnd(List<string> andExpressions, BracketExpressionCollection brackets, ComputeValueCollection args)
        {
            //将只有 AND 的表达式计算成 bool 值结果 
            if (andExpressions == null || andExpressions.Count <= 0)
                return true;

            foreach (string andExpression in andExpressions)
            {
                bool? result = ComputeAndOr(andExpression, brackets, args);
                if (result != null && !result.Value)
                    return false;                    //对于 AND 链接的 任意结果 返回 false,则立即返回 false：而不做后面的多于计算
            }
            return true;
        }
        private bool? ComputeOr(List<string> orExpressions, BracketExpressionCollection brackets, ComputeValueCollection args)
        {
            //将只有 OR 的表达式计算成 bool 值结果
            if (orExpressions == null || orExpressions.Count <= 0)
                return true;

            foreach (string orExpression in orExpressions)
            {
                bool? result = ComputeAndOr(orExpression, brackets, args);
                if (result != null && result.Value)
                    return true;                    //对于 OR 链接的 任意结果 返回 true,则立即返回 true：而不做后面的多于计算
            }
            return false;
        }

        private ComputeValue cacheComputeValue;
        private bool? cacheBooleanComputeValue;
        private readonly Dictionary<object, ComputeValue> cacheValues = new Dictionary<object, ComputeValue>();
        private readonly Dictionary<object, bool?> cacheBooleanValues = new Dictionary<object, bool?>();

        private ComputeValue GetCacheValues(object obj)
        {
            if (obj == null) return null;

            ComputeValue cacheValue = null;
            bool exist = cacheValues.ContainsKey(obj);
            if (exist)
                cacheValue = cacheValues[obj];

            return cacheValue;
        }
        private bool? GetCacheBooleanValues(object obj)
        {
            if (obj == null) return null;

            bool? cacheValue = null;
            bool exist = cacheBooleanValues.ContainsKey(obj);
            if (exist)
                cacheValue = cacheBooleanValues[obj];

            return cacheValue;
        }

        #endregion


        #region  资 源 释 放

        private bool disposed;
        ~ComputeExecuter()
		{
			Dispose(false);
		}
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        internal void Dispose(bool disposing)
        {
            if (disposed) return;
            disposed = true;

            listArg.Clear();
            listBracket.Clear();
            methodAttributes.Clear();
            cacheValues.Clear();
            cacheBooleanValues.Clear();
        }

        #endregion


        private static IList<string> GetPropertyNames(Type objType)
        {
            if (objType == null) return null;
            List<PropertyInfo> propertyInfos = Tools.GetPropertyInfos(objType);
            List<string> list = new List<string>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
                if (propertyInfo != null)
                    list.Add(propertyInfo.Name);

            return list;
        }

    }

    [Serializable]
    internal class AndOrExpres
    {
        internal List<string> BooleanSchemaExpres { get; set; }
        internal AndOrType ExpresType { get; set; }
    }

    [Serializable]
    internal enum AndOrType { And, Or }
}



//MatchCollection matchArgs = regexArg.Matches(expression);
//int indexRArgTemp = -1;
//string expresArgTemp = regexArg.Replace(expression, match =>
//{
//    indexRArgTemp++;
//    return " {Sys_" + indexRArgTemp + "} ";
//});

//int indexArgTemp = -1;
//foreach (Match match in matchArgs)
//{
//    indexArgTemp++;
//    ComputeValue argValue = new ComputeValue { Value = match.Value, ArgTemp = "{Sys_" + indexArgTemp + "}" };
//    listArg.Add(argValue);
//}

        //private readonly IgnoreDictionary<FieldFormat> cacheFieldFormat = new IgnoreDictionary<FieldFormat>();
        //private FieldFormat ChacheFieldFormat(string format)
        //{
        //    //禁止枚举 cacheFieldFormat 字典
        //    if (StringExtend.IsNullOrWhiteSpace(format)) return null;

        //    FieldFormat fieldFormat = cacheFieldFormat[format];
        //    if (fieldFormat == null)
        //    {
        //        fieldFormat = new FieldFormat(format);
        //        cacheFieldFormat.Add(format, fieldFormat);
        //    }
        //    return fieldFormat;
        //}

//private List<string> booleanSchemaExpres;

//private List<string> BooleanSchemaExpres
//{
//    get { return booleanSchemaExpres ?? (booleanSchemaExpres = new List<string>()); }
//    set
//    {
//        if (booleanSchemaExpres == null) booleanSchemaExpres = new List<string>();
//        if (value != null && value.Count >= 1)
//            booleanSchemaExpres.AddRange(value);
//    }
//}