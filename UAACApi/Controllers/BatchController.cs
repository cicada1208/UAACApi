using Lib;
using Lib.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Repositorys;
using Services;
using System.Threading.Tasks;

namespace UAACApi.Controllers
{
    public class BatchController : BaseController
    {
        private readonly BatchService batchService;

        public BatchController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils,
            BatchService batchService)
            : base(settings.CurrentValue, db, apiUtils, utils)
        {
            this.batchService = batchService;
        }

        //[HttpPost("[action]")]
        //public Task<ApiResult<object>> InitSysApp() =>
        //    batchService.InitSysApp();

        //[HttpPost("[action]")]
        //public Task<ApiResult<object>> InitSysCatalogBasicRoot() =>
        //    batchService.InitSysCatalogBasicRoot();

        //[HttpPost("[action]")]
        //public Task<ApiResult<object>> InitUserPermissionEMR() =>
        //    batchService.InitUserPermissionEMR();

        //[HttpPost("[action]")]
        //public Task<ApiResult<object>> InitUserPermissionCychMis() =>
        //    batchService.InitUserPermissionCychMis();

        //[HttpPost("[action]")]
        //public Task<ApiResult<object>> InitRoleUserPermissionCychWebAndHIS() =>
        //    batchService.InitRoleUserPermissionCychWebAndHIS();

        //[HttpPost("[action]")]
        //public Task<ApiResult<object>> InitDataPortal() =>
        //    batchService.InitDataPortal();

    }
}
