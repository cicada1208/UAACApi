using Params;
using System;

namespace Lib.Attributes
{
    /// <summary>
    /// mapped model to table
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false), Serializable]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// DB類型
        /// </summary>
        public DBParam.DBType DBType;
        /// <summary>
        /// DB名稱
        /// </summary>
        public string DBName;
        /// <summary>
        /// 表格名稱
        /// </summary>
        public string TableName;

        public TableAttribute(DBParam.DBType dbType, string dbName, string tbName = "")
        {
            DBType = dbType;
            DBName = dbName;
            TableName = tbName;
        }
    }
}
