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
    public class RolePermissionController : BaseController
    {
        public RolePermissionController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<RolePermission>>> Get([FromQuery] RolePermission param) =>
            DB.RolePermissionRepository.Get(param);

        [HttpPost]
        public Task<ApiResult<RolePermission>> Insert(RolePermission param) =>
            DB.RolePermissionRepository.Insert(param);

        [HttpPut]
        public Task<ApiResult<RolePermission>> Update(RolePermission param) =>
            DB.RolePermissionRepository.Update(param);

        [HttpPatch("[action]")]
        public Task<ApiResult<List<RolePermission>>> BatchPatchAuth(List<RolePermission> rolePermissionList) =>
            DB.RolePermissionRepository.BatchPatchAuth(rolePermissionList);
    }
}
