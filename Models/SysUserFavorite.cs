using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("SysUserFavorite")]
    public class SysUserFavorite
    {
        [Key]
        public string SysUserFavoriteId { get; set; }

        public string UserId { get; set; }

        public string SysId { get; set; }

        public short? Seq { get; set; }

        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
