﻿using System;

namespace Laura.Compute
{
    /// <summary>
    /// 表达式 片段类型
    /// </summary>
    [Serializable]
    public enum ExpressType
    {
        String,
        Double,
        Boolean,
        SingleArgument,
        IndexArgument,
        FullArgument,
        SimpleArgument,
        ArrayList,
        Computer
    }
}
