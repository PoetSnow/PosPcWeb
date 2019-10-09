using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Enums;
using System;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EF.Common;
using System.Data;
using System.Reflection;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class RoomTypeService : CRUDService<RoomType>, IRoomTypeService, IBasicDataCopyService<RoomType>
    {
        public RoomTypeService(DbHotelPmsContext db) : base(db, db.RoomTypes)
        {
            _pmsContext = db;
        }
        protected override RoomType GetTById(string id)
        {
            return new RoomType { Id = id };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的房间类型id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        public JsonResultData BatchUpdateStatus(string ids, EntityStatus status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔");
                }
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    RoomType update = _pmsContext.RoomTypes.Find(id);
                    if (update.Status != status)
                    {
                        update.Status = status;
                        _pmsContext.Entry(update).State = EntityState.Modified;
                    }
                }
                _pmsContext.AddDataChangeLogs(OpLogType.房型启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 获取房间类型键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> List(string hid)
        {
            var list = _pmsContext.RoomTypes.Where(c => c.Hid == hid && c.Status == EntityStatus.启用).OrderBy(o => o.Seqid).Select(c => new { c.Id, c.Name }).ToList();
            List<KeyValuePair<string, string>> returnList = new List<KeyValuePair<string, string>>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    returnList.Add(new KeyValuePair<string, string>(item.Id, item.Name));
                }
            }
            return returnList;
        }

        /// <summary>
        /// 可以由子类来重写，实现一些在删除之前的业务处理逻辑，默认为空
        /// </summary>
        /// <param name="obj">需要删除的业务实体类</param>
        protected override void BeforeDelete(RoomType obj)
        {
            var entity = _pmsContext.RoomTypes.AsNoTracking().FirstOrDefault(c => c.Id == obj.Id);
            if (entity == null)
            {
                throw new System.Exception("此房间类型不存在。");
            }
            var list = _pmsContext.Rooms.Where(c => c.Hid == entity.Hid && c.RoomTypeid == entity.Id).Select(c => c.RoomNo).ToList();
            if (list != null && list.Count > 0)
            {
                string roomNoList = "";
                foreach (var item in list)
                {
                    roomNoList += string.Format("[{0}]", item);
                }
                roomNoList = string.Format("\r\n{0}房号列表:{1}", entity.Name, roomNoList);
                throw new System.Exception("不能删除当前房间类型，因为有房间属于此类型。" + roomNoList);
            }
        }
        /// <summary>
        /// 保存房型图片地址
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomtypeid"></param>
        /// <param name="picaddress"></param>
        /// <returns></returns>
        public JsonResultData AddRoomtypePic(string hid, string roomtypeid, string picaddress)
        {
            picaddress = (picaddress == null ? "" : picaddress);
            string[] picarr = picaddress.Trim('&').Split('&'); string picaddresses = "";
            for (int i = 0; i < picarr.Length; i++)
            {
                if (picarr[i].Trim('&') == "") { continue; }
                picaddresses += picarr[i] + ",";
            }
            int a = _pmsContext.Database.ExecuteSqlCommand("exec up_Add_RoomtypePic @h99hid={0},@roomtypeid={1},@address={2}", hid, roomtypeid, picaddresses.Trim(','));
            if (a > 0)
            {
                return JsonResultData.Successed("");
            }
            else
            {
                return JsonResultData.Failure("");
            }

        }
        public override List<RoomType> AddAndCopy(RoomType groupModel, string groupId, string dataControlCode, List<string> selectedResortHids)
        {
            var addPara = new BasicDataAddAndCopyModel<RoomType>
            {
                BasicDataCode = M_V_BasicDataType.BasicDataCodeRoomType,
                BasicDataService = this,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _pmsContext,
                GroupId = groupId,
                GroupModel = groupModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.AddAndCopy(addPara);
        }
        public override List<RoomType> EditAndCopy(RoomType groupModel, RoomType originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties)
        {
            var editPara = new BasicDataEditAndCopyModel<RoomType>
            {
                BasicDataCode = M_V_BasicDataType.BasicDataCodeRoomType,
                BasicDataService = this,
                CopyedUpdateProperties = updateProperties,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _pmsContext,
                GroupId = groupId,
                GroupModel = groupModel,
                GroupModelUpdateFieldNames = fieldNames,
                OriginGroupModel = originModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.EditAndCopy(editPara);
        }
        /// <summary>
        /// 启用禁用集团记录并且同时启用禁用分发到分店的记录
        /// </summary>
        /// <param name="model">要启用禁用的集团记录</param>
        /// <param name="status">新的状态</param>
        /// <returns>启用禁用的实体列表</returns>
        public override List<RoomType> ChangeStatusAndCopy(RoomType model, EntityStatus status)
        {
            var changePara = new BasicDataStatusChangeAndCopyModel<RoomType>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = model,
                Status = status
            };
            return BasicDataServiceHelper.ChangeGroupAndHotelCopiedStatus(changePara);
        }
        public override List<RoomType> DeleteGroupAndHotelCopied(RoomType groupModel)
        {
            var deletePara = new BasicDataDeleteGroupAndHotelCopiedModel<RoomType>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = groupModel
            };
            return BasicDataServiceHelper.DeleteGroupAndHotelCopied(deletePara);
        }
        public RoomType GetNewHotelBasicData(string hid, RoomType groupModel)
        {
            var hotelModel = new RoomType();
            AutoSetValueHelper.SetValues(groupModel, hotelModel);
            hotelModel.Hid = hid;
            hotelModel.Id = hid + hotelModel.Code;
            hotelModel.DataCopyId = groupModel.Id;
            hotelModel.DataSource = BasicDataDataSource.Copyed.Code;
            RoomType cl = _pmsContext.RoomTypes.FirstOrDefault(w => w.Id == hotelModel.Id && w.Hid == hid);
            if (cl != null) { return null; }
            return hotelModel;
        }

        public RoomType GetCopyedHotelBasicData(string hid, RoomType groupModel, bool iscopyed)
        {
            if (iscopyed)
            {
                return _pmsContext.RoomTypes.FirstOrDefault(w => w.DataCopyId == groupModel.Id && w.Hid == hid && w.DataSource == BasicDataDataSource.Copyed.Code);
            }
            else
            {
                return _pmsContext.RoomTypes.FirstOrDefault(w => w.Id == groupModel.Id.Replace(groupModel.Hid, hid) && w.Hid == hid);
            }
        }

        public List<RoomType> GetCopyedHotelBasicDatas(RoomType groupModel)
        {
            return _pmsContext.RoomTypes.Where(w => w.DataCopyId == groupModel.Id && w.DataSource == BasicDataDataSource.Copyed.Code).ToList();
        }

        public DataTable getroomtypeEquipment(string hid, string roomtypeid)
        {
            List<RtEqList> rt = _pmsContext.Database.SqlQuery<RtEqList>("exec up_list_roomtypeEquipment @h99hid={0},@roomtypeid={1}", hid, roomtypeid).ToList();
            DataTable dt = ListToDataTable<RtEqList>(rt);
            return dt;
        }

        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="list">泛类型集合</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }

        public void setroomtypeEquipment(List<RtEqList> para, string rtid)
        {
            string livetype = "";
            foreach (RtEqList p in para)
            {
                livetype = p.xuQuality.ToString();
                RoomTypeEquipment rt = _pmsContext.RoomTypeEquipments.Where(w => w.Roomtypeid == rtid && w.Goodsid == p.Goodsid && w.LiveType == livetype).FirstOrDefault();
                if (rt == null)
                {
                    RoomTypeEquipment rte = new RoomTypeEquipment()
                    {
                        Goodsid = p.Goodsid,
                        Id = Guid.NewGuid(),
                        LiveType = livetype,
                        Quality = int.Parse(p.name == "" ? "0" : p.name),
                        Roomtypeid = rtid
                    };
                    _pmsContext.RoomTypeEquipments.Add(rte);
                    _pmsContext.SaveChanges();
                }
                else
                {
                    rt.Quality = int.Parse(p.name == "" ? "0" : p.name);
                    _pmsContext.Entry(rt).State = EntityState.Modified;
                    _pmsContext.SaveChanges();
                }
            }
           
        }
    }
}
