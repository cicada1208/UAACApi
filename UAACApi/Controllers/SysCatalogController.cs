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
    public class SysCatalogController : BaseController
    {
        public SysCatalogController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpPost("[action]")]
        public Task<ApiResult<List<SysCatalog>>> BatchInsertSysApp(SysCatalogAddSysApp addParam) => DB.SysCatalogRepository.BatchInsertSysApp(addParam);

        [HttpPatch("[action]")]
        public Task<ApiResult<object>> BatchPatchDeactivate(SysCatalogRemoveParam removeParam) => DB.SysCatalogRepository.BatchPatchDeactivate(removeParam);

        [HttpPatch("[action]")]
        public Task<ApiResult<List<SysCatalog>>> BatchPatchSeq(List<SysCatalog> sysCatalogs) =>
            DB.SysCatalogRepository.BatchPatchSeq(sysCatalogs);

    }
}
