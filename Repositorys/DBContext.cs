using Lib;
using Params;
using Repositorys.MISSYS;
using Repositorys.NISDB;
using Repositorys.SYB2;
using Repositorys.UAAC;

namespace Repositorys
{
    public class DBContext
    {
        //private DBUtil _NIS;
        //public DBUtil NIS =>
        //    _NIS ??= new DBUtil(DBParam.DBName.NIS, DBParam.DBType.SYBASE);

        private DBUtil _NISDB;
        public DBUtil NISDB =>
            _NISDB ??= new DBUtil(DBParam.DBName.NISDB, DBParam.DBType.SQLSERVER);

        private DBUtil _Syb1;
        public DBUtil Syb1 =>
            _Syb1 ??= new DBUtil(DBParam.DBName.SYB1, DBParam.DBType.SYBASE);

        private DBUtil _Syb2;
        public DBUtil Syb2 =>
            _Syb2 ??= new DBUtil(DBParam.DBName.SYB2, DBParam.DBType.SYBASE);

        private DBUtil _UAAC;
        public DBUtil UAAC =>
            _UAAC ??= new DBUtil(DBParam.DBName.UAAC, DBParam.DBType.SQLSERVER);

        private DBUtil _MISSYS;
        public DBUtil MISSYS =>
            _MISSYS ??= new DBUtil(DBParam.DBName.MISSYS, DBParam.DBType.SQLSERVER);

        private Mg_mnidRepository _Mg_mnidRepository;
        public Mg_mnidRepository Mg_mnidRepository =>
            _Mg_mnidRepository ??= new Mg_mnidRepository();

        private SysParameterRepository _SysParameterRepository;
        public SysParameterRepository SysParameterRepository =>
            _SysParameterRepository ??= new SysParameterRepository();

        private Zp_mpwdRepository _Zp_mpwdRepository;
        public Zp_mpwdRepository Zp_mpwdRepository =>
            _Zp_mpwdRepository ??= new Zp_mpwdRepository();

        private SysLogRepository _SysLogRepository;
        public SysLogRepository SysLogRepository =>
            _SysLogRepository ??= new SysLogRepository();

        private MISSYSTEMRepository _MISSYSTEMRepository;
        public MISSYSTEMRepository MISSYSTEMRepository =>
            _MISSYSTEMRepository ??= new MISSYSTEMRepository();

        private WORKERSRepository _WORKERSRepository;
        public WORKERSRepository WORKERSRepository =>
            _WORKERSRepository ??= new WORKERSRepository();

        private SysAppRepository _SysAppRepository;
        public SysAppRepository SysAppRepository =>
            _SysAppRepository ??= new SysAppRepository();

        private SysCatalogRepository _SysCatalogRepository;
        public SysCatalogRepository SysCatalogRepository =>
            _SysCatalogRepository ??= new SysCatalogRepository();

        private RoleRepository _RoleRepository;
        public RoleRepository RoleRepository =>
            _RoleRepository ??= new RoleRepository();

        private RolePermissionRepository _RolePermissionRepository;
        public RolePermissionRepository RolePermissionRepository =>
            _RolePermissionRepository ??= new RolePermissionRepository();

        private RoleUserRepository _RoleUserRepository;
        public RoleUserRepository RoleUserRepository =>
            _RoleUserRepository ??= new RoleUserRepository();

        private UserPermissionRepository _UserPermissionRepository;
        public UserPermissionRepository UserPermissionRepository =>
            _UserPermissionRepository ??= new UserPermissionRepository();

        private FuncRepository _FuncRepository;
        public FuncRepository FuncRepository =>
            _FuncRepository ??= new FuncRepository();

        private FuncCatalogRepository _FuncCatalogRepository;
        public FuncCatalogRepository FuncCatalogRepository =>
            _FuncCatalogRepository ??= new FuncCatalogRepository();

        private USERWORKSRepository _USERWORKSRepository;
        public USERWORKSRepository USERWORKSRepository =>
            _USERWORKSRepository ??= new USERWORKSRepository();

        private SysUserFavoriteRepository _SysUserFavoriteRepository;
        public SysUserFavoriteRepository SysUserFavoriteRepository =>
            _SysUserFavoriteRepository ??= new SysUserFavoriteRepository();

        private FuncUserFavoriteRepository _FuncUserFavoriteRepository;
        public FuncUserFavoriteRepository FuncUserFavoriteRepository =>
            _FuncUserFavoriteRepository ??= new FuncUserFavoriteRepository();

    }
}
