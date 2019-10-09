using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    public class AuthLoginModel
    {
        public string OpenId { get; set; }
        public string QrcodeId { get; set; }
        public string QrcodeKey { get; set; }
        public string Mobile { get; set; }
        public string CheckCode { get; set; }
    }
}