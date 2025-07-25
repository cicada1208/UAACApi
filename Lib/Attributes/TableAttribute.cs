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
        /// 表格名稱
        /// </summary>
        public string TableName;

        /// <summary>
        /// DB名稱
        /// </summary>
        public string DBName;

        /// <summary>
        /// DB類型
        /// </summary>
        public DBParam.DBType DBType;

        public TableAttribute(string tbName, string dbName = "", DBParam.DBType dbType = DBParam.DBType.SQLSERVER)
        {
            TableName = tbName;
            DBName = dbName;
            DBType = dbType;
        }
    }
}
