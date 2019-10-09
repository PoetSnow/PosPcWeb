using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.WeixinManage
{
    /// <summary>
    /// 微信带参数二维码接口服务
    /// </summary>
    public interface IQrCodeService
    {
        /// <summary>
        /// 增加带参数二维码，执行成功后返回申请二维码需要的场景值
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="idType">二维码绑定数据类型</param>
        /// <param name="idValue">二维码绑定数据值</param>
        /// <param name="expireSeconds">有效期秒数，默认为5分钟</param>
        /// <returns>用于二维码申请的场景值，同时也是记录的主键值</returns>
        int AddQrCode(string hid, QrCodeType idType, string idValue, int expireSeconds=300);
        /// <summary>
        /// 创建成功后将相应二维码的详细内容更新回数据库
        /// </summary>
        /// <param name="sceneId">场景值</param>
        /// <param name="ticket">用于换取二维码显示</param>
        /// <param name="url">二维码内容</param>
        void UpdateQrCode(int sceneId, string ticket, string url);
        /// <summary>
        /// 获取场景值对应的带参二维码信息
        /// </summary>
        /// <param name="sceneId">场景值</param>
        /// <returns>如果存在，则返回带参二维码实例，否则返回null</returns>
        WeixinQrcodes GetBySceneId(int sceneId);
    }
}
