using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("VoucherSet")]
    [LogCName("凭证设置")]
    public class VoucherSet
    {
        [Key]
        [LogCName("id")]
        [LogIgnore]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        [LogIgnore]
        public string Hid { get; set; }

        public string Code { get; set; }

        [LogCName("消费类型  / 付款方式  / 合约单位中的一个")]//    type   varchar(30)  NOT NULL, --消费类型  / 付款方式  / 合约单位中的一个
        public string Type { get; set; }


        [LogCName("子类类型")]// 暂时只有一个就是市场分类， 以后可能会扩展
        public string TypeSub { get; set; }


        [LogCName("分类的id")]//假如分类是消费类型，那么这个字段就是保存具体消费类型的id,或者对应于type里的，付款方式id ,合单位 id
        public string Typeid { get; set; }


        [LogCName("分类的名称")]// 分类的名称消费类型的名称，付款方式名称，合单位 名称
        public string TypeName { get; set; }


        [LogCName("子类的id")]
        public string TypeSubid { get; set; }


        [LogCName("子类的名称")]
        public string TypeSubName { get; set; }


        [LogCName("科目代码")]
        public string SubjectCode { get; set; }


        [LogCName("科目名称")]
        public string SubjectName { get; set; }


        [LogCName("核算项目类型")]
        public string AccountType { get; set; }


        [LogCName("核算项目代码")]
        public string AccountCode { get; set; }

        [LogCName("核算项目名称")]
        public string AccountName { get; set; }


        [LogCName("顺序")]
        public int? Seqid { get; set; }


    }
}
