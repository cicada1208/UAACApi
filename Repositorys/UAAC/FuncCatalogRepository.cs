using Lib;
using Models;
using Params;
using Repositorys.NISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Params.FuncParam;

namespace Repositorys.UAAC
{
    public class FuncCatalogRepository : UAACBaseRepository<FuncCatalog>
    {
        public override Task<ApiResult<FuncCatalog>> Insert(FuncCatalog param)
        {
            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.FuncCatalogId.IsNullOrWhiteSpace())
                param.FuncCatalogId = Guid.NewGuid().ToString();
            return base.Insert(param);
        }

        public async Task<ApiResult<List<FuncCatalog>>> BatchInsertFunc(FuncCatalogAddFunc addParam)
        {
            if (addParam.RootId.IsNullOrWhiteSpace() || addParam.CatalogId.IsNullOrWhiteSpace())
                return new ApiResult<List<FuncCatalog>>(false, msg: "RootId 或 CatalogId 參數缺失。");

            if (addParam.Funcs.Count == 0)
                return new ApiResult<List<FuncCatalog>>(false, msg: MsgParam.SaveDataNone);

            if (addParam.Funcs.Exists(s => s.FuncType.Equals(FuncType.Root)))
                return new ApiResult<List<FuncCatalog>>(false, msg: $"FuncType 不能為 {Enum.GetName(typeof(FuncType), FuncType.Root)}。");

            if (addParam.RootId != addParam.CatalogId)
            {
                bool catalogExist = (await DBUtil.QueryAsync<FuncCatalog>(new FuncCatalog
                {
                    RootId = addParam.RootId,
                    FuncId = addParam.CatalogId,
                    Activate = true
                })).Any();
                if (!catalogExist)
                    return new ApiResult<List<FuncCatalog>>(false, msg: "Catalog 已停用無法加入。");
            }

            short nextSeq = (short)(((await Get(new FuncCatalog
            {
                RootId = addParam.RootId,
                CatalogId = addParam.CatalogId,
                Activate = true
            })).Data.Max(sc => sc.Seq) ?? 0) + 1);

            List<FuncCatalog> funcCatalogs = new List<FuncCatalog>();
            addParam.Funcs.ForEach(func =>
            {
                funcCatalogs.Add(new FuncCatalog
                {
                    FuncCatalogId = Guid.NewGuid().ToString(),
                    RootId = addParam.RootId,
                    CatalogId = addParam.CatalogId,
                    FuncId = func.FuncId,
                    Seq = nextSeq,
                    Activate = true,
                    MUserId = addParam.MUserId,
                    MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
                nextSeq++;
            });

            List<SqlCmdUtil> sqlCmds = new();
            funcCatalogs.ForEach(sc => sqlCmds.Add(Utils.SqlBuild.Insert(typeof(FuncCatalog), sc)));

            var funcIds = funcCatalogs.Select(sc => sc.FuncId);
            string sql = $@"
            select FuncId from FuncCatalog
            where RootId = @RootId
            and CatalogId = @CatalogId
            and FuncId in @FuncId
            and Activate = 1
            union
            -- RootId = @RootId 中已使用的 Catalog，不能再加入目錄階層
            select C.FuncId from FuncCatalog as C
            left join Func as F
            on (F.FuncId = C.FuncId)
            where C.RootId = @RootId
            and F.FuncType =  {(int)FuncType.Catalog} 
            and C.FuncId in @FuncId
            and C.Activate = 1";
            bool sysIdExist = (await DBUtil.QueryAsync<FuncCatalog>(sql, new
            {
                addParam.RootId,
                addParam.CatalogId,
                FuncId = funcIds
            })).Any();
            if (sysIdExist)
                return new ApiResult<List<FuncCatalog>>(false, msg: $"功能或目錄已加入過，無法再次加入。");

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<List<FuncCatalog>>(rowsAffected, msgType: ApiParam.ApiMsgType.INSERT);
        }

        public async Task<ApiResult<object>> BatchPatchDeactivate(FuncCatalogRemoveParam removeParam)
        {
            if (removeParam.FuncCatalogId.IsNullOrWhiteSpace() || removeParam.RootId.IsNullOrWhiteSpace())
                return new ApiResult<object>(false, msg: "FuncCatalogId 或 RootId 參數缺失。");

            List<SqlCmdUtil> sqlCmds = new();
            SqlCmdUtil sqlCmd;
            string sql = string.Empty;

            // 停用該項 FuncCatalogId
            sqlCmd = new SqlCmdUtil();
            sqlCmd.Builder.Append($@"
            update FuncCatalog set 
            Activate = 0, 
            MUserId = @MUserId, 
            MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
            where FuncCatalogId = @FuncCatalogId
            and Activate = 1");
            sqlCmd.Param = removeParam;
            sqlCmds.Add(sqlCmd);

            // 取得 RootId 目錄階層的所有 Catalog
            sql = $@"
            select C.* from FuncCatalog as C
            left join Func as F
            on (F.FuncId = C.FuncId)
            where C.RootId = @RootId
            and C.Activate = 1
            and F.FuncType = {(int)FuncType.Catalog}";
            var entireCatalogs = (await DBUtil.QueryAsync<FuncCatalog>(sql, removeParam)).ToList();

            var catalogs = entireCatalogs.Where(ec => ec.FuncCatalogId == removeParam.FuncCatalogId).ToList();
            AddSubItemOfDeactivateCatalog(sqlCmds, removeParam, entireCatalogs, catalogs);

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<object>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        }

        /// <summary>
        /// 加入停用目錄子項
        /// </summary>
        private void AddSubItemOfDeactivateCatalog(List<SqlCmdUtil> sqlCmds, FuncCatalogRemoveParam removeParam,
            List<FuncCatalog> entireCatalogs, List<FuncCatalog> catalogs)
        {
            SqlCmdUtil sqlCmd;

            catalogs.ForEach(catalog =>
            {
                // 停用該目錄子項
                sqlCmd = new SqlCmdUtil();
                sqlCmd.Builder.Append($@"
                update FuncCatalog set 
                Activate = 0, 
                MUserId = @MUserId, 
                MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                where RootId = @RootId
                and CatalogId = @CatalogId
                and Activate = 1");
                sqlCmd.Param = new FuncCatalog { RootId = catalog.RootId, CatalogId = catalog.FuncId, MUserId = removeParam.MUserId };
                sqlCmds.Add(sqlCmd);

                // 該目錄往下查子目錄
                AddSubItemOfDeactivateCatalog(sqlCmds, removeParam, entireCatalogs,
                    entireCatalogs.Where(ec => ec.CatalogId == catalog.FuncId).ToList());
            });
        }

        public async Task<ApiResult<List<FuncCatalog>>> BatchPatchSeq(List<FuncCatalog> funcCatalogs)
        {
            if (funcCatalogs == null || funcCatalogs.Count == 0)
                return new ApiResult<List<FuncCatalog>>(false, msg: MsgParam.SaveDataNone);

            List<SqlCmdUtil> sqlCmds = new();
            HashSet<string> updateCol = new();
            updateCol.AddProps<FuncCatalog>(m => new { m.FuncCatalogId, m.Seq, m.MUserId, m.MDateTime });
            funcCatalogs.ForEach(fc =>
            {
                if (fc.MDateTime.IsNullOrWhiteSpace())
                    fc.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sqlCmds.Add(Utils.SqlBuild.Patch(typeof(FuncCatalog), fc, updateCol));
            });

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<List<FuncCatalog>>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        }

    }
}
