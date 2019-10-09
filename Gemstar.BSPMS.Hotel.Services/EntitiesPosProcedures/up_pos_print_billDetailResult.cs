using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_print_billDetailResult
    {
        public long id { get; set; }
        public long? 序号 { get; set; }
        public string 客人姓名 { get; set; }
        public int? 人数 { get; set; }
        public string 餐台号 { get; set; }
        public string 开台卡号 { get; set; }
        public string 会员卡号 { get; set; }
        public string 合约单位名称 { get; set; }
        public string 消费人 { get; set; }
        public decimal? 金额折 { get; set; }
        public byte? 状态 { get; set; }
        public decimal? 服务费率 { get; set; }
        public int? 最低消费时长 { get; set; }

        public int? 账单打单次数 { get; set; }
        public int? 埋脚单打单次数 { get; set; }
        public string 开台备注 { get; set; }
        public string 收银备注 { get; set; }
        public string 可视单号 { get; set; }
        public string 手机号码 { get; set; }
        public DateTime? 登记营业日 { get; set; }
        public DateTime? 登记时间 { get; set; }
        public string 模块 { get; set; }
        public string 业务员 { get; set; }
        public string 关联号 { get; set; }
        public decimal? 最低消费 { get; set; }
        public string 自动备注 { get; set; }

        public string 项目代码 { get; set; }
        public string 消费项目 { get; set; }
        public string 单位代码 { get; set; }
        public string 单位 { get; set; }
        public decimal? 数量 { get; set; }
        public decimal? 称重条只 { get; set; }
        public decimal? 扣钝倍数 { get; set; }
        public decimal? 单价 { get; set; }
        public decimal? 作法加价 { get; set; }
        public decimal? 折前金额 { get; set; }
        public decimal? 折后金额 { get; set; }
        public decimal? 税额 { get; set; }
        public string 客位 { get; set; }
        public string 作法 { get; set; }
        public string 要求 { get; set; }
        public string 消费还是付款 { get; set; }
        public bool? 结账状态 { get; set; }
        public byte? 自动标志 { get; set; }

        public byte? 计费状态 { get; set; }
        public bool? 求和套餐 { get; set; }
        public bool? 求和套餐明细 { get; set; }
        public decimal? 套餐分摊金额 { get; set; }
        public decimal? 单道折扣 { get; set; }
        public decimal? 单道金额折 { get; set; }

        public string 批次 { get; set; }
        public decimal? 原价 { get; set; }
        public decimal? 会员价 { get; set; }
        public decimal? 会员折扣 { get; set; }
        public string 备注 { get; set; }
        public string 取消原因 { get; set; }

        public decimal? 折前总金额 { get; set; }
        public decimal? 折后总金额 { get; set; }
        public decimal? 折扣金额 { get; set; }
        public decimal? 折扣率 { get; set; }
        public decimal? 毛利率 { get; set; }
        public decimal? 服务费 { get; set; }
        public decimal? 消费余额 { get; set; }

        public string 班次 { get; set; }
        public string 市别 { get; set; }
        public decimal? 已付金额 { get; set; }
        public decimal? 未付金额 { get; set; }
        public decimal? 应付金额 { get; set; }

        public byte? 消费项目打单次数 { get; set; }
        public byte? 点菜单打单次数 { get; set; }
        public string 计费状态备注 { get; set; }
        public string 开台人 { get; set; }
    }
}