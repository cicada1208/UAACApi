﻿using Params;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SQLSERVER, DBParam.DBName.UAAC, "UserPermission")]
    public class UserPermission
    {
        [Key]
        public string UserPermissionId { get; set; }

        public string UserId { get; set; }

        public string SysId { get; set; }

        public string FuncId { get; set; }

        public bool? QueryAuth { get; set; }

        public bool? AddAuth { get; set; }

        public bool? ModifyAuth { get; set; }

        public bool? DeleteAuth { get; set; }

        public bool? ExportAuth { get; set; }

        public bool? PrintAuth { get; set; }

        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
