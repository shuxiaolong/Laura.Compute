using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Laura.Compute.Utils
{
    internal static class Tools
    {

        #region  类 型 转 换

        /// <summary>
        /// 将一个 Type字符串 转换成 Type
        /// </summary>
        public static Type FromTypeName(string typeName)
        {
            return FromTypeName(typeName, null);
        }
        /// <summary>
        /// 将一个 Type字符串 转换成 Type
        /// </summary>
        public static Type FromTypeName(string typeName, Type defaultValue)
        {
            if (string.IsNullOrEmpty(typeName)) return defaultValue;
            Type type = Type.GetType(typeName);
            return type;
        }
        /// <summary>
        /// 将一个 Type 转换成 Type字符串
        /// </summary>
        public static string ToTypeName(Type type)
        {
            if (type == null) return string.Empty;
            string typeName = type.AssemblyQualifiedName;
            return typeName;
        }

        #endregion


        #region  数 据 转 换

        public static int ToInt(object obj, int defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is int) return (int)obj;
#if (!WindowsCE && !PocketPC)
            int temp;
            if (int.TryParse(obj.ToString(), out temp))
                return temp;
            else
                return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToInt32(obj); }
            catch { return defaultValue; }
#endif
        }

        public static double ToDouble(object obj, double defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is double) return (double)obj;
#if (!WindowsCE && !PocketPC)
            double temp;
            if (double.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToDouble(obj); }
            catch { return defaultValue; }
#endif
        }
        public static float ToFloat(object obj, float defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is float) return (float)obj;
#if (!WindowsCE && !PocketPC)
            float temp;
            if (float.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToSingle(obj); }
            catch { return defaultValue; }
#endif
        }
        public static string ToString(object obj, string defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is string) return (string)obj;
            return obj.ToString();
        }
        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is DateTime) return (DateTime)obj;
#if (!WindowsCE && !PocketPC)
            DateTime temp;
            if (DateTime.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToDateTime(obj); }
            catch { return defaultValue; }
#endif
        }
        public static Guid ToGuid(object obj, Guid defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is Guid) return (Guid)obj;
            try { return new Guid(obj.ToString()); }
            catch { return defaultValue; }
        }
        public static byte ToByte(object obj, byte defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is byte) return (byte)obj;
#if (!WindowsCE && !PocketPC)
            byte temp;
            if (byte.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToByte(obj); }
            catch { return defaultValue; }
#endif
        }
        public static bool ToBoolean(object obj, bool defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is bool) return (bool)obj;
#if (!WindowsCE && !PocketPC)            
            bool temp;
            if (bool.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToBoolean(obj); }
            catch { return defaultValue; }
#endif
        }
        public static long ToLong(object obj, long defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is long) return (long)obj;
#if (!WindowsCE && !PocketPC)            
            long temp;
            if (long.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToInt64(obj); }
            catch { return defaultValue; }
#endif
        }
        public static char ToChar(object obj, char defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is char) return (char)obj;
#if (!WindowsCE && !PocketPC)            
            char temp;
            if (char.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToChar(obj); }
            catch { return defaultValue; }
#endif
        }
        public static decimal ToDecimal(object obj, decimal defaultValue)
        {
            if (obj == null) return defaultValue;
            if (obj is decimal) return (decimal)obj;
#if (!WindowsCE && !PocketPC)            
            decimal temp;
            if (decimal.TryParse(obj.ToString(), out temp))
                return temp;
            else return defaultValue;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToDecimal(obj); }
            catch { return defaultValue; }
#endif
        }

        public static int ToInt(object obj)
        {
            if (obj == null) return 0;
            if (obj is int) return (int)obj;
#if (!WindowsCE && !PocketPC)  
            int temp;
            int.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToInt32(obj); }
            catch { return 0; }
#endif
        }
        public static double ToDouble(object obj)
        {
            if (obj == null) return 0;
            if (obj is double) return (double)obj;
#if (!WindowsCE && !PocketPC)  
            double temp;
            double.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToDouble(obj); }
            catch { return 0; }
#endif
        }
        public static float ToFloat(object obj)
        {
            if (obj == null) return 0;
            if (obj is float) return (float)obj;
#if (!WindowsCE && !PocketPC) 
            float temp;
            float.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToSingle(obj); }
            catch { return 0; }
#endif
        }
        public static string ToString(object obj)
        {
            if (obj == null) return string.Empty;
            if (obj is string) return (string)obj;
            return obj.ToString();
        }
        public static DateTime ToDateTime(object obj)
        {
            if (obj == null) return DateTime.MinValue;
            if (obj is DateTime) return (DateTime)obj;
#if (!WindowsCE && !PocketPC) 
            DateTime temp;
            DateTime.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToDateTime(obj); }
            catch { return new DateTime(1900, 1, 1); }
#endif
        }
        public static Guid ToGuid(object obj)
        {
            if (obj == null) return Guid.Empty;
            if (obj is Guid) return (Guid)obj;
            try { return new Guid(obj.ToString()); }
            catch { return Guid.Empty; }
        }
        public static byte ToByte(object obj)
        {
            if (obj == null) return 0;
            if (obj is byte) return (byte)obj;
#if (!WindowsCE && !PocketPC) 
            byte temp;
            byte.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToByte(obj); }
            catch { return 0; }
#endif
        }
        public static bool ToBoolean(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
#if (!WindowsCE && !PocketPC) 
            bool temp;
            bool.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToBoolean(obj); }
            catch { return false; }
#endif
        }
        public static long ToLong(object obj)
        {
            if (obj == null) return 0;
            if (obj is long) return (long)obj;
#if (!WindowsCE && !PocketPC) 
            long temp;
            long.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToInt64(obj); }
            catch { return 0; }
#endif
        }
        public static char ToChar(object obj)
        {
            if (obj == null) return char.MinValue;
            if (obj is char) return (char)obj;
#if (!WindowsCE && !PocketPC) 
            char temp;
            char.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToChar(obj); }
            catch { return ' '; }
#endif

        }
        public static decimal ToDecimal(object obj)
        {
            if (obj == null) return 0;
            if (obj is decimal) return (decimal)obj;
#if (!WindowsCE && !PocketPC) 
            decimal temp;
            decimal.TryParse(obj.ToString(), out temp);
            return temp;
#endif
#if (WindowsCE || PocketPC)
            try { return Convert.ToDecimal(obj); }
            catch { return 0; }
#endif

        }

        #endregion


        #region  记 录 日 志

        public static readonly object objLogLocker = new object();

        public static void DebugLog(string logMsg, string dirName)
        {
            WriteLog(logMsg, dirName, LogType.Debug);
        }
        public static void WarnLog(string logMsg, string dirName)
        {
            WriteLog(logMsg, dirName, LogType.Warn);
        }
        public static void ErrorLog(string logMsg, string dirName)
        {
            WriteLog(logMsg, dirName, LogType.Error);
        }
        public static void InfoLog(string logMsg, string dirName)
        {
            WriteLog(logMsg, dirName, LogType.Info);
        }
        public static void FatalErrorLog(string logMsg, string dirName)
        {
            WriteLog(logMsg, dirName, LogType.FatalError);
        }
        public static void WriteLog(string logMsg, string dirName, LogType logType)
        {
            object obj;
            Monitor.Enter(obj = objLogLocker);
            try
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".config";
                if (string.IsNullOrEmpty(dirName)) dirName = "Logs";

                string logPathFormat = Tools.ToString(ConfigurationManager.AppSettings["LogPathFormat"]);
                string fullDirName = string.IsNullOrEmpty(logPathFormat)
                                         ? (AppDirectory + "/" + dirName)
                                         : logPathFormat.Replace("{Root}", AppDirectory) + "/" + dirName;

                if (!Directory.Exists(fullDirName)) Directory.CreateDirectory(fullDirName);

                string path = fullDirName + "/" + fileName;
                StreamWriter streamWriter = new StreamWriter(path, true);
                streamWriter.Write(logMsg + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff\r\n"));
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch (Exception) { }
            finally { Monitor.Exit(obj); }
        }

        #endregion


        #region  数 据 类 型 和 动 态 赋 值 取 值

        public static bool SetHashValue(IDictionary<string, object> hash, string key, object value)
        {
            try
            {
                if (hash != null && !string.IsNullOrEmpty(key))
                {
                    if (hash.ContainsKey(key)) hash[key] = value;
                    else hash.Add(key, value);
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }
        public static object GetHashValue(IDictionary<string, object> hash, string key)
        {
            try
            {
                if (hash != null && !string.IsNullOrEmpty(key) && hash.ContainsKey(key))
                    return hash[key];
                return null;
            }
            catch (Exception) { return null; }
        }
        public static bool SetDataRow(DataRow dataRow, string column, object value)
        {
            try
            {
                if (dataRow != null && !string.IsNullOrEmpty(column))
                {
                    //if (dataRow.Table.Columns.Contains(column))
                    //{
                    //    dataRow[column] = value;
                    //    return true;
                    //}
                    //else return false;

                    dataRow[column] = value;
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }
        public static object GetDataRow(DataRow dataRow, string column)
        {
            try
            {
                if (dataRow != null && !string.IsNullOrEmpty(column) /*&& dataRow.Table.Columns.Contains(column)*/)
                    return dataRow[column];
                return null;
            }
            catch (Exception) { return null; }
        }
        public static bool SetDataRowView(DataRowView dataRowView, string column, object value)
        {
            try
            {
                if (dataRowView != null && !string.IsNullOrEmpty(column))
                {
                    //if (dataRowView.Row.Table.Columns.Contains(column))
                    //{
                    //    dataRowView[column] = value;
                    //    return true;
                    //}
                    //else return false;

                    dataRowView[column] = value;
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }
        public static object GetDataRowView(DataRowView dataRowView, string column)
        {
            try
            {
                if (dataRowView != null && !string.IsNullOrEmpty(column) /*&& dataRowView.Row.Table.Columns.Contains(column)*/)
                    return dataRowView[column];
                return null;
            }
            catch (Exception) { return null; }
        }

        public static bool SetValue(object obj, string propertyName, object value)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName)) return false;
#if (!WindowsCE && !PocketPC)
            string[] propertyNames = propertyName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
#endif
#if (WindowsCE || PocketPC)
            string[] propertyNames = StringExtend.Split(propertyName, ".", StringSplitOptions.RemoveEmptyEntries);
#endif
            if (propertyNames == null || propertyNames.Length <= 0) return false;

            object tempResult = obj;
            int count = propertyNames.Length;
            for (int i = 0; i < count - 1; i++)
            {
                string property = propertyNames[i];
                TypeValue tempTypeValue = InnerGetValue(tempResult, property);
                if (tempTypeValue == null || tempTypeValue.Value == null) return false;
                else tempResult = tempTypeValue.Value;
            }
            return InnerSetValue(tempResult, propertyNames[count - 1], value);
        }
        public static TypeValue GetTypeValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName)) return null;
#if (!WindowsCE && !PocketPC)
            string[] propertyNames = propertyName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
#endif
#if (WindowsCE || PocketPC)
            string[] propertyNames = StringExtend.Split(propertyName, ".", StringSplitOptions.RemoveEmptyEntries);
#endif
            if (/*propertyNames == null || */propertyNames.Length <= 0) return null;

            TypeValue tempResult = new TypeValue(obj);
            int count = propertyNames.Length;
            for (int i = 0; i < count; i++)
            {
                string property = propertyNames[i];
                tempResult = InnerGetValue(tempResult.Value, property);
                if (tempResult == null || tempResult.Value == null) return null;
            }
            return tempResult;
        }
        public static object GetValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName)) return null;
#if (!WindowsCE && !PocketPC)
            string[] propertyNames = propertyName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
#endif
#if (WindowsCE || PocketPC)
            string[] propertyNames = StringExtend.Split(propertyName, ".", StringSplitOptions.RemoveEmptyEntries);
#endif
            if (/*propertyNames == null || */propertyNames.Length <= 0) return null;

            TypeValue tempResult = new TypeValue(obj);
            int count = propertyNames.Length;
            for (int i = 0; i < count; i++)
            {
                string property = propertyNames[i];
                tempResult = InnerGetValue(tempResult.Value, property);
                if (tempResult == null || tempResult.Value == null) return null;
            }
            return tempResult.Value;
        }
        public static T GetValue<T>(object obj, string propertyName)
        {
            TypeValue result = GetTypeValue(obj, propertyName);
            if (result == null || result.Value == null) return default(T);
            if (result.Value is T) return (T)result.Value;
            try
            {
                object value = ConvertValue(result.Value, typeof(T));
                return value is T ? (T)value : default(T);
            }
            catch (Exception) { return default(T); }
        }
        private static bool InnerSetValue(object obj, string propertyName, object value)
        {
            if (obj == null) return false;

            IDictionary<string, object> hash = obj as IDictionary<string, object>;
            if (hash != null) return SetHashValue(hash, propertyName, value);

            DataRow dataRow = obj as DataRow;
            if (dataRow != null) return SetDataRow(dataRow, propertyName, value);

            DataRowView dataRowView = obj as DataRowView;
            if (dataRowView != null) return SetDataRowView(dataRowView, propertyName, value);

            Type type = obj.GetType();
            PropertyInfo property = GetPropertyInfo(type, propertyName); //type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (property != null)
            {
                try
                {
                    object newValue = ConvertValue(value, property.PropertyType);
                    property.SetValue(obj, newValue, null); return true;
                }
                catch (Exception) { }
            }
            else
            {
                FieldInfo field = GetFieldInfo(type, propertyName); //type.GetField(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (field == null) return false;
                try
                {
                    object newValue = ConvertValue(value, field.FieldType);
                    field.SetValue(obj, newValue); return true;
                }
                catch (Exception) { }
            }
            return false;
        }
        private static TypeValue InnerGetValue(object obj, string propertyName)
        {
            if (obj == null) return null;

            IDictionary<string, object> hash = obj as IDictionary<string, object>;
            if (hash != null)
            {
                object value = GetHashValue(hash, propertyName);
                return new TypeValue(value, (value == null ? typeof(object) : value.GetType()));
            }

            DataRow dataRow = obj as DataRow;
            if (dataRow != null)
            {
                object value = GetDataRow(dataRow, propertyName);
                return new TypeValue(value, (value == null ? typeof(object) : value.GetType()));
            }

            DataRowView dataRowView = obj as DataRowView;
            if (dataRowView != null)
            {
                object value = GetDataRowView(dataRowView, propertyName);
                return new TypeValue(value, (value == null ? typeof(object) : value.GetType()));
            }

            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (property != null)
            {
                try
                {
                    object value = property.GetValue(obj, null);
                    return new TypeValue(value, property.PropertyType);
                }
                catch (Exception) { }
            }
            else
            {
                FieldInfo field = type.GetField(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (field == null) return null;
                try
                {
                    object value = field.GetValue(obj);
                    return new TypeValue(value, field.FieldType);
                }
                catch (Exception) { }
            }
            return null;
        }

        public static object ConvertValue(object obj, Type type)
        {
            return ReflectionHelper.ConvertValue(obj, type);
        }
        public static object DefaultForType(Type type)
        {
            if (type == null) return null;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }


        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            return ReflectionHelper.GetPropertyInfo(type, propertyName);
        }
        public static List<PropertyInfo> GetPropertyInfos(Type type)
        {
            return ReflectionHelper.GetPropertyInfos(type);
        }
        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            return ReflectionHelper.GetFieldInfo(type, fieldName);
        }
        public static List<FieldInfo> GetFieldInfos(Type type)
        {
            return ReflectionHelper.GetFieldInfos(type);
        }

        #endregion



        private static string appDirectory = string.Empty;
        /// <summary>
        /// 当前程序工作基本目录
        /// </summary>
        public static string AppDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(appDirectory))
                {
#if (!WindowsCE && !PocketPC)
                    appDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
#if (WindowsCE || PocketPC)
                    appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
#endif
                }
                return appDirectory;
            }
        }
    }

    [Serializable]
    internal enum LogType
    {
        /// <summary>
        /// 一般信息
        /// </summary>
        Info,
        /// <summary>
        /// 调试信息
        /// </summary>
        Debug,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 致命错误
        /// </summary>
        FatalError
    }
}
