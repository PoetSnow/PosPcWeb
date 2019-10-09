using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EF.Common;
using Gemstar.BSPMS.Hotel.Services.AuthManages;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class CodeListService : CRUDService<CodeList>, ICodeListService, IBasicDataCopyService<CodeList>
    {
        public CodeListService(DbHotelPmsContext db) : base(db, db.CodeLists)
        {
            _pmsContext = db;
        }
        protected override CodeList GetTById(string id)
        {
            return new CodeList { Pk = int.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        protected override void BeforeDelete(CodeList obj)
        {
            CodeList entity = null;
            var list = _pmsContext.Database.SqlQuery<CodeList>("select * from CodeList where pk={0}", obj.Pk);
            if (list.Count() > 0)
            {
                entity = list.FirstOrDefault();
            }
            if (entity == null)
            {
                throw new ArgumentException("参数不存在");
            }
            if (entity.TypeCode == "06" && _pmsContext.Rooms.Where(c => c.Hid == entity.Hid && c.Floorid == entity.Id).Any())//06=房间楼层
            {
                throw new ArgumentException("此楼层内有房间，请先删除房间。");
            }
            else if (entity.TypeCode == "11" && _pmsContext.Companys.Where(c => c.Hid == entity.Hid && c.CompanyTypeid == entity.Id).Any())//11=合约单位类别
            {
                throw new ArgumentException("此类型在合约单位中已使用，不能删除。");
            }
        }

        /// <summary>
        /// 获取代码类型列表
        /// </summary>
        /// <returns></returns>
        public List<CodeType> CodeTypeList()
        {
            return _pmsContext.Database.SqlQuery<CodeType>("select * from v_codeType where isvisible = {0}", "1").ToList();
        }
        /// <summary>
        /// 获取单个代码类型实体
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public CodeType GetCodeType(string code)
        {
            var list = _pmsContext.Database.SqlQuery<CodeType>("select * from v_codeType where code = {0}", code).ToList();
            if (list != null && list.Count > 0)
            {
                return list.FirstOrDefault();
            }
            throw new ArgumentException(string.Format("{0},此代码类型不存在。", code));
        }

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
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
                    CodeList update = _pmsContext.CodeLists.Find(int.Parse(id));
                    if (update.Status != status)
                    {
                        update.Status = status;
                        _pmsContext.Entry(update).State = EntityState.Modified;
                    }
                }
                _pmsContext.AddDataChangeLogs(OpLogType.通用代码启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 是否已存在此记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="typeCode">代码类型</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public bool IsExists(string hid, string typeCode, string code)
        {
            string id = hid + typeCode + code;
            return _pmsContext.CodeLists.Where(c => c.Hid == hid && c.TypeCode == typeCode && (c.Code == code || c.Id == id)).Any();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="typeCode">类型代码</param>
        /// <returns></returns>
        public List<CodeList> List(string hid, string typeCode)
        {
            return _pmsContext.CodeLists.Where(c => c.Hid == hid && c.TypeCode == typeCode && c.Status == EntityStatus.启用).OrderBy(o => o.Seqid).ToList();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="typeCode">类型代码</param>
        /// <returns></returns>
        public List<V_codeListPub> List(string typeCode)
        {
            return _pmsContext.Database.SqlQuery<V_codeListPub>("select * from v_codeListPub where typeCode = {0} and status=1", typeCode).OrderBy(w => w.seqid).ToList();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="typeCode">类型代码</param>
        /// <returns></returns>
        public List<V_codeListPub> List(string typeCode, ProductType productType)
        {
            var index = (byte)productType;
            return _pmsContext.Database.SqlQuery<V_codeListPub>("select * from v_codeListPub where typeCode = {0} and status=1 AND SUBSTRING(mask,{1},1) = '1'", typeCode, index).OrderBy(w => w.seqid).ToList();
        }
        /*
         下面的具体代码请以类型代码的升序排列，以方便查看是否有现成的方法可以直接调用
             */
        /// <summary>
        /// 获取客账中的消费项目类型和付款项目类型
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns></returns>
        public List<CodeList> GetFolioItemTypes(string hid)
        {
            List<CodeList> list = List(hid, "02");
            list.AddRange(List(hid, "03"));
            return list;
        }
        /// <summary>
        /// 获取指定酒店的市场分类
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的市场分类</returns>
        public List<CodeList> GetMarketCategory(string hid)
        {
            return List(hid, "04");
        }
        /// <summary>
        /// 获取指定酒店的客人来源
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的客人来源</returns>
        public List<CodeList> GetCustomerSource(string hid)
        {
            return List(hid, "05");
        }
        /// <summary>
        /// 获取指定酒店的房间特色
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的房间特色</returns>
        public List<CodeList> GetRoomFeatures(string hid)
        {
            return List(hid, "08");
        }
        /// <summary>
        /// 获取证件类型
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetCerType()
        {
            return List("09");
        }
        /// <summary>
        /// 获取指定酒店的合约单位类别
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的合约单位类别</returns>
        public List<CodeList> GetCompanyType(string hid)
        {
            return List(hid, "11");
        }
        /// <summary>
        /// 获取会员账户类型
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetAccountType()
        {
            return List("12");
        }
        /// <summary>
        /// 获取pos模块列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosModules()
        {
            return List("31");
        }
        /// <summary>
        /// 获取pos类别列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIstagclasss()
        {
            return List("32");
        }
        /// <summary>
        /// 获取pos出品方式列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosProduceTypes()
        {
            return List("33");
        }
        /// <summary>
        /// 获取pos微信点餐支付方式列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosWxPaytypes()
        {
            return List("34");
        }
        /// 获取pos临时台标志列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIstagtemps()
        {
            return List("35");
        }
        /// <summary>
        /// 获取pos餐台状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosStatnos()
        {
            return List("36");
        }
        /// <summary>
        /// 获取pos财务类型列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosAcClasss()
        {
            return List("37");
        }
        /// <summary>
        /// 获取pos币种
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosMontypeno()
        {
            return List("38");
        }
        /// <summary>
        /// 获取pos处理方式
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosPayType(ProductType productType)
        {
            return List("01", productType);
        }
        /// <summary>
        /// 获取pos要求操作列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosiTagOperator()
        {
            return List("40");
        }
        /// <summary>
        /// 获取pos联单打印列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsCombine()
        {
            return List("41");
        }
        /// <summary>
        /// 获取pos要求属性列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsTagProperty()
        {
            return List("42");
        }
        /// <summary>
        /// 获取pos出品状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsProduce()
        {
            return List("43");
        }
        /// <summary>
        /// 获取pos显示临时台列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsHideTab()
        {
            return List("44");
        }
        /// <summary>
        /// 获取pos出品名称列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsProdName()
        {
            return List("45");
        }
        /// <summary>
        /// 获取pos状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosStatus()
        {
            return List("46");
        }
        /// <summary>
        /// 获取pos点作法选项列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsOrderAction()
        {
            return List("47");
        }
        /// <summary>
        /// 获取pos减库存列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsStock()
        {
            return List("48");
        }
        /// <summary>
        /// 获取pos日期类型列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosITagperiod()
        {
            return List("49");
        }
        /// <summary>
        /// 获取pos最低消费计法
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsByPerson()
        {
            return List("50");
        }
        /// <summary>
        /// 获取pos数量方式列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosQuanMode()
        {
            return List("51");
        }
        /// <summary>
        /// 获取pos收费状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsCharge()
        {
            return List("52");
        }
        /// <summary>
        /// 获取pos串口号列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosComno()
        {
            return List("53");
        }
        /// <summary>
        /// 获取pos开台录入信息列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosOpenInfo()
        {
            return List("56");
        }
        /// <summary>
        /// 获取pos折扣类型列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsForce()
        {
            return List("57");
        }
        /// <summary>
        /// 获取pos金额折类型列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosDaType()
        {
            return List("58");
        }
        /// <summary>
        /// 获取pos账单状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosBillStatus()
        {
            return List("59");
        }
        /// <summary>
        /// 获取posKTV开台类型列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIKtvStatus()
        {
            return List("60");
        }
        /// <summary>
        /// 获取pos自动标志列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosIsauto()
        {
            return List("61");
        }
        /// <summary>
        /// 获取pos账单明细状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosBillDetailStatus()
        {
            return List("62");
        }
        /// <summary>
        /// 获取pos账单明细出品状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosBillDetailIsProduce()
        {
            return List("63");
        }
        /// <summary>
        /// 获取pos客人类型状态列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosCustomerTypeStatus()
        {
            return List("64");
        }
        /// <summary>
        /// 获取pos原因类型列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPosReasonIstagType()
        {
            return List("66");
        }
        /// <summary>
        /// 获取楼层
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<CodeList> GetFloorType(string hid)
        {
            return _pmsContext.CodeLists.Where(w => w.Hid == hid && w.TypeCode == "06").ToList();
        }
        /// <summary>
        /// 获取业务员提成数据类型
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPercentagesSalesmanType()
        {
            return List("27");
        }
        /// <summary>
        /// 获取操作员提成数据类型
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetPercentagesOperatorType()
        {
            return List("29");
        }
        /// <summary>
        /// 获取发票开票项目
        /// </summary>
        /// <returns></returns>
        public List<CodeList> GetInvoiceProjectType(string hid)
        {
            return List(hid, "13");
        }
        public string GetFloorName(string id)
        {
            return _pmsContext.CodeLists.Where(c => c.Id == id).First().Name;
        }
        /// <summary>
        /// 通过ID获取合约单位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CodeList GetCodeListByID(string id)
        {
            return _pmsContext.CodeLists.Where(c => c.Id == id).First();
        }
        /// <summary>
        /// 获取item IsQuantity是数量的codelist
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<CodeList> GetItemsProducts(string hid)
        {
            var itemids = _pmsContext.Items.Where(w => w.Hid == hid && w.IsQuantity == true && (byte)w.Status < (byte)EntityStatus.禁用).GroupBy(g => g.ItemTypeid).Select(s => s.Key).ToList();
            var data = _pmsContext.CodeLists.Where(w => itemids.Contains(w.Id)).ToList();
            return data;
        }
        /// <summary>
        /// 同步集团的公用代码到分店
        /// </summary>
        /// <param name="grpid"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="name2"></param>
        /// <param name="seqid"></param>
        /// <param name="typecode"></param>
        public void updateGrpHotelCodelist(string grpid, string typecode)
        {
            _pmsContext.Database.ExecuteSqlCommand("exec up_UpdateGrpHotelCodelist @grpid={0},@typecode={1}", grpid, typecode);
        }



        /// <summary>
        /// 增加集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        public override List<CodeList> AddAndCopy(CodeList groupModel, string groupId, string dataControlCode, List<string> selectedResortHids)
        {
            var addPara = new BasicDataAddAndCopyModel<CodeList>
            {
                GroupId = groupId,
                BasicDataCode = groupModel.TypeCode,
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
        public override List<CodeList> EditAndCopy(CodeList groupModel, CodeList originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties)
        {
            var editPara = new BasicDataEditAndCopyModel<CodeList>
            {
                BasicDataCode = groupModel.TypeCode,
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
        public override List<CodeList> ChangeStatusAndCopy(CodeList model, EntityStatus status)
        {
            var changePara = new BasicDataStatusChangeAndCopyModel<CodeList>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = model,
                Status = status
            };
            return BasicDataServiceHelper.ChangeGroupAndHotelCopiedStatus(changePara);
        }
        public override List<CodeList> DeleteGroupAndHotelCopied(CodeList groupModel)
        {
            var deletePara = new BasicDataDeleteGroupAndHotelCopiedModel<CodeList>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = groupModel
            };
            return BasicDataServiceHelper.DeleteGroupAndHotelCopied(deletePara);
        }

        public CodeList GetNewHotelBasicData(string hid, CodeList groupModel)
        {
            var hotelModel = new CodeList();
            AutoSetValueHelper.SetValues(groupModel, hotelModel);
            hotelModel.Hid = hid;
            hotelModel.Id = hid + hotelModel.TypeCode + hotelModel.Code;
            hotelModel.DataCopyId = groupModel.Id;
            hotelModel.DataSource = BasicDataDataSource.Copyed.Code;
            //判断id是否已经存在,当分店的数据存在时，而DataSource不是copy值时会出现重复
            CodeList cl = _pmsContext.CodeLists.FirstOrDefault(w => w.Id == hotelModel.Id && w.Hid == hid);
            if (cl != null) { return null; }
            return hotelModel;
        }

        public CodeList GetCopyedHotelBasicData(string hid, CodeList groupModel, bool iscopyed)
        {
            return _pmsContext.CodeLists.FirstOrDefault(w => w.DataCopyId == groupModel.Id && w.Hid == hid && w.DataSource == BasicDataDataSource.Copyed.Code);
        }

        public List<CodeList> GetCopyedHotelBasicDatas(CodeList groupModel)
        {
            return _pmsContext.CodeLists.Where(w => w.DataCopyId == groupModel.Id && w.DataSource == BasicDataDataSource.Copyed.Code).ToList();
        }

        public string checkIsExistOtherTable(string hid, string id)
        {
            string tbname = "";
            List<Res> list = _pmsContext.Reses.Where(c => c.Hid == hid && (c.Sourceid == id || c.Marketid == id)).ToList();
            if (list.Count > 0)
            {
                tbname += "订单主表、";
            }
            int i = _pmsContext.Database.SqlQuery<int>("select count(1) from resFolio where (Sourceid={0} or Marketid={0}) and hid={1}", id, hid).Single();
            if (i > 0)
            {
                tbname += "账务明细表、";
            }
            List<ResDetail> list2 = _pmsContext.ResDetails.Where(c => c.Hid == hid && (c.Sourceid == id || c.Marketid == id)).ToList();
            if (list2.Count > 0)
            {
                tbname += "订单明细表、";
            }
            List<Rate> list3 = _pmsContext.Rates.Where(c => c.Hid == hid && (c.Sourceid == id || c.Marketid == id)).ToList();
            if (list3.Count > 0)
            {
                tbname += "价格代码表、";
            }
            List<RoomStatus> list4 = _pmsContext.RoomStatuses.Where(c => c.Hid == hid && (c.Sourceid == id)).ToList();
            if (list4.Count > 0)
            {
                tbname += "房态表、";
            }
            List<Channel> list5 = _pmsContext.Channels.Where(c => c.Hid == hid && (c.Marketid == id)).ToList();
            if (list5.Count > 0)
            {
                tbname += "渠道表、";
            }
            return tbname;
        }

        /// <summary>
        /// 获取消费项目显示
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetShowSetList()
        {
            return List("87");
        }

        /// <summary>
        /// 日期类型列表
        /// </summary>
        /// <returns></returns>
        public List<V_codeListPub> GetiTagperiodList()
        {
            return List("88");
        }
    }
}
