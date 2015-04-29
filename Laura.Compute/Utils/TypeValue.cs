using System;

namespace Laura.Compute.Utils
{
    [Serializable]
    internal class TypeValue
    {
        public Type Type { get; set; }
        public object Value { get; set; }

        public TypeValue() { }
        public TypeValue(object value)
        {
            Value = value;
            Type = value == null ? null : value.GetType();
        }
        public TypeValue(object value, Type type)
        {
            Value = value;
            Type = type;
        }

    }
}
