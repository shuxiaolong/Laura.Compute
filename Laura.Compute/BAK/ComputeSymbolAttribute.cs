//2013-07-14 因为 再抽象,本特性 和 对应接口 被取消

//using System;
//using System.Text.RegularExpressions;

//namespace Laura.Compute
//{
//    public class ComputeSymbolAttribute : Attribute
//    {
//        private string symbol = string.Empty;
//        private Regex regex;

//        public string Symbol
//        {
//            get { return symbol; }
//            set
//            {
//                symbol = value ?? string.Empty;
//                if (!string.IsNullOrEmpty(symbol))
//                {
//                    string symbolTemp = StaticHelper.FormatRegex(symbol);
//                    string regexExpres = symbolTemp.Replace("{A}", "{Sys_[0-9]+}");
//                    regex = new Regex(regexExpres);
//                }
//            }
//        }
//        public int Level { get; set; }
//        public Regex Regex
//        {
//            get { return regex; }
//        }
//        public Type SymbolType { get; set; }
//    }
//}
