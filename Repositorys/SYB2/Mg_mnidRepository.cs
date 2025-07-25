using Lib;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys.SYB2
{
    public class Mg_mnidRepository : SYB2BaseRepository<Mg_mnid>
    {
        /// <summary>
        /// 查詢人員
        /// </summary>
        public async Task<ApiResult<List<Mg_mnid_User>>> GetUser(Mg_mnid param)
        {
            param.nid_id = "5100";
            var query = (await DBUtil.QueryAsync<Mg_mnid_User>(param)).ToList();
            return new ApiResult<List<Mg_mnid_User>>(query);
        }

        /// <summary>
        /// 查詢醫師
        /// </summary>
        public async Task<ApiResult<List<Mg_mnid_Dr>>> GetDr(Mg_mnid param)
        {
            param.nid_id = "0503";
            var query = (await DBUtil.QueryAsync<Mg_mnid_Dr>(param)).ToList();
            return new ApiResult<List<Mg_mnid_Dr>>(query);
        }

    }
}
