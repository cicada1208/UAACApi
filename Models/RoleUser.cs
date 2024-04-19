using Params;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SQLSERVER, DBParam.DBName.UAAC, "RoleUser")]
    public class RoleUser
    {
        [Key]
        public string RoleUserId { get; set; }

        public string RoleId { get; set; }

        public string CpnyId { get; set; }

        public string DeptNo { get; set; }

        public string Possie { get; set; }

        public string Attribute { get; set; }

        public string UserId { get; set; }

        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
