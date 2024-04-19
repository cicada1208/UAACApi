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
    public class FuncUserFavoriteController : BaseController
    {
        public FuncUserFavoriteController(IOptionsMonitor<AppSettings> settings,
             DBContext db,
             ApiUtilLocator apiUtils,
             UtilLocator utils)
             : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<FuncUserFavorite>>> Get([FromQuery] FuncUserFavorite param) =>
            DB.FuncUserFavoriteRepository.Get(param);

        [HttpPost]
        public Task<ApiResult<FuncUserFavorite>> Insert(FuncUserFavorite param) =>
            DB.FuncUserFavoriteRepository.Insert(param);

        [HttpPatch("[action]")]
        public Task<ApiResult<object>> PatchDeactivate(FuncUserFavorite param) =>
            DB.FuncUserFavoriteRepository.PatchDeactivate(param);

        [HttpPatch("[action]")]
        public Task<ApiResult<List<FuncUserFavorite>>> BatchPatchSeq(List<FuncUserFavorite> funcUserFavorites) =>
            DB.FuncUserFavoriteRepository.BatchPatchSeq(funcUserFavorites);

    }
}
