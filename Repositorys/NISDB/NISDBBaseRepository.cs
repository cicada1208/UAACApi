using Params;

namespace Repositorys.NISDB
{
    public abstract class NISDBBaseRepository<TModel> : BaseRepository<TModel> where TModel : class
    {
        public NISDBBaseRepository()
        {
            DBName = DBParam.DBName.NISDB;
            DBType = DBParam.DBType.SQLSERVER;
        }
    }
}
