using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Laura.Compute
{
    /// <summary>
    /// 计算一个表达式，返回一个 任意值 的 执行器
    /// </summary>
    public class ComputeValueExecuter
    {
        //UserAge>=20 AND LEN(UserName)>=3 AND REPLACE(UserName,\"舒\",\"龙\") LIKE \"龙%龙\"
        //UserAge>={Arg_0} AND LEN(UserName)>={Arg_1} AND REPLACE(UserName,{Arg_2},{Arg_3}) LIKE {Arg_4}


        /// <summary>
        /// 直接计算表达式 返回 ComputeValue
        /// </summary>
        public ComputeValue ComputeBoolean()
        {
            return null;
        }

        /// <summary>
        /// 需要一个 字典对象 完善表达式中的部分参数，然后计算表达式 返回 ComputeValue
        /// </summary>
        public ComputeValue ComputeBoolean(IDictionary parameter)
        {
            return null;
        }

        /// <summary>
        /// 需要一个 普通对象 完善表达式中的部分参数，然后计算表达式 返回 ComputeValue
        /// </summary>
        public ComputeValue ComputeBoolean(object parameter)
        {
            return null;
        }
    }
}
