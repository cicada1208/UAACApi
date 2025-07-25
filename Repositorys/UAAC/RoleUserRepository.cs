using Lib;
using Models;
using Params;
using Repositorys.NISDB;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys.UAAC
{
    public class RoleUserRepository : UAACBaseRepository<RoleUser>
    {
        public override async Task<ApiResult<RoleUser>> Insert(RoleUser param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            var result = await Get(new RoleUser
            {
                RoleId = param.RoleId,
                CpnyId = param.CpnyId.IsNullOrWhiteSpace() ? " " : param.CpnyId,
                DeptNo = param.DeptNo.IsNullOrWhiteSpace() ? " " : param.DeptNo,
                Possie = param.Possie.IsNullOrWhiteSpace() ? " " : param.Possie,
                Attribute = param.Attribute.IsNullOrWhiteSpace() ? " " : param.Attribute,
                UserId = param.UserId.IsNullOrWhiteSpace() ? " " : param.UserId,
                Activate = true
            });

            if (result.Data.Any())
                return new ApiResult<RoleUser>(false, msg: $"{MsgParam.DataDuplication} RoleId={param.RoleId}, CpnyId={param.CpnyId}, DeptNo={param.DeptNo}, Possie={param.Possie}, Attribute={param.Attribute}, UserId={param.UserId}");

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.RoleUserId.IsNullOrWhiteSpace())
                param.RoleUserId = Guid.NewGuid().ToString();
            if (param.CpnyId.IsNullOrWhiteSpace())
                param.CpnyId = string.Empty;
            if (param.DeptNo.IsNullOrWhiteSpace())
                param.DeptNo = string.Empty;
            if (param.Possie.IsNullOrWhiteSpace())
                param.Possie = string.Empty;
            if (param.Attribute.IsNullOrWhiteSpace())
                param.Attribute = string.Empty;
            if (param.UserId.IsNullOrWhiteSpace())
                param.UserId = string.Empty;
            return await base.Insert(param);
        }

        public override async Task<ApiResult<RoleUser>> Update(RoleUser param)
        {
            param.UserId = Utils.Common.PreProcessUserId(param.UserId);

            var result = (await Get(new RoleUser
            {
                RoleId = param.RoleId,
                CpnyId = param.CpnyId.IsNullOrWhiteSpace() ? " " : param.CpnyId,
                DeptNo = param.DeptNo.IsNullOrWhiteSpace() ? " " : param.DeptNo,
                Possie = param.Possie.IsNullOrWhiteSpace() ? " " : param.Possie,
                Attribute = param.Attribute.IsNullOrWhiteSpace() ? " " : param.Attribute,
                UserId = param.UserId.IsNullOrWhiteSpace() ? " " : param.UserId,
                Activate = true
            })).Data.Where(rp => rp.RoleUserId != param.RoleUserId);

            if (result.Any())
                return new ApiResult<RoleUser>(false, msg: $"{MsgParam.DataDuplication} RoleId={param.RoleId}, CpnyId={param.CpnyId}, DeptNo={param.DeptNo}, Possie={param.Possie}, Attribute={param.Attribute}, UserId={param.UserId}");

            param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.CpnyId.IsNullOrWhiteSpace())
                param.CpnyId = string.Empty;
            if (param.DeptNo.IsNullOrWhiteSpace())
                param.DeptNo = string.Empty;
            if (param.Possie.IsNullOrWhiteSpace())
                param.Possie = string.Empty;
            if (param.Attribute.IsNullOrWhiteSpace())
                param.Attribute = string.Empty;
            if (param.UserId.IsNullOrWhiteSpace())
                param.UserId = string.Empty;
            return await base.Update(param);
        }

    }
}
