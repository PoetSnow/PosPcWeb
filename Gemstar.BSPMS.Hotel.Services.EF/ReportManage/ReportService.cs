using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System.Data;
using System.Data.SqlClient;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.EF.ReportManage
{
    public class ReportService : CRUDService<ReportFormat>, IReportService
    {
        public ReportService(DbHotelPmsContext db) : base(db, db.ReportFormats)
        {
            _pmsContext = db;
        }
        protected override ReportFormat GetTById(string id)
        {
            return new ReportFormat { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;


        /// <summary>
        /// 获取报表中文名称
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public string GetReportName(string code, byte productmask)
        {
            return _pmsContext.Database.SqlQuery<string>("select top 1 name from v_reportlist where code={0} AND (SUBSTRING(mask,{1},1) = '1')", code, productmask).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 获取报表中文名称
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public string GetFileName(string code, byte productmask)
        {
            return _pmsContext.Database.SqlQuery<string>("select top 1 fileName from v_reportlist where code={0} AND (SUBSTRING(mask,{1},1) = '1')", code, productmask).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 获取指定报表的所有格式名称
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="templateName">报表号</param>
        /// <returns>报表号所有的格式名称</returns>
        public List<string> GetStyleNames(string hid, string templateName)
        {
            return _pmsContext.ReportFormats.Where(w => w.Hid == hid && w.ReportName == templateName).Select(w => w.StyleName).OrderBy(w => w).ToList();
        }
        /// <summary>
        /// 是否存在自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        public bool IsExistsTemplate(string hid, string templateName, string styleName)
        {
            styleName = GetStyleName(styleName);

            return _pmsContext.ReportFormats.Where(c => c.Hid == hid && c.ReportName == templateName && c.StyleName == styleName).Any();
        }

        /// <summary>
        /// 获取自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        public string GetTemplate(string hid, string templateName, string styleName)
        {
            styleName = GetStyleName(styleName);

            var entity = _pmsContext.ReportFormats.Where(c => c.Hid == hid && c.ReportName == templateName && c.StyleName == styleName).FirstOrDefault();
            if (entity != null)
            {
                return entity.ReportTemplate;
            }
            return null;
        }

        /// <summary>
        /// 获取自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        public ReportFormat GetReportFormat(string hid, string templateName)
        {


            return _pmsContext.ReportFormats.Where(c => c.Hid == hid && c.ReportName == templateName).FirstOrDefault();

        }

        /// <summary>
        /// 保存自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="templateValue">模板值</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        public bool SaveTemplate(string hid, string templateName, string templateValue, string styleName)
        {
            styleName = GetStyleName(styleName);

            var entity = _pmsContext.ReportFormats.Where(c => c.Hid == hid && c.ReportName == templateName && c.StyleName == styleName).FirstOrDefault();
            if (entity != null)
            {
                entity.ReportTemplate = templateValue;
                _pmsContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                _pmsContext.ReportFormats.Add(new ReportFormat
                {
                    Hid = hid,
                    Id = Guid.NewGuid(),
                    ReportName = templateName,
                    ReportTemplate = templateValue,
                    StyleName = styleName
                });
            }
            int i = _pmsContext.SaveChanges();
            return i > 0;
        }

        /// <summary>
        /// 删除自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        public bool DelTemplate(string hid, string templateName, string styleName)
        {
            styleName = GetStyleName(styleName);

            var entity = _pmsContext.ReportFormats.Where(c => c.Hid == hid && c.ReportName == templateName && c.StyleName == styleName).FirstOrDefault();
            if (entity != null)
            {
                _pmsContext.ReportFormats.Remove(entity);
                int i = _pmsContext.SaveChanges();
                return i > 0;
            }
            return false;
        }

        /// <summary>
        /// 添加查询参数临时数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public Guid? AddQueryParaTemp(string hid, string value)
        {
            if (!string.IsNullOrWhiteSpace(hid) && !string.IsNullOrWhiteSpace(value))
            {
                Guid id = Guid.NewGuid();
                _pmsContext.ReportQueryParaTemps.Add(new ReportQueryParaTemp
                {
                    Id = id,
                    Hid = hid,
                    Value = value,
                    Createdate = DateTime.Now
                });
                int i = _pmsContext.SaveChanges();
                if (i == 1)
                {
                    return id;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取并移除查询参数临时数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public string GetQueryParaTemp(string hid, Guid id)
        {
            string value = null;
            if (!string.IsNullOrWhiteSpace(hid))
            {
                var entity = _pmsContext.ReportQueryParaTemps.Where(c => c.Id == id && c.Hid == hid).SingleOrDefault();
                if (entity != null)
                {
                    value = entity.Value;
                    _pmsContext.ReportQueryParaTemps.Remove(entity);
                    _pmsContext.SaveChanges();
                }
            }
            return value;
        }

        public List<string> GetReportType(ProductType productType)
        {
            List<string> list = new List<string>();

            var typelist = _pmsContext.Database.SqlQuery<string>("select distinct type from v_reportlist where substring(mask,{0},1)='1'", (int)productType).ToList();
            for (int i = 0; i < typelist.Count; i++)
            {
                list.Add(typelist[i].ToString());
            }
            return list;
        }

        public bool IsReportauth(string uid, string hid, string reportCode)
        {
            var isreg = _pmsContext.Database.SqlQuery<int>("select COUNT(1) from pmsUser where isReg='1' and id={0}", uid).FirstOrDefault();
            if (isreg > 0)
            {
                return true;
            }
            var list = _pmsContext.Database.SqlQuery<string>("select reportCode from roleAuthReport where RoleId in(select roleid from userRole where userid ={0} and hid ={1}) and grpid ={1} and isAllow = 1 and reportCode={2}", uid, hid, reportCode).ToList();
            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsReportauth(string uid, string hid)
        {
            var isreg = _pmsContext.Database.SqlQuery<int>("select COUNT(1) from pmsUser where isReg='1' and id={0}", uid).FirstOrDefault();
            if (isreg > 0)
            {
                return true;
            }
            var list = _pmsContext.Database.SqlQuery<string>("select reportCode from roleAuthReport where RoleId in(select roleid from userRole where userid ={0} and hid ={1}) and grpid ={1} and isAllow = 1", uid, hid).ToList();
            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string GetStyleName(string styleName)
        {
            if (styleName == "自定义格式")
            {
                //现在数据库中的默认格式名称是null值
                return null;
            }
            return styleName;
        }
        /// <summary>
        /// 获取签名列表数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<SignatureLog> GetSignatureLogList(string sType, string hid)
        {
            if (string.IsNullOrWhiteSpace(sType))
                return _pmsContext.SignatureLog.Where(w => w.Hid == hid).OrderByDescending(o => o.SDate);
            var data = Byte.Parse(sType);
            return _pmsContext.SignatureLog.Where(w => w.SType == data && w.Hid == hid).OrderByDescending(o => o.SDate);
        }
        public bool InsertSignature(SignatureLog data)
        {
            _pmsContext.SignatureLog.Add(data);
            var number = _pmsContext.SaveChanges();
            return number > 0;
        }
        /// <summary>
        /// 删除电子签名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteSignature(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;
            var newid = Guid.Parse(id);
            var result = _pmsContext.SignatureLog.Where(s => s.Id == newid).FirstOrDefault();
            _pmsContext.SignatureLog.Remove(result);
            _pmsContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// 根据存储过程名称获取存储过程参数相关信息
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <returns></returns>
        public List<UpQueryProcedureParametersResult> GetProcedureParameters(string procedureName)
        {
            return _pmsContext.Database.SqlQuery<UpQueryProcedureParametersResult>("exec up_QueryProcedureParameters @procedureName=@procedureName", new SqlParameter("@procedureName", procedureName)).ToList();
        }

        /// <summary>
        /// 获取报表自定义格式
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public List<ReportFormat> GetReportFormatList(string hid, string templateName)
        {

            var s = _pmsContext.ReportFormats.Where(c => c.Hid == hid && c.ReportName == templateName).ToList();
            return s;
        }



        public void setUserReportCollect(string reportCode, bool isCollect, Guid userId)
        {
            UserReportCollect userCol = _pmsContext.UserReportCollects.Where(w => w.UserId == userId && w.ReportCode == reportCode).FirstOrDefault();
            if (isCollect && userCol == null)
            {
                UserReportCollect urc = new UserReportCollect { ReportCode = reportCode, UserId = userId };
                _pmsContext.UserReportCollects.Add(urc);
                _pmsContext.SaveChanges();
            }
            else if (!isCollect && userCol != null)
            {
                _pmsContext.UserReportCollects.Remove(userCol);
                _pmsContext.SaveChanges();
            }
        }
    }
}
