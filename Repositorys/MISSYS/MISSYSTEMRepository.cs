using Lib;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys.MISSYS
{
    public class MISSYSTEMRepository : MISSYSBaseRepository<MISSYSTEM>
    {
        public async Task<ApiResult<List<USERMISSYSTEM>>> GetUSERMISSYSTEM(string EMPNO)
        {
            string empNoWhere = string.Empty;

            if (EMPNO.IsNumeric())
                empNoWhere = "CAST(a.EMPNO as int) = @EMPNO AND (a.EMPNO < '99999')";
            else
                empNoWhere = "a.EMPNO = @EMPNO";

            string sql = $@"
            select b.*, a.ROLE, a.EMPNO
            from USERWORKS as a
            inner join MISSYSTEM as b
            on a.SYSID=b.SYSID 
            where {empNoWhere}
            and (b.SUBSYS = 'N'  or b.SUBSYS = 'G')   
            order by cast(a.usecount as int ) desc, b.SYSNAME";

            var query = (await DBUtil.QueryAsync<USERMISSYSTEM>(sql, new { EMPNO })).ToList();

            return new ApiResult<List<USERMISSYSTEM>>(query);
        }

        public async Task<ApiResult<List<MISSYSTEM>>> GetMainMISSYSTEM()
        {
            string sql = @"
            select * from MISSYSTEM
            where SUBSYS in ('N','G')
            order by FOLDERNAME,SYSID";

            var query = (await DBUtil.QueryAsync<MISSYSTEM>(sql)).ToList();

            return new ApiResult<List<MISSYSTEM>>(query);
        }

    }
}
