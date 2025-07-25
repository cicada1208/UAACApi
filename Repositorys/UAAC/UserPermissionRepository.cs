using Lib;
using Models;
using Repositorys.NISDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositorys.UAAC
{
    public class UserPermissionRepository : UAACBaseRepository<UserPermission>
    {
        public override Task<ApiResult<List<UserPermission>>> Get(UserPermission param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);
            return base.Get(param);
        }

        public override Task<ApiResult<UserPermission>> Insert(UserPermission param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            //var result = await Get(new UserPermission
            //{
            //    UserId = param.UserId,
            //    SysId = param.SysId,
            //    FuncId = param.FuncId.IsNullOrWhiteSpace() ? " " : param.FuncId,
            //    Activate = true
            //});

            //if (result.Data.Any())
            //    return new ApiResult<UserPermission>(false, msg: $"{MsgParam.DataDuplication} UserId={param.UserId}, SysId={param.SysId}, FuncId={param.FuncId}");

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.UserPermissionId.IsNullOrWhiteSpace())
                param.UserPermissionId = Guid.NewGuid().ToString();
            if (param.FuncId.IsNullOrWhiteSpace())
                param.FuncId = string.Empty;
            return base.Insert(param);
        }

        public override Task<ApiResult<UserPermission>> Update(UserPermission param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            //if ((bool)param.Activate)
            //{
            //    var result = (await Get(new UserPermission
            //    {
            //        UserId = param.UserId,
            //        SysId = param.SysId,
            //        FuncId = param.FuncId.IsNullOrWhiteSpace() ? " " : param.FuncId,
            //        Activate = true
            //    })).Data.Where(up => up.UserPermissionId != param.UserPermissionId);

            //    if (result.Any())
            //        return new ApiResult<UserPermission>(false, msg: $"{MsgParam.DataDuplication} UserId={param.UserId}, SysId={param.SysId}, FuncId={param.FuncId}");
            //}

            param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.FuncId.IsNullOrWhiteSpace())
                param.FuncId = string.Empty;
            return base.Update(param);
        }

        //public async Task<ApiResult<List<UserPermission>>> BatchPatchAuth(List<UserPermission> userPermissionList)
        //{
        //    HashSet<string> updateProps = new();
        //    List<SqlCmdUtil> sqlCmds = new();

        //    updateProps.AddProps<UserPermission>(m => new
        //    {
        //        m.UserId,
        //        m.QueryAuth,
        //        m.AddAuth,
        //        m.ModifyAuth,
        //        m.DeleteAuth,
        //        m.ExportAuth,
        //        m.PrintAuth,
        //        m.Activate,
        //        m.MUserId,
        //        m.MDateTime
        //    });

        //    userPermissionList.ForEach(up =>
        //    {
        //        up.UserId = Utils.Common.PreProcessUserId(up.UserId);
        //        up.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        sqlCmds.Add(Utils.SqlBuild.Patch(typeof(UserPermission), up, updateProps));
        //    });

        //    int rowsAffected = await DBUtil.ExecuteAsync(sqlCmds);
        //    return new ApiResult<List<UserPermission>>(rowsAffected, msgType: ApiParam.ApiMsgType.UPDATE);
        //}

    }
}
