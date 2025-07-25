using Lib;
using Lib.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Repositorys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UAACApi.Controllers
{
    public class FuncController : BaseController
    {
        public FuncController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<Func>>> Get([FromQuery] Func param) =>
            DB.FuncRepository.Get(param);

        /// <summary>
        /// 查詢功能目錄階層的加入清單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<Func>>> GetFuncCatalogAddList([FromQuery] FuncCatalogQueryParam param) =>
            DB.FuncRepository.GetFuncCatalogAddList(param);

        /// <summary>
        /// 查詢功能目錄階層的排序清單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<FuncGroup>>> GetFuncCatalogSortList([FromQuery] SysCatalogQueryParam param) =>
            DB.FuncRepository.GetFuncCatalogSortList(param);

        [HttpPost]
        public Task<ApiResult<Func>> Insert(Func param) =>
            DB.FuncRepository.Insert(param);

        [HttpPut]
        public Task<ApiResult<Func>> Update(Func param) =>
            DB.FuncRepository.Update(param);

    }
}
