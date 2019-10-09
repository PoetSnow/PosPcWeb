using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.WeixinManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.WeixinManage
{
    public class QrCodeService : IQrCodeService
    {
        private DbCommonContext _centerDb;
        public QrCodeService(DbCommonContext centerDb)
        {
            _centerDb = centerDb;
        }
        /// <summary>
        /// 增加带参数二维码，执行成功后返回申请二维码需要的场景值
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="idType">二维码绑定数据类型</param>
        /// <param name="idValue">二维码绑定数据值</param>
        /// <param name="expireSeconds">有效期秒数，默认为5分钟</param>
        /// <returns>用于二维码申请的场景值，同时也是记录的主键值</returns>
        public int AddQrCode(string hid, QrCodeType idType, string idValue, int expireSeconds = 300)
        {
            var sceneId = _centerDb.Database.SqlQuery<int>("exec up_update_weixinQrCodes @hid=@hid,@idType=@idType,@idValue=@idValue,@expireSeconds=@expireSeconds"
                , new SqlParameter("@hid",hid??"")
                , new SqlParameter("@idType",idType.ToString())
                , new SqlParameter("@idValue",idValue)
                , new SqlParameter("@expireSeconds",expireSeconds)
                ).Single();
            return sceneId;
        }
        /// <summary>
        /// 创建成功后将相应二维码的详细内容更新回数据库
        /// </summary>
        /// <param name="sceneId">场景值</param>
        /// <param name="ticket">用于换取二维码显示</param>
        /// <param name="url">二维码内容</param>
        public void UpdateQrCode(int sceneId, string ticket, string url)
        {
            _centerDb.Database.ExecuteSqlCommand("update weixinQrcodes set ticket = @ticket,qrcodeContent=@url where sceneId = @sceneId"
                , new SqlParameter("@ticket", ticket ?? "")
                , new SqlParameter("@url", url ?? "")
                , new SqlParameter("sceneId", sceneId)
                );
        }
        /// <summary>
        /// 获取场景值对应的带参二维码信息
        /// </summary>
        /// <param name="sceneId">场景值</param>
        /// <returns>如果存在，则返回带参二维码实例，否则返回null</returns>
        public WeixinQrcodes GetBySceneId(int sceneId)
        {
            var qrcode = _centerDb.Database.SqlQuery<WeixinQrcodes>("select * from weixinQrcodes where sceneId = @sceneId", new SqlParameter("@sceneId", sceneId)).FirstOrDefault();
            return qrcode;
        }
    }
}
