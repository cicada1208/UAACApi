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
    public class RoleUserController : BaseController
    {
        public RoleUserController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<RoleUser>>> Get([FromQuery] RoleUser param) =>
            DB.RoleUserRepository.Get(param);

        [HttpPost]
        public Task<ApiResult<RoleUser>> Insert(RoleUser param) =>
            DB.RoleUserRepository.Insert(param);

        [HttpPut]
        public Task<ApiResult<RoleUser>> Update(RoleUser param) =>
            DB.RoleUserRepository.Update(param);
    }
}
