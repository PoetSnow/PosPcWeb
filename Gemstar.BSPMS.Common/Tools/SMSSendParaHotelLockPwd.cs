using System;
using System.Text;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 发送密码锁的短信参数类
    /// </summary>
    public class SMSSendParaHotelLockPwd : SMSSendParaHotel
    {
        /// <summary>
        /// 客人名
        /// </summary>
        public string GuestName { get; set; }
        /// <summary>
        /// 房间密码
        /// </summary>
        public string Lockpwd { get; set; }
        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 有效期开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 有效期结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        protected override string SendContent
        {
            get
            {
                //尊敬的<客人>，您的<房间号>房间门锁密码为<房间密码>；有效期至<EndDate>,感谢您的光临！【捷信达】
                var content = new StringBuilder();
                content.Append("尊敬的").Append(GuestName)
                    .Append("，您的").Append(RoomNo).Append("房间门锁密码为").Append(Lockpwd);
                content.Append("；有效期至").Append(EndDate.ToString("MM-dd HH:mm"))
                    .Append("；感谢您的光临！【").Append(HotelName).Append("】");
                return content.ToString();
            }
        }
    }
}
