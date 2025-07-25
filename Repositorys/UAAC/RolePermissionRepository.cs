using Lib;
using Models;
using Params;
using Repositorys.NISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys.UAAC
{
    public class RolePermissionRepository : UAACBaseRepository<RolePermission>
    {
        public override async Task<ApiResult<RolePermission>> Insert(RolePermission param)
        {
            var result = await Get(new RolePermission
            {
                RoleId = param.RoleId,
                SysId = param.SysId,
                FuncId = param.FuncId.IsNullOrWhiteSpace() ? " " : param.FuncId,
                Activate = true
            });

            if (result.Data.Any())
                return new ApiResult<RolePermission>(false, msg: $"{MsgParam.DataDuplication} RoleId={param.RoleId}, SysId={param.SysId}, FuncId={param.FuncId}");

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.RolePermissionId.IsNullOrWhiteSpace())
                param.RolePermissionId = Guid.NewGuid().ToString();
            if (param.FuncId.IsNullOrWhiteSpace())
                param.FuncId = string.Empty;
            return await base.Insert(param);
        }

        public override async Task<ApiResult<RolePermission>> Update(RolePermission param)
        {
            if ((bool)param.Activate)
            {
                var result = (await Get(new RolePermission
                {
                    RoleId = param.RoleId,
                    SysId = param.SysId,
                    FuncId = param.FuncId.IsNullOrWhiteSpace() ? " " : param.FuncId,
                    Activate = true
                })).Data.Where(rp => rp.RolePermissionId != param.RolePermissionId);

                if (result.Any())
                    return new ApiResult<RolePermission>(false, msg: $"{MsgParam.DataDuplication} RoleId={param.RoleId}, SysId={param.SysId}, FuncId={param.FuncId}");
            }

            param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.FuncId.IsNullOrWhiteSpace())
                param.FuncId = string.Empty;
            return await base.Update(param);
        }

        public async Task<ApiResult<List<RolePermission>>> BatchPatchAuth(List<RolePermission> rolePermissionList)
        {
            HashSet<string> updateProps = new();
            List<SqlCmdUtil> sqlCmds = new();
            IEnumerable<RolePermission> dataDuplicationCheck;

            updateProps.AddProps<RolePermission>(m => new
            {
                m.RolePermissionId,
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

            foreach (var rolePermission in rolePermissionList)
            {
                rolePermission.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if ((bool)rolePermission.Activate)
                {
                    if (rolePermissionList.Exists(rp =>
                     rp.RolePermissionId != rolePermission.RolePermissionId &&
                     rp.RoleId == rolePermission.RoleId &&
                     rp.SysId == rolePermission.SysId &&
                     rp.FuncId == rolePermission.FuncId &&
                     rp.Activate == true))
                        return new ApiResult<List<RolePermission>>(false,
                            msg: $"{MsgParam.DataDuplication} RoleId={rolePermission.RoleId}, SysId={rolePermission.SysId}, FuncId={rolePermission.FuncId}");

                    dataDuplicationCheck = (await Get(new RolePermission
                    {
                        RoleId = rolePermission.RoleId,
                        SysId = rolePermission.SysId,
                        FuncId = rolePermission.FuncId.IsNullOrWhiteSpace() ? " " : rolePermission.FuncId,
                        Activate = true
                    })).Data.Where(up => up.RolePermissionId != rolePermission.RolePermissionId);

                    if (dataDuplicationCheck.Any())
                        return new ApiResult<List<RolePermission>>(false,
                            msg: $"{MsgParam.DataDuplication} RoleId={rolePermission.RoleId}, SysId={rolePermission.SysId}, FuncId={rolePermission.FuncId}");
                }

                sqlCmds.Add(Utils.SqlBuild.Patch(typeof(RolePermission), rolePermission, updateProps));
            }

            int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
            return new ApiResult<List<RolePermission>>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        }

    }
}
