using Lib;
using Models;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys.MISSYS
{
    public class WORKERSRepository : MISSYSBaseRepository<WORKERS>
    {
        public async Task<ApiResult<WORKERS>> GetWORKER(string EMPNO)
        {
            string empNoWhere = EMPNO.IsNumeric() ? "CAST(EMPNO as int) = @EMPNO AND (EMPNO < '99999')" : "EMPNO = @EMPNO";

            string sql = $"select * from WORKERS where {empNoWhere}";

            var worker = (await DBUtil.QueryAsync<WORKERS>(sql, new { EMPNO })).FirstOrDefault();

            return new ApiResult<WORKERS>(worker);
        }
    }
}
