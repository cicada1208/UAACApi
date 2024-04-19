using AdoNetCore.AseClient;
using Dapper;
using Lib.Attributes;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Params;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lib
{
    public class DBUtil
    {
        /// <summary>
        /// DB類型
        /// </summary>
        public DBParam.DBType DBType { get; set; } = DBParam.DBType.SYBASE;
        /// <summary>
        /// DB名稱
        /// </summary>
        public string DBName { get; set; } = string.Empty;

        /// <summary>
        /// DB Connection String
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        ///// <summary>
        ///// 預設 Dapper param 編碼格式
        ///// </summary>
        //public bool IsUnicodeParamDefault { get; set; } = false;

        //public DBUtil()
        //{
        //    DBType = DBParam.DBType.SYBASE;
        //    DBName = DBParam.DBName.NIS;
        //    ConnectionString = GetConnString(DBName);
        //    IsUnicodeParamDefault = GetIsUnicodeParamDefault(DBType);
        //}

        public DBUtil(string dbName, DBParam.DBType dbType)
        {
            DBType = dbType;
            DBName = dbName;
            ConnectionString = GetConnString(dbName);
            //IsUnicodeParamDefault = GetIsUnicodeParamDefault(dbType);
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
        }

        /// <summary>
        /// Get Connnection String By DBName
        /// </summary>
        public static string GetConnString(string dbName)
        {
            IConfigurationRoot configuration = ConfigUtil.ConfigAppSettings();
            string connString = configuration.GetConnectionString(dbName);
            return connString;
        }

        ///// <summary>
        ///// 院內 Sybase 編碼 big5(Ansi)，其他 SqlServer、Oracle 預設 Unicode
        ///// </summary>
        //public static bool GetIsUnicodeParamDefault(DBParam.DBType dbType)
        //{
        //    if (dbType == DBParam.DBType.SYBASE)
        //        return false;
        //    else
        //        return true;
        //}

        ///// <summary>
        ///// 設定 Dapper param 編碼格式
        ///// </summary>
        //public static void SetUnicodeParam(bool isUnicodeParam)
        //{
        //    if (isUnicodeParam)
        //        Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.String);
        //    else
        //        Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
        //}

        /// <summary>
        /// 建立Connection：依Command
        /// </summary>
        public IDbConnection CreateConn(IDbCommand cmd)
        {
            IDbConnection conn = null;

            if (cmd.GetType() == typeof(AseCommand))
                conn = new AseConnection(ConnectionString);
            else if (cmd.GetType() == typeof(SqlCommand))
                conn = new SqlConnection(ConnectionString);
            else if (cmd.GetType() == typeof(OracleCommand))
                conn = new OracleConnection(ConnectionString);

            if (conn.State == ConnectionState.Closed) conn.Open();

            return conn;
        }

        /// <summary>
        /// 建立Connection：依DBType
        /// </summary>
        public IDbConnection CreateConn(DBParam.DBType dbType)
        {
            IDbConnection conn = null;

            if (dbType == DBParam.DBType.SYBASE)
                conn = new AseConnection(ConnectionString);
            else if (dbType == DBParam.DBType.SQLSERVER)
                conn = new SqlConnection(ConnectionString);
            else if (dbType == DBParam.DBType.ORACLE)
                conn = new OracleConnection(ConnectionString);

            if (conn.State == ConnectionState.Closed) conn.Open();

            return conn;
        }

        /// <summary>
        /// 建立Connection：依DBName、DBType
        /// </summary>
        public IDbConnection CreateConn(string dbName, DBParam.DBType dbType)
        {
            IDbConnection conn = null;
            string connString = GetConnString(dbName);

            if (dbType == DBParam.DBType.SYBASE)
                conn = new AseConnection(connString);
            else if (dbType == DBParam.DBType.SQLSERVER)
                conn = new SqlConnection(connString);
            else if (dbType == DBParam.DBType.ORACLE)
                conn = new OracleConnection(connString);

            if (conn.State == ConnectionState.Closed) conn.Open();

            return conn;
        }

        public DataTable ExecQuery(IDbCommand cmd, DataTable dtData = null, int cmdTimeout = 30,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true)
        {
            IDbConnection conn = null;
            IDataReader reader = null;
            DataTable dtSchema = null;

            try
            {
                conn = this.CreateConn(cmd);
                cmd.Connection = conn;
                cmd.CommandTimeout = cmdTimeout;
                reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                dtSchema = reader.GetSchemaTable();
                if (dtData != null)
                {
                    dtData.Load(reader);
                    if (trimType != StrParam.TrimType.None || nullToEmpty)
                        dtData = dtData.StrProcess(trimType, nullToEmpty);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(cmd), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                cmd?.Dispose();
                reader?.Close(); reader?.Dispose();
            }

            return dtSchema;
        }

        public DataTable ExecQuery(string sql, DataTable dtData = null, int cmdTimeout = 30,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true)
        {
            IDbConnection conn = null;
            IDbCommand cmd = null;
            IDataReader reader = null;
            DataTable dtSchema = null;

            try
            {
                conn = this.CreateConn(this.DBName, this.DBType);
                cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandTimeout = cmdTimeout;
                reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                dtSchema = reader.GetSchemaTable();
                if (dtData != null)
                {
                    dtData.Load(reader);
                    if (trimType != StrParam.TrimType.None || nullToEmpty)
                        dtData = dtData.StrProcess(trimType, nullToEmpty);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sql), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                cmd?.Dispose();
                reader?.Close(); reader?.Dispose();
            }

            return dtSchema;
        }

        public string ExecuteScalar(IDbCommand cmd, int cmdTimeout = 30)
        {
            IDbConnection conn = null;
            string result = string.Empty;

            try
            {
                conn = this.CreateConn(cmd);
                cmd.Connection = conn;
                cmd.CommandTimeout = cmdTimeout;

                // cmd.ExecuteScalar() is null: ?? => string.Empty
                // cmd.ExecuteScalar() is DBNull.Value: .ToString() => string.Empty
                result = (cmd.ExecuteScalar() ?? string.Empty).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(cmd), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                cmd?.Dispose();
            }

            return result;
        }

        public bool ExecuteNonQuery(IDbCommand cmd, int cmdTimeout = 30)
        {
            IDbConnection conn = null;
            IDbTransaction tran = null;
            bool succ = true;

            try
            {
                conn = this.CreateConn(cmd);
                tran = conn.BeginTransaction();
                cmd.Connection = conn;
                cmd.CommandTimeout = cmdTimeout;
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                tran.Commit();
                succ = true;
            }
            catch (Exception ex)
            {
                if (tran != null) tran.Rollback();
                succ = false;
                throw new Exception(LogSql(cmd), ex);
            }
            finally
            {
                tran?.Dispose();
                conn?.Close(); conn?.Dispose();
                cmd?.Dispose();
            }

            return succ;
        }

        public bool ExecuteNonQuery(IEnumerable<IDbCommand> cmds, int cmdTimeout = 30)
        {
            IDbConnection conn = null;
            IDbTransaction tran = null;
            bool succ = true;

            try
            {
                conn = this.CreateConn(cmds.First());
                tran = conn.BeginTransaction();
                foreach (var cmd in cmds)
                {
                    cmd.Connection = conn;
                    cmd.CommandTimeout = cmdTimeout;
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                succ = true;
            }
            catch (Exception ex)
            {
                if (tran != null) tran.Rollback();
                succ = false;
                throw new Exception(LogSql(cmds), ex);
            }
            finally
            {
                tran?.Dispose();
                conn?.Close(); conn?.Dispose();
                if (cmds != null)
                {
                    foreach (var cmd in cmds)
                        cmd?.Dispose();
                }
            }

            return succ;
        }

        public bool ExecuteNonQueryWithAffected(IDbCommand cmd, ref string msg, int cmdTimeout = 30)
        {
            IDbConnection conn = null;
            IDbTransaction tran = null;
            bool succ = true;
            int rowsAffected = 0;

            try
            {
                conn = this.CreateConn(cmd);
                tran = conn.BeginTransaction();
                cmd.Connection = conn;
                cmd.CommandTimeout = cmdTimeout;
                cmd.Transaction = tran;
                rowsAffected = cmd.ExecuteNonQuery();
                tran.Commit();

                succ = (rowsAffected == 0) ? false : true;
                msg = rowsAffected.ToString() + " Row Affected！";
            }
            catch (Exception ex)
            {
                if (tran != null) tran.Rollback();
                succ = false;
                throw new Exception(LogSql(cmd), ex);
            }
            finally
            {
                tran?.Dispose();
                conn?.Close(); conn?.Dispose();
                cmd?.Dispose();
            }

            return succ;
        }

        /// <summary>
        /// 取得DB類型
        /// </summary>
        public static DBParam.DBType GetDBType<TModel>()
        {
            Attribute attr = Attribute.GetCustomAttribute(typeof(TModel), typeof(TableAttribute));
            DBParam.DBType dbType = (attr as dynamic)?.DBType ?? DBParam.DBType.SYBASE;
            return dbType;
        }

        /// <summary>
        /// 取得DB類型(Nullable)
        /// </summary>
        public static DBParam.DBType? GetDBTypeNullable<TModel>()
        {
            Attribute attr = Attribute.GetCustomAttribute(typeof(TModel), typeof(TableAttribute));
            DBParam.DBType? dbType = (attr as dynamic)?.DBType;
            return dbType;
        }

        /// <summary>
        /// 取得DB名稱
        /// </summary>
        public static string GetDBName<TModel>()
        {
            Attribute attr = Attribute.GetCustomAttribute(typeof(TModel), typeof(TableAttribute));
            var dbName = (attr as dynamic)?.DBName ?? string.Empty;
            return dbName;
        }

        /// <summary>
        /// 取得表格名稱
        /// </summary>
        public static string GetTableName<TModel>()
        {
            Attribute attr = Attribute.GetCustomAttribute(typeof(TModel), typeof(TableAttribute));
            var tableName = (attr as dynamic)?.TableName ?? string.Empty;
            return tableName;
        }

        /// <summary>
        /// 取得表格名稱
        /// </summary>
        public static string GetTableName(Type type)
        {
            Attribute attr = Attribute.GetCustomAttribute(type, typeof(TableAttribute));
            var tableName = (attr as dynamic)?.TableName ?? string.Empty;
            return tableName;
        }

        /// <summary>
        /// Dapper Query：自動建立條件相等查詢，若無 entity params 且 schemaOnly = false 則 select all
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        /// <param name="schemaOnly">true:僅表結構，預設false</param>
        public IEnumerable<TModel> Query<TModel>(object entityParam = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true, bool schemaOnly = false) =>
            QueryAsync<TModel>(entityParam, cmdTimeout, trimType, nullToEmpty, schemaOnly).Result;

        /// <summary>
        /// Dapper Query：自動建立條件相等查詢，若無 entityParam 且 schemaOnly = false 則 select all
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        /// <param name="schemaOnly">true:僅表結構，預設false</param>
        public async Task<IEnumerable<TModel>> QueryAsync<TModel>(object entityParam = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true, bool schemaOnly = false)
        {
            IDbConnection conn = null;
            SqlCmdUtil sqlCmd = null;
            IEnumerable<TModel> result = null;

            try
            {
                //string tableName = GetTableName<TModel>();
                //SqlBuildUtil sqlBuild = new SqlBuildUtil(tableName);
                SqlBuildUtil sqlBuild = new SqlBuildUtil();
                sqlCmd = sqlBuild.Select(typeof(TModel), entityParam, schemaOnly);
                DBParam.DBType dbType = GetDBType<TModel>();
                string dbName = GetDBName<TModel>();
                conn = this.CreateConn(dbName, dbType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                result = await conn.QueryAsync<TModel>(sqlCmd.Sql, sqlCmd.Param, commandTimeout: cmdTimeout);
                if (trimType != StrParam.TrimType.None || nullToEmpty)
                    result = result.ToList().StrProcess(trimType, nullToEmpty);
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sqlCmd), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return result;
        }

        /// <summary>
        /// Dapper Query：自訂 sql 查詢
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName，若 TModel 無定義則取自建構子定義</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public IEnumerable<TModel> Query<TModel>(string sql, object entityParam = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true) =>
            QueryAsync<TModel>(sql, entityParam, cmdTimeout, trimType, nullToEmpty).Result;

        /// <summary>
        /// Dapper Query：自訂 sql 查詢
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName，若 TModel 無定義則取自建構子定義</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public async Task<IEnumerable<TModel>> QueryAsync<TModel>(string sql, object entityParam = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true)
        {
            IDbConnection conn = null;
            IEnumerable<TModel> result = null;

            try
            {
                DBParam.DBType? dbType = GetDBTypeNullable<TModel>();
                if (dbType == null) dbType = this.DBType;
                string dbName = GetDBName<TModel>();
                if (dbName == string.Empty) dbName = this.DBName;
                conn = this.CreateConn(dbName, (DBParam.DBType)dbType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                result = await conn.QueryAsync<TModel>(sql, entityParam, commandTimeout: cmdTimeout);
                if (trimType != StrParam.TrimType.None || nullToEmpty)
                    result = result.ToList().StrProcess(trimType, nullToEmpty);
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sql, entityParam), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return result;
        }

        /// <summary>
        /// Dapper Query：整合查詢
        /// </summary>
        public IEnumerable<TModel> QueryIntgr<TModel>(string sql = "", object entityParam = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true, bool schemaOnly = false)
        {
            return sql == string.Empty ?
                Query<TModel>(entityParam, cmdTimeout, trimType, nullToEmpty, schemaOnly) :
                Query<TModel>(sql, entityParam, cmdTimeout, trimType, nullToEmpty);
        }

        /// <summary>
        /// Dapper Query：整合查詢
        /// </summary>
        public Task<IEnumerable<TModel>> QueryIntgrAsync<TModel>(string sql = "", object entityParam = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true, bool schemaOnly = false)
        {
            return sql == string.Empty ?
                QueryAsync<TModel>(entityParam, cmdTimeout, trimType, nullToEmpty, schemaOnly) :
                QueryAsync<TModel>(sql, entityParam, cmdTimeout, trimType, nullToEmpty);
        }

        /// <summary>
        /// Dapper Query：自訂 sql 查詢
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        /// <param name="dtData">資料</param>
        /// <returns>table schema</returns>
        public DataTable Query(string sql, object entityParam = null, DataTable dtData = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true) =>
            QueryAsync(sql, entityParam, dtData, cmdTimeout, trimType, nullToEmpty).Result;

        /// <summary>
        /// Dapper Query：自訂 sql 查詢
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        /// <param name="dtData">資料</param>
        /// <returns>table schema</returns>
        public async Task<DataTable> QueryAsync(string sql, object entityParam = null, DataTable dtData = null, int? cmdTimeout = null,
            StrParam.TrimType trimType = StrParam.TrimType.TrimEnd, bool nullToEmpty = true)
        {
            IDbConnection conn = null;
            CommandDefinition cmd;
            IDataReader reader = null;
            DataTable dtSchema = null;

            try
            {
                conn = this.CreateConn(this.DBName, this.DBType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                //reader = conn.ExecuteReader(sql, entityParam, commandTimeout: cmdTimeout);
                cmd = new CommandDefinition(sql, entityParam, commandTimeout: cmdTimeout);
                reader = await conn.ExecuteReaderAsync(cmd, CommandBehavior.KeyInfo); // 不只primary key，所有key皆會回傳
                dtSchema = reader.GetSchemaTable();
                if (dtData != null)
                {
                    dtData.Load(reader);
                    if (trimType != StrParam.TrimType.None || nullToEmpty)
                        dtData = dtData.StrProcess(trimType, nullToEmpty);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sql, entityParam), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                reader?.Close(); reader?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return dtSchema;
        }

        /// <summary>
        /// Dapper Execute Insert：自動建立新增
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public int Insert<TModel>(object entityParam, int? cmdTimeout = null) =>
            InsertAsync<TModel>(entityParam, cmdTimeout).Result;

        /// <summary>
        /// Dapper Execute Insert：自動建立新增
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public async Task<int> InsertAsync<TModel>(object entityParam, int? cmdTimeout = null)
        {
            IDbConnection conn = null;
            SqlCmdUtil sqlCmd = null;
            int rowsAffected = 0;

            try
            {
                //string tableName = GetTableName<TModel>();
                //SqlBuildUtil sqlBuild = new SqlBuildUtil(tableName);
                SqlBuildUtil sqlBuild = new SqlBuildUtil();
                sqlCmd = sqlBuild.Insert(typeof(TModel), entityParam);
                DBParam.DBType dbType = GetDBType<TModel>();
                string dbName = GetDBName<TModel>();
                conn = this.CreateConn(dbName, dbType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                rowsAffected = await conn.ExecuteAsync(sqlCmd.Sql, sqlCmd.Param, commandTimeout: cmdTimeout);
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sqlCmd), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Dapper Execute Update：自動建立整筆更新
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public int Update<TModel>(object entityParam, int? cmdTimeout = null) =>
            UpdateAsync<TModel>(entityParam, cmdTimeout).Result;

        /// <summary>
        /// Dapper Execute Update：自動建立整筆更新
        /// </summary>
        /// <typeparam name="TModel">依此查詢DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public async Task<int> UpdateAsync<TModel>(object entityParam, int? cmdTimeout = null)
        {
            IDbConnection conn = null;
            SqlCmdUtil sqlCmd = null;
            int rowsAffected = 0;

            try
            {
                //string tableName = GetTableName<TModel>();
                //SqlBuildUtil sqlBuild = new SqlBuildUtil(tableName);
                SqlBuildUtil sqlBuild = new SqlBuildUtil();
                sqlCmd = sqlBuild.Update(typeof(TModel), entityParam);
                DBParam.DBType dbType = GetDBType<TModel>();
                string dbName = GetDBName<TModel>();
                conn = this.CreateConn(dbName, dbType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                rowsAffected = await conn.ExecuteAsync(sqlCmd.Sql, sqlCmd.Param, commandTimeout: cmdTimeout);
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sqlCmd), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Dapper Execute Patch：自動建立部份欄位更新
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        /// <param name="updateCol">update 欄位</param>
        public int Patch<TModel>(object entityParam, HashSet<string> updateCol, int? cmdTimeout = null) =>
            PatchAsync<TModel>(entityParam, updateCol, cmdTimeout).Result;

        /// <summary>
        /// Dapper Execute Patch：自動建立部份欄位更新
        /// </summary>
        /// <typeparam name="TModel">依此查詢 DBType、DBName、TableName，具有定義 TableAttribute</typeparam>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        /// <param name="updateCol">update 欄位</param>
        public async Task<int> PatchAsync<TModel>(object entityParam, HashSet<string> updateCol, int? cmdTimeout = null)
        {
            IDbConnection conn = null;
            SqlCmdUtil sqlCmd = null;
            int rowsAffected = 0;

            try
            {
                //string tableName = GetTableName<TModel>();
                //SqlBuildUtil sqlBuild = new SqlBuildUtil(tableName);
                SqlBuildUtil sqlBuild = new SqlBuildUtil();
                sqlCmd = sqlBuild.Patch(typeof(TModel), entityParam, updateCol);
                DBParam.DBType dbType = GetDBType<TModel>();
                string dbName = GetDBName<TModel>();
                conn = this.CreateConn(dbName, dbType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                rowsAffected = await conn.ExecuteAsync(sqlCmd.Sql, sqlCmd.Param, commandTimeout: cmdTimeout);
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sqlCmd), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Dapper Execute Insert、Update、Delete：自訂 sql
        /// </summary>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public int Execute(string sql, object entityParam = null, int? cmdTimeout = null) =>
            ExecuteAsync(sql, entityParam, cmdTimeout).Result;

        /// <summary>
        /// Dapper Execute Insert、Update、Delete：自訂 sql
        /// </summary>
        /// <param name="entityParam">entity, 未必定義 TableAttribute</param>
        public async Task<int> ExecuteAsync(string sql, object entityParam = null, int? cmdTimeout = null)
        {
            IDbConnection conn = null;
            int rowsAffected = 0;

            try
            {
                conn = this.CreateConn(this.DBName, this.DBType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                rowsAffected = await conn.ExecuteAsync(sql, entityParam, commandTimeout: cmdTimeout);
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sql, entityParam), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return rowsAffected;
        }

        public int Execute(IEnumerable<SqlCmdUtil> sqlCmds, int? cmdTimeout = null, bool isTransaction = true) =>
            ExecuteAsync(sqlCmds, cmdTimeout, isTransaction).Result;

        public async Task<int> ExecuteAsync(IEnumerable<SqlCmdUtil> sqlCmds, int? cmdTimeout = null, bool isTransaction = true)
        {
            IDbConnection conn = null;
            int rowsAffected = 0;

            try
            {
                conn = this.CreateConn(this.DBName, this.DBType);
                //if (isUnicodeParam == null) isUnicodeParam = IsUnicodeParamDefault;
                //SetUnicodeParam((bool)isUnicodeParam);
                using (var tran = isTransaction ? conn.BeginTransaction() : null)
                {
                    foreach (var cmd in sqlCmds)
                        rowsAffected += await conn.ExecuteAsync(cmd.Sql, cmd.Param, tran, cmdTimeout);
                    tran?.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(LogSql(sqlCmds), ex);
            }
            finally
            {
                conn?.Close(); conn?.Dispose();
                //SetUnicodeParam(IsUnicodeParamDefault);
            }

            return rowsAffected;
        }

        public static string LogSql(IDbCommand cmd)
        {
            string sql = cmd?.CommandText ?? string.Empty;
            string paramName = string.Empty;
            object paramValue;

            try
            {
                if (cmd?.Parameters != null)
                {
                    foreach (IDbDataParameter p in cmd.Parameters)
                    {
                        paramName = p.ParameterName.StartsWith("@") ? p.ParameterName : "@" + p.ParameterName;
                        paramValue = p.Value ?? "null";
                        //if (paramValue.GetType() == typeof(string))
                        //    paramValue = "'" + (paramValue ?? string.Empty).ToString().Replace("'", "''") + "'";
                        if (paramValue.GetType() == typeof(string) && paramValue.ToString() != "null")
                            paramValue = "'" + paramValue.ToString().Replace("'", "''") + "'";
                        sql = Regex.Replace(sql, @"\B" + paramName + @"\b", paramValue.ToString());
                    }
                }
            }
            catch (Exception) { }

            sql += Environment.NewLine;
            return sql;
        }

        public static string LogSql(IEnumerable<IDbCommand> cmds)
        {
            string sqls = string.Empty;

            try
            {
                if (cmds != null)
                {
                    foreach (var cmd in cmds)
                        sqls += LogSql(cmd);
                }
            }
            catch (Exception) { }

            return sqls;
        }

        public static string LogSql(string sql, object entity = null)
        {
            //string paramName = string.Empty;
            object paramValue;

            try
            {
                if (entity != null)
                {
                    //foreach (var p in entity.GetType().GetProperties())
                    //{
                    //    paramName = p.Name.StartsWith("@") ? p.Name : "@" + p.Name;
                    //    paramValue = p.GetValue(entity) ?? "null";
                    //    if (paramValue.GetType() == typeof(string) && paramValue.ToString() != "null")
                    //        paramValue = "'" + paramValue.ToString().Replace("'", "''") + "'";
                    //    sql = Regex.Replace(sql, @"\B" + paramName + @"\b", paramValue.ToString());
                    //}
                    string pattern = @"\@[\w\.\$]+";
                    var paramNames = Regex.Matches(sql, pattern).Cast<Match>().Select(m => m.Value).Distinct().ToArray();
                    foreach (var paramName in paramNames)
                    {
                        paramValue = entity.GetType().GetProperty(paramName.TrimStart('@'))?.GetValue(entity) ?? "null";
                        if (paramValue.GetType() == typeof(string) && paramValue.ToString() != "null")
                            paramValue = "'" + paramValue.ToString().Replace("'", "''") + "'";
                        sql = Regex.Replace(sql, @"\B" + paramName + @"\b", paramValue.ToString());
                    }
                }
            }
            catch (Exception) { }

            sql += Environment.NewLine;
            return sql;
        }

        public static string LogSql(SqlCmdUtil cmd)
        {
            string sql = string.Empty;

            try
            {
                if (cmd != null)
                    sql = LogSql(cmd.Sql, cmd.Param);
            }
            catch (Exception) { }

            return sql;
        }

        public static string LogSql(IEnumerable<SqlCmdUtil> cmds)
        {
            string sqls = string.Empty;

            try
            {
                if (cmds != null)
                {
                    foreach (var cmd in cmds)
                        sqls += LogSql(cmd);
                }
            }
            catch (Exception) { }

            return sqls;
        }

    }
}