using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Laura.Compute.Utils
{

    /// <summary>
    /// 操作 程序集 的类；
    /// 2013-03-10 不再提供动态编译；
    /// 本类中的所有函数，不再使用缓存，对本类的操作的结果如果不进行缓存，可能导致性能低下；
    /// </summary>
    internal static class AssemblyHelper
    {
        private static readonly object listAssemblyLocker = new object();
        private static readonly List<Assembly> listAssembly = new List<Assembly>();

        #region  当 前 所 有 程 序 集

        /// <summary>
        /// 获取程序加载的 程序集（需要提前调用 Load 函数加载）
        /// </summary>
        public static Assembly[] GetCurrentAssemblies()
        {
            List<Assembly> list = new List<Assembly>();
            if (listAssembly != null && listAssembly.Count >= 1)
                lock (listAssemblyLocker)
                {
                    foreach (Assembly assembly in listAssembly)
                        if (!list.Contains(assembly))
                            list.Add(assembly);
                }

#if (!WindowsCE && !PocketPC)
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblyArray)
                if (!list.Contains(assembly))
                    list.Add(assembly);

            string appDirectory = Tools.AppDirectory;
            string[] dllFiles = Directory.GetFiles(appDirectory, "*.dll");
            lock (listAssemblyLocker)
            {
                foreach (string dllFile in dllFiles)
                {
                    Assembly assembly = AssemblyHelper.Load(Path.GetFileNameWithoutExtension(dllFile));
                    if (!list.Contains(assembly))
                        list.Add(assembly);
                }
            }
#endif

            return list.ToArray();
        }

        /// <summary>
        /// 往当前程序中添加程序集
        /// </summary>
        public static Assembly Load(string typeName)
        {
            Assembly assembly = Assembly.Load(typeName);
            if (assembly != null && !listAssembly.Contains(assembly))
                listAssembly.Add(assembly);

            return assembly;
        }

        #endregion

        
        #region  特 性 高 级 用 法

        /// <summary>
        /// 查找所有程序集，获取 所有指定的类特性 T 的 类Type；
        /// </summary>
        public static Dictionary<Type, T> GetAttributes<T>() where T : Attribute
        {
            Assembly[] allAssemblys = GetCurrentAssemblies();
            return GetAttributes<T>(allAssemblys);
        }

        /// <summary>
        /// 查找指定程序集，获取 所有指定的类特性 T 的 类Type；
        /// </summary>
        public static Dictionary<Type, T> GetAttributes<T>(IEnumerable<Assembly> allAssemblys) where T : Attribute
        {
            Dictionary<Type, T> list = new Dictionary<Type, T>();

            if (allAssemblys!=null)
                foreach (Assembly assembly in allAssemblys/*listAssembly*/)
                {
                    try
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsDefined(typeof(T), false))
                            {
                                Attribute attribute = Attribute.GetCustomAttribute(type, typeof(T), false);
                                if (attribute != null)
                                {
                                    T wtattri = (T)attribute;
                                    list.Add(type, wtattri);
                                }
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        string logMsg = string.Format("Laura.Compute.Utils.AssemblyHelper.GetAttributes<T>() 反射程序集 {0} 时发生错误,请确保程序集文件是正确的,Laura.Serialization 将跳过该程序集:\r\n{1}", assembly.FullName, exp);
                        Tools.WarnLog(logMsg, "Logs/Laura.Compute/WarnLog/");
                    }
                }

            return list;
        }

        #endregion


    }
}
