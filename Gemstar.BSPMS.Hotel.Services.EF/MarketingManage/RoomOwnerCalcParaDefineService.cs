using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services;
using System.Data.SqlClient;
using System.Data;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class RoomOwnerCalcParaDefineService : CRUDService<RoomOwnerCalcParaDefine>, IRoomOwnerCalcParaDefineService
    {
        private DbHotelPmsContext _db;
        public RoomOwnerCalcParaDefineService(DbHotelPmsContext db) : base(db, db.RoomOwnerCalcParaDefines)
        {
            _db = db;
        }

        public List<RoomOwnerCalcParaDefine> List(string hid)
        {
            return _db.RoomOwnerCalcParaDefines.Where(c => c.Hid == hid).ToList();
        }
        /// <summary>
        /// 根据参数类型获取业主分成参数定义列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="paratype"></param>
        /// <returns></returns>
        public List<RoomOwnerCalcParaDefine> ListbyParatype(string hid, string paratype)
        {
            return _db.RoomOwnerCalcParaDefines.Where(c => c.Hid == hid && c.ParaType == paratype && c.ParaName != "" && c.ParaName != null).OrderBy(w => w.ParaSeqId).ToList();
        }
        /// <summary>
        /// 获取可用于业主计算的函数列表
        /// </summary>
        /// <returns>可用于业主计算的函数列表</returns>
        public List<string> GetCalcFunctions()
        {
            return _db.Database.SqlQuery<string>("select exampleStr from v_roomOwnerCalcFunctions").ToList();
        }
        /// <summary>
        /// 判断参数名是否存在。
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="paraname"></param>
        /// <param name="paraId"></param>
        /// <returns></returns>
        private bool IsExsitParaName(string hid, string paraname, Guid paraId)
        {
            var a = _db.RoomOwnerCalcParaDefines.Where(c => c.Hid == hid && c.ParaName == paraname && c.ParaId != paraId).ToList().Count();
            if(a > 0)
            {
                return true;
            }else
            {
                //判断名称是否与计算函数保留关键字相同
                a = _db.Database.SqlQuery<int>("select COUNT(*) from v_roomOwnerCalcFunctions where keyword = {0}", paraname).Single();
            }
            return a > 0 ? true : false;
        }
        /// <summary>
        /// 根据参数名称反查参数代码
        /// </summary>
        /// <param name="paraTypeName"></param>
        /// <returns></returns>
        public string getparatype(string paraTypeName)
        {
            return _db.Database.SqlQuery<string>("select distinct paraType from v_roomOwnerCalcParaTemplate where paraTypeName=@paraTypeName", new SqlParameter("@paraTypeName", paraTypeName)).SingleOrDefault();
        }
        /// <summary>
        /// 保存修改的业主分成参数定义。
        /// </summary>
        /// <param name="listRoomOwner"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        public string updateRoomOwnerCalcParaDefine(List<RoomOwnerCalcParaDefine> listRoomOwner, string hid)
        {
            string retvalue = "";
            if (listRoomOwner != null && listRoomOwner.Count > 0)
            {
                foreach (var item in listRoomOwner)
                {

                    if (IsExsitParaName(hid, item.ParaName, item.ParaId))//检查参数名称是否重复
                    {
                        return string.Format("参数名称【{0}】已存在或者为系统保留关键字，请使用其他名称！", item.ParaName);
                    }
                    if (string.IsNullOrWhiteSpace(item.ParaName))//参数名为空 
                    {
                        if (_db.RoomOwnerCalcParaDefines.Find(item.ParaId) != null)//表中存在这条数据
                        {
                            _db.RoomOwnerCalcParaDefines.Remove(Get(item.ParaId));//删除表中的这条数据
                        }
                    }
                    else if (_db.RoomOwnerCalcParaDefines.Find(item.ParaId) == null)//表中不存在这条数据则增加一条
                    {
                        _db.RoomOwnerCalcParaDefines.Add(new RoomOwnerCalcParaDefine
                        {
                            Hid = hid,
                            ParaId = item.ParaId,
                            ParaCode = item.ParaCode,
                            ParaName = item.ParaName,
                            ParaSeqId = item.ParaSeqId,
                            ParaType = (string.IsNullOrWhiteSpace(getparatype(item.ParaType)) ? item.ParaType : getparatype(item.ParaType)),
                            isHidden = item.isHidden,
                            dataType = item.dataType
                        });
                    }
                    else//表中存在这条数据则修改
                    {
                        var mctype = _db.RoomOwnerCalcParaDefines.Find(item.ParaId);
                        //if (mctype.ParaType == "fixed" && mctype.ParaName != item.ParaName)
                        //{
                        //    return ("固定参数的参数名称不允许修改！");
                        //}
                        _db.Entry(mctype).State = EntityState.Modified;
                        //if (mctype.ParaName != item.ParaName)
                        //{
                        //     retvalue = "参数名称有修改，请修改相应的分成参数公式！";
                        //}
                        mctype.ParaName = item.ParaName;
                        mctype.ParaSeqId = item.ParaSeqId;
                        mctype.isHidden = item.isHidden;
                        mctype.dataType = item.dataType;
                        
                    }
                }
                _db.SaveChanges();

            }
            //同步视图中的数据到RoomOwnerCalcParaDefine表中
            _db.Database.ExecuteSqlCommand("exec up_input_RoomOwnerCalcParaDefine @h99hid={0}", hid);
            return retvalue;
        }

        protected override RoomOwnerCalcParaDefine GetTById(string id)
        {
            return _db.RoomOwnerCalcParaDefines.Find(id);
        }
        public List<V_dataType> getDataType()
        {
            List<V_dataType> datatb = _db.Database.SqlQuery<V_dataType>("select * from v_dataType").ToList();         
            return datatb;
        }
        public V_dataType getDataTypeName(string code)
        {
            try
            {
                V_dataType name = _db.Database.SqlQuery<V_dataType>("select name from v_dataType where code={0}", code).FirstOrDefault();
                return name;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
