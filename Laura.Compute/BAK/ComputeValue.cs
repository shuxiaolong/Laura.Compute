using System;
using System.Collections.Generic;
using Laura.Compute.Utils;

namespace Laura.Compute
{
    [Serializable]
    public class ComputeValue
    {
        protected string value;
        public Type Type { get; set; }
        public string ArgTemp { get; set; }
        public string Value
        {
            get { return value; }
            set
            {
                this.value = value ?? string.Empty;
                if (this.value.StartsWith("\"") && this.value.EndsWith("\""))
                {
                    this.value = this.value.Substring(1, this.value.Length - 2);
                    //this.value = this.value.Replace("\\\t", "\t").Replace("\\\r", "\r").Replace("\\\n", "\n");  //已经转义成功，不需要再次转义
                }
                else if (this.value.StartsWith("\'") && this.value.EndsWith("\'"))
                {
                    this.value = this.value.Substring(1, this.value.Length - 2);
                    //this.value = this.value.Replace("\\\t", "\t").Replace("\\\r", "\r").Replace("\\\n", "\n");  //已经转义成功，不需要再次转义
                }
                
            }
        }

        public ComputeValue() { }
        public ComputeValue(string argTemp, string value)
        {
            ArgTemp = argTemp;
            Value = value;
            Type = value.GetType();
        }
        public ComputeValue(string argTemp, string value, Type type)
        {
            ArgTemp = argTemp;
            Value = value;
            Type = type;
        }
    }

    [Serializable]
    internal class PropertyComputeValue : ComputeValue
    {
        //某些 对象的属性，在 具体对象指定之前 是无法获取 确定的值：该实体类表示这类数据：通过对象获取实际值
        public string FieldName { get; set; }
        public void InitValue(object obj)
        {
            if (obj == null) return;
            if (StringExtend.IsNullOrWhiteSpace(FieldName))
                throw new Exception("PropertyComputeValue Can Not InitValue By Invalid FieldName!");

            TypeValue typeValue = Tools.GetTypeValue(obj, FieldName);
            if (typeValue != null)
            {
                Value = typeValue.Value.ToString();
                Type = typeValue.Type;
            }
        }

        public PropertyComputeValue(){ }
        public PropertyComputeValue(string fieldName)
        {
            FieldName = fieldName;
        }
        public PropertyComputeValue(string fieldName, object obj)
        {
            FieldName = fieldName;
            InitValue(obj);
        }
    }

    [Serializable]
    internal class ParameterComputeValue : ComputeValue
    {
        //某些 表达式的参数，在 具体参数字典指定之前 是无法获取 确定的值：该实体类表示这类数据：通过参数字典获取数据
        public string ParamName { get; set; }
        public void InitValue(IgnoreHashtable parameters)
        {
            if (parameters == null || parameters.Count <= 0) return;
            if (StringExtend.IsNullOrWhiteSpace(ParamName))
                throw new Exception("ParameterComputeValue Can Not InitValue By Invalid ParamName!");

            object paramValue = parameters[ParamName];
            if(paramValue==null) paramValue = parameters[ParamName.TrimStart('@')];

            if (paramValue != null)
            {
                Value = paramValue.ToString();
                Type = paramValue.GetType();
            }
        }

        public ParameterComputeValue() { }
        public ParameterComputeValue(string paramName)
        {
            ParamName = paramName;
        }
        public ParameterComputeValue(string paramName, IgnoreHashtable parameters)
        {
            ParamName = paramName;
            InitValue(parameters);
        }
    }


    [Serializable]
    public class ComputeValueCollection : List<ComputeValue>
    {
        private readonly Dictionary<string, ComputeValue> mapping = new Dictionary<string, ComputeValue>();

        public new void Add(ComputeValue item)
        {
            base.Add(item);
            mapping.Add(item.ArgTemp, item);
        }
        public new bool Remove(ComputeValue item)
        {
            bool result = base.Remove(item);
            if (result)
                mapping.Remove(item.ArgTemp);
            return result;
        }
        public new void Clear()
        {
            base.Clear();
            mapping.Clear();
        }
        public new void AddRange(IEnumerable<ComputeValue> collection)
        {
            if (collection == null) return;
            base.AddRange(collection);
            foreach (ComputeValue item in collection)
                mapping.Add(item.ArgTemp, item);
        }

        public ComputeValue this[string argTemp]
        {
            get { return mapping.ContainsKey(argTemp) ? mapping[argTemp] : null; }
        }
        public new ComputeValue this[int argTempIndex]
        {
            get
            {
                string argTemp = "{Sys_" + argTempIndex + "})";
                return mapping[argTemp];
            }
        }
    }


    [Serializable]
    public class BracketExpression
    {
        public string BracketTemp { get; set; }
        public string Expression { get; set; }
    }

    [Serializable]
    public class BracketExpressionCollection : List<BracketExpression>
    {
        private readonly Dictionary<string, BracketExpression> mapping = new Dictionary<string, BracketExpression>();

        public new void Add(BracketExpression item)
        {
            base.Add(item);
            mapping.Add(item.BracketTemp, item);
        }
        public new bool Remove(BracketExpression item)
        {
            bool result = base.Remove(item);
            if (result)
                mapping.Remove(item.BracketTemp);
            return result;
        }
        public new void Clear()
        {
            base.Clear();
            mapping.Clear();
        }
        public new void AddRange(IEnumerable<BracketExpression> collection)
        {
            if (collection == null) return;
            base.AddRange(collection);
            foreach (BracketExpression item in collection)
                mapping.Add(item.BracketTemp, item);
        }

        public BracketExpression this[string bracketTemp]
        {
            get { return mapping.ContainsKey(bracketTemp) ? mapping[bracketTemp] : null; }
        }
        public new BracketExpression this[int bracketTempIndex]
        {
            get
            {
                string bracketTemp = "{Sys_Bracket_" + bracketTempIndex + "})";
                return mapping[bracketTemp];
            }
        }
    }


}
