using Params;

namespace Repositorys.SYB2
{
    public abstract class SYB2BaseRepository<TModel> : BaseRepository<TModel> where TModel : class
    {
        public SYB2BaseRepository()
        {
            DBName = DBParam.DBName.SYB2;
            DBType = DBParam.DBType.SYBASE;
        }
    }
}
