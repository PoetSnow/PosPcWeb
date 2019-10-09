using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_Pos_list_Producelist 执行后的结果集对象
    /// </summary>
    public class up_Pos_list_ProducelistResult
    {
        public string 酒店代码 { get; set; }
        
        public string 单号 { get; set; }
        
        public Int64? 消费ID { get; set; }
        
        public string 营业点代码 { get; set; }
        
        public string 营业点名称 { get; set; }
        
        public string 台号 { get; set; }
        
        public string 台名 { get; set; }
        
        public string 客人姓名 { get; set; }
        
        public int? 人数 { get; set; }
        
        public string 点菜人 { get; set; }
        
        public DateTime? 点菜时间 { get; set; }
        
        public string 酒席编码 { get; set; }
        
        public string 酒席名称 { get; set; }
        
        public decimal? 酒席数量 { get; set; }
        
        public string 酒席单位 { get; set; }
        
        public decimal? 酒席单价 { get; set; }
        
        public decimal? 酒席金额 { get; set; }
        
        public string 项目编码 { get; set; }
        
        public string 项目名称 { get; set; }
        
        public string 项目英文名 { get; set; }
        
        public string 项目其它名 { get; set; }
        
        public string 单位 { get; set; }
        
        public string 单位英文名 { get; set; }
        
        public decimal? 数量 { get; set; }
        
        public decimal? 条只 { get; set; }
        
        public decimal? 单价 { get; set; }
        
        public decimal? 金额 { get; set; }
        
        public string 出品条码 { get; set; }
        
        public string 出品状态 { get; set; }
        
        public byte? 出品次数 { get; set; }
        
        public string 出品端口 { get; set; }
        
        public string 作法 { get; set; }
        
        public string 要求 { get; set; }
        
        public string 客位 { get; set; }
        
        public string 厨师 { get; set; }
        
        public string 推销员 { get; set; }
        
        public string 部门类别 { get; set; }
        
        public string 酒卡号 { get; set; }
        
        public string 备注 { get; set; }

        public DateTime? 处理时间 { get; set; }

        public string 消费状态 { get; set; }

        public long 流水号 { get; set; }
    }
}
