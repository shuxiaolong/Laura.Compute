using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Laura.Compute.Test
{
    public class Program 
    {
        private const string SplitLine = "--------------------------------------------------";
        const string LogoExpress = "REPLACE('Laura.Compute - Love Java.', 'Love Java.', 'Love C#.')";

        public static void Main()
        {
            do
            {
                string input = ShowDesktop();
                if (input == "1") TestBaseExpress.StartTest();
                else if (input == "2") TestObjectExpress.StartTest();
                else if (input == "3") TestCustomExpress.StartTest();
                else if (input == "4") break;
                else Console.Clear();
            } while (true); 




            //分析一个 表达式，得到 表达式结构 对象
            ExpressSchema expressSchema3 = ExpressSchema.Create("\"ShuXiaolong\" IN (\"ShuXiaolong\",\"JiangXiaoya\")");
            //给定参数，计算这个 表达式结构 对象 在指定参数下 的运行结果
            object value = expressSchema3.Compute(null);
            Console.WriteLine(value);


            TestNormalExpress01();
            TestSingleArgumentExpress02();
            TestNormalExpress03();
            TestNormalExpress04();

            TestWordFilter();
            TestWordSort();
            TestNewGuidMethod();
            TestDateMethod();

            TestObject();
            TestSort();
            TestFilter();

            Console.WriteLine("\r\n\r\n执行已经结束！");
            Console.ReadKey();
        }

        public static string ShowDesktop()
        {
            ExpressSchema logoExpressSchema = ExpressSchema.Create(LogoExpress);
            object logoExpressValue = logoExpressSchema.Compute(null);
            Console.WriteLine("Logo Express: \t{0}\r\nExpress Value: \t{1}", LogoExpress, logoExpressValue);

            Console.WriteLine(SplitLine);

            Console.WriteLine("  1: Test BaseExpress.   (测试 简单 表达式)");
            Console.WriteLine("  2: Test ObjectExpress. (测试 对象 表达式)");
            Console.WriteLine("  3: Test CustomExpress. (测试 自定义 表达式)");
            Console.WriteLine("  4: Exit.               (退出)");

            Console.WriteLine(SplitLine);
            Console.Write("Input Choice:");

            string input = Console.ReadLine();
            return input;
        }

        public static void TestNormalExpress01()
        {
            Console.WriteLine("\r\n\r\nTestNormalExpress01");
            Console.WriteLine("---------------------------------------");

            object result2 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema2 = ExpressSchema.Create("\"AABBCC\" LIKE \"%BB%\"");
                return expressSchema2.Compute(null);
            });
            Console.WriteLine(result2);


            object result3 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema3 = ExpressSchema.Create("\"ShuXiaolong\" IN (\"ShuXiaolong\",\"JiangXiaoya\")");
                return expressSchema3.Compute(null);
            });
            Console.WriteLine(result3);


            object result4 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema4 = ExpressSchema.Create("LEN(\"ShuXiaolong\")");
                return expressSchema4.Compute(null);
            });
            Console.WriteLine(result4);


            object result5 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema5 = ExpressSchema.Create("4*3/6*9/2"); //9
                return expressSchema5.Compute(null);
            });
            Console.WriteLine(result5);


            //double doubleValue = Convert.ToDouble("-0.000021323E+12");
            ////decimal decimalValue = Convert.ToDecimal("-0.21323E+2");  //decimal 不支持E表达式
            //Console.WriteLine(doubleValue);
            ////Console.WriteLine(decimalValue);
        }
        public static void TestSingleArgumentExpress02()
        {
            Console.WriteLine("\r\n\r\nTestSingleArgumentExpress02");
            Console.WriteLine("---------------------------------------");

            object value = ComputeHelper.ComputePropertyValue(new List<string> { "ShuXiaolong", "JiangXiaoya" }, "1");
            Console.WriteLine(value);
            value = ComputeHelper.ComputePropertyValue(new MyIndex(), "1");
            Console.WriteLine(value);
            value = ComputeHelper.ComputePropertyValue(new MyIndex(), "SXL");
            Console.WriteLine(value);
        }
        public static void TestNormalExpress03()
        {
            Console.WriteLine("\r\n\r\nTestNormalExpress03");
            Console.WriteLine("---------------------------------------");

            object result = ActionCounter(20000, () =>
            {
                string sTemp = string.Empty;
                sTemp += ComputeHelper.Compute("(2+8)*(1+2+77+(12+8))") + "\r\n";
                sTemp += ComputeHelper.Compute("345*657") + "\r\n";
                sTemp += ComputeHelper.Compute("2+(235*(2+3))") + "\r\n";
                sTemp += ComputeHelper.Compute("123+65+234+132+432") + "\r\n";
                sTemp += ComputeHelper.Compute("1+1+1+1+1") + "\r\n";
                sTemp += ComputeHelper.Compute("123*654*907") + "\r\n";
                sTemp += ComputeHelper.Compute("(1234+987)*765") + "\r\n";
                sTemp += ComputeHelper.Compute("7868*989+5678") + "\r\n";
                sTemp += ComputeHelper.Compute("1234*546") + "\r\n";
                sTemp += ComputeHelper.Compute("(122+5654)*(2+976)") + "\r\n";
                sTemp += ComputeHelper.Compute("100-50+50-50") + "\r\n";
                sTemp += ComputeHelper.Compute("False?111:222") + "\r\n";
                sTemp += ComputeHelper.Compute("11111===11111") + "\r\n";
                sTemp += ComputeHelper.Compute("(11111===2222)?111:222") + "\r\n";
                sTemp += ComputeHelper.Compute("10-1+2-3+4-5+6") + "\r\n";
                sTemp += ComputeHelper.Compute("10-1*2-3*4-5/6") + "\r\n";
                sTemp += ComputeHelper.Compute("10-1*2-3*4-5/6") + "\r\n";
                sTemp += ComputeHelper.Compute("\"QWERTYUIOP{}:\" LIKE \"%ERT%U%\"") + "\r\n";
                sTemp += ComputeHelper.Compute("(\"QWERTYUIOP{}:\" LIKE \"%ERT%U\")?1111+1111:2222+2222") + "\r\n";
                sTemp += ComputeHelper.Compute("-2+5*(-2+7)") + "\r\n";
                sTemp += ComputeHelper.Compute("(REPLACE(REPLACE(\"AAAAAAAAKKK\",\"K\",\"M\"),\"A\",\"B\") == \"BBBBBBBBMMM\")?\"HHHHHHHHH\":\"IIIIIIIIII\"") + "\r\n";
                sTemp += ComputeHelper.Compute("3^4") + "\r\n";
                sTemp += ComputeHelper.Compute("81^(1/4)") + "\r\n";
                return sTemp;
            });

            Console.WriteLine(result);
        }
        public static void TestNormalExpress04()
        {
            Console.WriteLine("\r\n\r\nTestNormalExpress04");
            Console.WriteLine("---------------------------------------");

            object result = ActionCounter(20000, () =>
            {
                object value = ComputeHelper.Compute("LEN(\"ShuXiaolong\"+\"JiangXiaoya\")");                                         
                return value;
            });

            Console.WriteLine(result);
        }

        public static void TestWordFilter()
        {
            Console.WriteLine("\r\n\r\nTestWordFilter");
            Console.WriteLine("---------------------------------------");


            DataSet dataSet = GetTableRecord();
            DataTable dataWord = dataSet.Tables[0];

            DateTime dt1 = DateTime.Now;
            DataRow[] dataRows = dataWord.Select("Word LIKE '%cat%'");
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("DataTable检索时间:" + (dt2 - dt1).TotalMilliseconds);

            ExpressSchema filter = ExpressSchema.Create("[Word] LIKE '%cat%'");
            List<DataRow> listDataRow = new List<DataRow>();

            DateTime dt3 = DateTime.Now;
            foreach (DataRow dataRow in dataWord.Rows)
            {
                object value = filter.Compute(dataRow);
                if (Equals(value, true))
                    listDataRow.Add(dataRow);
            }
            DateTime dt4 = DateTime.Now;
            Console.WriteLine("内存检索时间:" + (dt4 - dt3).TotalMilliseconds);

            Console.WriteLine(dataRows + "|" + listDataRow);
        }
        public static void TestWordSort()
        {
            Console.WriteLine("\r\n\r\nTestWordSort");
            Console.WriteLine("---------------------------------------");

            DataSet dataSet = GetTableRecord();
            DataTable dataWord = dataSet.Tables[0];
            DateTime dt1 = DateTime.Now;
            DataRow[] dataRows = dataWord.Select(string.Empty, "[Word],[Comment]");  //DESC 倒序 //ASC 正序 //默认 ASC
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("DataTable排序时间:" + (dt2 - dt1).TotalMilliseconds);

             //List<DataRow> listDataRow = new List<DataRow>();
             //foreach (DataRow dataRow in dataWord.Rows)
             //    listDataRow.Add(dataRow);

            DateTime dt3 = DateTime.Now;
            SortSchema sortSchema = ComputeHelper.InitSortSchema("[Word],[Comment]", true);
            IList listResult = sortSchema.Sort(dataWord.Rows);
            DateTime dt4 = DateTime.Now;
            Console.WriteLine("内存排序时间:" + (dt4 - dt3).TotalMilliseconds);

            DateTime dt5 = DateTime.Now;
            SortSchema sortSchema2 = ComputeHelper.InitSortSchema("[Word],[Comment]", true);
            IList listResult2;
            using (SortExecuter sortExecuter = SortExecuter.Create(sortSchema2, dataWord.Rows))
            {
                listResult2 = sortExecuter.Sort();
            }
            DateTime dt6 = DateTime.Now;
            Console.WriteLine("内存SortExecuter排序时间:" + (dt6 - dt5).TotalMilliseconds);

            
            DateTime dt7 = DateTime.Now;
            IList listResult3 = ComputeHelper.Sort("[Word],[Comment]", dataWord.Rows);
            DateTime dt8 = DateTime.Now;
            Console.WriteLine("ComputeHelper.Sort()排序时间:" + (dt8 - dt7).TotalMilliseconds);

            Console.WriteLine(dataRows + "|" + listResult + "|" + listResult2 + "|" + listResult3);
        }
        public static void TestNewGuidMethod()
        {
            Console.WriteLine("\r\n\r\nTestNewGuidMethod");
            Console.WriteLine("---------------------------------------");

            object result = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema = ExpressSchema.Create(" NEWID()");                 //随机产生一个 Guid 
                object expressValue = expressSchema.Compute(null);
                //Console.WriteLine(expressValue);  //控制台输出会 浪费时间
                return expressValue;
            });

            object result2 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema2 = ExpressSchema.Create(" NEWID() LIKE '%ABC%'");   //随机产生一个 Guid 且 模糊匹配
                object expressValue = expressSchema2.Compute(null);
                //Console.WriteLine(expressValue);  //控制台输出会 浪费时间
                return expressValue;
            });
            Console.WriteLine(result + "|" + result2);




        }
        public static void TestDateMethod()
        {
            Console.WriteLine("\r\n\r\nTestDateMethod");
            Console.WriteLine("---------------------------------------");

            object result = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema = ExpressSchema.Create(" DATEADD('YEAR',2,GETDATE())");                 //当前时间加2年
                object expressValue = expressSchema.Compute(null);
                //Console.WriteLine(expressValue);  //控制台输出会 浪费时间
                return expressValue;
            });

            object result2 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema2 = ExpressSchema.Create(" DATEDIFF ('YEAR','1989-11-27',GETDATE())");   //获取两个时间 年份差值
                object expressValue = expressSchema2.Compute(null);
                //Console.WriteLine(expressValue);  //控制台输出会 浪费时间
                return expressValue;
            });

            object result3 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema2 = ExpressSchema.Create(" DATEPART ('YEAR',GETDATE())");   //获取两个时间 年份差值
                object expressValue = expressSchema2.Compute(null);
                //Console.WriteLine(expressValue);  //控制台输出会 浪费时间
                return expressValue;
            });

            object result4 = ActionCounter(20000, () =>
            {
                ExpressSchema expressSchema2 = ExpressSchema.Create(" DATEPART ('WEEK',GETDATE())");   //获取两个时间 年份差值
                object expressValue = expressSchema2.Compute(null);
                //Console.WriteLine(expressValue);  //控制台输出会 浪费时间
                return expressValue;
            });

            Console.WriteLine(result + "|" + result2 + "|" + result3 + "|" + result4);




        }

        public static void TestObject()
        {
            Student stu01 = new Student { Name = "舒小龙", Number = "ShuXiaolong"};
            Student stu02 = new Student { Name = "江小雅", Number = "JiangXiaoya" };
            Student stu03 = new Student { Name = "舒珊", Number = "ShuShan" };

            ExpressSchema expressSchema = ExpressSchema.Create("[Number] LIKE '%Shu%'");
            bool result1_1 = (bool) expressSchema.Compute(stu01);
            bool result1_2 = (bool)expressSchema.Compute(stu02);
            bool result1_3 = (bool)expressSchema.Compute(stu03);
            Console.WriteLine(result1_1 + "|" + result1_2 + "|" + result1_3);

            ExpressSchema expressSchema2 = ExpressSchema.Create("[Name] + [Number]");
            string result2_1 = (string)expressSchema2.Compute(stu01);
            string result2_2 = (string)expressSchema2.Compute(stu02);
            string result2_3 = (string)expressSchema2.Compute(stu03);
            Console.WriteLine(result2_1 + "|" + result2_2 + "|" + result2_3);
        }
        public static void TestSort()
        {
            DataSet dataSet = GetTableRecord();  //从数据库 读取 50000 个单词
            DataTable dataWord = dataSet.Tables[0];

            DateTime dt7 = DateTime.Now;
            IList listResult3 = ComputeHelper.Sort("[Word],[Comment]", dataWord.Rows);  //用封装好的 排序函数 排序
            DateTime dt8 = DateTime.Now;
            Console.WriteLine("ComputeHelper.Sort()排序时间:" + (dt8 - dt7).TotalMilliseconds);

            Console.WriteLine(listResult3);
        }
        public static void TestFilter()
        {
            DataSet dataSet = GetTableRecord();  //从数据库 读取 50000 个单词
            DataTable dataWord = dataSet.Tables[0];

            DateTime dt7 = DateTime.Now;
            IList listResult3 = ComputeHelper.Filter("[Word] LIKE '%cat%'", dataWord.Rows);  //用封装好的 排序函数 排序
            DateTime dt8 = DateTime.Now;
            Console.WriteLine("ComputeHelper.Filter()筛选时间:" + (dt8 - dt7).TotalMilliseconds);

            Console.WriteLine(listResult3 + " 数目：" + listResult3.Count);
        }


        private static DataSet GetTableRecord()
        {
            DataSet ds = new DataSet("Test");
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=MyWord;User Id=sa; Pwd=123.com;"))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM [Get]; SELECT COUNT(*) FROM [Get] ", conn))
                {
                    adapter.Fill(ds);
                }
            }
            return ds;
        }



        public static object ActionCounter(int count, Action action)
        {
            object result = null;
            DateTime dt1 = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                result = action();
            }
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("执行:" + count + "次用时 " + (dt2 - dt1).TotalMilliseconds + " 毫秒");
            return result;
        }

    }







    public delegate object Action();

    public class MyIndex
    {
        public string this[string index]
        {
            get {
                if (index == "SXL") return "ShuXiaolong";
                else if (index == "QFL") return "JiangXiaoya";
                return string.Empty;
            }
        }

        public string this[int index]
        {
            get
            {
                if (index == 0) return "ShuXiaolong";
                else if (index == 1) return "JiangXiaoya";
                return string.Empty;
            }
        }
    }

    public class Student
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }


}
