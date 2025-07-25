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
    public class SysAppRepository : UAACBaseRepository<SysApp>
    {
        /// <summary>
        /// 查詢系統目錄階層的加入清單
        /// </summary>
        public async Task<ApiResult<List<SysApp>>> GetSysCatalogAddList(SysCatalogQueryParam param)
        {
            string sql = $@"
            select * from SysApp
            where SysType not in ({(int)SysType.Root}) 
            and SysId not in (
                select SysId from SysCatalog
                where RootId = @RootId
                and CatalogId = @CatalogId
                and Activate = 1
                union
                -- RootId = @RootId 中已使用的 Catalog，不能再加入目錄階層
                select C.SysId from SysCatalog as C
                left join SysApp as S
                on (S.SysId = C.SysId)
                where C.RootId = @RootId
                and S.SysType = {(int)SysType.Catalog} 
                and C.Activate = 1
            )
            and Activate = 1";

            var query = (await DBUtil.QueryAsync<SysApp>(sql, param)).ToList();
            return new ApiResult<List<SysApp>>(query);
        }

        /// <summary>
        /// 查詢系統目錄階層的排序清單
        /// </summary>
        public async Task<ApiResult<List<SysGroup>>> GetSysCatalogSortList(SysCatalogQueryParam param)
        {
            string sql = @"
            select S.*, C.SysCatalogId, C.RootId, C.CatalogId, C.Seq
            from SysCatalog as C
            left join SysApp as S
            on (S.SysId = C.SysId)
            where C.RootId = @RootId
            and C.CatalogId = @CatalogId
            and C.Activate = 1
            order by C.Seq";

            var query = (await DBUtil.QueryAsync<SysGroup>(sql, param)).ToList();
            return new ApiResult<List<SysGroup>>(query);
        }

        public override async Task<ApiResult<SysApp>> Insert(SysApp param)
        {
            var result = await Get(new SysApp
            {
                SysId = param.SysId
            });

            if (result.Data.Any())
                return new ApiResult<SysApp>(false, msg: MsgParam.KeyDuplication);

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return await base.Insert(param);
        }

        public override Task<ApiResult<SysApp>> Update(SysApp param)
        {
            param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return base.Update(param);
        }

    }
}
