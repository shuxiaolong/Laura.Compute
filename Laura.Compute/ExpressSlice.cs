using System;
using System.Collections;
using Laura.Compute.Utils;

namespace Laura.Compute
{
    /// <summary>
    /// 表达式片段 对象
    /// </summary>
    [Serializable]
    public class ExpressSlice
    {
        private string express = string.Empty;

        /// <summary>
        /// 数据类型
        /// </summary>
        public ExpressType ExpressType { get; internal set; }

        /// <summary>
        /// 表达式
        /// </summary>
        public string Express
        {
            get { return express; }
            set { express = (value ?? string.Empty).Trim(); }
        }


        /// <summary>
        /// 计算器：计算器可以对 当前表达式片段 进行 计算
        /// </summary>
        public ICompute Computer { get; set; }

        /// <summary>
        /// 常量值：当前表达式 是 元数据，没有 Computer 对象 且 也不需要 Computer 对象进行计算 就已经 判定的值 
        /// </summary>
        public object MetaValue { get; set; }






        #region  计 算 表 达 式 片 段

        /// <summary>
        /// 计算表达式的结果
        /// </summary>
        public object Compute(ExpressSchema expressSchema, object objOrHash)
        {
            if (MetaValue != null) return MetaValue;
            else if (Computer != null)
            {
                object value = Computer.Compute(expressSchema, objOrHash);
                value = AnalyseSingleArrayItem(value);
                return value;
            }
            return null;
        }
        /// <summary>
        /// 计算表达式的结果 并转换成 String类型
        /// </summary>
        public string ComputeString(ExpressSchema expressSchema, object objOrHash)
        {
            if (MetaValue != null) return MetaValue.ToString();
            else if (Computer != null)
            {
                object value = Computer.Compute(expressSchema, objOrHash);
                value = AnalyseSingleArrayItem(value);
                return (value ?? string.Empty).ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// 计算表达式的结果 并转换成 Double类型
        /// </summary>
        public double ComputeDouble(ExpressSchema expressSchema, object objOrHash)
        {
            if (MetaValue != null) return MetaValue is double ? (double)MetaValue : Tools.ToDouble(MetaValue);
            else if (Computer != null)
            {
                object value = Computer.Compute(expressSchema, objOrHash);
                value = AnalyseSingleArrayItem(value);
                return value is double ? (double)value : Tools.ToDouble(value);
            }
            return 0;
        }
        /// <summary>
        /// 计算表达式的结果 并转换成 Doolean类型
        /// </summary>
        public bool ComputeDoolean(ExpressSchema expressSchema, object objOrHash)
        {
            if (MetaValue != null) return MetaValue is bool ? (bool)MetaValue : Tools.ToBoolean(MetaValue);
            else if (Computer != null)
            {
                object value = Computer.Compute(expressSchema, objOrHash);
                value = AnalyseSingleArrayItem(value);
                return value is bool ? (bool)value : Tools.ToBoolean(value);
            }
            return false;
        }
        /// <summary>
        /// 计算表达式的结果 并转换成 Array类型
        /// </summary>
        public ArrayList ComputeArray(ExpressSchema expressSchema, object objOrHash)
        {
            if (MetaValue is ArrayList) return (ArrayList)MetaValue;
            else if (Computer != null)
            {
                object value = Computer.Compute(expressSchema, objOrHash);
                if (value is ArrayList) return (ArrayList)value;
            }
            return null;
        }
        /// <summary>
        /// 计算表达式的结果 并转换成 DateTime类型
        /// </summary>
        public DateTime ComputeDate(ExpressSchema expressSchema, object objOrHash)
        {
            if (MetaValue != null) return MetaValue is DateTime ? (DateTime)MetaValue : Tools.ToDateTime(MetaValue);
            else if (Computer != null)
            {
                object value = Computer.Compute(expressSchema, objOrHash);
                value = AnalyseSingleArrayItem(value);
                return value is DateTime ? (DateTime)value : Tools.ToDateTime(value);
            }
            return DateTime.MinValue;
        }
        

        private object AnalyseSingleArrayItem(object value)
        {
            ArrayList list = value as ArrayList;
            if (list == null) return value;
            if (list.Count == 1) return list[0];
            return value;
        }

        #endregion
    }
}
