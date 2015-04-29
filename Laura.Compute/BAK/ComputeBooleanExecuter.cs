using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Laura.Compute.Utils;

namespace Laura.Compute
{
    /// <summary>
    /// 计算一个表达式，返回一个 bool 值 的 执行器
    /// </summary>
    public class ComputeBooleanExecuter
    {
        /// <summary>
        /// 直接计算表达式 返回 bool 值
        /// </summary>
        public bool ComputeBoolean()
        {
            return true;
        }

        /// <summary>
        /// 需要一个 字典对象 完善表达式中的部分参数，然后计算表达式 返回 bool 值
        /// </summary>
        public bool ComputeBoolean(IDictionary parameter)
        {
            return true;
        }

        /// <summary>
        /// 需要一个 普通对象 完善表达式中的部分参数，然后计算表达式 返回 bool 值
        /// </summary>
        public bool ComputeBoolean(object parameter)
        {
            return true;
        }

    }
}
