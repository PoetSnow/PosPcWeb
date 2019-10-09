using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using System.Web;
using System.Web.Caching;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class HotelInfoService : CRUDService<CenterHotel>, IHotelInfoService
    {
        private DbCommonContext _db;
        public HotelInfoService(DbCommonContext db) : base(db, db.Hotels)
        {
            _db = db;
        }
        protected override CenterHotel GetTById(string hid)
        {
            var hotel = new CenterHotel();
            hotel.Hid = hid;
            return hotel;
        }

        /// <summary>
        /// 获取所有酒店列表
        /// </summary>
        /// <returns>所有酒店列表</returns>
        public List<CenterHotel> ListValidHotels() {
            return _db.Database.SqlQuery<CenterHotel>("SELECT * FROM dbo.hotel WHERE status < 50 AND expiryDate >= GETDATE()").ToList();
        }
        public UpHardwareInterfacecs GetDownloadFile(string versionId)
        {
            return _db.Database.SqlQuery<UpHardwareInterfacecs>("select * from dbo.hardwareInterface where versionId=@versionId", new SqlParameter("@versionId", versionId)).FirstOrDefault();
        }
        public UpQueryHotelInfoByIdResult GetHotelInfo(string hid)
        {
            return _db.Database.SqlQuery<UpQueryHotelInfoByIdResult>("exec up_queryHotelInfoById @hid=@hid"
                , new SqlParameter("@hid", hid)).SingleOrDefault();
        }
        /// <summary>
        /// 根据渠道代码，渠道里酒店的签约代码查找对应的酒店信息
        /// </summary>
        /// <param name="channelExtCode">渠道外部代码</param>
        /// <param name="hotelCodeInChannel">渠道里酒店的签约代码</param>
        /// <returns>null：如果渠道里签约代码没有对应酒店，酒店信息实例对象：对应酒店的详细信息</returns>
        public UpQueryHotelInfoByIdResult GetHotelInfo(string channelExtCode, string hotelCodeInChannel)
        {
            return _db.Database.SqlQuery<UpQueryHotelInfoByIdResult>("exec up_queryHotelInfoByChannelInfo @channelExtCode=@channelExtCode,@hotelCodeInChannel=@hotelCodeInChannel"
                , new SqlParameter("@channelExtCode", channelExtCode ?? "")
                , new SqlParameter("@hotelCodeInChannel", hotelCodeInChannel ?? "")
                ).SingleOrDefault();
        }

        public IQueryable<CenterHotel> LoadGroup()
        {
            return _db.Hotels.Where(e => e.Grpid == e.Hid);
        }

        public JsonResultData Enable(string id)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var hotel = _db.Hotels.Find(ids[i]);
                    _db.Entry(hotel).State = EntityState.Modified;
                    hotel.Status = (byte)EntityStatus.启用;
                }
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 获取当前酒店的logo地址
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public string GetLogoUrl(string hid)
        {
            var hotels = _db.Hotels.Where(h => h.Hid == hid).FirstOrDefault();
            if (hotels != null)
                return hotels.PicLink;
            return null;
        }
        public JsonResultData Disable(string id)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var hotel = _db.Hotels.Find(ids[i]);
                    _db.Entry(hotel).State = EntityState.Modified;
                    hotel.Status = (byte)EntityStatus.禁用;
                }
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        //public JsonResultData CheckHotelId(string hid)
        //{
        //    hid = hid.Trim();
        //    var hotel = _db.Hotels.Where(e => e.Hid == hid);
        //    if (hotel.Any())
        //    {
        //        if (hotel.First().Status != (byte)EntityStatus.正常)
        //        {
        //            return JsonResultData.Failure("酒店代码已失效");
        //        }
        //        else
        //        {
        //            return JsonResultData.Successed("");
        //        }
        //    }
        //    else
        //    {
        //        return JsonResultData.Failure("酒店代码不存在");
        //    }
        //}



        public JsonResultData RepeatCheck(string name, string mobile, string hid = "")
        {
            if (hid != null)
            {
                hid = hid.Trim();
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return JsonResultData.Failure("请输入名称");
            }
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return JsonResultData.Failure("请输入手机号");
            }
            mobile = mobile.Trim();
            var result = _db.Hotels.AsQueryable();
            if (hid == "")
            {
                result = result.Where(e => e.Name == name || e.Mobile == mobile);
            }
            else
            {
                result = result.Where(e => e.Hid != hid && (e.Name == name || e.Mobile == mobile));
            }
            var resultList = result.ToList();
            if (resultList.Any())
            {
                StringBuilder sb = new StringBuilder();
                if (resultList.Where(e => e.Name == name).Any())
                {
                    sb.AppendLine("酒店名称已存在");
                }
                if (resultList.Where(e => e.Mobile == mobile).Any())
                {
                    sb.AppendLine("手机号码已注册");
                }
                return JsonResultData.Failure(sb.ToString());
            }
            else
            {
                return JsonResultData.Successed();
            }
        }

        public List<UpQueryHotelInterfaceByIdResult> GetHotelInterface(string hid)
        {
            return _db.Database.SqlQuery<UpQueryHotelInterfaceByIdResult>("exec up_queryHotelInterfaceById @hid=@hid", new SqlParameter("@hid", hid)).ToList();
        }

        public IQueryable<M_v_codeListPub> GetLockType()
        {
            return _db.M_v_codeListPubs.Where(e => e.TypeCode == "01").OrderBy(w => w.Seqid);
        }

        public IQueryable<M_v_codeListPub> GetIdCardType()
        {
            return _db.M_v_codeListPubs.Where(e => e.TypeCode == "02").OrderBy(w => w.Seqid);
        }
        public IQueryable<M_v_codeListPub> GetMbrCardType()
        {
            return _db.M_v_codeListPubs.Where(e => e.TypeCode == "07").OrderBy(w => w.Seqid);
        }
        public IQueryable<M_v_channelCode> GetChannelCode()
        {
            return _db.M_v_channelCodes;
        }

        public IQueryable<M_v_codeListPub> GetItemAction()
        {
            return _db.M_v_codeListPubs.Where(e => e.TypeCode == "03");
        }

        public IQueryable<M_v_codeListPub> GetOnlineLockType()
        {
            return _db.M_v_codeListPubs.Where(e => e.TypeCode == "04").OrderBy(w => w.Seqid);
        }
        public IQueryable<M_v_codeListPub> GetManageType()
        {
            return _db.M_v_codeListPubs.Where(e => e.TypeCode == "06");
        }
        public DataTable getHotelPara(string hid)
        {
            var hotelInfo = GetHotelInfo(hid);
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,_db.IsConnectViaInternetIp());
            //将参数模板中的参数同步到参数表中
            string sql = string.Format("exec up_list_PmsPara @h99hid='{0}'", hid);
            int a = ADOHelper.ExecNonQuery(sql, dbConnStr);
            //取出参数表中不允许酒店自行设置的参数来在运营后台进行设置
            sql = string.Format(@"select * from pmspara where hid='{0}' and isVisible=0 order by seqid", hid);
            var db = ADOHelper.ExecSql(sql, dbConnStr);
            return db;
        }

        public int saveChangetHotelPara(string hid, Dictionary<string, string> para)
        {
            var hotelInfo = GetHotelInfo(hid);
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,_db.IsConnectViaInternetIp());
            string[] sqls = new string[para.Count]; int i = 0;
            foreach (var dic in para)
            {
                sqls[i] = string.Format("update pmspara set value='{2}'  where hid= '{0}' and isVisible=0 and code='{1}'", hid, dic.Key, dic.Value);
                i++;
            }
            int a = ADOHelper.ExecBatchNonQuery(sqls, dbConnStr);
            return a;
        }
        public bool IsExistCleanRoom(string hid)
        {
            var hotelInfo = GetHotelInfo(hid);
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,_db.IsConnectViaInternetIp());
            string sql = string.Format(@"select count(1) from RoomStatus where hid={0} and IsDirty=2", hid);
            bool i = int.Parse(ADOHelper.ExecSql(sql, dbConnStr).Rows[0][0].ToString()) > 0;
            return i;
        }
        /// <summary>
        /// 更新总裁驾驶舱功能
        /// </summary>
        /// <param name="hid"></param>
        public void SaveChangeIsOpenAnalysis(string hid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid)) { return; }
                bool isOpenAnalysis = _db.Hotels.Where(e => e.Hid == hid).Select(c => c.isOpenAnalysis).FirstOrDefault();

                var hotelInfo = GetHotelInfo(hid);
                var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,_db.IsConnectViaInternetIp());
                string sql = "update pmsHotel set isOpenAnalysis = 0 where hid = @hid";
                var para = new List<SqlParameter> { new SqlParameter("@hid", hid) };
                if (isOpenAnalysis)
                {
                    sql = @"update pmsHotel set isOpenAnalysis = 1,lastAnalysisDate = case 
                        when lastAnalysisDate is null then DATEADD(DAY,-1,HotelStatus.bsnsDate) else lastAnalysisDate end
                        from pmsHotel
                        inner join HotelStatus on pmsHotel.hid = HotelStatus.hid
                        where pmsHotel.hid = @hid";
                }
                ADOHelper.ExecNonQuery(sql, dbConnStr, para);
            }
            catch { }
        }

        public string GetHotelShortName(string hid)
        {
            var hotels = _db.Hotels.Where(h => h.Hid == hid).FirstOrDefault();
            if (hotels != null)
                return hotels.Hotelshortname;
            return null;
        }
        public DataTable GetHotelExpire()
        {
            List<CenterHotel> infos = _db.Hotels.ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("酒店编号");
            dt.Columns.Add("酒店名称");
            dt.Columns.Add("酒店简称");
            dt.Columns.Add("业务员");
            dt.Columns.Add("酒店联系人");
            dt.Columns.Add("酒店联系人电话");
            dt.Columns.Add("到期日期");
            dt.Columns.Add("备注"); 
            foreach (var info in infos)
            {
                DataRow dr = dt.NewRow();
                dr["酒店编号"] = info.Hid;
                dr["酒店名称"] = info.Name;
                dr["酒店简称"] = info.Hotelshortname;
                dr["业务员"] = info.Sales;
                dr["酒店联系人"] = info.Contactsname;
                dr["酒店联系人电话"] = info.Mobile;
                dr["到期日期"] = info.ExpiryDate;
                dr["备注"] = info.Remark; 
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 获取基础资料列表
        /// </summary>
        /// <returns>基础资料列表</returns>
        public List<M_V_BasicDataType> GetBasicDataForAll()
        {
            Cache cache = null;
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                cache = HttpContext.Current.Cache;
            }
            //有缓存时直接使用缓存中的数据
            List<M_V_BasicDataType> result = null;
            var cacheKey = "HotelInfoService.BasicDataForAll";
            if (cache != null && cache[cacheKey] != null)
            {

                result = cache[cacheKey] as List<M_V_BasicDataType>;
            }
            if (result == null)
            {
                //没有缓存，则从数据库中提取数据后，缓存到缓存里面，然后返回
                result = _db.Database.SqlQuery<M_V_BasicDataType>("select * from m_v_basicDataType").ToList();
                if (cache != null)
                {
                    cache.Remove(cacheKey);
                    cache.Add(cacheKey, result, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), CacheItemPriority.Default, null);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取管控属性为集团分发的基础资料列表
        /// </summary>
        /// <returns>管控属性为集团分发的基础资料列表</returns>
        public List<M_V_BasicDataType> GetBasicDataForCopy()
        {
            Cache cache = null;
            if(HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                cache = HttpContext.Current.Cache;
            }
            //有缓存时直接使用缓存中的数据
            List<M_V_BasicDataType> result = null;
            var cacheKey = "HotelInfoService.BasicDataForCopy";
            if(cache != null && cache[cacheKey] != null)
            { 
                result = cache[cacheKey] as List<M_V_BasicDataType>;
            }
            if(result == null)
            {
                //没有缓存，则从数据库中提取数据后，缓存到缓存里面，然后返回
                result = _db.Database.SqlQuery<M_V_BasicDataType>("select * from m_v_basicDataType where dataControl = '集团分发'").ToList();
                if(cache != null)
                {
                    cache.Remove(cacheKey);
                    cache.Add(cacheKey,result, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), CacheItemPriority.Default, null);
                }
            }
            return result;
        }
        /// <summary>
        /// 当前的主数据库是否通过外网ip进行连接的
        /// </summary>
        /// <returns>true:是，false：否</returns>
        public bool IsConnectViaInternte()
        {
            return _db.IsConnectViaInternetIp();
        }

    }
}
