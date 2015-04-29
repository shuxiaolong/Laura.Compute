using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Laura.Compute.Utils;

namespace Laura.Compute
{

    public class ComputeMethodManage
    {
        private static string methodRegexTemp = string.Empty;
        private static Regex methodRegex;
        private static List<ComputeMethodAttribute> typeMethods;

        public static List<ComputeMethodAttribute> Methods
        {
            get
            {
                if (typeMethods == null) typeMethods = new List<ComputeMethodAttribute>();
                if (typeMethods.Count <= 0)
                {
                    typeMethods.Clear();
                    methodRegex = null;

                    Dictionary<Type, ComputeMethodAttribute> tempSymbols = AssemblyHelper.GetAttributes<ComputeMethodAttribute>();
                    typeMethods.AddRange(tempSymbols.Values);
                    methodRegexTemp = GetMethodsRegex(tempSymbols.Values);
                    //typeMethods.Sort((x, y) => y.Level - x.Level);
                }
                return typeMethods;
            }
        }
        public static Regex MethodsRegex
        {
            get { return methodRegex ?? (methodRegex = new Regex(methodRegexTemp)); }
        }

        public static string GetMethodsRegex(IEnumerable<ComputeMethodAttribute> attributes)
        {
            if(attributes==null) return null;

            StringBuilder sb = new StringBuilder();
            foreach (ComputeMethodAttribute attribute in attributes)
            {
                string sTemp = "(" + attribute.Method + "\\s*{Sys_Bracket_[0-9]+})";
                sb.Append((sb.Length <= 0 ? sTemp : "|" + sTemp));
            }
            string regexExpres = sb.ToString();
            return regexExpres;
        }
    }

    public class ComputeSymbolManage
    {
        private static List<ComputeSymbolAttribute> typeSymbols;
        public static List<ComputeSymbolAttribute> Symbols
        {
            get
            {
                if (typeSymbols == null) typeSymbols = new List<ComputeSymbolAttribute>();
                if (typeSymbols.Count <= 0)
                {
                    Dictionary<Type, ComputeSymbolAttribute> tempSymbols = AssemblyHelper.GetAttributes<ComputeSymbolAttribute>();
                    typeSymbols.AddRange(tempSymbols.Values);
                    typeSymbols.Sort((x, y) => y.Level - x.Level);
                }
                return typeSymbols;
            }
        }
    }
}
