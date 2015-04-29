//2013-07-14 因为 再抽象,本特性 和 对应接口 被取消

//using System;
//using System.Text.RegularExpressions;

//namespace Laura.Compute
//{
//    public class ComputeMethodAttribute : Attribute
//    {
//        private string method = string.Empty;
//        private Regex regex;

//        public string Method
//        {
//            get { return method; }
//            set
//            {
//                method = value ?? string.Empty;
//                if (!string.IsNullOrEmpty(method))
//                {
//                    string regexExpres = method + "\\s*{Sys_Bracket_[0-9]+}";
//                    regex = new Regex(regexExpres);
//                }
//            }
//        }
//        public Regex Regex
//        {
//            get { return regex; }
//        }
//        public Type MethodType { get; set; }
//    }
//}
