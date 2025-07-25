using Params;

namespace Repositorys.MISSYS
{
    public abstract class MISSYSBaseRepository<TModel> : BaseRepository<TModel> where TModel : class
    {
        public MISSYSBaseRepository()
        {
            DBName = DBParam.DBName.MISSYS;
            DBType = DBParam.DBType.SQLSERVER;
        }
    }
}
