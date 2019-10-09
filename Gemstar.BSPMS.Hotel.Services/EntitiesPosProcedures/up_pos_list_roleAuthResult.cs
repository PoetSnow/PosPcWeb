using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 查询用户权限列表
    /// 存储过程 up_pos_list_roleAuth 执行后的结果集对象
    /// </summary>
    public class up_pos_list_roleAuthResult
    {
        [Display(Name = "酒店ID")]
        public string hid { get; set; }
        
        [Display(Name= "角色ID")]
        public Guid RoleId { get; set; }
        
        [Display(Name= "权限ID")]
        public string AuthCode { get; set; }
        
        [Display(Name= "功能按钮权限值")]
        public long AuthButtonValue { get; set; }
    }
}
