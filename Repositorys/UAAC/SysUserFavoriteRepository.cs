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
    public class SysUserFavoriteRepository : UAACBaseRepository<SysUserFavorite>
    {
        public override Task<ApiResult<List<SysUserFavorite>>> Get(SysUserFavorite param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);
            return base.Get(param);
        }

        public override async Task<ApiResult<SysUserFavorite>> Insert(SysUserFavorite param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            var result = await Get(new SysUserFavorite
            {
                UserId = param.UserId,
                SysId = param.SysId,
                Activate = true
            });

            if (result.Data.Any())
                return new ApiResult<SysUserFavorite>(true, msg: "重複加入。");

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.SysUserFavoriteId.IsNullOrWhiteSpace())
                param.SysUserFavoriteId = Guid.NewGuid().ToString();
            if (param.Seq.IsNullOrWhiteSpace())
            {
                short nextSeq = (short)(((await Get(new SysUserFavorite
                {
                    UserId = param.UserId,
                    Activate = true
                })).Data.Max(f => f.Seq) ?? 0) + 1);
                param.Seq = nextSeq;
            }

            return await base.Insert(param);
        }

        public async Task<ApiResult<object>> PatchDeactivate(SysUserFavorite param)
        {
            string sql;
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            if (!param.SysUserFavoriteId.IsNullOrWhiteSpace())
            {
                sql = $@"
                update SysUserFavorite set
                Activate = 0, 
                MUserId = @MUserId, 
                MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                where SysUserFavoriteId = @SysUserFavoriteId
                and Activate = 1";
                await DBUtil.ExecuteAsync(sql, param);
                return new ApiResult<object>(true);
            }
            else if (!param.UserId.IsNullOrWhiteSpace() && !param.SysId.IsNullOrWhiteSpace())
            {
                sql = $@"
                update SysUserFavorite set
                Activate = 0, 
                MUserId = @MUserId, 
                MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                where UserId = @UserId
                and SysId = @SysId
                and Activate = 1";
                await DBUtil.ExecuteAsync(sql, param);
                return new ApiResult<object>(true);
            }
            else
                return new ApiResult<object>(false, msg: MsgParam.SaveDataNone);
        }

        public async Task<ApiResult<List<SysUserFavorite>>> BatchPatchSeq(List<SysUserFavorite> sysUserFavorites)
        {
            if (sysUserFavorites == null || sysUserFavorites.Count == 0)
                return new ApiResult<List<SysUserFavorite>>(false, msg: MsgParam.SaveDataNone);

            List<SqlCmdUtil> sqlCmds = new();
            HashSet<string> updateCol = new();
            updateCol.AddProps<SysUserFavorite>(m => new { m.SysUserFavoriteId, m.Seq, m.MUserId, m.MDateTime });
            sysUserFavorites.ForEach(f =>
            {
                if (f.MDateTime.IsNullOrWhiteSpace())
                    f.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sqlCmds.Add(Utils.SqlBuild.Patch(typeof(SysUserFavorite), f, updateCol));
            });

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<List<SysUserFavorite>>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        }

    }
}
