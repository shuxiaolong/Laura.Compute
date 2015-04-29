using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
#if (!WindowsCE && !PocketPC)
using System.Reflection.Emit;
#endif


namespace Laura.Compute.Utils
{
    internal static class ReflectionHelper
    {
        internal const BindingFlags Property_Field_BindingFlags = BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;


        #region  动 态 赋 值

        private static SetValueTypeEnum currentSetValueTypeEnum = SetValueTypeEnum.None;
        internal static SetValueTypeEnum CurrentSetValueTypeEnum
        {
            get
            {
#if (!WindowsCE && !PocketPC)
                if (currentSetValueTypeEnum == SetValueTypeEnum.None)
                    currentSetValueTypeEnum = string.Equals(ConfigurationManager.AppSettings["ReflectionHelper_SetValueType"], "Emit", StringComparison.CurrentCultureIgnoreCase)
                            ? SetValueTypeEnum.Emit
                            : SetValueTypeEnum.Reflection;

                return currentSetValueTypeEnum;
#endif
#if (WindowsCE || PocketPC)
                return SetValueTypeEnum.Reflection;
#endif
            }
        }

#if (!WindowsCE && !PocketPC)

        #region  Emit 委 托 赋 值

        internal static SetValueDelegate InnerCreateSetValueDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || propertyInfo.DeclaringType == null || !propertyInfo.CanWrite) return null;
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (setMethod == null) return null;

            Type propertyType = propertyInfo.PropertyType;

            DynamicMethod dm = new DynamicMethod("SetPropertyValue", null, new [] { typeof(object), typeof(object) }, propertyInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(propertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, propertyType);  //拆箱
            generator.Emit(OpCodes.Callvirt, setMethod);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ret);

            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
        }
        internal static SetValueDelegate InnerCreateSetValueDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.DeclaringType == null || fieldInfo.IsStatic || fieldInfo.IsInitOnly || fieldInfo.IsLiteral) return null;

            Type fieldType = fieldInfo.FieldType;

            DynamicMethod dm = new DynamicMethod("SetFieldValue", null, new [] { typeof(object), typeof(object) }, fieldInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(fieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, fieldType);  //拆箱
            generator.Emit(OpCodes.Stfld, fieldInfo);
            generator.Emit(OpCodes.Ret);

            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
        }
        internal static StaticSetValueDelegate InnerCreateStaticSetValueDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || propertyInfo.DeclaringType == null || !propertyInfo.CanWrite) return null;
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (setMethod == null) return null;

            Type propertyType = propertyInfo.PropertyType;

            DynamicMethod dm = new DynamicMethod("StaticSetPropertyValue", null, new [] { typeof(object) }, propertyInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(propertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, propertyType);  //拆箱
            generator.Emit(OpCodes.Call, setMethod);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ret);

            return (StaticSetValueDelegate)dm.CreateDelegate(typeof(StaticSetValueDelegate));
        }
        internal static StaticSetValueDelegate InnerCreateStaticSetValueDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.DeclaringType == null || !fieldInfo.IsStatic || fieldInfo.IsInitOnly || fieldInfo.IsLiteral) return null;

            Type fieldType = fieldInfo.FieldType;

            DynamicMethod dm = new DynamicMethod("StaticSetFieldValue", null, new [] { typeof(object) }, fieldInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(fieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, fieldType);  //拆箱
            generator.Emit(OpCodes.Stsfld, fieldInfo);
            generator.Emit(OpCodes.Ret);

            return (StaticSetValueDelegate)dm.CreateDelegate(typeof(StaticSetValueDelegate));
        }

        private static readonly Hashtable setPropertyValueDelegates = new Hashtable();
        private static readonly Hashtable setFieldValueDelegates = new Hashtable();
        private static readonly Hashtable setStaticPropertyValueDelegates = new Hashtable();
        private static readonly Hashtable setStaticFieldValueDelegates = new Hashtable();

        internal static SetValueDelegate CreateSetValueDelegate(PropertyInfo propertyInfo)
        {
            SetValueDelegate setValueDelegate = setPropertyValueDelegates[propertyInfo] as SetValueDelegate;
            if (setValueDelegate == null)
            {
                setValueDelegate = InnerCreateSetValueDelegate(propertyInfo);
                setPropertyValueDelegates[propertyInfo] = setValueDelegate;
            }
            return setValueDelegate;
        }
        internal static SetValueDelegate CreateSetValueDelegate(FieldInfo fieldInfo)
        {
            SetValueDelegate setValueDelegate = setFieldValueDelegates[fieldInfo] as SetValueDelegate;
            if (setValueDelegate == null)
            {
                setValueDelegate = InnerCreateSetValueDelegate(fieldInfo);
                setFieldValueDelegates[fieldInfo] = setValueDelegate;
            }
            return setValueDelegate;
        }
        internal static StaticSetValueDelegate CreateStaticSetValueDelegate(PropertyInfo propertyInfo)
        {
            StaticSetValueDelegate setValueDelegate = setStaticPropertyValueDelegates[propertyInfo] as StaticSetValueDelegate;
            if (setValueDelegate == null)
            {
                setValueDelegate = InnerCreateStaticSetValueDelegate(propertyInfo);
                setStaticPropertyValueDelegates[propertyInfo] = setValueDelegate;
            }
            return setValueDelegate;
        }
        internal static StaticSetValueDelegate CreateStaticSetValueDelegate(FieldInfo fieldInfo)
        {
            StaticSetValueDelegate setValueDelegate = setStaticFieldValueDelegates[fieldInfo] as StaticSetValueDelegate;
            if (setValueDelegate == null)
            {
                setValueDelegate = InnerCreateStaticSetValueDelegate(fieldInfo);
                setStaticFieldValueDelegates[fieldInfo] = setValueDelegate;
            }
            return setValueDelegate;
        }


        public static bool EmitSetValue(object target, string propertyOrFieldName, object value)
        {
            if (target == null || string.IsNullOrEmpty(propertyOrFieldName)) return false;

            Type type = target.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                SetValueDelegate setter = CreateSetValueDelegate(propertyInfo);
                if (setter != null)
                {
                    object newValue = ConvertValue(value, propertyInfo.PropertyType);
                    setter(target, newValue); 
                    return true;
                }
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    SetValueDelegate setter = CreateSetValueDelegate(fieldInfo);
                    if (setter != null)
                    {
                        object newValue = ConvertValue(value, fieldInfo.FieldType); 
                        setter(target, newValue); 
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool EmitSetStaticValue(Type type, string propertyOrFieldName, object value)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                StaticSetValueDelegate setter = CreateStaticSetValueDelegate(propertyInfo);
                if (setter != null)
                {
                    object newValue = ConvertValue(value, propertyInfo.PropertyType);
                    setter(newValue); 
                    return true;
                }
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    StaticSetValueDelegate setter = CreateStaticSetValueDelegate(fieldInfo);
                    if (setter != null)
                    {
                        object newValue = ConvertValue(value, fieldInfo.FieldType);
                        setter(newValue); 
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion


#endif


        #region  纯 反 射 赋 值

        public static bool ReflectionSetValue(object target, string propertyOrFieldName, object value)
        {
            if (target == null || string.IsNullOrEmpty(propertyOrFieldName)) return false;

            Type type = target.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                object newValue = ConvertValue(value, propertyInfo.PropertyType);
                propertyInfo.SetValue(target, newValue, null);
                return true;
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    object newValue = ConvertValue(value, fieldInfo.FieldType);
                    fieldInfo.SetValue(target, newValue);
                    return true;
                }
            }

            return false;
        }
        public static bool ReflectionSetStaticValue(Type type, string propertyOrFieldName, object value)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                object newValue = ConvertValue(value, propertyInfo.PropertyType);
                propertyInfo.SetValue(null, newValue, null);
                return true;
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    object newValue = ConvertValue(value, fieldInfo.FieldType);
                    fieldInfo.SetValue(null, newValue);
                    return true;
                }
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 动态为 某个对象的属性 赋值，本函数不支持 多级赋值（注意：如果该函数不稳定，请在 配置文件中 将 appSettings.ReflectionHelper_SetValueType 试着赋值为 [默认]Reflection, Emit）
        /// </summary>
        public static bool SetValue(object target, string propertyOrFieldName, object value)
        {
#if (!WindowsCE && !PocketPC)
            switch (CurrentSetValueTypeEnum)
            {
                case SetValueTypeEnum.Emit:
                    return EmitSetValue(target, propertyOrFieldName, value);
                default:
                    return ReflectionSetValue(target, propertyOrFieldName, value);
            }
#endif
#if (WindowsCE || PocketPC)
            return ReflectionSetValue(target, propertyOrFieldName, value);
#endif
        }
        /// <summary>
        /// 动态为 某个类的静态属性 赋值，本函数不支持 多级赋值（注意：如果该函数不稳定，请在 配置文件中 将 appSettings.ReflectionHelper_SetValueType 试着赋值为 [默认]Reflection, Emit）
        /// </summary>
        public static bool SetStaticValue(Type type, string propertyOrFieldName, object value)
        {
#if (!WindowsCE && !PocketPC)
            switch (CurrentSetValueTypeEnum)
            {
                case SetValueTypeEnum.Emit:
                    return EmitSetStaticValue(type, propertyOrFieldName, value);
                default:
                    return ReflectionSetStaticValue(type, propertyOrFieldName, value);
            }
#endif
#if (WindowsCE || PocketPC)
            return ReflectionSetStaticValue(type, propertyOrFieldName, value);
#endif
        }

        #endregion

        #region  动 态 取 值

#if (!WindowsCE && !PocketPC)
        private static GetValueTypeEnum currentGetValueTypeEnum = GetValueTypeEnum.None;
#endif
        internal static GetValueTypeEnum CurrentGetValueTypeEnum
        {
            get
            {
#if (!WindowsCE && !PocketPC)
                if (currentGetValueTypeEnum == GetValueTypeEnum.None)
                    currentGetValueTypeEnum = string.Equals(ConfigurationManager.AppSettings["ReflectionHelper_GetValueType"], "Emit", StringComparison.CurrentCultureIgnoreCase)
                            ? GetValueTypeEnum.Emit
                            : GetValueTypeEnum.Reflection;

                return currentGetValueTypeEnum;
#endif
#if (WindowsCE || PocketPC)
                return GetValueTypeEnum.Reflection;
#endif
            }
        }

#if (!WindowsCE && !PocketPC)

        #region  Emit 委 托 取 值

        internal static GetValueDelegate InnerCreateGetValueDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || propertyInfo.DeclaringType == null || !propertyInfo.CanWrite) return null;
            MethodInfo getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null) return null;

            Type propertyType = propertyInfo.PropertyType;

            DynamicMethod dm = new DynamicMethod("GetPropertyValue", typeof(object), new [] { typeof(object) }, propertyInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Callvirt, getMethod);
            //if(propertyType.IsValueType) 
                generator.Emit(OpCodes.Box, propertyType);  //装箱
            generator.Emit(OpCodes.Ret);

            return (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
        }
        internal static GetValueDelegate InnerCreateGetValueDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.DeclaringType == null || fieldInfo.IsStatic || fieldInfo.IsInitOnly || fieldInfo.IsLiteral) return null;

            Type fieldType = fieldInfo.FieldType;

            DynamicMethod dm = new DynamicMethod("GetFieldValue", typeof(object), new [] { typeof(object) }, fieldInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, fieldInfo);
            //if (fieldType.IsValueType) 
                generator.Emit(OpCodes.Box, fieldType);  //装箱
            generator.Emit(OpCodes.Ret);

            return (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
        }
        internal static StaticGetValueDelegate InnerCreateStaticGetValueDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || propertyInfo.DeclaringType == null || !propertyInfo.CanWrite) return null;
            MethodInfo getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null) return null;

            Type propertyType = propertyInfo.PropertyType;

            DynamicMethod dm = new DynamicMethod("StaticGetPropertyValue", typeof(object), new Type[] { }, propertyInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Call, getMethod);
            //if (propertyType.IsValueType) 
                generator.Emit(OpCodes.Box, propertyType);  //装箱
            generator.Emit(OpCodes.Ret);

            return (StaticGetValueDelegate)dm.CreateDelegate(typeof(StaticGetValueDelegate));
        }
        internal static StaticGetValueDelegate InnerCreateStaticGetValueDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.DeclaringType == null || !fieldInfo.IsStatic || fieldInfo.IsInitOnly || fieldInfo.IsLiteral) return null;

            Type fieldType = fieldInfo.FieldType;

            DynamicMethod dm = new DynamicMethod("StaticGetFieldValue", typeof(object), new Type[] { }, fieldInfo.DeclaringType, true);
            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldsfld, fieldInfo);
            //if (fieldType.IsValueType) 
                generator.Emit(OpCodes.Box, fieldType);  //装箱
            generator.Emit(OpCodes.Ret);

            return (StaticGetValueDelegate)dm.CreateDelegate(typeof(StaticGetValueDelegate));
        }

        private static readonly Hashtable getPropertyValueDelegates = new Hashtable();
        private static readonly Hashtable getFieldValueDelegates = new Hashtable();
        private static readonly Hashtable getStaticPropertyValueDelegates = new Hashtable();
        private static readonly Hashtable getStaticFieldValueDelegates = new Hashtable();

        internal static GetValueDelegate CreateGetValueDelegate(PropertyInfo propertyInfo)
        {
            GetValueDelegate getValueDelegate = getPropertyValueDelegates[propertyInfo] as GetValueDelegate;
            if (getValueDelegate == null)
            {
                getValueDelegate = InnerCreateGetValueDelegate(propertyInfo);
                getPropertyValueDelegates[propertyInfo] = getValueDelegate;
            }
            return getValueDelegate;
        }
        internal static GetValueDelegate CreateGetValueDelegate(FieldInfo fieldInfo)
        {
            GetValueDelegate getValueDelegate = getFieldValueDelegates[fieldInfo] as GetValueDelegate;
            if (getValueDelegate == null)
            {
                getValueDelegate = InnerCreateGetValueDelegate(fieldInfo);
                getFieldValueDelegates[fieldInfo] = getValueDelegate;
            }
            return getValueDelegate;
        }
        internal static StaticGetValueDelegate CreateStaticGetValueDelegate(PropertyInfo propertyInfo)
        {
            StaticGetValueDelegate getValueDelegate = getStaticPropertyValueDelegates[propertyInfo] as StaticGetValueDelegate;
            if (getValueDelegate == null)
            {
                getValueDelegate = InnerCreateStaticGetValueDelegate(propertyInfo);
                getStaticPropertyValueDelegates[propertyInfo] = getValueDelegate;
            }
            return getValueDelegate;
        }
        internal static StaticGetValueDelegate CreateStaticGetValueDelegate(FieldInfo fieldInfo)
        {
            StaticGetValueDelegate getValueDelegate = getStaticFieldValueDelegates[fieldInfo] as StaticGetValueDelegate;
            if (getValueDelegate == null)
            {
                getValueDelegate = InnerCreateStaticGetValueDelegate(fieldInfo);
                getStaticFieldValueDelegates[fieldInfo] = getValueDelegate;
            }
            return getValueDelegate;
        }


        public static object EmitGetValue(object target, string propertyOrFieldName)
        {
            if (target == null || string.IsNullOrEmpty(propertyOrFieldName)) return false;

            Type type = target.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                GetValueDelegate getter = CreateGetValueDelegate(propertyInfo);
                if (getter != null)
                {
                    return getter(target);
                }
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    GetValueDelegate getter = CreateGetValueDelegate(fieldInfo);
                    if (getter != null)
                    {
                        return getter(target);
                    }
                }
            }

            return null;
        }
        public static object EmitGetStaticValue(Type type, string propertyOrFieldName)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                StaticGetValueDelegate getter = CreateStaticGetValueDelegate(propertyInfo);
                if (getter != null)
                {
                    return getter();
                }
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    StaticGetValueDelegate getter = CreateStaticGetValueDelegate(fieldInfo);
                    if (getter != null)
                    {
                        return getter();
                    }
                }
            }

            return null;
        }

        #endregion

#endif

        #region  纯 反 射 取 值

        public static object ReflectionGetValue(object target, string propertyOrFieldName)
        {
            if (target == null || string.IsNullOrEmpty(propertyOrFieldName)) return false;

            Type type = target.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                return propertyInfo.GetValue(target, null);
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    return fieldInfo.GetValue(target);
                }
            }

            return null;
        }
        public static object ReflectionGetStaticValue(Type type, string propertyOrFieldName)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyOrFieldName, Property_Field_BindingFlags);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                return propertyInfo.GetValue(null, null);
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyOrFieldName, Property_Field_BindingFlags);
                if (fieldInfo != null && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
                {
                    return fieldInfo.GetValue(null);
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// 动态从 某个对象的属性 取值，本函数不支持 多级取值（注意：如果该函数不稳定，请在 配置文件中 将 appSettings.ReflectionHelper_GetValueType 试着赋值为 [默认]Reflection, Emit）
        /// </summary>
        public static object GetValue(object target, string propertyOrFieldName)
        {
#if (!WindowsCE && !PocketPC)
            switch (CurrentGetValueTypeEnum)
            {
                case GetValueTypeEnum.Emit:
                    return EmitGetValue(target, propertyOrFieldName);
                default:
                    return ReflectionGetValue(target, propertyOrFieldName);
            }
#endif
            return ReflectionGetValue(target, propertyOrFieldName);
        }
        /// <summary>
        /// 动态从 某个类的静态属性 取值，本函数不支持 多级取值（注意：如果该函数不稳定，请在 配置文件中 将 appSettings.ReflectionHelper_SetValueType 试着赋值为 [默认]Reflection, Emit）
        /// </summary>
        public static object GetStaticValue(Type type, string propertyOrFieldName)
        {
#if (!WindowsCE && !PocketPC)
            switch (CurrentGetValueTypeEnum)
            {
                case GetValueTypeEnum.Emit:
                    return EmitGetStaticValue(type, propertyOrFieldName);
                default:
                    return ReflectionGetStaticValue(type, propertyOrFieldName);
            }
#endif

            return ReflectionGetStaticValue(type, propertyOrFieldName);
        }

        #endregion



        #region  数 据 转 换

        #region  基 本 数 据 类 型

        public static bool IsMetaType(Type type)
        {
            if (type.IsEnum) return true;       //枚举视为 基本类型
            return metaTypes.Contains(type);
        }
        public static Type GetTypeBySimpleTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            switch (typeName.ToLower())
            {
                case "string": case "str": return typeofString;
                case "bool": case "boolean": return typeofBoolean;
                case "byte": return typeofByte;
                case "char": return typeofChar;
                case "decimal": return typeofDecimal;
                case "double": return typeofDouble;
                case "short": case "int16": return typeofInt16;
                case "int": case "int32": return typeofInt32;
                case "long": case "int64": return typeofInt64;
                case "sbyte": return typeofSByte;
                case "float": case "single": return typeofSingle;
                case "timespan": return typeofTimeSpan;
                case "datetime": return typeofDateTime;
                case "ushort": case "uint16": return typeofUInt16;
                case "uint": case "uint32": return typeofUInt32;
                case "ulong": case "uint64": return typeofUInt64;
                case "object": case "obj": return typeofObject;
                case "byte[]": case "bytes": return typeofByteArray;
            }

            return Type.GetType(typeName);
        }

        internal static Assembly urtAssembly = Assembly.Load("mscorlib");//Assembly.GetAssembly(Converter.typeofString);
        internal static Type typeofString = typeof(string);
        internal static Type typeofBoolean = typeof(bool);
        internal static Type typeofByte = typeof(byte);
        internal static Type typeofChar = typeof(char);
        internal static Type typeofDecimal = typeof(decimal);
        internal static Type typeofDouble = typeof(double);
        internal static Type typeofInt16 = typeof(short);
        internal static Type typeofInt32 = typeof(int);
        internal static Type typeofInt64 = typeof(long);
        internal static Type typeofSByte = typeof(sbyte);
        internal static Type typeofSingle = typeof(float);
        internal static Type typeofTimeSpan = typeof(TimeSpan);
        internal static Type typeofDateTime = typeof(DateTime);
        internal static Type typeofUInt16 = typeof(ushort);
        internal static Type typeofUInt32 = typeof(uint);
        internal static Type typeofUInt64 = typeof(ulong);

        internal static Type typeofObject = typeof(object);  //不是基本数据类型
        //internal static Type typeofSystemVoid = typeof(void);
        //internal static Type typeofTypeArray = typeof(Type[]);
        //internal static Type typeofObjectArray = typeof(object[]);
        //internal static Type typeofStringArray = typeof(string[]);
        //internal static Type typeofBooleanArray = typeof(bool[]);
        internal static Type typeofByteArray = typeof(byte[]);
        //internal static Type typeofCharArray = typeof(char[]);
        //internal static Type typeofDecimalArray = typeof(decimal[]);
        //internal static Type typeofDoubleArray = typeof(double[]);
        //internal static Type typeofInt16Array = typeof(short[]);
        //internal static Type typeofInt32Array = typeof(int[]);
        //internal static Type typeofInt64Array = typeof(long[]);
        //internal static Type typeofSByteArray = typeof(sbyte[]);
        //internal static Type typeofSingleArray = typeof(float[]);
        //internal static Type typeofTimeSpanArray = typeof(TimeSpan[]);
        //internal static Type typeofDateTimeArray = typeof(DateTime[]);
        //internal static Type typeofUInt16Array = typeof(ushort[]);
        //internal static Type typeofUInt32Array = typeof(uint[]);
        //internal static Type typeofUInt64Array = typeof(ulong[]);

        internal static List<Type> metaTypes = new List<Type>
                                                   {
                                                       typeofString,
                                                       typeofBoolean,
                                                       typeofByte,
                                                       typeofChar,
                                                       typeofDecimal,
                                                       typeofDouble,
                                                       typeofInt16,
                                                       typeofInt32,
                                                       typeofInt64,
                                                       typeofSByte,
                                                       typeofSingle,
                                                       typeofTimeSpan,
                                                       typeofDateTime,
                                                       typeofUInt16,
                                                       typeofUInt32,
                                                       typeofUInt64,
                                                       //typeofObject,
                                                       //typeofSystemVoid,
                                                       //typeofTypeArray,
                                                       //typeofObjectArray,
                                                       //typeofStringArray,
                                                       //typeofBooleanArray,
                                                       typeofByteArray,
                                                       //typeofCharArray,
                                                       //typeofDecimalArray,
                                                       //typeofDoubleArray,
                                                       //typeofInt16Array,
                                                       //typeofInt32Array,
                                                       //typeofInt64Array,
                                                       //typeofSByteArray,
                                                       //typeofSingleArray,
                                                       //typeofTimeSpanArray,
                                                       //typeofDateTimeArray,
                                                       //typeofUInt16Array,
                                                       //typeofUInt32Array,
                                                       //typeofUInt64Array
                                                   };

        #endregion

        public static object ConvertValue(object obj, Type type)
        {
            if (type == null || obj == null || type == typeofObject) return obj;

            Type objType = obj.GetType();
            if (objType == type || type.IsAssignableFrom(objType)) return obj;

            try
            {
#if (!WindowsCE && !PocketPC)
                object newResult = Convert.ChangeType(obj, type);
                return newResult;
#endif

#if (WindowsCE || PocketPC)

                #region  转换类型
                if (type == typeofBoolean)
                    return Convert.ToBoolean(obj);
                if (type == typeofChar)
                    return Convert.ToChar(obj);
                if (type == typeofSByte)
                    return Convert.ToSByte(obj);
                if (type == typeofByte)
                    return Convert.ToByte(obj);
                if (type == typeofInt16)
                    return Convert.ToInt16(obj);
                if (type == typeofUInt16)
                    return Convert.ToUInt16(obj);
                if (type == typeofInt32)
                    return Convert.ToInt32(obj);
                if (type == typeofUInt32)
                    return Convert.ToUInt32(obj);
                if (type == typeofInt64)
                    return Convert.ToInt64(obj);
                if (type == typeofUInt64)
                    return Convert.ToUInt64(obj);
                if (type == typeofSingle)
                    return Convert.ToSingle(obj);
                if (type == typeofDouble)
                    return Convert.ToDouble(obj);
                if (type == typeofDecimal)
                    return Convert.ToDecimal(obj);
                if (type == typeofDateTime)
                    return Convert.ToDateTime(obj);
                if (type == typeofString)
                    return Convert.ToString(obj);
                if (type == typeofObject)
                    return obj;
                #endregion
                return null;
#endif
            }
            catch (Exception) { /*return DefaultForType(type); */return obj; }
        }


        #endregion

        #region  获 取 成 员

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            if (type == null || StringExtend.IsNullOrWhiteSpace(propertyName)) return null;

            PropertyInfo propertyInfo = type.GetProperty(propertyName, Property_Field_BindingFlags);
            return propertyInfo;
        }
        public static List<PropertyInfo> GetPropertyInfos(Type type)
        {
            if (type == null) return null;
            PropertyInfo[] propertyInfos = type.GetProperties(Property_Field_BindingFlags);
            List<PropertyInfo> listProperty = new List<PropertyInfo>();
            listProperty.AddRange(propertyInfos);
            return listProperty;
        }

        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            if (type == null || StringExtend.IsNullOrWhiteSpace(fieldName)) return null;

            FieldInfo fieldInfo = type.GetField(fieldName, Property_Field_BindingFlags);
            return fieldInfo;
        }
        public static List<FieldInfo> GetFieldInfos(Type type)
        {
            if (type == null) return null;
            FieldInfo[] fieldInfos = type.GetFields(Property_Field_BindingFlags);
            List<FieldInfo> listField = new List<FieldInfo>();
            listField.AddRange(fieldInfos);
            return listField;
        }

        #endregion




        [Serializable]
        internal enum SetValueTypeEnum { None, Reflection, Emit }
        [Serializable]
        internal enum GetValueTypeEnum { None, Reflection, Emit }
        internal delegate void SetValueDelegate(object target, object value);
        internal delegate void StaticSetValueDelegate(object value);
        internal delegate object GetValueDelegate(object target);
        internal delegate object StaticGetValueDelegate();
    }
}
