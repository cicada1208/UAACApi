using Lib;
using Params;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys
{
    public abstract class BaseRepository<TModel> where TModel : class
    {
        public BaseRepository()
        {
            TableName = DBUtil.GetTableName<TModel>();
            DBName = DBUtil.GetDBName<TModel>();
            DBType = DBUtil.GetDBType<TModel>();
        }

        /// <summary>
        /// 表格名稱
        /// </summary>
        public string TableName { get; set; } = string.Empty;
        /// <summary>
        /// DB名稱
        /// </summary>
        public string DBName { get; set; } = string.Empty;
        /// <summary>
        /// DB類型
        /// </summary>
        public DBParam.DBType DBType { get; set; } = DBParam.DBType.SQLSERVER;

        private DBUtil _DBUtil;
        protected DBUtil DBUtil =>
            _DBUtil ??= new DBUtil(DBName, DBType);

        private UtilLocator _utils;
        protected UtilLocator Utils =>
            _utils ??= new UtilLocator();

        /// <summary>
        /// 查詢 TModel 資料表
        /// </summary>
        public virtual async Task<ApiResult<List<TModel>>> Get(TModel param)
        {
            var query = (await DBUtil.QueryAsync<TModel>(param)).ToList();
            return new ApiResult<List<TModel>>(query);
        }

        /// <summary>
        /// 新增 TModel 資料表
        /// </summary>
        public virtual async Task<ApiResult<TModel>> Insert(TModel param)
        {
            int rowsAffected = await DBUtil.InsertAsync<TModel>(param);
            return new ApiResult<TModel>(rowsAffected, param, msgType: ApiParam.ApiMsgType.INSERT);
        }

        /// <summary>
        /// 整筆更新 TModel 資料表
        /// </summary>
        public virtual async Task<ApiResult<TModel>> Update(TModel param)
        {
            int rowsAffected = await DBUtil.UpdateAsync<TModel>(param);
            return new ApiResult<TModel>(rowsAffected, param, msgType: ApiParam.ApiMsgType.UPDATE);
        }

        /// <summary>
        /// 部份欄位更新 TModel 資料表
        /// </summary>
        public virtual async Task<ApiResult<TModel>> Patch(TModel param, HashSet<string> updateCol)
        {
            int rowsAffected = await DBUtil.PatchAsync<TModel>(param, updateCol);
            return new ApiResult<TModel>(rowsAffected, param, msgType: ApiParam.ApiMsgType.UPDATE);
        }

    }
}
