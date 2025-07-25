using Lib;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    /// <summary>
    /// nid_id = '0503'
    /// </summary>
    [Lib.Attributes.Table("mg_mnid")]
    public class Mg_mnid_Dr : Mg_mnid
    {
        /// <summary>
        /// 醫師代碼
        /// </summary>
        [NotMapped]
        public string DrNo => nid_code.TrimEnd();

        /// <summary>
        /// 醫師員編
        /// </summary>
        [NotMapped]
        public string EmpNo => nid_rec.SybSubStr(29, 5).TrimEnd();

        /// <summary>
        /// 姓名
        /// </summary>
        [NotMapped]
        public string Name => nid_name.SybSubStr(1, 12).TrimEnd();

    }
}
