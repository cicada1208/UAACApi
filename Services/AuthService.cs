using Lib;
using Lib.Api;
using Lib.Api.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Models;
using Params;
using Repositorys;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService : BaseService
    {
        private WORKERS userInfoCychMis { get; set; }

        public AuthService(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        public Task<ApiResult<Auth>> GetUserInfo(string userId) =>
             GetUserInfoADVerify(userId, true);

        private async Task<ApiResult<Auth>> GetUserInfoADVerify(string userId, bool ADVerify)
        {
            Auth auth;

            userId = Utils.Common.PreProcessUserId(userId);

            // Hr
            auth = await ApiUtil.HttpClientExAsync<Auth>(
                HrRoute.Service(), HrRoute.BasicInfo.UserInfosV2 + userId,
                method: ApiParam.HttpVerbs.Get);

            // iEMR
            if (auth == null)
            {
                var userInfoiEMR = (await DB.Mg_mnidRepository.GetUser(
                    new Mg_mnid { nid_code = userId })).Data.FirstOrDefault();
                if (userInfoiEMR != null)
                    auth = new Auth { EmpId = userId, Name = userInfoiEMR.Name };
            }

            // CychMis
            if (auth == null)
            {
                if (userInfoCychMis == null)
                    userInfoCychMis = (await DB.WORKERSRepository.GetWORKER(userId)).Data;
                if (userInfoCychMis != null)
                    auth = new Auth { EmpId = userInfoCychMis.EMPNO, Name = userInfoCychMis.NAME };
            }

            // 若皆無資料可能為 AD 公用帳號
            if (auth == null)
            {
                if (ADVerify)
                {
                    bool result = await ServiceUtil.ADClient.VerifyUserAsync(userId);
                    if (result) auth = new Auth { EmpId = userId, Name = userId };
                }
                else
                    auth = new Auth { EmpId = userId, Name = userId };
            }

            if (auth != null)
                auth.EmpIdHis = Utils.Common.PostProcessUserId(auth.EmpId);

            return new ApiResult<Auth>(auth != null, data: auth, msg: auth != null ? MsgParam.ApiSuccess : MsgParam.LoginNoData);
        }

        public async Task<ApiResult<Auth>> GetHrUserInfo(string userId)
        {
            Auth auth;

            userId = Utils.Common.PreProcessUserId(userId);

            // Hr
            auth = await ApiUtil.HttpClientExAsync<Auth>(
                HrRoute.Service(), HrRoute.BasicInfo.UserInfosV2 + userId,
                method: ApiParam.HttpVerbs.Get);

            if (auth == null)
                auth = new Auth { EmpId = userId, Name = userId };

            auth.EmpIdHis = Utils.Common.PostProcessUserId(auth.EmpId);

            return new ApiResult<Auth>(auth);
        }

        public async Task<ApiResult<Auth>> PostLogin(HttpRequest request)
        {
            AuthMsg authMsg;
            bool decodeOK = request.Headers.DecodeBasicCredentials("Basic", out string userId, out string password);

            if (!decodeOK)
                authMsg = new AuthMsg { LoginSucc = false, ErrorMsg = MsgParam.LoginErrorFormat };
            else
            {
                userId = Utils.Common.PreProcessUserId(userId);
                authMsg = await Authentication(userId, password);
            }

            ApiResult<Auth> result;
            if (!authMsg.LoginSucc)
                result = new ApiResult<Auth>() { Succ = false, Code = HttpStatusCode.Unauthorized, Msg = authMsg.ErrorMsg };
            else
            {
                result = await GetUserInfoADVerify(userId, false);
                result.Data.Token = ApiUtils.Jwt.GenerateToken(result.Data.EmpId, expireMinutes: 7 * 24 * 60);
            }

            return result;
        }

        private async Task<bool> PassAD(string password)
        {
            string sql = "select * from ni_SysParameter where parameterName in @parameterName";

            var sysParam = await DB.NISDB.QueryAsync<SysParameter>(sql,
                 new { parameterName = new string[] { "CloseAD", "SysAdmKey" } });

            string closeAD = sysParam?.FirstOrDefault(p => p.parameterName == "CloseAD")?.value;
            string sysAdmKey = sysParam?.FirstOrDefault(p => p.parameterName == "SysAdmKey")?.value;
            bool passAD = (closeAD == "1") || (sysAdmKey == password);

            return passAD;
        }

        private async Task<AuthMsg> Authentication(string userId, string password)
        {
            AuthMsg authMsg = new AuthMsg { LoginSucc = false, ErrorMsg = string.Empty };
            bool passAD = await PassAD(password);
            authMsg.LoginSucc = passAD || await ServiceUtil.ADClient.VerifyUserAsync(userId);

            if (!authMsg.LoginSucc)
            {
                // 驗證 iEMR 帳密：實習帳號以AD驗證，不在此段驗證
                var authiEMR = (await DB.Zp_mpwdRepository.GetiEMRAuth(userId)).Data;
                if (authiEMR.FirstOrDefault(u => u.pwd_user.StartsWith("R99"))?.pwd_pass == password)
                    authMsg.LoginSucc = true;
                else if (authiEMR.FirstOrDefault(u => u.pwd_user.StartsWith("P11"))?.pwd_pass == password)
                    authMsg.LoginSucc = true;

                // 驗證 CychMis 帳密
                if (!authMsg.LoginSucc)
                {
                    userInfoCychMis = (await DB.WORKERSRepository.GetWORKER(userId)).Data;
                    if (userInfoCychMis?.PASSWORD == password)
                        authMsg.LoginSucc = true;
                }

                if ((!authMsg.LoginSucc) && (!authiEMR.Any()) && userInfoCychMis == null)
                    authMsg.ErrorMsg = MsgParam.LoginErrorId;
                else if (!authMsg.LoginSucc)
                    authMsg.ErrorMsg = MsgParam.LoginErrorPw;
            }
            else
            {
                authMsg.LoginSucc = passAD || await ServiceUtil.ADClient.VerifyAsync(userId, password);
                if (!authMsg.LoginSucc)
                    authMsg.ErrorMsg = MsgParam.LoginErrorPw;
            }

            return authMsg;
        }

        class AuthMsg
        {
            public bool LoginSucc { get; set; }
            public string ErrorMsg { get; set; }
        }

    }
}
