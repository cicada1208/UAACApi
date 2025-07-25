using Params;

namespace Repositorys.NISDB
{
    public abstract class UAACBaseRepository<TModel> : BaseRepository<TModel> where TModel : class
    {
        public UAACBaseRepository()
        {
            DBName = DBParam.DBName.UAAC;
            DBType = DBParam.DBType.SQLSERVER;
        }
    }
}
