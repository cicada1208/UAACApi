using AutoMapper;
using Lib;
using Lib.Api;
using Microsoft.Extensions.Options;
using Models;
using Params;
using Repositorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MoreLinq.Extensions.LeftJoinExtension;
using static Params.FuncParam;
using static Params.SysAppParam;

namespace Services
{
    public class PermissionService : BaseService
    {
        private readonly IMapper mapper;
        private readonly AuthService authService;

        public PermissionService(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils,
            IMapper mapper,
            AuthService authService)
            : base(settings.CurrentValue, db, apiUtils, utils)
        {
            this.mapper = mapper;
            this.authService = authService;
        }

        /// <summary>
        /// 查詢使用者系統選單
        /// </summary>
        public async Task<ApiResult<List<SysGroup>>> GetSysGroup(string userId, string rootId)
        {
            List<SysGroup> userSysGroups = new();

            if (userId.IsNullOrWhiteSpace() || rootId.IsNullOrWhiteSpace())
                return new ApiResult<List<SysGroup>>(false, data: userSysGroups, msg: MsgParam.QueryDataNone);

            bool isRootActive = (await DB.SysAppRepository.Get(new SysApp { SysId = rootId, Activate = true })).Data.Any();
            if (!isRootActive)
                return new ApiResult<List<SysGroup>>(false, data: userSysGroups, msg: MsgParam.QueryDataNone);

            List<SysCatalog> sysCatalogs = (await DB.SysCatalogRepository.Get(
                new SysCatalog
                {
                    RootId = rootId,
                    Activate = true
                })).Data;

            var userSysFuncPermissions = (await GetSysFuncPermission(userId)).Data;
            List<string> sysIds = MoreLinq.MoreEnumerable.DistinctBy(userSysFuncPermissions, s => s.SysId)
                .Select(s => s.SysId).ToList();
            AddCatalogSysId(sysIds, sysCatalogs, sysIds, rootId);
            string sql = @"select * from SysApp where SysId in @SysId and Activate = 1";
            var userSysApps = (await DB.UAAC.QueryAsync<SysApp>(sql, new { SysId = sysIds })).ToList();

            List<SysGroup> userSysCatalogs = userSysApps.Join(sysCatalogs,
                s => s.SysId, c => c.SysId,
                (s, c) => new SysGroup
                {
                    SysCatalogId = c.SysCatalogId,
                    RootId = c.RootId,
                    CatalogId = c.CatalogId,
                    Seq = c.Seq,
                    SysId = s.SysId,
                    SysName = s.SysName,
                    SysType = s.SysType,
                    BasePath = s.BasePath,
                    SubPath = s.SubPath,
                    Assembly = s.Assembly,
                    Limit = s.Limit,
                    Activate = s.Activate,
                    MUserId = s.MUserId,
                    MDateTime = s.MDateTime
                }).ToList();

            List<SysUserFavorite> sysUserFavorites = (await DB.SysUserFavoriteRepository.Get(
                new SysUserFavorite { UserId = userId, Activate = true })).Data;
            userSysCatalogs.ForEach(sc => sc.Favorite = sysUserFavorites.Exists(f => f.SysId == sc.SysId));

            SetSysGroup(userSysCatalogs, userSysGroups, rootId, SysType.Root);

            return new ApiResult<List<SysGroup>>(userSysGroups);
        }

        /// <summary>
        /// 查詢完整系統選單
        /// </summary>
        public async Task<ApiResult<List<SysGroup>>> GetEntireSysGroup(string rootId)
        {
            List<SysGroup> sysGroups = new();

            if (rootId.IsNullOrWhiteSpace())
                return new ApiResult<List<SysGroup>>(false, data: sysGroups, msg: MsgParam.QueryDataNone);

            List<SysCatalog> sysCatalogs = (await DB.SysCatalogRepository.Get(
                new SysCatalog
                {
                    RootId = rootId,
                    Activate = true
                })).Data;

            List<string> sysIds = MoreLinq.MoreEnumerable.DistinctBy(sysCatalogs, s => s.SysId)
                .Select(s => s.SysId).ToList();
            string sql = @"select * from SysApp where SysId in @SysId";
            var sysApps = (await DB.UAAC.QueryAsync<SysApp>(sql, new { SysId = sysIds })).ToList();

            List<SysGroup> detailSysCatalogs = sysApps.Join(sysCatalogs,
                s => s.SysId, c => c.SysId,
                (s, c) => new SysGroup
                {
                    SysCatalogId = c.SysCatalogId,
                    RootId = c.RootId,
                    CatalogId = c.CatalogId,
                    Seq = c.Seq,
                    SysId = s.SysId,
                    SysName = s.SysName,
                    SysType = s.SysType,
                    BasePath = s.BasePath,
                    SubPath = s.SubPath,
                    Assembly = s.Assembly,
                    Limit = s.Limit,
                    Activate = s.Activate,
                    MUserId = s.MUserId,
                    MDateTime = s.MDateTime
                }).ToList();

            SetSysGroup(detailSysCatalogs, sysGroups, rootId, SysType.Root);

            return new ApiResult<List<SysGroup>>(sysGroups);
        }

        /// <summary>
        /// 查詢使用者最愛系統選單
        /// </summary>
        public async Task<ApiResult<List<SysGroup>>> GetFavoriteSysGroup(string userId)
        {
            List<SysGroup> userSysGroups = new();

            if (userId.IsNullOrWhiteSpace())
                return new ApiResult<List<SysGroup>>(false, data: userSysGroups, msg: MsgParam.QueryDataNone);

            var userSysFuncPermissions = (await GetSysFuncPermission(userId)).Data;
            List<string> sysIds = MoreLinq.MoreEnumerable.DistinctBy(userSysFuncPermissions, s => s.SysId)
                .Select(s => s.SysId).ToList();

            List<SysUserFavorite> sysUserFavorites = (await DB.SysUserFavoriteRepository.Get(
                new SysUserFavorite { UserId = userId, Activate = true })).Data;
            sysUserFavorites = sysUserFavorites.Where(f => sysIds.Contains(f.SysId)).OrderBy(f => f.Seq).ToList();
            sysIds = MoreLinq.MoreEnumerable.DistinctBy(sysUserFavorites, s => s.SysId)
                .Select(s => s.SysId).ToList();

            string sql = @"select * from SysApp where SysId in @SysId and Activate = 1";
            var userSysApps = (await DB.UAAC.QueryAsync<SysApp>(sql, new { SysId = sysIds })).ToList();

            userSysGroups = sysUserFavorites.Join(userSysApps,
                f => f.SysId, s => s.SysId,
                (f, s) => new SysGroup
                {
                    Seq = f.Seq,
                    SysUserFavoriteId = f.SysUserFavoriteId,
                    Favorite = true,
                    SysId = s.SysId,
                    SysName = s.SysName,
                    SysType = s.SysType,
                    BasePath = s.BasePath,
                    SubPath = s.SubPath,
                    Assembly = s.Assembly,
                    Limit = s.Limit,
                    Activate = s.Activate,
                    MUserId = s.MUserId,
                    MDateTime = s.MDateTime
                }).ToList();

            return new ApiResult<List<SysGroup>>(userSysGroups);
        }

        /// <summary>
        /// 查詢使用者功能選單
        /// </summary>
        public async Task<ApiResult<List<FuncGroup>>> GetFuncGroup(string userId, string sysId, string rootId)
        {
            List<FuncGroup> userFuncGroups = new();

            if (userId.IsNullOrWhiteSpace() || sysId.IsNullOrWhiteSpace() || rootId.IsNullOrWhiteSpace())
                return new ApiResult<List<FuncGroup>>(false, data: userFuncGroups, msg: MsgParam.QueryDataNone);

            bool isRootActive = (await DB.FuncRepository.Get(new Func { FuncId = rootId, Activate = true })).Data.Any();
            if (!isRootActive)
                return new ApiResult<List<FuncGroup>>(false, data: userFuncGroups, msg: MsgParam.QueryDataNone);

            List<FuncCatalog> funcCatalogs = (await DB.FuncCatalogRepository.Get(
                new FuncCatalog
                {
                    RootId = rootId,
                    Activate = true
                })).Data;

            var userSysFuncPermissions = (await GetSysFuncPermission(userId, sysId)).Data;
            List<string> funcIds = MoreLinq.MoreEnumerable.DistinctBy(userSysFuncPermissions,
                s => s.FuncId).Select(s => s.FuncId).ToList();
            AddCatalogFuncId(funcIds, funcCatalogs, funcIds, rootId);
            string sql = @"select * from Func where FuncId in @FuncId and Activate = 1";
            var userFuncs = (await DB.UAAC.QueryAsync<Func>(sql, new { FuncId = funcIds })).ToList();

            List<FuncGroup> userFuncCatalogs = userFuncs.Join(funcCatalogs,
                f => f.FuncId, c => c.FuncId,
                (f, c) => new FuncGroup
                {
                    FuncCatalogId = c.FuncCatalogId,
                    RootId = c.RootId,
                    CatalogId = c.CatalogId,
                    Seq = c.Seq,
                    FuncId = f.FuncId,
                    SysId = f.SysId,
                    FuncName = f.FuncName,
                    FuncType = f.FuncType,
                    BasePath = f.BasePath,
                    SubPath = f.SubPath,
                    Assembly = f.Assembly,
                    ViewName = f.ViewName,
                    ViewComponent = f.ViewComponent,
                    IconType = f.IconType,
                    Icon = f.Icon,
                    Limit = f.Limit,
                    Activate = f.Activate,
                    MUserId = f.MUserId,
                    MDateTime = f.MDateTime
                }).ToList();

            List<FuncUserFavorite> funcUserFavorites = (await DB.FuncUserFavoriteRepository.Get(
                new FuncUserFavorite { UserId = userId, SysId = sysId, Activate = true })).Data;
            userFuncCatalogs.ForEach(fc => fc.Favorite = funcUserFavorites.Exists(f => f.FuncId == fc.FuncId));

            SetFuncGroup(userFuncCatalogs, userFuncGroups, rootId, FuncType.Root);

            return new ApiResult<List<FuncGroup>>(userFuncGroups);
        }

        /// <summary>
        /// 查詢完整功能選單
        /// </summary>
        public async Task<ApiResult<List<FuncGroup>>> GetEntireFuncGroup(string rootId)
        {
            List<FuncGroup> funcGroups = new();

            if (rootId.IsNullOrWhiteSpace())
                return new ApiResult<List<FuncGroup>>(false, data: funcGroups, msg: MsgParam.QueryDataNone);

            List<FuncCatalog> funcCatalogs = (await DB.FuncCatalogRepository.Get(
                new FuncCatalog
                {
                    RootId = rootId,
                    Activate = true
                })).Data;

            List<string> funcIds = MoreLinq.MoreEnumerable.DistinctBy(funcCatalogs, f => f.FuncId)
                .Select(f => f.FuncId).ToList();
            string sql = @"select * from Func where FuncId in @FuncId";
            var funcs = (await DB.UAAC.QueryAsync<Func>(sql, new { FuncId = funcIds })).ToList();

            List<FuncGroup> detailFuncCatalogs = funcs.Join(funcCatalogs,
                f => f.FuncId, c => c.FuncId,
                (f, c) => new FuncGroup
                {
                    FuncCatalogId = c.FuncCatalogId,
                    RootId = c.RootId,
                    CatalogId = c.CatalogId,
                    Seq = c.Seq,
                    FuncId = f.FuncId,
                    SysId = f.SysId,
                    FuncName = f.FuncName,
                    FuncType = f.FuncType,
                    BasePath = f.BasePath,
                    SubPath = f.SubPath,
                    Assembly = f.Assembly,
                    ViewName = f.ViewName,
                    ViewComponent = f.ViewComponent,
                    IconType = f.IconType,
                    Icon = f.Icon,
                    Limit = f.Limit,
                    Activate = f.Activate,
                    MUserId = f.MUserId,
                    MDateTime = f.MDateTime
                }).ToList();

            SetFuncGroup(detailFuncCatalogs, funcGroups, rootId, FuncType.Root);

            return new ApiResult<List<FuncGroup>>(funcGroups);
        }

        /// <summary>
        /// 查詢使用者最愛功能選單
        /// </summary>
        public async Task<ApiResult<List<FuncGroup>>> GetFavoriteFuncGroup(string userId, string sysId)
        {
            List<FuncGroup> userFuncGroups = new();

            if (userId.IsNullOrWhiteSpace() || sysId.IsNullOrWhiteSpace())
                return new ApiResult<List<FuncGroup>>(false, data: userFuncGroups, msg: MsgParam.QueryDataNone);

            var userSysFuncPermissions = (await GetSysFuncPermission(userId, sysId)).Data;
            List<string> funcIds = MoreLinq.MoreEnumerable.DistinctBy(userSysFuncPermissions,
                s => s.FuncId).Select(s => s.FuncId).ToList();

            List<FuncUserFavorite> funcUserFavorites = (await DB.FuncUserFavoriteRepository.Get(
                new FuncUserFavorite { UserId = userId, SysId = sysId, Activate = true })).Data;
            funcUserFavorites = funcUserFavorites.Where(f => funcIds.Contains(f.FuncId)).OrderBy(f => f.Seq).ToList();
            funcIds = MoreLinq.MoreEnumerable.DistinctBy(funcUserFavorites, s => s.FuncId)
                .Select(s => s.FuncId).ToList();

            string sql = @"select * from Func where FuncId in @FuncId and Activate = 1";
            var userFuncs = (await DB.UAAC.QueryAsync<Func>(sql, new { FuncId = funcIds })).ToList();

            userFuncGroups = funcUserFavorites.Join(userFuncs,
                ff => ff.FuncId, f => f.FuncId,
                (ff, f) => new FuncGroup
                {
                    Seq = ff.Seq,
                    FuncUserFavoriteId = ff.FuncUserFavoriteId,
                    Favorite = true,
                    FuncId = f.FuncId,
                    SysId = f.SysId,
                    FuncName = f.FuncName,
                    FuncType = f.FuncType,
                    BasePath = f.BasePath,
                    SubPath = f.SubPath,
                    Assembly = f.Assembly,
                    ViewName = f.ViewName,
                    ViewComponent = f.ViewComponent,
                    IconType = f.IconType,
                    Icon = f.Icon,
                    Limit = f.Limit,
                    Activate = f.Activate,
                    MUserId = f.MUserId,
                    MDateTime = f.MDateTime
                }).ToList();

            return new ApiResult<List<FuncGroup>>(userFuncGroups);
        }



        /// <summary>
        /// 查詢使用者系統功能權限
        /// </summary>
        public async Task<ApiResult<List<SysFuncPermission>>> GetSysFuncPermission(string userId, string sysId = "")
        {
            if (userId.IsNullOrWhiteSpace())
                return new ApiResult<List<SysFuncPermission>>(false, msg: MsgParam.QueryDataNone);

            var userPermissions = (await GetUserPermission(userId, sysId)).Data;
            var rolePermissions = (await GetRolePermission(userId, sysId)).Data;

            List<SysFuncPermission> sysFuncPermissions = mapper.Map<List<SysFuncPermission>>(userPermissions);
            sysFuncPermissions.AddRange(mapper.Map<List<SysFuncPermission>>(rolePermissions));

            return new ApiResult<List<SysFuncPermission>>(sysFuncPermissions);
        }

        /// <summary>
        /// 查詢使用者系統功能權限(詳細)
        /// </summary>
        public async Task<ApiResult<List<SysFuncPermissionDetail>>> GetSysFuncPermissionDetail(string userId, string sysId = "")
        {
            if (userId.IsNullOrWhiteSpace())
                return new ApiResult<List<SysFuncPermissionDetail>>(false, msg: MsgParam.QueryDataNone);

            var sysFuncPermissions = (await GetSysFuncPermission(userId, sysId)).Data;
            var funcIds = MoreLinq.MoreEnumerable.DistinctBy(sysFuncPermissions, fp => fp.FuncId)
                .Select(fp => fp.FuncId).ToList();

            List<Func> funcs;
            if (funcIds.Any())
            {
                string sql = @"select * from Func where FuncId in @FuncId and Activate = 1";
                funcs = (await DB.UAAC.QueryAsync<Func>(sql, new { FuncId = funcIds })).ToList();
            }
            else
                funcs = new();

            Func defaultFunc = null;
            var pfList = sysFuncPermissions.LeftJoin(funcs, p => p.FuncId, f => f.FuncId,
                p => new { p, f = defaultFunc }, (p, f) => new { p, f });

            List<SysFuncPermissionDetail> sysFuncPermissionDetails = new();
            SysFuncPermissionDetail sysFuncPermissionDetail;
            foreach (var pf in pfList)
            {
                sysFuncPermissionDetail = mapper.Map<SysFuncPermissionDetail>(pf.f);
                sysFuncPermissionDetail = mapper.Map(pf.p, sysFuncPermissionDetail);
                sysFuncPermissionDetails.Add(sysFuncPermissionDetail);
            }

            return new ApiResult<List<SysFuncPermissionDetail>>(sysFuncPermissionDetails);
        }

        /// <summary>
        /// 查詢使用者系統功能權限(詳細 Distinct)
        /// </summary>
        public async Task<ApiResult<List<SysFuncPermissionDetailDistinct>>> GetSysFuncPermissionDetailDistinct(string userId, string sysId = "")
        {
            if (userId.IsNullOrWhiteSpace())
                return new ApiResult<List<SysFuncPermissionDetailDistinct>>(false, msg: MsgParam.QueryDataNone);

            var sysFuncPermissionDetails = (await GetSysFuncPermissionDetail(userId, sysId)).Data;

            var sysFuncPermissionDetailDistincts = mapper.Map<List<SysFuncPermissionDetailDistinct>>(sysFuncPermissionDetails);

            List<SysFuncPermissionDetailDistinct> result = new();
            foreach (var distinctP in MoreLinq.MoreEnumerable.DistinctBy(sysFuncPermissionDetailDistincts, p => new { p.SysId, p.FuncId }))
            {
                distinctP.QueryAuth = sysFuncPermissionDetailDistincts.Exists(p =>
                p.SysId == distinctP.SysId && p.FuncId == distinctP.FuncId && p.QueryAuth == true);

                distinctP.AddAuth = sysFuncPermissionDetailDistincts.Exists(p =>
                p.SysId == distinctP.SysId && p.FuncId == distinctP.FuncId && p.AddAuth == true);

                distinctP.ModifyAuth = sysFuncPermissionDetailDistincts.Exists(p =>
                p.SysId == distinctP.SysId && p.FuncId == distinctP.FuncId && p.ModifyAuth == true);

                distinctP.DeleteAuth = sysFuncPermissionDetailDistincts.Exists(p =>
                p.SysId == distinctP.SysId && p.FuncId == distinctP.FuncId && p.DeleteAuth == true);

                distinctP.ExportAuth = sysFuncPermissionDetailDistincts.Exists(p =>
                p.SysId == distinctP.SysId && p.FuncId == distinctP.FuncId && p.ExportAuth == true);

                distinctP.PrintAuth = sysFuncPermissionDetailDistincts.Exists(p =>
                p.SysId == distinctP.SysId && p.FuncId == distinctP.FuncId && p.PrintAuth == true);

                result.Add(distinctP);
            }

            return new ApiResult<List<SysFuncPermissionDetailDistinct>>(result);
        }

        /// <summary>
        /// 查詢使用者 UserPermission (作用中)
        /// </summary>
        public async Task<ApiResult<List<UserPermission>>> GetUserPermission(string userId, string sysId = "")
        {
            if (userId.IsNullOrWhiteSpace())
                return new ApiResult<List<UserPermission>>(false, msg: MsgParam.QueryDataNone);

            userId = Utils.Common.PreProcessUserId(userId);

            string sql = @"
            select UP.* from UserPermission as UP
            left join SysApp as S
            on (S.SysId = UP.SysId)
            left join Func as F
            on (F.FuncId = UP.FuncId)
            where UP.UserId = @UserId
            and UP.Activate = 1
            and S.Activate = 1
            and (UP.FuncId='' or F.Activate = 1)";
            if (!sysId.IsNullOrWhiteSpace())
                sql += @"
            and UP.SysId = @SysId";

            var query = (await DB.UAAC.QueryAsync<UserPermission>(sql.ToString(),
                new UserPermission { UserId = userId, SysId = sysId })).ToList();

            return new ApiResult<List<UserPermission>>(query);
        }

        /// <summary>
        /// 查詢使用者 RolePermission (作用中)
        /// </summary>
        public async Task<ApiResult<List<RolePermission>>> GetRolePermission(string userId, string sysId = "")
        {
            if (userId.IsNullOrWhiteSpace())
                return new ApiResult<List<RolePermission>>(false, msg: MsgParam.QueryDataNone);

            userId = Utils.Common.PreProcessUserId(userId);
            Auth auth = (await authService.GetHrUserInfo(userId)).Data;

            StringBuilder sql = new();
            sql.AppendLine("select RP.* from RolePermission as RP");
            sql.AppendLine("left join SysApp as S");
            sql.AppendLine("on (S.SysId = RP.SysId)");
            sql.AppendLine("left join Func as F");
            sql.AppendLine("on (F.FuncId = RP.FuncId)");
            sql.AppendLine("left join Role as R");
            sql.AppendLine("on (R.RoleId = RP.RoleId)");
            sql.AppendLine("where RP.RoleId in (");
            sql.AppendLine("select RoleId from RoleUser where");
            sql.AppendLine(SetRoleUserCondition(auth));
            sql.AppendLine("and Activate = 1");
            sql.AppendLine(")");
            if (!sysId.IsNullOrWhiteSpace())
                sql.AppendLine("and RP.SysId = @SysId");
            sql.AppendLine("and RP.Activate = 1");
            sql.AppendLine("and S.Activate = 1");
            sql.AppendLine("and (RP.FuncId='' or F.Activate = 1)");
            sql.AppendLine("and R.Activate = 1");

            var query = (await DB.UAAC.QueryAsync<RolePermission>(sql.ToString(),
                new RolePermission { SysId = sysId })).ToList();

            return new ApiResult<List<RolePermission>>(query);
        }

        /// <summary>
        /// 查詢 RoleUser、RolePermission
        /// </summary>
        public async Task<ApiResult<RoleUserPermission>> GetRoleUserPermission(RoleUserPermissionParam param)
        {
            List<RoleUser> roleUsers = null;
            List<RolePermission> rolePermissions = null;
            List<string> roleIds = null;

            StringBuilder roleUserSql = new();
            roleUserSql.AppendLine("select * from RoleUser where");
            StringBuilder rolePermissionSql = new();
            rolePermissionSql.AppendLine("select * from RolePermission where");

            switch (param.Option)
            {
                case RoleUserPermissionParamOption.ByUserInfo:
                    if (param.UserId.IsNullOrWhiteSpace())
                        return new ApiResult<RoleUserPermission>(false, msg: MsgParam.ParamNone);

                    param.UserId = Utils.Common.PreProcessUserId(param.UserId);
                    Auth auth = (await authService.GetHrUserInfo(param.UserId)).Data;
                    roleUserSql.AppendLine(SetRoleUserCondition(auth));
                    if (param.Activate.HasValue && param.Activate.Value)
                        roleUserSql.AppendLine("and Activate = @Activate");
                    roleUsers = (await DB.UAAC.QueryAsync<RoleUser>(roleUserSql.ToString(), param)).ToList();
                    roleIds = MoreLinq.MoreEnumerable.DistinctBy(roleUsers, ru => ru.RoleId)
                        .Select(ru => ru.RoleId).ToList();
                    if (param.Activate.HasValue && !param.Activate.Value)
                        roleUsers = roleUsers.Where(r => r.Activate == param.Activate).ToList();

                    rolePermissionSql.AppendLine("RoleId in @RoleIds");
                    if (!param.SysId.IsNullOrWhiteSpace())
                        rolePermissionSql.AppendLine("and SysId = @SysId");
                    if (!param.Activate.IsNullOrWhiteSpace())
                        rolePermissionSql.AppendLine("and Activate = @Activate");
                    rolePermissions = (await DB.UAAC.QueryAsync<RolePermission>(rolePermissionSql.ToString(),
                        new { RoleIds = roleIds, SysId = param.SysId, Activate = param.Activate })).ToList();
                    break;
                case RoleUserPermissionParamOption.BySysId:
                    if (param.SysId.IsNullOrWhiteSpace())
                        return new ApiResult<RoleUserPermission>(false, msg: MsgParam.ParamNone);

                    rolePermissionSql.AppendLine("SysId = @SysId");
                    if (param.Activate.HasValue && param.Activate.Value)
                        rolePermissionSql.AppendLine("and Activate = @Activate");
                    rolePermissions = (await DB.UAAC.QueryAsync<RolePermission>(rolePermissionSql.ToString(), param)).ToList();
                    roleIds = MoreLinq.MoreEnumerable.DistinctBy(rolePermissions, ru => ru.RoleId)
                        .Select(ru => ru.RoleId).ToList();
                    if (param.Activate.HasValue && !param.Activate.Value)
                        rolePermissions = rolePermissions.Where(r => r.Activate == param.Activate).ToList();

                    roleUserSql.AppendLine("RoleId in @RoleIds");
                    if (!param.Activate.IsNullOrWhiteSpace())
                        roleUserSql.AppendLine("and Activate = @Activate");
                    roleUsers = (await DB.UAAC.QueryAsync<RoleUser>(roleUserSql.ToString(),
                        new { RoleIds = roleIds, Activate = param.Activate })).ToList();
                    break;
                case RoleUserPermissionParamOption.ByRoleId:
                    if (param.RoleId.IsNullOrWhiteSpace())
                        return new ApiResult<RoleUserPermission>(false, msg: MsgParam.ParamNone);

                    roleUserSql.AppendLine("RoleId = @RoleId");
                    if (!param.Activate.IsNullOrWhiteSpace())
                        roleUserSql.AppendLine("and Activate = @Activate");
                    roleUsers = (await DB.UAAC.QueryAsync<RoleUser>(roleUserSql.ToString(), param)).ToList();

                    rolePermissionSql.AppendLine("RoleId = @RoleId");
                    if (!param.SysId.IsNullOrWhiteSpace())
                        rolePermissionSql.AppendLine("and SysId = @SysId");
                    if (!param.Activate.IsNullOrWhiteSpace())
                        rolePermissionSql.AppendLine("and Activate = @Activate");
                    rolePermissions = (await DB.UAAC.QueryAsync<RolePermission>(rolePermissionSql.ToString(), param)).ToList();
                    break;
                default:
                    return new ApiResult<RoleUserPermission>(false, msg: MsgParam.ParamNone);

            }

            ApiResult<RoleUserPermission> result = new(new RoleUserPermission()
            {
                RoleUsers = roleUsers,
                RolePermissions = rolePermissions
            });
            return result;
        }

        /// <summary>
        /// 查詢使用者的角色群組 (作用中)
        /// </summary>
        public async Task<ApiResult<List<Role>>> GetUserRole(string userId)
        {
            if (userId.IsNullOrWhiteSpace())
                return new ApiResult<List<Role>>(false, msg: MsgParam.QueryDataNone);

            userId = Utils.Common.PreProcessUserId(userId);
            Auth auth = (await authService.GetHrUserInfo(userId)).Data;

            StringBuilder sql = new();
            sql.AppendLine("select * from Role");
            sql.AppendLine("where RoleId in (");
            sql.AppendLine("select RoleId from RoleUser where");
            sql.AppendLine(SetRoleUserCondition(auth));
            sql.AppendLine("and Activate = 1");
            sql.AppendLine(") and Activate = 1");

            var query = (await DB.UAAC.QueryAsync<Role>(sql.ToString())).ToList();

            return new ApiResult<List<Role>>(query);
        }

        /// <summary>
        /// 依使用者資訊組成 RoleUser 篩選條件
        /// </summary>
        private string SetRoleUserCondition(Auth auth)
        {
            Dictionary<string, string> colMap = new();
            colMap.Add(nameof(RoleUser.CpnyId), nameof(Auth.CpnyID));
            colMap.Add(nameof(RoleUser.DeptNo), nameof(Auth.DeptNo));
            colMap.Add(nameof(RoleUser.Possie), nameof(Auth.Possie));
            colMap.Add(nameof(RoleUser.Attribute), nameof(Auth.AttributeID));
            colMap.Add(nameof(RoleUser.UserId), nameof(Auth.EmpId));

            HashSet<string> allCols = colMap.Keys.ToHashSet();

            HashSet<string> hasValCols = new();
            if (!auth.CpnyID.IsNullOrWhiteSpace())
                hasValCols.Add(nameof(RoleUser.CpnyId));
            if (!auth.DeptNo.IsNullOrWhiteSpace())
                hasValCols.Add(nameof(RoleUser.DeptNo));
            if (!auth.Possie.IsNullOrWhiteSpace())
                hasValCols.Add(nameof(RoleUser.Possie));
            if (!auth.Attribute.IsNullOrWhiteSpace())
                hasValCols.Add(nameof(RoleUser.Attribute));
            if (!auth.EmpId.IsNullOrWhiteSpace())
                hasValCols.Add(nameof(RoleUser.UserId));
            int hasValColsNum = hasValCols.Count;

            StringBuilder where = new();
            StringBuilder filter = new();
            for (int k = 1; k <= hasValColsNum; k++)
            {
                foreach (var combs in hasValCols.Combinations(k))
                {
                    filter.Clear();
                    foreach (var col in allCols)
                    {
                        if (filter.Length > 0) filter.Append($"and ");
                        if (!combs.Contains(col))
                            filter.Append($"{col} = '' ");
                        else
                            filter.Append($"{col} = '{auth.GetPropertyValue(colMap[col])}' ");
                    }
                    if (where.Length > 0) where.Append("or ");
                    where.AppendLine($"({filter}) ");
                }
            }

            // CpnyId、DeptNo、Possie、Attribute、UserId皆為空白，代表所有通過驗證UserId皆屬於此群組
            if (where.Length > 0) where.Append("or ");
            where.Append("(CpnyId = '' and DeptNo = '' and Possie = '' and Attribute = '' and UserId = '') ");

            return $"( {where})";
        }

        /// <summary>
        /// 加入系統所屬的目錄
        /// </summary>
        /// <param name="result">結果</param>
        /// <param name="sysCatalogs">系統目錄階層</param>
        /// <param name="sysIds">使用者系統</param>
        private void AddCatalogSysId(List<string> result, List<SysCatalog> sysCatalogs, List<string> sysIds, string rootId)
        {
            var nextSysIds = sysCatalogs.Where(sc => sysIds.Contains(sc.SysId) && sc.CatalogId != rootId)
                .Select(sc => sc.CatalogId).Distinct().ToList();

            if (nextSysIds.Any())
            {
                result.AddRange(nextSysIds);
                AddCatalogSysId(result, sysCatalogs, nextSysIds, rootId);
            }
        }

        /// <summary>
        /// 設定 SysGroup
        /// </summary>
        /// <param name="userSysCatalogs">Raw Data</param>
        /// <param name="userSysGroups">所求樹狀群組格式</param>
        private void SetSysGroup(List<SysGroup> userSysCatalogs, List<SysGroup> userSysGroups,
            string catalogId, SysType? type)
        {
            switch (type)
            {
                case SysType.Root:
                case SysType.Catalog:
                    userSysGroups.AddRange(userSysCatalogs.Where(c => c.CatalogId == catalogId).OrderBy(c => c.Seq));
                    userSysGroups.ForEach(g => SetSysGroup(userSysCatalogs, g.SysApps, g.SysId, g.SysType));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 加入功能所屬的目錄
        /// </summary>
        /// <param name="result">結果</param>
        /// <param name="funcCatalogs">功能目錄階層</param>
        /// <param name="funcIds">使用者功能</param>
        private void AddCatalogFuncId(List<string> result, List<FuncCatalog> funcCatalogs, List<string> funcIds, string rootId)
        {
            var nextFuncIds = funcCatalogs.Where(fc => funcIds.Contains(fc.FuncId) && fc.CatalogId != rootId)
                .Select(sc => sc.CatalogId).Distinct().ToList();

            if (nextFuncIds.Any())
            {
                result.AddRange(nextFuncIds);
                AddCatalogFuncId(result, funcCatalogs, nextFuncIds, rootId);
            }
        }

        /// <summary>
        /// 設定 FuncGroup
        /// </summary>
        /// <param name="userFuncCatalogs">Raw Data</param>
        /// <param name="userFuncGroups">所求樹狀群組格式</param>
        private void SetFuncGroup(List<FuncGroup> userFuncCatalogs, List<FuncGroup> userFuncGroups,
            string catalogId, FuncType? type)
        {
            switch (type)
            {
                case FuncType.Root:
                case FuncType.Catalog:
                    userFuncGroups.AddRange(userFuncCatalogs.Where(c => c.CatalogId == catalogId).OrderBy(c => c.Seq));
                    userFuncGroups.ForEach(g => SetFuncGroup(userFuncCatalogs, g.Funcs, g.FuncId, g.FuncType));
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// iEMR and CychMis check，並設定 SqlCmd：USERWORKS
        /// </summary>
        private async Task<ApiResult<SqlCmdUtil>> SetSqlCmdUSERWORKS(UserPermission userPermission)
        {
            if (userPermission.SysId == SysId.iEMR)
            { // iEMR check
                if ((bool)userPermission.Activate)
                {
                    bool iEMRAuth = (await DB.Zp_mpwdRepository.GetiEMRAuth(userPermission.UserId)).Data.Any();
                    if (!iEMRAuth)
                        return new ApiResult<SqlCmdUtil>(false, msg: $"{userPermission.UserId} 不是 iEMR 系統使用者，無法建立權限。");
                    else
                        return new ApiResult<SqlCmdUtil>(true);
                }
                else
                    return new ApiResult<SqlCmdUtil>(true);
            }
            else
            {  // CychMis check
                SysApp cychMisSysApp = (await DB.SysAppRepository.Get(new SysApp
                {
                    SysId = userPermission.SysId,
                    SysType = SysType.CychMisExe
                })).Data.FirstOrDefault();

                if (cychMisSysApp == null) // 不是嘉基資訊系統則 pass
                    return new ApiResult<SqlCmdUtil>(true);
                else
                {
                    SqlCmdUtil sqlCmd;
                    if ((bool)!userPermission.Activate)
                    { // delete USERWORKS
                        string empNoWhere = userPermission.UserId.IsNumeric() ? "CAST(EMPNO as int) = @EMPNO AND (EMPNO < '99999')" : "EMPNO = @EMPNO";
                        sqlCmd = new();
                        sqlCmd.Builder.Append($"delete from USERWORKS where {empNoWhere} and SYSID = @SYSID");
                        sqlCmd.Param = new USERWORKS
                        {
                            EMPNO = userPermission.UserId,
                            SYSID = userPermission.SysId
                        };
                        return new ApiResult<SqlCmdUtil>(sqlCmd);
                    }
                    else
                    {
                        WORKERS cychMisUserInfo = (await DB.WORKERSRepository.GetWORKER(userPermission.UserId)).Data;
                        if (cychMisUserInfo == null)
                            return new ApiResult<SqlCmdUtil>(false, msg: $"{userPermission.UserId} 不是嘉基資訊系統使用者，無法建立權限。");

                        USERWORKS userWorks = (await DB.USERWORKSRepository.Get(new USERWORKS
                        {
                            EMPNO = cychMisUserInfo.EMPNO,
                            SYSID = userPermission.SysId
                        })).Data.FirstOrDefault();

                        string ROLE = $"{((bool)userPermission.QueryAuth ? "S" : "N")}{((bool)userPermission.AddAuth ? "I" : "N")}{((bool)userPermission.DeleteAuth ? "D" : "N")}{((bool)userPermission.ModifyAuth ? "U" : "N")}";

                        if (userWorks == null)
                        { // insert USERWORKS
                            sqlCmd = Utils.SqlBuild.Insert(typeof(USERWORKS), new USERWORKS
                            {
                                EMPNO = cychMisUserInfo.EMPNO,
                                SYSID = userPermission.SysId,
                                ROLE = ROLE,
                                USECOUNT = "0"
                            });
                            return new ApiResult<SqlCmdUtil>(sqlCmd);
                        }
                        else
                        { // update USERWORKS
                            userWorks.ROLE = ROLE;
                            sqlCmd = Utils.SqlBuild.Update(typeof(USERWORKS), userWorks);
                            return new ApiResult<SqlCmdUtil>(sqlCmd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 設定 SqlCmd：UserPermission and USERWORKS
        /// </summary>
        private async Task<ApiResult<SqlCmdUserPermissionAndUSERWORKS>> SetSqlCmdUserPermissionAndUSERWORKS(List<UserPermission> userPermissionList)
        {
            SqlCmdUserPermissionAndUSERWORKS sqlCmd = new();
            HashSet<string> updateProps = new();
            ApiResult<SqlCmdUtil> userWorksSqlCmdResult = null;
            IEnumerable<UserPermission> userPermissionDataDuplicationCheck;

            updateProps.AddProps<UserPermission>(m => new
            {
                m.UserId,
                m.QueryAuth,
                m.AddAuth,
                m.ModifyAuth,
                m.DeleteAuth,
                m.ExportAuth,
                m.PrintAuth,
                m.Activate,
                m.MUserId,
                m.MDateTime
            });

            foreach (var userPermission in userPermissionList)
            {
                userPermission.UserId = Utils.Common.PreProcessUserId(userPermission.UserId);
                userPermission.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if ((bool)userPermission.Activate)
                { // UserPermission check
                    if (userPermissionList.Exists(up =>
                     up.UserPermissionId != userPermission.UserPermissionId &&
                     up.UserId == userPermission.UserId &&
                     up.SysId == userPermission.SysId &&
                     up.FuncId == userPermission.FuncId &&
                     up.Activate == true))
                        return new ApiResult<SqlCmdUserPermissionAndUSERWORKS>(false,
                            msg: $"{MsgParam.DataDuplication} UserId={userPermission.UserId}, SysId={userPermission.SysId}, FuncId={userPermission.FuncId}");

                    userPermissionDataDuplicationCheck = (await DB.UserPermissionRepository.Get(new UserPermission
                    {
                        UserId = userPermission.UserId,
                        SysId = userPermission.SysId,
                        FuncId = userPermission.FuncId.IsNullOrWhiteSpace() ? " " : userPermission.FuncId,
                        Activate = true
                    })).Data.Where(up => up.UserPermissionId != userPermission.UserPermissionId);

                    if (userPermissionDataDuplicationCheck.Any())
                        return new ApiResult<SqlCmdUserPermissionAndUSERWORKS>(false,
                            msg: $"{MsgParam.DataDuplication} UserId={userPermission.UserId}, SysId={userPermission.SysId}, FuncId={userPermission.FuncId}");
                }

                userWorksSqlCmdResult = await SetSqlCmdUSERWORKS(userPermission);
                if (!userWorksSqlCmdResult.Succ)
                    return new ApiResult<SqlCmdUserPermissionAndUSERWORKS>(userWorksSqlCmdResult.Succ, msg: userWorksSqlCmdResult.Msg);
                if (userWorksSqlCmdResult?.Data != null)
                    sqlCmd.UserWorksSqlCmds.Add(userWorksSqlCmdResult.Data);

                sqlCmd.UserPermissionSqlCmds.Add(Utils.SqlBuild.Patch(typeof(UserPermission), userPermission, updateProps));
            }

            return new ApiResult<SqlCmdUserPermissionAndUSERWORKS>(sqlCmd);
        }

        /// <summary>
        /// 整合性新增 UserPermission
        /// </summary>
        public async Task<ApiResult<UserPermission>> IntegrateInsertUserPermission(UserPermission param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            if ((bool)param.Activate)
            {   // UserPermission check
                var userPermissionDataDuplicationCheck = await DB.UserPermissionRepository.Get(new UserPermission
                {
                    UserId = param.UserId,
                    SysId = param.SysId,
                    FuncId = param.FuncId.IsNullOrWhiteSpace() ? " " : param.FuncId,
                    Activate = true
                });

                if (userPermissionDataDuplicationCheck.Data.Any())
                    return new ApiResult<UserPermission>(false,
                        msg: $"{MsgParam.DataDuplication} UserId={param.UserId}, SysId={param.SysId}, FuncId={param.FuncId}");
            }

            ApiResult<SqlCmdUtil> userWorksSqlCmdResult = await SetSqlCmdUSERWORKS(param);
            if (!userWorksSqlCmdResult.Succ)
                return new ApiResult<UserPermission>(userWorksSqlCmdResult.Succ, msg: userWorksSqlCmdResult.Msg);

            bool userWorksSucc = true;
            if (userWorksSqlCmdResult?.Data != null)
                userWorksSucc = (await DB.MISSYS.ExecuteAsync(userWorksSqlCmdResult.Data.Sql, userWorksSqlCmdResult.Data.Param)) > 0;

            if (!userWorksSucc)
                return new ApiResult<UserPermission>(userWorksSucc, msg: $"嘉基資訊系統權限{MsgParam.SaveFailure}");
            else
                return await DB.UserPermissionRepository.Insert(param);
        }

        /// <summary>
        /// 整合性更新 UserPermission
        /// </summary>
        public async Task<ApiResult<UserPermission>> IntegrateUpdateUserPermission(UserPermission param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            if ((bool)param.Activate)
            { // UserPermission check
                var userPermissionDataDuplicationCheck = (await DB.UserPermissionRepository.Get(new UserPermission
                {
                    UserId = param.UserId,
                    SysId = param.SysId,
                    FuncId = param.FuncId.IsNullOrWhiteSpace() ? " " : param.FuncId,
                    Activate = true
                })).Data.Where(up => up.UserPermissionId != param.UserPermissionId);

                if (userPermissionDataDuplicationCheck.Any())
                    return new ApiResult<UserPermission>(false,
                        msg: $"{MsgParam.DataDuplication} UserId={param.UserId}, SysId={param.SysId}, FuncId={param.FuncId}");
            }

            ApiResult<SqlCmdUtil> userWorksSqlCmdResult = await SetSqlCmdUSERWORKS(param);
            if (!userWorksSqlCmdResult.Succ)
                return new ApiResult<UserPermission>(userWorksSqlCmdResult.Succ, msg: userWorksSqlCmdResult.Msg);

            bool userWorksSucc = true;
            if (userWorksSqlCmdResult?.Data != null)
                userWorksSucc = (await DB.MISSYS.ExecuteAsync(userWorksSqlCmdResult.Data.Sql, userWorksSqlCmdResult.Data.Param)) > 0;

            if (!userWorksSucc)
                return new ApiResult<UserPermission>(userWorksSucc, msg: $"嘉基資訊系統權限{MsgParam.SaveFailure}");
            else
                return await DB.UserPermissionRepository.Update(param);
        }

        /// <summary>
        /// 整合性批次更新 UserPermission Auth
        /// </summary>
        public async Task<ApiResult<List<UserPermission>>> IntegrateBatchPatchUserPermissionAuth(List<UserPermission> userPermissionList)
        {
            var result = await SetSqlCmdUserPermissionAndUSERWORKS(userPermissionList);

            if (!result.Succ)
                return new ApiResult<List<UserPermission>>(result.Succ, msg: result.Msg);
            else
            {
                bool userWorksSucc = true;
                if (result.Data.UserWorksSqlCmds.Any())
                    userWorksSucc = (await DB.MISSYS.ExecuteAsync(result.Data.UserWorksSqlCmds)) > 0;

                if (!userWorksSucc)
                    return new ApiResult<List<UserPermission>>(userWorksSucc, msg: $"嘉基資訊系統權限{MsgParam.SaveFailure}");
                else
                {
                    int rowsAffected = await DB.UAAC.ExecuteAsync(result.Data.UserPermissionSqlCmds);
                    return new ApiResult<List<UserPermission>>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
                }
            }
        }

        class SqlCmdUserPermissionAndUSERWORKS
        {
            public List<SqlCmdUtil> UserPermissionSqlCmds { get; set; } = new();
            public List<SqlCmdUtil> UserWorksSqlCmds { get; set; } = new();
        }

    }
}
