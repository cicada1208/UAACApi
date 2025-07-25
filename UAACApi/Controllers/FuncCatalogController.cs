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
    public class FuncCatalogController : BaseController
    {
        public FuncCatalogController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpPost("[action]")]
        public Task<ApiResult<List<FuncCatalog>>> BatchInsertFunc(FuncCatalogAddFunc addParam) =>
            DB.FuncCatalogRepository.BatchInsertFunc(addParam);

        [HttpPatch("[action]")]
        public Task<ApiResult<object>> BatchPatchDeactivate(FuncCatalogRemoveParam removeParam) =>
            DB.FuncCatalogRepository.BatchPatchDeactivate(removeParam);

        [HttpPatch("[action]")]
        public Task<ApiResult<List<FuncCatalog>>> BatchPatchSeq(List<FuncCatalog> funcCatalogs) =>
            DB.FuncCatalogRepository.BatchPatchSeq(funcCatalogs);

    }
}
