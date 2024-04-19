using Lib;
using Lib.Api;
using Microsoft.Extensions.Options;
using Models;
using Repositorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Params.FuncParam;
using static Params.RoleParam;
using static Params.SysAppParam;

namespace Services
{
    public class BatchService : BaseService
    {
        private readonly string mUserId = "PortalBatch";

        public BatchService(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        /// <summary>
        /// SysApp 初始資料(只執行一次)
        /// </summary>
        public async Task<ApiResult<object>> InitSysApp()
        {
            var result = await DB.SysAppRepository.Insert(new SysApp
            {
                SysId = SysId.BasicRoot,
                SysName = "系統目錄階層",
                SysType = SysType.Root,
                Activate = true,
                MUserId = mUserId
            });

            result = await DB.SysAppRepository.Insert(new SysApp
            {
                SysId = SysId.CychWeb,
                SysName = "電子表單",
                SysType = SysType.Site,
                BasePath = @"http://web09.cych.org.tw/ASPNET/CychWebSites/NET2012/SSO/login.aspx",
                Activate = true,
                MUserId = mUserId
            });

            result = await DB.SysAppRepository.Insert(new SysApp
            {
                SysId = SysId.HIS,
                SysName = "醫療系統",
                SysType = SysType.HISExe,
                BasePath = @"C:\Acu62\wrun32.exe",
                Activate = true,
                MUserId = mUserId
            });

            result = await DB.SysAppRepository.Insert(new SysApp
            {
                SysId = SysId.iEMR,
                SysName = SysId.iEMR,
                SysType = SysType.VersionExe,
                BasePath = @"\\tfsrepo",
                SubPath = @"iemr2",
                Assembly = "WpfiEMR.exe",
                Limit = 1,
                Activate = true,
                MUserId = mUserId
            });

            result = await DB.SysAppRepository.Insert(new SysApp
            {
                SysId = SysId.CychPeri,
                SysName = "排檢系統",
                SysType = SysType.VersionExe,
                BasePath = @"\\tfsrepo",
                SubPath = @"desktop2\CychPeri-CI",
                Assembly = "CychPeri.exe",
                Limit = 1,
                Activate = true,
                MUserId = mUserId
            });

            var misSysList = (await DB.MISSYSTEMRepository.GetMainMISSYSTEM()).Data;

            if (misSysList.Any())
                result = await DB.SysAppRepository.Insert(new SysApp
                {
                    SysId = Guid.NewGuid().ToString(),
                    SysName = "嘉基資訊系統",
                    SysType = SysType.Catalog,
                    Activate = true,
                    MUserId = mUserId
                });

            foreach (var s in misSysList)
            {
                result = await DB.SysAppRepository.Insert(new SysApp
                {
                    SysId = s.SYSID.NullableToStr(),
                    SysName = s.SYSNAME.NullableToStr(),
                    SysType = SysType.CychMisExe,
                    Limit = 1,
                    Activate = true,
                    MUserId = mUserId
                });
            }

            result = await DB.SysAppRepository.Insert(new SysApp
            {
                SysId = SysId.Portal,
                SysName = SysId.Portal,
                SysType = SysType.VersionExe,
                BasePath = "...",
                Assembly = "...",
                Activate = true,
                MUserId = mUserId
            });

            return new ApiResult<object>(true);
        }

        /// <summary>
        /// SysCatalog：BasicRoot 初始資料(只執行一次)
        /// </summary>
        public async Task<ApiResult<object>> InitSysCatalogBasicRoot()
        {
            var result = await DB.SysCatalogRepository.Insert(new SysCatalog
            {
                RootId = SysId.BasicRoot,
                CatalogId = SysId.BasicRoot,
                SysId = SysId.CychWeb,
                Seq = 1,
                Activate = true,
                MUserId = mUserId
            });

            result = await DB.SysCatalogRepository.Insert(new SysCatalog
            {
                RootId = SysId.BasicRoot,
                CatalogId = SysId.BasicRoot,
                SysId = SysId.HIS,
                Seq = 2,
                Activate = true,
                MUserId = mUserId
            });

            result = await DB.SysCatalogRepository.Insert(new SysCatalog
            {
                RootId = SysId.BasicRoot,
                CatalogId = SysId.BasicRoot,
                SysId = SysId.iEMR,
                Seq = 3,
                Activate = true,
                MUserId = mUserId
            });

            result = await DB.SysCatalogRepository.Insert(new SysCatalog
            {
                RootId = SysId.BasicRoot,
                CatalogId = SysId.BasicRoot,
                SysId = SysId.CychPeri,
                Seq = 4,
                Activate = true,
                MUserId = mUserId
            });

            var cychMisCatalog = (await DB.SysAppRepository.Get(new SysApp
            {
                SysName = "嘉基資訊系統",
                SysType = SysType.Catalog,
                Activate = true
            })).Data.FirstOrDefault();

            if (cychMisCatalog != null)
            {
                result = await DB.SysCatalogRepository.Insert(new SysCatalog
                {
                    RootId = SysId.BasicRoot,
                    CatalogId = SysId.BasicRoot,
                    SysId = cychMisCatalog.SysId,
                    Seq = 5,
                    Activate = true,
                    MUserId = mUserId
                });

                var misSysList = (await DB.MISSYSTEMRepository.GetMainMISSYSTEM()).Data;
                short seq = 0;
                foreach (var s in misSysList)
                {
                    result = await DB.SysCatalogRepository.Insert(new SysCatalog
                    {
                        RootId = SysId.BasicRoot,
                        CatalogId = cychMisCatalog.SysId,
                        SysId = s.SYSID.NullableToStr(),
                        Seq = ++seq,
                        Activate = true,
                        MUserId = mUserId
                    });
                }
            }

            return new ApiResult<object>(true);
        }

        /// <summary>
        /// UserPermission：iEMR 初始資料(只執行一次)
        /// </summary>
        public async Task<ApiResult<object>> InitUserPermissionEMR()
        {
            string sql = @"
            select pwd.*
            from zp_mpwd as pwd
            inner join mg_mnid as nid
            on (
                nid_id = '5100' 
                and (substring(pwd_user,1,8) = 'R99'+nid_code or substring(pwd_user,1,8) = 'P11'+nid_code)
            )
            where substring(nid_rec,35,2) <> 'Z0' --非離職";

            var iEMRUsers = MoreLinq.MoreEnumerable.DistinctBy(await DB.Syb2.QueryAsync<Zp_mpwd>(sql),
                user => user.pwd_user.SybSubStr(4, 5)).ToList();

            ApiResult<UserPermission> userPermissionResult;
            string userId = string.Empty;
            foreach (var user in iEMRUsers)
            {
                userId = user.pwd_user.SybSubStr(4, 5);
                userId = Utils.Common.PreProcessUserId(userId);
                userPermissionResult = await DB.UserPermissionRepository.Insert(new UserPermission
                {
                    UserId = userId,
                    SysId = SysId.iEMR,
                    FuncId = string.Empty,
                    QueryAuth = true,
                    AddAuth = false,
                    ModifyAuth = false,
                    DeleteAuth = false,
                    ExportAuth = false,
                    PrintAuth = false,
                    Activate = true,
                    MUserId = mUserId
                });
            }

            return new ApiResult<object>(true);
        }

        /// <summary>
        /// UserPermission：CychMis 初始資料(只執行一次)
        /// </summary>
        public async Task<ApiResult<object>> InitUserPermissionCychMis()
        {
            string sql = @"
            select a.EMPNO, a.ROLE, b.*
            from USERWORKS as a
            inner join MISSYSTEM as b
            on a.SYSID=b.SYSID 
            where (b.SUBSYS = 'N'  or b.SUBSYS = 'G')   
            order by b.SYSID, a.EMPNO";

            var userMisSys = (await DB.MISSYS.QueryAsync<USERMISSYSTEM>(sql)).ToList();

            List<USERMISSYSTEM> groupSys;
            ApiResult<UserPermission> userPermissionResult;
            string userId = string.Empty;
            foreach (var g in userMisSys.GroupBy(s => s.SYSID))
            {
                groupSys = g.ToList();

                foreach (var gs in groupSys)
                {
                    userId = Utils.Common.PreProcessUserId(gs.EMPNO);

                    userPermissionResult = await DB.UserPermissionRepository.Insert(new UserPermission
                    {
                        UserId = userId,
                        SysId = g.Key,
                        FuncId = string.Empty,
                        QueryAuth = gs.ROLE.Contains("S"),
                        AddAuth = gs.ROLE.Contains("I"),
                        ModifyAuth = gs.ROLE.Contains("U"),
                        DeleteAuth = gs.ROLE.Contains("D"),
                        ExportAuth = false,
                        PrintAuth = false,
                        Activate = true,
                        MUserId = mUserId
                    });
                }
            }

            return new ApiResult<object>(true);
        }

        /// <summary>
        /// <para>RoleUser、RolePermission：CychWeb and HIS 初始資料(只執行一次)</para>
        /// <para>Role：基本功能群組</para>
        /// </summary>
        public async Task<ApiResult<object>> InitRoleUserPermissionCychWebAndHIS()
        {
            string basicRoleId = RoleId.BasicRole;

            var roleResult = (await DB.RoleRepository.Insert(new Role
            {
                RoleId = basicRoleId,
                RoleName = "基本功能群組",
                Activate = true,
                MUserId = mUserId
            }));

            var roleUserResult = (await DB.RoleUserRepository.Insert(new RoleUser
            {
                RoleId = basicRoleId,
                CpnyId = string.Empty,
                DeptNo = string.Empty,
                Possie = string.Empty,
                Attribute = string.Empty,
                UserId = string.Empty,
                Activate = true,
                MUserId = mUserId
            }));

            var rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = basicRoleId,
                SysId = SysId.CychWeb,
                FuncId = string.Empty,
                QueryAuth = true,
                AddAuth = false,
                ModifyAuth = false,
                DeleteAuth = false,
                ExportAuth = false,
                PrintAuth = false,
                Activate = true,
                MUserId = mUserId
            }));

            rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = basicRoleId,
                SysId = SysId.HIS,
                FuncId = string.Empty,
                QueryAuth = true,
                AddAuth = false,
                ModifyAuth = false,
                DeleteAuth = false,
                ExportAuth = false,
                PrintAuth = false,
                Activate = true,
                MUserId = mUserId
            }));

            return new ApiResult<object>(true);
        }

        /// <summary>
        /// Func、FuncCatalog、RoleUser、RolePermission：Portal 初始資料(只執行一次)
        /// <para>Role：Portal-系統管理員</para>
        /// </summary>
        public async Task<ApiResult<object>> InitDataPortal()
        {
            var funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = FuncId.PortalRoot,
                SysId = SysId.Portal,
                FuncName = "Portal 功能目錄階層",
                FuncType = FuncType.Root,
                Activate = true,
                MUserId = mUserId
            });

            string sysFuncMaintainCatalgId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = sysFuncMaintainCatalgId,
                SysId = SysId.Portal,
                FuncName = "系統功能維護作業",
                FuncType = FuncType.Catalog,
                Activate = true,
                MUserId = mUserId
            });

            string sysAppPageFuncId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = sysAppPageFuncId,
                SysId = SysId.Portal,
                FuncName = "系統設定",
                FuncType = FuncType.WpfPage,
                BasePath = "SysAppPage",
                ViewName = "SysAppPage",
                Activate = true,
                MUserId = mUserId
            });

            string funcPageFuncId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = funcPageFuncId,
                SysId = SysId.Portal,
                FuncName = "功能設定",
                FuncType = FuncType.WpfPage,
                BasePath = "FuncPage",
                ViewName = "FuncPage",
                Activate = true,
                MUserId = mUserId
            });

            string sysCatalogPageFuncId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = sysCatalogPageFuncId,
                SysId = SysId.Portal,
                FuncName = "系統目錄階層設定",
                FuncType = FuncType.WpfPage,
                BasePath = "SysCatalogPage",
                ViewName = "SysCatalogPage",
                Activate = true,
                MUserId = mUserId
            });

            string funcCatalogPageFuncId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = funcCatalogPageFuncId,
                SysId = SysId.Portal,
                FuncName = "功能目錄階層設定",
                FuncType = FuncType.WpfPage,
                BasePath = "FuncCatalogPage",
                ViewName = "FuncCatalogPage",
                Activate = true,
                MUserId = mUserId
            });

            string permissionMaintainCatalgId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = permissionMaintainCatalgId,
                SysId = SysId.Portal,
                FuncName = "權限維護作業",
                FuncType = FuncType.Catalog,
                Activate = true,
                MUserId = mUserId
            });

            string rolePageFuncId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = rolePageFuncId,
                SysId = SysId.Portal,
                FuncName = "角色群組設定",
                FuncType = FuncType.WpfPage,
                BasePath = "RolePage",
                ViewName = "RolePage",
                Activate = true,
                MUserId = mUserId
            });

            string permissionPageFuncId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = permissionPageFuncId,
                SysId = SysId.Portal,
                FuncName = "權限設定",
                FuncType = FuncType.WpfPage,
                BasePath = "PermissionPage",
                ViewName = "PermissionPage",
                Activate = true,
                MUserId = mUserId
            });

            string loginDeptFolderMethodId = Guid.NewGuid().ToString();
            funcResult = await DB.FuncRepository.Insert(new Func
            {
                FuncId = loginDeptFolderMethodId,
                SysId = SysId.Portal,
                FuncName = "登入科室資料夾",
                FuncType = FuncType.WpfMethod,
                BasePath = "ViewModels.FuncListViewModel",
                SubPath = "LoginDeptFolder",
                Activate = true,
                MUserId = mUserId
            });

            var funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = FuncId.PortalRoot,
                FuncId = loginDeptFolderMethodId,
                Seq = 1,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = FuncId.PortalRoot,
                FuncId = sysFuncMaintainCatalgId,
                Seq = 2,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = sysFuncMaintainCatalgId,
                FuncId = sysAppPageFuncId,
                Seq = 1,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = sysFuncMaintainCatalgId,
                FuncId = funcPageFuncId,
                Seq = 2,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = sysFuncMaintainCatalgId,
                FuncId = sysCatalogPageFuncId,
                Seq = 3,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = sysFuncMaintainCatalgId,
                FuncId = funcCatalogPageFuncId,
                Seq = 4,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = FuncId.PortalRoot,
                FuncId = permissionMaintainCatalgId,
                Seq = 3,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = permissionMaintainCatalgId,
                FuncId = rolePageFuncId,
                Seq = 1,
                Activate = true,
                MUserId = mUserId
            });

            funcCatalogResult = await DB.FuncCatalogRepository.Insert(new FuncCatalog
            {
                RootId = FuncId.PortalRoot,
                CatalogId = permissionMaintainCatalgId,
                FuncId = permissionPageFuncId,
                Seq = 2,
                Activate = true,
                MUserId = mUserId
            });

            var roleResult = (await DB.RoleRepository.Insert(new Role
            {
                RoleId = RoleId.PortalSysAdmin,
                RoleName = "Portal-系統管理員",
                Activate = true,
                MUserId = mUserId
            }));

            var roleUserResult = (await DB.RoleUserRepository.Insert(new RoleUser
            {
                RoleId = RoleId.PortalSysAdmin,
                CpnyId = "CYCH",
                DeptNo = "3240",
                Possie = string.Empty,
                Attribute = string.Empty,
                UserId = string.Empty,
                Activate = true,
                MUserId = mUserId
            }));

            var rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = RoleId.PortalSysAdmin,
                SysId = SysId.Portal,
                FuncId = sysAppPageFuncId,
                QueryAuth = true,
                AddAuth = true,
                ModifyAuth = true,
                DeleteAuth = true,
                ExportAuth = true,
                PrintAuth = true,
                Activate = true,
                MUserId = mUserId
            }));

            rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = RoleId.PortalSysAdmin,
                SysId = SysId.Portal,
                FuncId = funcPageFuncId,
                QueryAuth = true,
                AddAuth = true,
                ModifyAuth = true,
                DeleteAuth = true,
                ExportAuth = true,
                PrintAuth = true,
                Activate = true,
                MUserId = mUserId
            }));

            rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = RoleId.PortalSysAdmin,
                SysId = SysId.Portal,
                FuncId = sysCatalogPageFuncId,
                QueryAuth = true,
                AddAuth = true,
                ModifyAuth = true,
                DeleteAuth = true,
                ExportAuth = true,
                PrintAuth = true,
                Activate = true,
                MUserId = mUserId
            }));

            rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = RoleId.PortalSysAdmin,
                SysId = SysId.Portal,
                FuncId = funcCatalogPageFuncId,
                QueryAuth = true,
                AddAuth = true,
                ModifyAuth = true,
                DeleteAuth = true,
                ExportAuth = true,
                PrintAuth = true,
                Activate = true,
                MUserId = mUserId
            }));

            rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = RoleId.PortalSysAdmin,
                SysId = SysId.Portal,
                FuncId = rolePageFuncId,
                QueryAuth = true,
                AddAuth = true,
                ModifyAuth = true,
                DeleteAuth = true,
                ExportAuth = true,
                PrintAuth = true,
                Activate = true,
                MUserId = mUserId
            }));

            rolePermissionResult = (await DB.RolePermissionRepository.Insert(new RolePermission
            {
                RoleId = RoleId.PortalSysAdmin,
                SysId = SysId.Portal,
                FuncId = permissionPageFuncId,
                QueryAuth = true,
                AddAuth = true,
                ModifyAuth = true,
                DeleteAuth = true,
                ExportAuth = true,
                PrintAuth = true,
                Activate = true,
                MUserId = mUserId
            }));

            return new ApiResult<object>(true);
        }

    }
}
