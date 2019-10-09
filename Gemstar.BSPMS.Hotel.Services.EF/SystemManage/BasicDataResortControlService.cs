using System;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using System.Linq;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Hotel.Services.EF.SystemManage
{
    public class BasicDataResortControlService : CRUDService<BasicDataResortControl>, IBasicDataResortControlService
    {
        private DbHotelPmsContext _db;
        public BasicDataResortControlService(DbHotelPmsContext db) : base(db, db.BasicDataResortControls)
        {
            _db = db;
        }

        protected override BasicDataResortControl GetTById(string id)
        {
            return new BasicDataResortControl
            {
                Id = Guid.Parse(id)
            };
        }

        /// <summary>
        /// 将分发的基础资料同步到集团中，默认为分店可以增加，可以修改，可以禁用，并且分发类型为全部分店
        /// </summary>
        /// <param name="basicDatas">分发型的基础数据列表</param>
        /// <param name="grpId">集团id</param>
        public void SyncBasicDatasForCopy(List<M_V_BasicDataType> basicDatas, string grpId)
        {
            var groupBasicDatas = _db.BasicDataResortControls.Where(w => w.Grpid == grpId).ToList();
            var needSave = false;
            foreach(var data in basicDatas)
            {
                var exist = groupBasicDatas.FirstOrDefault(w => w.BasicDataCode == data.Code);
                if(exist == null)
                {
                    exist = new BasicDataResortControl
                    {
                        BasicDataCode = data.Code,
                        BasicDataName = data.Name,
                        DataCopyType = "all",
                        Grpid = grpId,
                        Id = Guid.NewGuid(),
                        ResortCanAdd = true,
                        ResortCanDisable = true,
                        ResortCanUpdate = true
                    };
                    _db.BasicDataResortControls.Add(exist);
                    needSave = true;
                }
            }
            if (needSave)
            {
                _db.SaveChanges();
            }
        }
        /// <summary>
        /// 获取基础数据控制属性
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="hid">当前酒店id</param>
        /// <param name="grpid">当前集团id</param>
        /// <param name="basicDatas">主数据库中的基础资料分发属性列表</param>
        /// <returns>基础数据控制属性</returns>
        public BasicDataControl GetBasicDataControl(string basicDataCode, string hid, string grpid, List<M_V_BasicDataType> basicDatas)
        {
            var data = basicDatas.FirstOrDefault(w => w.Code == basicDataCode);
            //在基础资料管控视图中不存在，则表示不控制，直接返回允许所有操作
            if(data == null)
            {
                return BasicDataControl.CanDoAll;
            }
            //如果不是集团，是单体酒店，则直接返回允许所有操作
            if (string.IsNullOrWhiteSpace(grpid))
            {
                return BasicDataControl.CanDoAll;
            }
            //集团管控的
            if(data.DataControl == "集团管控")
            {
                //如果当前是集团管理公司，则允许所有操作
                //如果当前是集团分店，则禁止所有操作
                var control = hid == grpid ? BasicDataControl.CanDoAll : BasicDataControl.CanDoNothing;
                control.MsgStr = "集团管控型基础资料分店只能查询和使用";
                return control;
            }
            //集团分发的
            if (data.DataControl == "集团分发")
            {
                //如果当前是集团管理公司，则允许所有操作
                if(hid == grpid)
                {
                    return BasicDataControl.CanDoAll;
                }
                //如果当前是集团分店，则根据集团设置的属性来返回
                var groupSet = _db.BasicDataResortControls.FirstOrDefault(w => w.Grpid == grpid && w.BasicDataCode == basicDataCode);
                if(groupSet == null)
                {
                    //集团还没有设置，则默认为都允许
                    return BasicDataControl.CanDoAll;
                }
                //如果允许分店增加，则直接返回都允许，因为能增加，则必须可以修改和禁用启用自己增加的部分，至于具体记录是分发的还是自主增加的，在各业务模块中进行判断
                if (groupSet.ResortCanAdd)
                {
                    return BasicDataControl.CanDoAll;
                }
                return new BasicDataControl(groupSet.ResortCanAdd, true, groupSet.ResortCanAdd, groupSet.ResortCanDisable)//分店是可以修改自己增加的数据所以修改默认给赋值为true
                {
                    MsgStr = "集团分发型基础资料，被集团设置为分店不允许此操作"
                };
            }
            //其他的，则认为是分店自主的
            //分店自主的，则集团不允许所有操作，分店允许所有操作
            var result = hid == grpid ? BasicDataControl.CanDoNothing : BasicDataControl.CanDoAll;
            result.MsgStr = "分店自主型基础资料，集团只能查询";
            return result;
        }
        /// <summary>
        /// 获取集团设置的，指定基础数据的分发属性
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="grpid">当前集团id</param>
        /// <returns>集团设置的，指定基础数据的分发属性</returns>
        public BasicDataResortControl GetResortControl(string basicDataCode, string grpid)
        {
            return _db.BasicDataResortControls.FirstOrDefault(w => w.Grpid == grpid && w.BasicDataCode == basicDataCode);
        }
        /// <summary>
        /// 更新指定基础数据的最后一次选中的分店信息
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="grpid">集团id</param>
        /// <param name="selectedHids">选中的分店信息</param>
        public void UpdateResortControlSelectedHids(string basicDataCode, string grpid, string selectedHids)
        {
            _db.Database.ExecuteSqlCommand("update BasicDataResortControl set SelectedHids = @selectedHids where BasicDataCode = @code and grpid = @grpid",
                new SqlParameter("@selectedHids",selectedHids??"")
                ,new SqlParameter("@code",basicDataCode??"")
                ,new SqlParameter("@grpid",grpid??"")
                );
        }
        /// <summary>
        /// 获取集团设置的基础数据分发方式
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="hid">当前酒店id</param>
        /// <param name="grpid">当前集团id</param>
        /// <returns>集团设置的基础数据分发方式</returns>
        public DataControlType GetGroupSetDataCopyType(string basicDataCode, string hid, string grpid)
        {
            var groupSet = _db.BasicDataResortControls.FirstOrDefault(w => w.Grpid == grpid && w.BasicDataCode == basicDataCode);
            if (groupSet == null)
            {
                //集团还没有设置，则默认为都允许
                return DataControlType.AllResorts;
            }
            //判断是否是固定分发类型
            if(groupSet.DataCopyType == DataControlType.AllResorts.Code)
            {
                return DataControlType.AllResorts;
            }
            if(groupSet.DataCopyType == DataControlType.SelectedResorts.Code)
            {
                DataControlType.SelectedResorts.SelectedHids = groupSet.SelectedHids;
                return DataControlType.SelectedResorts;
            }
            //取出分店属性分发类型
            var dataControl = _db.Database.SqlQuery<DataControlType>("select code,name from v_codeListPub where typeCode = '26' and [status]=1 and code = {0}", groupSet.DataCopyType).SingleOrDefault();
            if(dataControl == null)
            {
                return DataControlType.AllResorts;
            }
            return dataControl;
        }

        /// <summary>
        /// 获取所有支持的数据分发类型
        /// </summary>
        /// <returns>数据分发类型列表</returns>
        public List<DataControlType> GetDataControlTypes()
        {
            var result = new List<DataControlType>();
            //增加固定分发类型
            result.Add(DataControlType.AllResorts);
            result.Add(DataControlType.SelectedResorts);
            //增加分店属性中的直营，加盟等数据，集团除外
            var resortCodeList = _db.Database.SqlQuery<DataControlType>("select * from v_codeListPub where typeCode = '26' and [status]=1 and name <> '集团' order by seqid").ToList();

            result.AddRange(resortCodeList);

            return result;
        }
    }
}
