using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.Enums
{
    /// <summary>
    /// 微信扫码登录状态
    /// </summary>
    public enum WeixinQrcodeLoginStatus : int
    {
        /// <summary>
        /// 等待扫码
        /// </summary>
        QRCODE_WAIT = 0,
        /// <summary>
        /// 二维码过期
        /// </summary>
        QRCODE_INVALID = -2,
        /// <summary>
        /// 扫码成功
        /// </summary>
        QRCODE_SCANED = 100,
        /// <summary>
        /// 没有绑定操作员
        /// </summary>
        QRCODE_EMPTY_ACCOUNT = -1,
        /// <summary>
        /// 拒绝登录
        /// </summary>
        QRCODE_REFUSE_LOGIN = 102,
        /// <summary>
        /// 其他错误
        /// </summary>
        QRCODE_ERROR = 999,
        /// <summary>
        /// 登录成功
        /// </summary>
        QRCODE_LOGIN_SUCCESS = 101,
    }
}
