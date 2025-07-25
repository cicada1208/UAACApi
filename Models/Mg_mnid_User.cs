using Lib;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    /// <summary>
    /// nid_id = '5100'
    /// </summary>
    [Lib.Attributes.Table("mg_mnid")]
    public class Mg_mnid_User : Mg_mnid
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [NotMapped]
        public string Name => nid_name.SybSubStr(1, 24).TrimEnd();

        /// <summary>
        /// 身份證號
        /// </summary>
        [NotMapped]
        public string ID => nid_trn.SybSubStr(1, 10).TrimEnd();

        /// <summary>
        /// 機關代碼
        /// </summary>
        [NotMapped]
        public string AgencyCode => nid_rec.SybSubStr(111, 10).TrimEnd();

        /// <summary>
        /// 單位
        /// </summary>
        [NotMapped]
        public string Unit => nid_rec.SybSubStr(1, 5).TrimEnd();

        /// <summary>
        /// 職稱
        /// </summary>
        [NotMapped]
        public string JobTitle => nid_rec.SybSubStr(6, 4).TrimEnd();

        /// <summary>
        /// 成本中心
        /// </summary>
        [NotMapped]
        public string CostCenter => nid_rec.SybSubStr(10, 5).TrimEnd();

        /// <summary>
        /// 職務編號1
        /// </summary>
        [NotMapped]
        public string JobNumber1 => nid_rec.SybSubStr(21, 7).TrimEnd();

        /// <summary>
        /// 職務編號2
        /// </summary>
        [NotMapped]
        public string JobNumber2 => nid_rec.SybSubStr(99, 10).TrimEnd();

        /// <summary>
        /// 到職日期
        /// </summary>
        [NotMapped]
        public string AppointmentDate => nid_rec.SybSubStr(76, 7).TrimEnd();

        /// <summary>
        /// 異動狀況
        /// </summary>
        [NotMapped]
        public string ChangeStatus => nid_rec.SybSubStr(35, 2).TrimEnd();

        /// <summary>
        /// 異動日期
        /// </summary>
        [NotMapped]
        public string ChangeDate => nid_rec.SybSubStr(37, 7).TrimEnd();

        /// <summary>
        /// 連絡分機
        /// </summary>
        [NotMapped]
        public string ExtensionNumber => nid_rec.SybSubStr(126, 6).TrimEnd();

        /// <summary>
        /// 專業證照
        /// </summary>
        [NotMapped]
        public string ProfessionalLicense => nid_rec.SybSubStr(164, 35).TrimEnd();

        /// <summary>
        /// 離職日期
        /// </summary>
        [NotMapped]
        public string ResignationDate => nid_rec.SybSubStr(132, 7).TrimEnd();

        /// <summary>
        /// 計薪類別
        /// </summary>
        [NotMapped]
        public string PayableCategory => nid_rec.SybSubStr(163, 1).TrimEnd();

        /// <summary>
        /// 屬性
        /// </summary>
        [NotMapped]
        public string Category => nid_rec.SybSubStr(17, 4).TrimEnd();

        /// <summary>
        /// 護理進階
        /// </summary>
        [NotMapped]
        public string NursingCompetence => nid_rec.SybSubStr(159, 4).TrimEnd();

        /// <summary>
        /// 護生老師
        /// </summary>
        [NotMapped]
        public string NursingTeacher => nid_rec.SybSubStr(199, 1).TrimEnd();

        /// <summary>
        /// 生日
        /// </summary>
        [NotMapped]
        public string Birthday => nid_rec.SybSubStr(48, 7).TrimEnd();

        /// <summary>
        /// 英文姓名
        /// </summary>
        [NotMapped]
        public string NameEng => nid_rec.SybSubStr(139, 20).TrimEnd();

    }
}
