using Lib;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys.SYB2
{
    public class Zp_mpwdRepository : SYB2BaseRepository<Zp_mpwd>
    {
        /// <summary>
        /// 查詢 iEMR 權限
        /// </summary>
        public async Task<ApiResult<List<Zp_mpwd>>> GetiEMRAuth(string userId)
        {
            userId = Utils.Common.PostProcessUserId(userId);

            string sql = $@"
            select pwd.* 
            from zp_mpwd as pwd
            inner join mg_mnid as nid
            on (nid_id = '5100' and nid_code = '{userId}')
            where substring(pwd_user,1,8) in @pwd_user
            and substring(nid_rec,35,2) <> 'Z0'";

            var query = (await DBUtil.QueryAsync<Zp_mpwd>(sql,
                new { pwd_user = new string[] { $"R99{userId}", $"P11{userId}" } })).ToList();

            return new ApiResult<List<Zp_mpwd>>(query);
        }

    }
}
