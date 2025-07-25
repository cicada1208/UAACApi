using Lib;
using Models;
using Params;
using Repositorys.NISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Params.SysAppParam;

namespace Repositorys.UAAC
{
    public class SysCatalogRepository : UAACBaseRepository<SysCatalog>
    {
        public override Task<ApiResult<SysCatalog>> Insert(SysCatalog param)
        {
            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.SysCatalogId.IsNullOrWhiteSpace())
                param.SysCatalogId = Guid.NewGuid().ToString();
            return base.Insert(param);
        }

        public async Task<ApiResult<List<SysCatalog>>> BatchInsertSysApp(SysCatalogAddSysApp addParam)
        {
            if (addParam.RootId.IsNullOrWhiteSpace() || addParam.CatalogId.IsNullOrWhiteSpace())
                return new ApiResult<List<SysCatalog>>(false, msg: "RootId 或 CatalogId 參數缺失。");

            if (addParam.SysApps.Count == 0)
                return new ApiResult<List<SysCatalog>>(false, msg: MsgParam.SaveDataNone);

            if (addParam.SysApps.Exists(s => s.SysType.Equals(SysType.Root)))
                return new ApiResult<List<SysCatalog>>(false, msg: $"SysType 不能為 {Enum.GetName(typeof(SysType), SysType.Root)}。");

            if (addParam.RootId != addParam.CatalogId)
            {
                bool catalogExist = (await DBUtil.QueryAsync<SysCatalog>(new SysCatalog
                {
                    RootId = addParam.RootId,
                    SysId = addParam.CatalogId,
                    Activate = true
                })).Any();
                if (!catalogExist)
                    return new ApiResult<List<SysCatalog>>(false, msg: "Catalog 已停用無法加入。");
            }

            short nextSeq = (short)(((await Get(new SysCatalog
            {
                RootId = addParam.RootId,
                CatalogId = addParam.CatalogId,
                Activate = true
            })).Data.Max(sc => sc.Seq) ?? 0) + 1);

            List<SysCatalog> sysCatalogs = new List<SysCatalog>();
            addParam.SysApps.ForEach(sysApp =>
            {
                sysCatalogs.Add(new SysCatalog
                {
                    SysCatalogId = Guid.NewGuid().ToString(),
                    RootId = addParam.RootId,
                    CatalogId = addParam.CatalogId,
                    SysId = sysApp.SysId,
                    Seq = nextSeq,
                    Activate = true,
                    MUserId = addParam.MUserId,
                    MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
                nextSeq++;
            });

            List<SqlCmdUtil> sqlCmds = new();
            sysCatalogs.ForEach(sc => sqlCmds.Add(Utils.SqlBuild.Insert(typeof(SysCatalog), sc)));

            var sysIds = sysCatalogs.Select(sc => sc.SysId);
            string sql = $@"
            select SysId from SysCatalog
            where RootId = @RootId
            and CatalogId = @CatalogId
            and SysId in @SysId
            and Activate = 1
            union
            -- RootId = @RootId 中已使用的 Catalog，不能再加入目錄階層
            select C.SysId from SysCatalog as C
            left join SysApp as S
            on (S.SysId = C.SysId)
            where C.RootId = @RootId
            and S.SysType =  {(int)SysType.Catalog} 
            and C.SysId in @SysId
            and C.Activate = 1";
            bool sysIdExist = (await DBUtil.QueryAsync<SysCatalog>(sql, new
            {
                addParam.RootId,
                addParam.CatalogId,
                SysId = sysIds
            })).Any();
            if (sysIdExist)
                return new ApiResult<List<SysCatalog>>(false, msg: $"系統或目錄已加入過，無法再次加入。");

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<List<SysCatalog>>(rowsAffected, msgType: ApiParam.ApiMsgType.INSERT);
        }

        public async Task<ApiResult<object>> BatchPatchDeactivate(SysCatalogRemoveParam removeParam)
        {
            if (removeParam.SysCatalogId.IsNullOrWhiteSpace() || removeParam.RootId.IsNullOrWhiteSpace())
                return new ApiResult<object>(false, msg: "SysCatalogId 或 RootId 參數缺失。");

            List<SqlCmdUtil> sqlCmds = new();
            SqlCmdUtil sqlCmd;
            string sql = string.Empty;

            // 停用該項 SysCatalogId
            sqlCmd = new SqlCmdUtil();
            sqlCmd.Builder.Append($@"
            update SysCatalog set 
            Activate = 0, 
            MUserId = @MUserId, 
            MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
            where SysCatalogId = @SysCatalogId
            and Activate = 1");
            sqlCmd.Param = removeParam;
            sqlCmds.Add(sqlCmd);

            // 取得 RootId 目錄階層的所有 Catalog
            sql = $@"
            select C.* from SysCatalog as C
            left join SysApp as S
            on (S.SysId = C.SysId)
            where C.RootId = @RootId
            and C.Activate = 1
            and S.SysType = {(int)SysType.Catalog}";
            var entireCatalogs = (await DBUtil.QueryAsync<SysCatalog>(sql, removeParam)).ToList();

            var catalogs = entireCatalogs.Where(ec => ec.SysCatalogId == removeParam.SysCatalogId).ToList();
            AddSubItemOfDeactivateCatalog(sqlCmds, removeParam, entireCatalogs, catalogs);

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<object>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        }

        /// <summary>
        /// 加入停用目錄子項
        /// </summary>
        private void AddSubItemOfDeactivateCatalog(List<SqlCmdUtil> sqlCmds, SysCatalogRemoveParam removeParam,
            List<SysCatalog> entireCatalogs, List<SysCatalog> catalogs)
        {
            SqlCmdUtil sqlCmd;

            catalogs.ForEach(catalog =>
            {
                // 停用該目錄子項
                sqlCmd = new SqlCmdUtil();
                sqlCmd.Builder.Append($@"
                update SysCatalog set 
                Activate = 0, 
                MUserId = @MUserId, 
                MDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                where RootId = @RootId
                and CatalogId = @CatalogId
                and Activate = 1");
                sqlCmd.Param = new SysCatalog { RootId = catalog.RootId, CatalogId = catalog.SysId, MUserId = removeParam.MUserId };
                sqlCmds.Add(sqlCmd);

                // 該目錄往下查子目錄
                AddSubItemOfDeactivateCatalog(sqlCmds, removeParam, entireCatalogs,
                    entireCatalogs.Where(ec => ec.CatalogId == catalog.SysId).ToList());
            });
        }

        public async Task<ApiResult<List<SysCatalog>>> BatchPatchSeq(List<SysCatalog> sysCatalogs)
        {
            if (sysCatalogs == null || sysCatalogs.Count == 0)
                return new ApiResult<List<SysCatalog>>(false, msg: MsgParam.SaveDataNone);

            List<SqlCmdUtil> sqlCmds = new();
            HashSet<string> updateCol = new();
            updateCol.AddProps<SysCatalog>(m => new { m.SysCatalogId, m.Seq, m.MUserId, m.MDateTime });
            sysCatalogs.ForEach(sc =>
            {
                if (sc.MDateTime.IsNullOrWhiteSpace())
                    sc.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sqlCmds.Add(Utils.SqlBuild.Patch(typeof(SysCatalog), sc, updateCol));
            });

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<List<SysCatalog>>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        }

    }
}
