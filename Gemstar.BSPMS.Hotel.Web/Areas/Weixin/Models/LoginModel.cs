using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    public class LoginModel
    {
        /// <summary>
        /// 酒店列表
        /// </summary>
        public List<Common.Tools.KeyValuePairModel<string, string>> HotelList { get; set; }

        /// <summary>
        /// 主键ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 键ID
        /// </summary>
        public Guid KeyId { get; set; }
        /// <summary>
        /// OpenID
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public LoginModelStatus Status { get; set; }
    }

    public class LoginModelStatus
    {
        /// <summary>
        /// 图标
        /// </summary>
        public Icons Icon { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { get; set; }
    }

    public enum Icons
    {
        /// <summary>
        /// 成功，用于表示操作顺利达成
        /// </summary>
        success,
        /// <summary>
        /// 提示，用于表示信息提示；也常用于缺乏条件的操作拦截，提示用户所需信息
        /// </summary>
        info,
        /// <summary>
        /// 警告，用于表示操作后将引起后果的情况
        /// </summary>
        warn,
        /// <summary>
        /// 等待，用于表示等待
        /// </summary>
        waiting,
    }

}