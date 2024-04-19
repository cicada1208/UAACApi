using Lib;
using Lib.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Repositorys;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UAACApi.Controllers
{
    public class UserPermissionController : BaseController
    {
        private readonly PermissionService permissionService;

        public UserPermissionController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils,
            PermissionService permissionService)
            : base(settings.CurrentValue, db, apiUtils, utils)
        {
            this.permissionService = permissionService;
        }

        [HttpGet]
        public Task<ApiResult<List<UserPermission>>> Get([FromQuery] UserPermission param) =>
            DB.UserPermissionRepository.Get(param);

        [HttpPost]
        public Task<ApiResult<UserPermission>> Insert(UserPermission param) =>
            permissionService.IntegrateInsertUserPermission(param);

        [HttpPut]
        public Task<ApiResult<UserPermission>> Update(UserPermission param) =>
            permissionService.IntegrateUpdateUserPermission(param);

        [HttpPatch("[action]")]
        public Task<ApiResult<List<UserPermission>>> BatchPatchAuth(List<UserPermission> userPermissionList) =>
            permissionService.IntegrateBatchPatchUserPermissionAuth(userPermissionList);

    }
}
