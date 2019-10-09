using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{

    [Table("Item")]
    [LogCName("项目")]
    public class Item : IBasicDataCopyEntity, IEntityEnable
    {
        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        [LogAnywayWhenEdit]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        [BasicDataUpdate(UpdateName = "项目名称")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("别名")] 
        public string Alias { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("类别代码")]
        [BasicDataUpdate(UpdateName = "付款或消费类型")]
        public string ItemTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("类别名称")] 
        public string ItemTypeName { get; set; }

        [LogCName("价格")]
        [BasicDataUpdate(UpdateName = "含税单价")]
        public decimal? Price { get; set; }

        [LogCName("汇率")]
        [BasicDataUpdate(UpdateName = "汇率")]
        public decimal? Rate { get; set; }

        [LogCName("增值税率")]
        [BasicDataUpdate(UpdateName = "增值税率")]
        public decimal Taxrate { get; set; }

        [LogCName("计算间夜数")]
        [BasicDataUpdate(UpdateName = "统计间夜数")]
        public decimal? Nights { get; set; }

        [LogCName("是否找回")]
        [LogBool("是","否")]
        [BasicDataUpdate(UpdateName = "是否找回")]
        public bool? IsRetun { get; set; }

        [LogCName("是否可充值")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否可充值")]
        public bool? IsCharge { get; set; }

        [LogCName("是否可手工录入")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否可手工录入")]
        public bool? IsInput { get; set; }

        [LogCName("是否需要录入数量")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否录入数量")]
        public bool? IsQuantity { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("付款还是消费")]
        [LogIgnore]
        public string DcFlag { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("统计分类")]
        [BasicDataUpdate(UpdateName = "统计分类")]
        public string StaType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("处理方式")]
        [BasicDataUpdate(UpdateName = "处理方式")]
        public string Action { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票项目")]
        [BasicDataUpdate(UpdateName = "发票项目")]
        public string InvoiceItemid { get; set; }

        [LogCName("顺序")]
        [BasicDataUpdate(UpdateName = "排序号")]
        public int? Seqid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("拼音码")]
        public string Py { get; set; }

        /// <summary>
        /// 代码加上项目名，该字段在数据库中不存在
        /// </summary>
        [NotMapped]
        public string CodeName { get; set; }

        [LogCName("是否可积分")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否可积分")]
        public bool? Notscore { get; set; }

        [LogCName("是否业主费用")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "业主属性")]
        public bool IsOwnerFee { get; set; }

        [LogCName("是否计入业主房租")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否计入业主房租")]
        public bool IsOwnerAmount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("数据来源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 数据分发id
        /// </summary>
        [LogCName("数据分发id")]
        public string DataCopyId { get; set; }
    }
}
