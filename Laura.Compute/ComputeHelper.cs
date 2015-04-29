using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Laura.Compute.Utils;

namespace Laura.Compute
{
    public static class ComputeHelper
    {
        private static readonly Hashtable cacheExpress = new Hashtable();
        private static readonly Hashtable cacheSort = new Hashtable();



        /// <summary>
        /// 计算一个 对象 的 指定属性名称的 值
        /// </summary>
        public static object ComputePropertyValue(object objOrHash, string propertyName)
        {
            if (objOrHash == null || StringExtend.IsNullOrWhiteSpace(propertyName)) return objOrHash;

            //取值 方式：
            //如果 对象是 IDictionary
            //如果 对象是 DataRowView
            //如果 对象是 DataRow
            //如果 对象是 普通对象
            //  如果 参数是 普通属性名
            //  如果 参数是 普通字段名
            //  如果 参数是 文本索引器
            //  如果 参数是 数字索引器


            IDictionary dictionary = objOrHash as IDictionary;
            if (dictionary != null)
            {
                //从 IDictionary 中取值
                return dictionary.Contains(propertyName) ? dictionary[propertyName] : dictionary;
            }
            else
            {
                DataRow dataRow = objOrHash as DataRow;
                if (dataRow == null)
                {
                    DataRowView dataRowView = objOrHash as DataRowView;
                    if (dataRowView != null)
                        dataRow = dataRowView.Row;
                }

                if (dataRow != null)
                {
                    //从 DataRow 中取值
                    return dataRow.IsNull(propertyName) ? dataRow : dataRow[propertyName];
                }
                else
                {
                    #region  从 属性 或者 字段 或者 索引器 中取值

                    Type type = objOrHash.GetType();

                    PropertyInfo property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                    if (property != null)
                    {
                        //从 属性 中取值
                        try
                        {
                            object value = property.GetValue(objOrHash, null);
                            return value;
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        FieldInfo field = type.GetField(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                        if (field != null)
                        {
                            try
                            {
                                //从 字段 中取值
                                object value = field.GetValue(objOrHash);
                                return value;
                            }
                            catch (Exception) { }
                        }

                        #region  从 索引器 中取值

                        else
                        {
                            try
                            {
                                int index = 0;
                                if (int.TryParse(propertyName, out index))
                                {
                                    MethodInfo indexMethod = type.GetMethod("get_Item", new[] { typeof(int) });
                                    return indexMethod.Invoke(objOrHash, new object[] { index });
                                }
                                else
                                {
                                    MethodInfo indexMethod = type.GetMethod("get_Item", new[] { typeof(string) });
                                    return indexMethod.Invoke(objOrHash, new object[] { propertyName });
                                }
                            }
                            catch (Exception exp) { }
                        }

                        #endregion
                    }

                    #endregion


                }
            }

            return objOrHash;
        }


        /// <summary>
        /// 计算一个表达式，对于已有 表达式 则默认直接使用缓存的 ExpressSchema
        /// </summary>
        public static object Compute(string express)
        {
            return Compute(express, true);
        }
        /// <summary>
        /// 计算一个表达式，需要手动指定 是否 需要缓存 ExpressSchema
        /// </summary>
        public static object Compute(string express, bool withCache)
        {
            ExpressSchema expressSchema = InitExpressSchema(express, withCache);
            if (expressSchema != null)
            {
                object value = expressSchema.Compute(null);
                return value;
            }
            return null;
        }
        /// <summary>
        /// 计算一个表达式，对于已有 表达式 则默认直接使用缓存的 ExpressSchema
        /// </summary>
        public static object Compute(string express, object objOrHash)
        {
            return Compute(express, objOrHash, true);
        }
        /// <summary>
        /// 计算一个表达式，需要手动指定 是否 需要缓存 ExpressSchema withCache：注意 参数 withCache 只缓存 结构对象，并不缓存计算结果(每次执行 都会重复计算结果)
        /// </summary>
        public static object Compute(string express, object objOrHash, bool withCache)
        {
            ExpressSchema expressSchema = InitExpressSchema(express, withCache);
            if (expressSchema != null)
            {
                object value = expressSchema.Compute(objOrHash);
                return value;
            }
            return null;
        }


        /// <summary>
        /// 对一个集合排序(返回排序后的新集合对象)，对于已有 表达式 则默认直接使用缓存的 SortSchema
        /// </summary>
        public static ArrayList Sort(string sortExpress, ICollection collection)
        {
            return Sort(sortExpress, collection, true);
        }
        /// <summary>
        /// 对一个集合排序(返回排序后的新集合对象)，需要手动指定 是否 需要缓存 SortSchema withCache：注意 参数 withCache 只缓存 结构对象，并不缓存排序结果(每次执行 都会重复计算排序结果)
        /// </summary>
        public static ArrayList Sort(string sortExpress, ICollection collection, bool withCache)
        {
            SortSchema sortSchema = InitSortSchema(sortExpress, withCache);
            using (SortExecuter sortExecuter = SortExecuter.Create(sortSchema, collection))
            {
                ArrayList result = sortExecuter.Sort();
                return result;
            }
        }
        /// <summary>
        /// 对一个集合排序(对集合本身执行排序)，对于已有 表达式 则默认直接使用缓存的 SortSchema
        /// </summary>
        public static void SortList<T>(string sortExpress, List<T> list)
        {
            SortList<T>(sortExpress, list, true);
        }
        /// <summary>
        /// 对一个集合排序(对集合本身执行排序)，需要手动指定 是否 需要缓存 SortSchema withCache：注意 参数 withCache 只缓存 结构对象，并不缓存排序结果(每次执行 都会重复计算排序结果)
        /// </summary>
        public static void SortList<T>(string sortExpress, List<T> list, bool withCache)
        {
            SortSchema sortSchema = InitSortSchema(sortExpress, withCache);
            using (SortExecuter<T> sortExecuter = SortExecuter<T>.Create(sortSchema, list))
            {
                sortExecuter.Sort();
            }
        }


        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static ArrayList Filter(string filterExpress, ICollection collection)
        {
            return Filter(filterExpress, collection, true);
        }
        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static ArrayList Filter(string filterExpress, ICollection collection, bool withCache)
        {
            ExpressSchema filterSchema = InitExpressSchema(filterExpress, withCache);

            ArrayList arrayList = new ArrayList();
            foreach (object item in collection)
            {
                object value = filterSchema.Compute(item);
                if (Equals(value, true)) arrayList.Add(item);
            }
            return arrayList;
        }
        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static List<T> Filter<T>(string filterExpress, IList<T> list)
        {
            return Filter<T>(filterExpress, list, true);
        }
        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static List<T> Filter<T>(string filterExpress, IList<T> list, bool withCache)
        {
            ExpressSchema filterSchema = InitExpressSchema(filterExpress, withCache);

            List<T> listT = new List<T>();
            foreach (T item in list)
            {
                object value = filterSchema.Compute(item);
                if (Equals(value, true)) listT.Add(item);
            }
            return listT;
        }


        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static ArrayList Select(string filterExpress, string sortExpress, ICollection collection)
        {
            return Select(filterExpress, sortExpress, collection, true);
        }
        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static ArrayList Select(string filterExpress, string sortExpress, ICollection collection, bool withCache)
        {
            ArrayList filterArrayList = Filter(filterExpress, collection, withCache);
            ArrayList sortArrayList = Sort(sortExpress, filterArrayList, withCache);

            filterArrayList.Clear();
            return sortArrayList;
        }
        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static List<T> Select<T>(string filterExpress, string sortExpress, IList<T> list)
        {
            return Select<T>(filterExpress, sortExpress, list, true);
        }
        /// <summary>
        /// 对一个集合 进行过滤 并返回 新集合
        /// </summary>
        public static List<T> Select<T>(string filterExpress, string sortExpress, IList<T> list, bool withCache)
        {
            List<T> filterArrayList = Filter<T>(filterExpress, list, withCache);
            SortList<T>(sortExpress, filterArrayList, withCache);
            return filterArrayList;
        }




        /// <summary>
        /// 通过一个表达式 初始化 一个 表达式 的 结构对象
        /// </summary>
        public static ExpressSchema InitExpressSchema(string express, bool withCache)
        {
            if (StringExtend.IsNullOrWhiteSpace(express)) return null;

            ExpressSchema expressSchema = null;

            if (withCache)
            {
                try { expressSchema = cacheExpress[express] as ExpressSchema; }
                catch (Exception) { }
            }

            if (expressSchema == null)
            {
                expressSchema = ExpressSchema.Create(express);
                if (expressSchema != null && withCache)
                {
                    try
                    {
                        if (cacheExpress.Count >= MAX_CACHE_COUNT) cacheExpress.Clear();   //最大缓存 MAX_CACHE_COUNT
                        cacheExpress[express] = expressSchema;
                    }
                    catch (Exception) { }
                }
            }

            return expressSchema;
        }
        /// <summary>
        /// 通过一个表达式 初始化 一个 排序表达式 的 结构对象
        /// </summary>
        public static SortSchema InitSortSchema(string sortExpress, bool withCache)
        {
            if (StringExtend.IsNullOrWhiteSpace(sortExpress)) return null;

            SortSchema sortSchema = null;

            if (withCache)
            {
                try { sortSchema = cacheSort[sortExpress] as SortSchema; }
                catch (Exception) { }
            }

            if (sortSchema == null)
            {
                sortSchema = SortSchema.Create(sortExpress);
                if (sortSchema != null && withCache)
                {
                    try
                    {
                        if (cacheExpress.Count >= MAX_CACHE_COUNT) cacheExpress.Clear();   //最大缓存 MAX_CACHE_COUNT
                        cacheSort[sortExpress] = sortSchema;
                    }
                    catch (Exception) { }
                }
            }

            return sortSchema;
        }

        private const int MAX_CACHE_COUNT = 200000;

    }
}
