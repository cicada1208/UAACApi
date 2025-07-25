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
    public class FuncRepository : UAACBaseRepository<Func>
    {
        /// <summary>
        /// 查詢功能目錄階層的加入清單
        /// </summary>
        public async Task<ApiResult<List<Func>>> GetFuncCatalogAddList(FuncCatalogQueryParam param)
        {
            string sql = $@"
            select * from Func
            where SysId = @SysId
            and FuncType not in ({(int)FuncType.Root}) 
            and FuncId not in (
                select FuncId from FuncCatalog
                where RootId = @RootId
                and CatalogId = @CatalogId
                and Activate = 1
                union
                -- RootId = @RootId 中已使用的 Catalog，不能再加入目錄階層
                select C.FuncId from FuncCatalog as C
                left join Func as F
                on (F.FuncId = C.FuncId)
                where C.RootId = @RootId
                and F.FuncType = {(int)FuncType.Catalog} 
                and C.Activate = 1
            )
            and Activate = 1";

            var query = (await DBUtil.QueryAsync<Func>(sql, param)).ToList();
            return new ApiResult<List<Func>>(query);
        }

        /// <summary>
        /// 查詢功能目錄階層的排序清單
        /// </summary>
        public async Task<ApiResult<List<FuncGroup>>> GetFuncCatalogSortList(SysCatalogQueryParam param)
        {
            string sql = @"
            select F.*, C.FuncCatalogId, C.RootId, C.CatalogId, C.Seq
            from FuncCatalog as C
            left join Func as F
            on (F.FuncId = C.FuncId)
            where C.RootId = @RootId
            and C.CatalogId = @CatalogId
            and C.Activate = 1
            order by C.Seq";

            var query = (await DBUtil.QueryAsync<FuncGroup>(sql, param)).ToList();
            return new ApiResult<List<FuncGroup>>(query);
        }

        public override async Task<ApiResult<Func>> Insert(Func param)
        {
            var keyResult = await Get(new Func
            {
                FuncId = param.FuncId
            });

            if (keyResult.Data.Any())
                return new ApiResult<Func>(false, msg: MsgParam.KeyDuplication);

            if (!param.ViewName.IsNullOrWhiteSpace())
            {
                var viewNameResult = await Get(new Func
                {
                    SysId = param.SysId,
                    ViewName = param.ViewName
                });

                if (viewNameResult.Data.Any())
                    return new ApiResult<Func>(false, msg: $"{MsgParam.DataDuplication} SysId={param.SysId}, ViewName={param.ViewName}");
            }

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return await base.Insert(param);

        }

        public override async Task<ApiResult<Func>> Update(Func param)
        {
            if (!param.ViewName.IsNullOrWhiteSpace())
            {
                var viewNameResult = (await Get(new Func
                {
                    SysId = param.SysId,
                    ViewName = param.ViewName
                })).Data.Where(f => f.FuncId != param.FuncId);

                if (viewNameResult.Any())
                    return new ApiResult<Func>(false, msg: $"{MsgParam.DataDuplication} SysId={param.SysId}, ViewName={param.ViewName}");
            }

            param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return await base.Update(param);
        }

    }
}
