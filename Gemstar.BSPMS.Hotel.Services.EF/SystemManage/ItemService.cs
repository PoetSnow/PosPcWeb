using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services;
using System.Data.SqlClient;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Hotel.Services.EF.Common;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class ItemService : CRUDService<Item>, IItemService, IBasicDataCopyService<Item>
    {
        public ItemService(DbHotelPmsContext db) : base(db, db.Items)
        {
            _pmsContext = db;
        }
        public List<Item> GetItem(string hid, string dcflag)
        {
            return _pmsContext.Items.Where(w => w.Hid == hid && w.DcFlag == dcflag && w.Name != null && (byte)w.Status < (byte)EntityStatus.禁用).OrderBy(w => w.Seqid).ToList();
        }
        protected override Item GetTById(string id)
        {
            var item = new Item { Id = id };
            return item;
        }
        protected override void AfterUpdate(Item obj, Item originObj, List<string> needUpdateFieldNames = null)
        {
            var hotel = _pmsContext.PmsHotels.Where(w => w.Hid == obj.Hid && w.Grpid != "").FirstOrDefault();
            if (hotel != null)
            { 
                obj.ItemTypeid = string.IsNullOrEmpty(obj.ItemTypeid) ? "" : obj.ItemTypeid.Replace(hotel.Grpid, obj.Hid);
                obj.InvoiceItemid = string.IsNullOrEmpty(obj.InvoiceItemid) ? "" : obj.InvoiceItemid.Replace(hotel.Grpid, obj.Hid);  
            }
            base.AfterUpdate(obj, originObj, needUpdateFieldNames); 
        }
        private DbHotelPmsContext _pmsContext;
        public JsonResultData Enable(string id, string _dcflag)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.Items.Find(ids[i]);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.Status = EntityStatus.启用;
                }
                if (_dcflag == "C")
                {
                    _pmsContext.AddDataChangeLogs(OpLogType.付款方式启用禁用);
                }
                else
                {
                    _pmsContext.AddDataChangeLogs(OpLogType.消费项目启用禁用);
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
                throw;
            }
        }

        public JsonResultData Disable(string id, string _dcflag)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.Items.Find(ids[i]);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.Status = EntityStatus.禁用;
                }
                if (_dcflag == "C")
                {
                    _pmsContext.AddDataChangeLogs(OpLogType.付款方式启用禁用);
                }
                else
                {
                    _pmsContext.AddDataChangeLogs(OpLogType.消费项目启用禁用);
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
                throw;
            }
        }
        public List<CodeList> GetCodeList(string typecode, string hid)
        {
            return _pmsContext.CodeLists.Where(w => w.Hid == hid && w.TypeCode == typecode && (byte)w.Status < (byte)EntityStatus.禁用).OrderBy(w => w.Seqid).ToList();
        }
        public string GetCodeList(string typecode, string hid, string id)
        {
            if (id == "") { return "0"; }
            else
            {
                return _pmsContext.CodeLists.Where(w => w.Hid == hid && w.TypeCode == typecode && (byte)w.Status < (byte)EntityStatus.禁用 && w.Id == id).OrderBy(w => w.Seqid).FirstOrDefault().Name2;
            }
        }
        public List<V_codeListPub> GetCodeListPub(string typeCode)
        {
            return _pmsContext.Database.SqlQuery<V_codeListPub>("select * from v_codeListPub where typeCode = {0} and [status]=1 ", typeCode).OrderBy(w => w.seqid).ToList();
        }
        //public List<V_codeListPub> GetCodeListPub(string typeCode,string wherecode)
        //{
        //    return _pmsContext.Database.SqlQuery<V_codeListPub>("select * from v_codeListPub where typeCode = {0} and [status]=1 and code in({})", typeCode).OrderBy(w => w.seqid).ToList(); 
        //}
        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="dcFlag">D:消费 C:付款</param>
        /// <param name="isCharge">true:可充值 false:不可充值</param>
        /// <returns></returns>
        public IQueryable<Item> GetItems(string hid, string dcFlag = "", bool? isCharge = null)
        {
            var items = _pmsContext.Items.Where(e => e.Hid == hid && (byte)e.Status < (byte)EntityStatus.禁用);
            if (!string.IsNullOrWhiteSpace(dcFlag))
            {
                items = items.Where(e => e.DcFlag.ToLower() == dcFlag.ToLower());
            }
            if (isCharge != null)
            {
                items = items.Where(e => e.IsCharge == isCharge);
            }
            return items.OrderBy(c => c.Seqid);
        }

        public SelectList GetCodeListPubforSel(string typeCode)
        {
            var list = new SelectList(_pmsContext.Database.SqlQuery<V_codeListPub>("select '' code,'' name union all select code,name from v_codeListPub where typeCode = {0}", typeCode), "Code", "Name");
            return list;
        }

        public List<V_codeListPub> GetCodeListPub(string typeCode, string code)
        {
            return _pmsContext.Database.SqlQuery<V_codeListPub>("select * from v_codeListPub where typeCode = {0} and [status]=1 and code={1}", typeCode, code).ToList();
        }

        public List<Item> GetItembyAction(string hid, string action, string dcflag, string id)
        {
            List<Item> items = null;
            if (id == "")
            {
                items = _pmsContext.Items.Where(e => e.Hid == hid && e.Action == action && e.DcFlag.ToLower() == dcflag.ToLower()).ToList();
            }
            else
            {
                items = _pmsContext.Items.Where(e => e.Hid == hid && e.Action == action && e.DcFlag.ToLower() == dcflag.ToLower() && e.Id != id).ToList();
            }
            return items;
        }

        public SelectList GetStatypesellist()
        {

            return new SelectList(_pmsContext.Database.SqlQuery<V_codeListPub>("select * from v_codeListPub where typeCode = '07' and [status]=1 "), "Code", "Name");
        }

        public string GetCodeListFornameBycode(string typecode, string hid, string id)
        {
            List<CodeList> list = _pmsContext.CodeLists.Where(w => w.TypeCode == typecode && w.Id == id && w.Hid == hid).ToList();
            return list.Count > 0 ? list[0].Name : "";
        }

        public List<Item> GetCodeListbyitemTypeid(string hid, string itemtypeid, string DCflag)
        {
            List<Item> list = _pmsContext.Items.Where(w => w.ItemTypeid == itemtypeid && w.Hid == hid && w.DcFlag == DCflag).ToList();
            return list;
        }
        /// <summary>
        /// 查询指定条件的项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="dcFlag">消费还是付款</param>
        /// <param name="keyword">关键字</param>
        /// <param name="isInput">是否可输入，默认只查询可输入的项目</param>
        /// <returns>满足条件的项目列表</returns>
        public List<Item> Query(string hid, string dcFlag, string keyword, bool isInput = true)
        {
            return _pmsContext.Database.SqlQuery<Item>("exec up_queryItems @hid=@hid,@dcFlag=@dcFlag,@keyword=@keyword"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@dcFlag", dcFlag ?? "D")
                , new SqlParameter("@keyword", keyword ?? "")
                ).ToList();
        }


        public List<V_itemReserv> IsexistV_itemReserv(string itemcode, string dcflag)
        {
            return _pmsContext.Database.SqlQuery<V_itemReserv>("select * from V_itemReserv where Itemcode = {0} and dcflag={1}", itemcode, dcflag).ToList();

        }

        public List<V_codeListReserve> IsexistV_codeListReserve(string code, string typecode)
        {
            return _pmsContext.Database.SqlQuery<V_codeListReserve>("select * from V_codeListReserve where code = {0} and typecode={1}", code, typecode).ToList();
        }
        public string IsexistItemId(string hid, string itemid)
        {
            return _pmsContext.Database.SqlQuery<string>("exec up_checkexsititem {0},{1}", hid, itemid).FirstOrDefault();
        }

        public void syncRate(string hid, string invoiceItemid, decimal rate)
        {
            List<Item> items = _pmsContext.Items.Where(w => w.Hid == hid && w.InvoiceItemid == invoiceItemid).ToList();
            for (int i = 0; i < items.Count; i++)
            {
                var mctype = _pmsContext.Items.Find(items[i].Id);
                _pmsContext.Entry(mctype).State = EntityState.Modified;
                mctype.Taxrate = rate;
            }
            _pmsContext.SaveChanges();

        }
        //业主消费项目
        public List<Item> getOwnerfeeItem(string hid)
        {
            return _pmsContext.Items.Where(w => w.Hid == hid && w.DcFlag == "D" && (w.IsOwnerFee == true || w.IsOwnerAmount == true) && w.Name != null && (byte)w.Status < (byte)EntityStatus.禁用).OrderBy(w => w.Seqid).ToList();
        }


        /// <summary>
        /// 增加集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        public override List<Item> AddAndCopy(Item groupModel, string groupId, string dataControlCode, List<string> selectedResortHids)
        {
            var datacode = "";
            if (groupModel.DcFlag == "d")
            {
                datacode = M_V_BasicDataType.BasicDataCodeItemConsume;
            }
            else
            {
                datacode = M_V_BasicDataType.BasicDataCodeItemPay;
            }
            var addPara = new BasicDataAddAndCopyModel<Item>
            {
                GroupId = groupId,
                BasicDataCode = datacode,
                BasicDataService = this,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _pmsContext,
                GroupModel = groupModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.AddAndCopy(addPara);
        }
        /// <summary>
        /// 修改并分发
        /// </summary>
        /// <param name="groupModel"></param>
        /// <param name="originModel"></param>
        /// <param name="fieldNames"></param>
        /// <param name="groupId"></param>
        /// <param name="dataControlCode"></param>
        /// <param name="selectedResortHids"></param>
        /// <param name="updateProperties"></param>
        /// <returns></returns>
        public override List<Item> EditAndCopy(Item groupModel, Item originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties)
        {
            var datacode = "";
            if (originModel.DcFlag == "d")
            {
                datacode = M_V_BasicDataType.BasicDataCodeItemConsume;
            }
            else
            {
                datacode = M_V_BasicDataType.BasicDataCodeItemPay;
            }
            var editPara = new BasicDataEditAndCopyModel<Item>
            {

                BasicDataCode = datacode,
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
        public override List<Item> ChangeStatusAndCopy(Item model, EntityStatus status)
        {
            var changePara = new BasicDataStatusChangeAndCopyModel<Item>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = model,
                Status = status
            };
            return BasicDataServiceHelper.ChangeGroupAndHotelCopiedStatus(changePara);
        }
        public override List<Item> DeleteGroupAndHotelCopied(Item groupModel)
        {
            var deletePara = new BasicDataDeleteGroupAndHotelCopiedModel<Item>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = groupModel
            };
            return BasicDataServiceHelper.DeleteGroupAndHotelCopied(deletePara);
        }
        public Item GetNewHotelBasicData(string hid, Item groupModel)
        {
            var hotelModel = new Item();
            AutoSetValueHelper.SetValues(groupModel, hotelModel);
            hotelModel.Hid = hid;
            hotelModel.Id = hid + hotelModel.Code;
            hotelModel.ItemTypeid = string.IsNullOrEmpty(groupModel.ItemTypeid) ? "" : groupModel.ItemTypeid.Replace(groupModel.ItemTypeid.Substring(0, 6), hid);
            hotelModel.InvoiceItemid = string.IsNullOrEmpty(groupModel.InvoiceItemid) ? "" : groupModel.InvoiceItemid.Replace(groupModel.InvoiceItemid.Substring(0, 6), hid);
            hotelModel.DataCopyId = groupModel.Id;
            hotelModel.DataSource = BasicDataDataSource.Copyed.Code;
            Item cl = _pmsContext.Items.FirstOrDefault(w => w.Id == hotelModel.Id && w.Hid == hid);
            if (cl != null) { return null; }
            return hotelModel;
        }

        public Item GetCopyedHotelBasicData(string hid, Item groupModel, bool iscopyed)
        {
            if (iscopyed)
            {
                return _pmsContext.Items.FirstOrDefault(w => w.DataCopyId == groupModel.Id && w.Hid == hid && w.DataSource == BasicDataDataSource.Copyed.Code);
            }
            else
            {
                return _pmsContext.Items.FirstOrDefault(w => w.Id == groupModel.Id.Replace(groupModel.Hid, hid) && w.Hid == hid);
            }

        }

        public List<Item> GetCopyedHotelBasicDatas(Item groupModel)
        {
            return _pmsContext.Items.Where(w => w.DataCopyId == groupModel.Id && w.DataSource == BasicDataDataSource.Copyed.Code).ToList();
        }
    }
}

