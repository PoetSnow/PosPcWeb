using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Data.SqlClient;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.EF.SystemManage
{
    public class PmsParaService : CRUDService<PmsPara>, IPmsParaService
    {
        private DbHotelPmsContext _pmsContext;
        public PmsParaService(DbHotelPmsContext db) : base(db, db.PmsParas)
        {
            _pmsContext = db;
        }

        public List<PmsPara> getHotelPara(string hid)
        {
            return _pmsContext.PmsParas.Where(w => w.Hid == hid && w.IsVisible == 0).ToList();
        }

        public List<PmsPara> GetPmsParas(string hid)
        {
            return _pmsContext.PmsParas.Where(w => w.Hid == hid).ToList();
        }

        /// <summary>
        /// 获取指定酒店的指定代码的系统参数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public string GetValue(string hid, string code)
        {
            var entity = _pmsContext.PmsParas.Where(w => w.Hid == hid && w.Code == code).FirstOrDefault();
            if (entity != null)
            {
                if (string.IsNullOrWhiteSpace(entity.Value))
                {
                    return entity.DefaultValue;
                }
                else
                {
                    return entity.Value;
                }
            }
            return null;
        }

        public bool isAllowOwner(string hid)
        {
            //得到是否启用业主功能的参数值
            var PmsPara = _pmsContext.PmsParas.Where(w => w.Hid == hid && w.Code == "isAllowOwner").FirstOrDefault();
            var isAllowOwner = "";
            if (PmsPara != null)
            {
                isAllowOwner = PmsPara.Value;
            }
            if (isAllowOwner == "1")
            {
                return true;
            }
            else {
                return false;
            }
        }

        public bool IsExistCleanRoom(string hid)
        {
            var list = _pmsContext.RoomStatuses.Where(w => w.Hid == hid && w.IsDirty == 2).ToList();
            return list.Count > 0;
        }

        /// <summary>
        /// 检查指定酒店是否启用清洁房检查功能
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>false:不启用，脏房转为净房，true:启用时，脏房先转为清洁房，</returns>
        public bool IsRoomCheck(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isRoomCheck");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查指定酒店是否启用短信模块，并且返回短信用户名和密码
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="username">短信用户名</param>
        /// <param name="password">短信密码</param>
        /// <returns>true:酒店或者酒店所属集团启用了短信模块，false:没有启用</returns>
        public bool IsSmsEnable(string hid, out string username, out string password)
        {
            var smsInfo = _pmsContext.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", hid ?? "")).SingleOrDefault();
            if(smsInfo == null)
            {
                username = "";
                password = "";
                return false;
            }else
            {
                username = smsInfo.UserName;
                password = smsInfo.Password;
                return smsInfo.Enable == "1";
            }
        }
        protected override PmsPara GetTById(string id)
        {
            var pmsPara = new PmsPara { Id = Guid.Parse(id) };
            return pmsPara;
        }

        /// <summary>
        /// 是否启用授权调价
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public bool IsAllowAuthAdjustPrice(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isAllowAuthAdjustPrice");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 是否启用授权冲减账务
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public bool isAllowAuthAbatementFolio(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isAllowAuthAbatementFolio");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 是否启用授权房租加收修改
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public bool isAllowAuthDayChargeFolio(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isAllowAuthDayChargeFolio");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否启用长租管理功能
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public bool IsPermanentRoom(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isPermanentRoom");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 是否启用扫描身份证保存图片功能
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public bool IsScanSavePhoto(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isSavephoto");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否启用脏房转净房生成报表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public bool IsDirtyLogToReportForm(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isDirtyLog");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 入帐付款后是否打印押金单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public bool IsPayPrintDeposit(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isPayPrintDeposit");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "0";//0:打印，1:不打印
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 团体默认价格代码
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public string GroupDefaultRateCode(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "groupDefaultRateCode");
                if (para == null)
                {
                    return null;
                }
                return string.IsNullOrWhiteSpace(para.Value) ? null : (hid + para.Value);//营销管理-价格体系设置-价格代码中的[代码]项
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 入住时是否自动打印RC单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public bool IsCheckInAutoPrintRC(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "isCheckInAutoPrintRC");
                if (para == null)
                {
                    return false;
                }
                return para.Value == "1";//0:不打印，1:打印 seqid=206
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取酒店自定义logo和名称信息，以便前端界面根据此信息进行显示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="hotelInfoService">用于获取酒店logo</param>
        /// <returns>酒店自定义logo和名称信息</returns>
        public HotelLogoAndNameInfo GetHotelLogoAndNameInfo(string hid, IHotelInfoService hotelInfoService)
        {
            var codeList = new List<string> {
                      "ShowLogoOfMain"
                    , "ShowGemStarLogo"
                    , "GemStarSystemTitle"
            };
            var paraList = _pmsContext.PmsParas.Where(c => c.Hid == hid && codeList.Contains(c.Code)).ToList();
            string LogoUrl = "";
            if (paraList.SingleOrDefault(w => w.Code == "ShowLogoOfMain")?.Value == "1")
            {
                LogoUrl = hotelInfoService.GetLogoUrl(hid);
            }
            var logoAndNameInfo = new HotelLogoAndNameInfo
            {
                LogoUrl = LogoUrl,
                GSSysTitle = paraList.SingleOrDefault(w => w.Code == "GemStarSystemTitle")?.Value,
                ShowGSLogo = paraList.SingleOrDefault(w => w.Code == "ShowGemStarLogo")?.Value
            };
            return logoAndNameInfo;
        }

        /// <summary>
        /// 是否自动切换市别
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns>true ： 自动，false 手动</returns>
        public bool IsSelfChangeShuffle(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "PosSelfChangeShuffle");
                if (para == null)
                {
                    return true;  //本来就是自动的
                }
                if (para.Value != null &&  para.Value == "0")
                {
                    return false; //手动
                }
                return true;//0:手动，1:打印
            }
            catch
            {
                return true;
            }
        }
        /// <summary>
        /// 是否打印会员结账单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns>true ： 是，false 否</returns>
        public bool IsPrintMemberBill(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "PosPrintMemberBill");
                if (para == null)
                {
                    return false;  
                }
                if (para.Value != null && para.Value == "0")
                {
                    return false; //不打印
                }
                return true;//0:不打印，1:打印
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 支付账单是否可以再次消费
        /// </summary>
        /// <returns></returns>
        public bool IsPayOrderAgain(string hid)
        {
            try
            {
                var para = _pmsContext.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "PosPayOrderAgain");
                if (para.Value == "1")
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


    }
}
