using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    public class CustomMessageContext : MessageContext<IRequestMessageBase, IResponseMessageBase>
    {
        public CustomMessageContext()
        {
        }
    }
}