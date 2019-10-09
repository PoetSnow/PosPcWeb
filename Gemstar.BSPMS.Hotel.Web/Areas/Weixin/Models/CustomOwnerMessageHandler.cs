using System.IO;
using System.Linq;
using System.Text;
using Gemstar.BSPMS.Common.Services.EF;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomOwnerMessageHandler : MessageHandler<CustomMessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */
        private const string _defaultText = "欢迎光临家信短租，点击进行身份绑定，享受更多服务。";

        public CustomOwnerMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            WeixinContext.ExpireMinutes = 3;
        }


        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var centerDbConnStr = MvcApplication.GetCenterDBConnStr();
            var centerDb = new DbCommonContext(centerDbConnStr);
            var domain = centerDb.Database.SqlQuery<string>("select name2 from m_v_codeListPub where typeCode = '05' and code = 'callbackDomain'").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(domain))
            {
                domain = "pms.gshis.com";
            }
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();
            var urlBuilder = new StringBuilder();
            urlBuilder.Append("https://open.weixin.qq.com/connect/oauth2/authorize?appid=").Append(MvcApplication.OwnerWeixinAppId)
                .Append("&redirect_uri=http%3a%2f%2f").Append(domain).Append("%2fweixin%2fownerlisten%2fquery&response_type=code&scope=snsapi_base&state=JxdPms#wechat_redirect");
            var article = new Article
            {
                Title = "欢迎",
                Description = _defaultText,
                Url = urlBuilder.ToString()
            };
            responseMessage.Articles.Add(article);
            return responseMessage;
        }
    }
}
