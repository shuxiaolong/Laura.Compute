using System;
using System.Text.RegularExpressions;

namespace Laura.Compute
{
    /// <summary>
    /// 标注在 某个 扩展插件上,以标明 某些单词是否是 系统关键字
    /// </summary>
    public class ComputeKeyword : Attribute
    {
        private string method = string.Empty;
        private Regex regex;

        public string Method
        {
            get { return method; }
            set
            {
                method = value ?? string.Empty;
                if (!string.IsNullOrEmpty(method))
                {
                    string regexExpres = method + "\\s*{Sys_Bracket_[0-9]+}";
                    regex = new Regex(regexExpres);
                }
            }
        }
        public Regex Regex
        {
            get { return regex; }
        }
        public Type MethodType { get; set; }
    }
}
