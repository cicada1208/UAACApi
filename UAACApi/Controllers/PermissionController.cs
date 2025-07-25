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
    public class PermissionController : BaseController
    {
        private readonly PermissionService permissionService;

        public PermissionController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils,
            PermissionService permissionService)
            : base(settings.CurrentValue, db, apiUtils, utils)
        {
            this.permissionService = permissionService;
        }

        /// <summary>
        /// 查詢使用者系統選單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysGroup>>> GetSysGroup(string userId, string rootId) =>
            permissionService.GetSysGroup(userId, rootId);

        /// <summary>
        /// 查詢完整系統選單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysGroup>>> GetEntireSysGroup(string rootId) =>
            permissionService.GetEntireSysGroup(rootId);

        /// <summary>
        /// 查詢使用者最愛系統選單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysGroup>>> GetFavoriteSysGroup(string userId) =>
            permissionService.GetFavoriteSysGroup(userId);

        /// <summary>
        /// 查詢使用者功能選單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<FuncGroup>>> GetFuncGroup(string userId, string sysId, string rootId) =>
            permissionService.GetFuncGroup(userId, sysId, rootId);

        /// <summary>
        /// 查詢完整功能選單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<FuncGroup>>> GetEntireFuncGroup(string rootId) =>
            permissionService.GetEntireFuncGroup(rootId);

        /// <summary>
        /// 查詢使用者最愛功能選單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<FuncGroup>>> GetFavoriteFuncGroup(string userId, string sysId) =>
            permissionService.GetFavoriteFuncGroup(userId, sysId);

        /// <summary>
        /// 查詢使用者系統功能權限
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysFuncPermission>>> GetSysFuncPermission(string userId, string sysId = "") =>
            permissionService.GetSysFuncPermission(userId, sysId);

        /// <summary>
        /// 查詢使用者系統功能權限(詳細)
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysFuncPermissionDetail>>> GetSysFuncPermissionDetail(string userId, string sysId = "") =>
            permissionService.GetSysFuncPermissionDetail(userId, sysId);

        /// <summary>
        /// 查詢使用者系統功能權限(詳細 Distinct)
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysFuncPermissionDetailDistinct>>> GetSysFuncPermissionDetailDistinct(string userId, string sysId = "") =>
            permissionService.GetSysFuncPermissionDetailDistinct(userId, sysId);

        /// <summary>
        /// 查詢使用者 UserPermission (作用中)
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<UserPermission>>> GetUserPermission(string userId, string sysId = "") =>
            permissionService.GetUserPermission(userId, sysId);

        /// <summary>
        /// 查詢使用者 RolePermission (作用中)
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<RolePermission>>> GetRolePermission(string userId, string sysId = "") =>
            permissionService.GetRolePermission(userId, sysId);

        /// <summary>
        /// 查詢 RoleUser、RolePermission
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<RoleUserPermission>> GetRoleUserPermission([FromQuery] RoleUserPermissionParam param) =>
             permissionService.GetRoleUserPermission(param);

        /// <summary>
        /// 查詢使用者的角色群組 (作用中)
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<Role>>> GetUserRole(string userId) =>
             permissionService.GetUserRole(userId);

    }
}
