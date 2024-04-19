using Params;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SQLSERVER, DBParam.DBName.UAAC, "SysUserFavorite")]
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
