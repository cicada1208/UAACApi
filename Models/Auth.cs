using System;

namespace Models
{
    public class Auth
    {
        /// <summary>
        /// 機構別代碼
        /// </summary>
        public string CpnyID { get; set; }

        /// <summary>
        /// 機構別名稱
        /// </summary>
        public string CoscName { get; set; }

        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string EmpId { get; set; }

        /// <summary>
        /// 使用者帳號 (醫療系統用：實習帳號截為5碼)
        /// </summary>
        public string EmpIdHis { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名字
        /// </summary>
        public string EngName { get; set; }

        /// <summary>
        /// 部門代碼
        /// </summary>
        public string DeptNo { get; set; }

        /// <summary>
        /// 組別代碼
        /// </summary>
        public string SubDeptNo { get; set; }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 職稱代碼
        /// </summary>
        public string Possie { get; set; }

        /// <summary>
        /// 職稱
        /// </summary>
        public string PosName { get; set; }

        /// <summary>
        /// 屬性代碼
        /// </summary>
        public string AttributeID { get; set; }

        /// <summary>
        /// 屬性
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// 在職狀態
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 分機
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// e-mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string TelNo { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        public string CellNo { get; set; }

        /// <summary>
        /// 職等職級
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 進階
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// 是否為主治醫師(完訓醫師以上)
        /// </summary>
        public bool IsVs { get; set; }

        /// <summary>
        /// 是否為R或PGY
        /// </summary>
        public bool IsPgyR { get; set; }

        /// <summary>
        /// 是否為醫師
        /// </summary>
        public bool IsDr { get; set; }

        /// <summary>
        /// 是否免刷卡
        /// </summary>
        public bool NotCard { get; set; }

        public string QuitDateStr { get; set; }

        /// <summary>
        /// 到職日
        /// </summary>
        public DateTime? ArrivalDate { get; set; }

        /// <summary>
        /// 休假基準日
        /// </summary>
        public DateTime? VacBaseDate { get; set; }

        /// <summary>
        /// 工時
        /// </summary>
        public double WorkHour { get; set; }

        public string Token { get; set; }
    }
}
