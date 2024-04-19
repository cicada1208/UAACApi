using System;
using System.Collections.Generic;

namespace Params
{
    public class DBParam
    {
        /// <summary>
        /// DB類型
        /// </summary>
        public enum DBType
        {
            SYBASE = 1,
            SQLSERVER = 2,
            ORACLE = 3,
        }

        /// <summary>
        /// DB名稱
        /// </summary>
        public class DBName
        {
            public const string NIS = "NIS";
            public const string SYB1 = "SYB1";
            public const string SYB2 = "SYB2";
            public const string PeriExam = "PeriExam";
            public const string UAAC = "UAAC";
            public const string MISSYS = "MISSYS";
        }

        public static readonly Dictionary<Type, string> ColumnTypeAliases = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(byte[]), "byte[]" },
            { typeof(short), "short" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(bool), "bool" },
            { typeof(string), "string" }
        };

        public static readonly HashSet<Type> ColumnNullableTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime)
        };
    }
}
