using Lib;
using Models;
using Params;
using Repositorys.NISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Repositorys.UAAC
{
    public class FuncUserFavoriteRepository : UAACBaseRepository<FuncUserFavorite>
    {
        public override Task<ApiResult<List<FuncUserFavorite>>> Get(FuncUserFavorite param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);
            return base.Get(param);
        }

        public override async Task<ApiResult<FuncUserFavorite>> Insert(FuncUserFavorite param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            var result = await Get(new FuncUserFavorite
            {
                UserId = param.UserId,
                SysId = param.SysId,
                FuncId = param.FuncId,
                Activate = true
            });

            if (result.Data.Any())
                return new ApiResult<FuncUserFavorite>(true, msg: "重複加入。");

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.FuncUserFavoriteId.IsNullOrWhiteSpace())
                param.FuncUserFavoriteId = Guid.NewGuid().ToString();
            if (param.Seq.IsNullOrWhiteSpace())
            {
                short nextSeq = (short)(((await Get(new FuncUserFavorite
                {
                    UserId = param.UserId,
                    SysId = param.SysId,
                    Activate = true
                })).Data.Max(f => f.Seq) ?? 0) + 1);
                param.Seq = nextSeq;
            }

            return await base.Insert(param);
        }

        public async Task<ApiResult<object>> PatchDeactivate(FuncUserFavorite param)
        {
            string sql;
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            if (!param.FuncUserFavoriteId.IsNullOrWhiteSpace())
            {
                sql = $@"
                update FuncUserFavorite set
                Activate = 0, 
                MUserId = @MUserId, 
                MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                where FuncUserFavoriteId = @FuncUserFavoriteId
                and Activate = 1";
                await DBUtil.ExecuteAsync(sql, param);
                return new ApiResult<object>(true);
            }
            else if (!param.UserId.IsNullOrWhiteSpace() && !param.SysId.IsNullOrWhiteSpace() && !param.FuncId.IsNullOrWhiteSpace())
            {
                sql = $@"
                update FuncUserFavorite set
                Activate = 0, 
                MUserId = @MUserId, 
                MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                where UserId = @UserId
                and SysId = @SysId
                and FuncId = @FuncId
                and Activate = 1";
                await DBUtil.ExecuteAsync(sql, param);
                return new ApiResult<object>(true);
            }
            else
                return new ApiResult<object>(false, msg: MsgParam.SaveDataNone);
        }

        public async Task<ApiResult<List<FuncUserFavorite>>> BatchPatchSeq(List<FuncUserFavorite> funcUserFavorites)
        {
            if (funcUserFavorites == null || funcUserFavorites.Count == 0)
                return new ApiResult<List<FuncUserFavorite>>(false, msg: MsgParam.SaveDataNone);

            List<SqlCmdUtil> sqlCmds = new();
            HashSet<string> updateCol = new();
            updateCol.AddProps<FuncUserFavorite>(m => new { m.FuncUserFavoriteId, m.Seq, m.MUserId, m.MDateTime });
            funcUserFavorites.ForEach(f =>
            {
                if (f.MDateTime.IsNullOrWhiteSpace())
                    f.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sqlCmds.Add(Utils.SqlBuild.Patch(typeof(FuncUserFavorite), f, updateCol));
            });

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<List<FuncUserFavorite>>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        }

    }
}
