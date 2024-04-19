using System.Collections.Generic;

namespace Models
{
    public class FuncCatalogAddFunc
    {
        public string RootId { get; set; }

        public string CatalogId { get; set; }

        public string MUserId { get; set; }

        public List<Func> Funcs { get; set; }
    }
}
