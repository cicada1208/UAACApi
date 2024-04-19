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
    public class SysUserFavoriteController : BaseController
    {
        public SysUserFavoriteController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<SysUserFavorite>>> Get([FromQuery] SysUserFavorite param) =>
            DB.SysUserFavoriteRepository.Get(param);

        [HttpPost]
        public Task<ApiResult<SysUserFavorite>> Insert(SysUserFavorite param) =>
            DB.SysUserFavoriteRepository.Insert(param);

        [HttpPatch("[action]")]
        public Task<ApiResult<object>> PatchDeactivate(SysUserFavorite param) =>
            DB.SysUserFavoriteRepository.PatchDeactivate(param);

        [HttpPatch("[action]")]
        public Task<ApiResult<List<SysUserFavorite>>> BatchPatchSeq(List<SysUserFavorite> sysUserFavorites) =>
            DB.SysUserFavoriteRepository.BatchPatchSeq(sysUserFavorites);

    }
}
