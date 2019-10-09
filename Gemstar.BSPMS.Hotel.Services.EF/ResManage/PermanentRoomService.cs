using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using System.Data.SqlClient;
using Gemstar.BSPMS.Hotel.Services.ResManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.ResManage
{
    public class PermanentRoomService: CRUDService<PermanentRoomSet>, IPermanentRoomService
    {
        public PermanentRoomService(DbHotelPmsContext db) : base(db, db.PermanentRoomSets)
        {
            _pmsContext = db;
        }
        protected override PermanentRoomSet GetTById(string id)
        {
            return new PermanentRoomSet { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 根据账号获取订单是否属于长包房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号ID</param>
        /// <returns></returns>
        public bool IsPermanentRoom(string hid, string regid)
        {
            string rateCode = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid && (c.Status == "R" || c.Status == "I" || c.Status == "O" || c.Status == "C")).Select(c => c.RateCode).AsNoTracking().FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(rateCode))
            {
                if(_pmsContext.Rates.AsNoTracking().Any(c => c.Hid == hid && c.Id == rateCode && c.isMonth == true))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 根据账号获取订单的长包房设置
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号ID</param>
        /// <returns></returns>
        public PermanentRoomInfo.PermanentRoomSet Get(string hid, string regid)
        {
            var entity = _pmsContext.PermanentRoomSets.Where(c => c.Hid == hid && c.Regid == regid).FirstOrDefault();
            if (entity != null)
            {
                PermanentRoomInfo.PermanentRoomSet result = new PermanentRoomInfo.PermanentRoomSet();
                result.Hid = entity.Hid;
                result.Id = entity.Id;
                result.Regid = entity.Regid;
                result.RentType = entity.RentType;
                result.WaterMeter = entity.WaterMeter;
                result.WattMeter = entity.WattMeter;
                result.NaturalGas = entity.NaturalGas;
                result.PermanentRoomFixedCostSets = new List<PermanentRoomInfo.PermanentRoomFixedCostSet>();
                var subList = _pmsContext.PermanentRoomFixedCostSets.Where(c => c.Hid == hid && c.PermanentRoomSetId == entity.Id).ToList();
                if(subList != null)
                {
                    foreach(var item in subList)
                    {
                        result.PermanentRoomFixedCostSets.Add(new PermanentRoomInfo.PermanentRoomFixedCostSet {
                            Hid = item.Hid,
                            Id = item.Id,
                            PermanentRoomSetId = item.PermanentRoomSetId,
                            Itemid = item.Itemid,
                            Amount = item.Amount,
                            Type = item.Type,
                        });
                    }
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// 保存长包房设置
        /// </summary>
        /// <param name="model">数据</param>
        /// <returns></returns>
        public JsonResultData Save(ICurrentInfo currentInfo, PermanentRoomInfo.PermanentRoomSetPara model)
        {
            string hid = currentInfo.HotelId;
            if (string.IsNullOrWhiteSpace(hid))//当前登录用户
            {
                return JsonResultData.Failure("无法获取当前登录的用户信息，请重新登录！", 1);
            }

            #region 参数验证

            if (model == null || string.IsNullOrWhiteSpace(model.Hid) || string.IsNullOrWhiteSpace(model.Regid))//参数 酒店ID和账号ID
            {
                return JsonResultData.Failure("参数错误，账号不能为空！");
            }

            if(model.RentType != 1 && model.RentType != 2 && model.RentType != 3)
            {
                return JsonResultData.Failure("参数错误，请选择正确的过租方式！");
            }

            if(model.WaterMeter < 0)
            {
                return JsonResultData.Failure("请填写大于0的水表初始读数！");
            }
            if (model.WattMeter < 0)
            {
                return JsonResultData.Failure("请填写大于0的电表初始读数！");
            }
            if (model.NaturalGas < 0)
            {
                return JsonResultData.Failure("请填写大于0的燃气初始读数！");
            }

            var isFixedCost = (model.FixedCost != null && model.FixedCost.Count > 0);
            List<KeyValuePairModel<string,string>> newItemids = new List<KeyValuePairModel<string, string>>();
            if (isFixedCost)
            {
                List<string> oldItemids = model.FixedCost.Select(c => c.Itemid).Distinct().ToList();
                newItemids = _pmsContext.Items.Where(c => c.Hid == hid && c.DcFlag == "D" && oldItemids.Contains(c.Id)).Select(c => new KeyValuePairModel<string, string> { Key = c.Id, Value = c.Name }).AsNoTracking().ToList();
                foreach(var item in model.FixedCost)
                {
                    if (string.IsNullOrWhiteSpace(item.Itemid))
                    {
                        return JsonResultData.Failure("请选择消费项目！");
                    }
                    if (!newItemids.Any(c => c.Key == item.Itemid))
                    {
                        return JsonResultData.Failure("请选择正确的消费项目！");
                    }
                    if(item.Amount <= 0)
                    {
                        return JsonResultData.Failure("请填写大于0的每月价格！");
                    }
                    if (item.Type != 1 && item.Type != 2)
                    {
                        return JsonResultData.Failure("请选择类型！");
                    }
                    if(model.RentType == 1 && item.Type == 2)
                    {
                        return JsonResultData.Failure("每天过租不能使用包费！");
                    }
                }
            }
            else
            {
                model.FixedCost = new List<PermanentRoomInfo.PermanentRoomFixedCostSetPara>();
            }

            #endregion

            if (hid != model.Hid)//当前登录酒店ID 和 参数酒店ID
            {
                return JsonResultData.Failure("参数错误，请刷新当前页面后重试！");
            }

            if (!IsPermanentRoom(model.Hid, model.Regid))//是否长包房
            {
                return JsonResultData.Failure("此订单不是在住的长包房！");
            }

            var entity = _pmsContext.PermanentRoomSets.Where(c => c.Hid == model.Hid && c.Regid == model.Regid).FirstOrDefault();

            if(!( ((model.Id == null || model.Id.HasValue == false) && entity == null)
                  ||
                  ((model.Id != null && model.Id.HasValue == true) && entity != null && model.Id == entity.Id)
                ))//与数据库中验证
            {
                return JsonResultData.Failure("参数错误！");
            }

            string log = "";
            Guid id = Guid.NewGuid();
            if (entity == null)
            {//添加新的设置
                _pmsContext.PermanentRoomSets.Add(new PermanentRoomSet {
                    Id = id,
                    Hid = hid,
                    Regid = model.Regid,
                    RentType = model.RentType,
                    WaterMeter = model.WaterMeter,
                    WattMeter = model.WattMeter,
                    NaturalGas = model.NaturalGas,
                });
                log += string.Format("过租方式：{0}，", GetRentTypeName(model.RentType));
                log += string.Format("水表初始读数：{0}，", model.WaterMeter);
                log += string.Format("电表初始读数：{0}，", model.WattMeter);
                log += string.Format("燃气初始读数：{0}，", model.NaturalGas);
            }
            else
            {//修改设置
                id = entity.Id;
                if(entity.RentType != model.RentType)
                {
                    log += string.Format("过租方式：{0}=>{1}，", GetRentTypeName(entity.RentType), GetRentTypeName(model.RentType));
                    entity.RentType = model.RentType;
                }
                if(entity.WaterMeter != model.WaterMeter)
                {
                    log += string.Format("水表初始读数：{0}=>{1}，", entity.WaterMeter, model.WaterMeter);
                    entity.WaterMeter = model.WaterMeter;
                }
                if(entity.WattMeter != model.WattMeter)
                {
                    log += string.Format("电表初始读数：{0}=>{1}，", entity.WattMeter, model.WattMeter);
                    entity.WattMeter = model.WattMeter;
                }
                if(entity.NaturalGas != model.NaturalGas)
                {
                    log += string.Format("燃气初始读数：{0}=>{1}，", entity.NaturalGas, model.NaturalGas);
                    entity.NaturalGas = model.NaturalGas;
                }
                _pmsContext.Entry(entity).State = EntityState.Modified;
            }

            //删除固定费用
            var delFixedCosts = _pmsContext.PermanentRoomFixedCostSets.Where(c => c.Hid == hid && c.PermanentRoomSetId == id).ToList();

            //是否记录固定费用日志
            bool isAddLog = false;
            if(delFixedCosts.Count == model.FixedCost.Count)
            {
                foreach (var item in delFixedCosts)
                {
                    if (!model.FixedCost.Where(c => c.Itemid == item.Itemid && c.Amount == item.Amount && c.Type == item.Type).Any())
                    {
                        isAddLog = true;
                        break;
                    }
                }
            }
            else
            {
                isAddLog = true;
            }

            if (delFixedCosts != null && delFixedCosts.Count > 0)
            {
                _pmsContext.PermanentRoomFixedCostSets.RemoveRange(delFixedCosts);
            }
            //添加固定费用
            string logFixedCost = "";
            if (isFixedCost)
            {
                foreach (var item in model.FixedCost)
                {
                    _pmsContext.PermanentRoomFixedCostSets.Add(new PermanentRoomFixedCostSet
                    {
                        Hid = hid,
                        PermanentRoomSetId = id,
                        Itemid = item.Itemid,
                        Amount = item.Amount,
                        Type = item.Type,
                    });
                    if (isAddLog)
                    {
                        var itemTemp = newItemids.FirstOrDefault(c => c.Key == item.Itemid);
                        logFixedCost += string.Format("[消费项目：{0}，金额：{1}，类型：{2}]，", (itemTemp != null ? itemTemp.Value : item.Itemid), item.Amount, GetCostSetTypeName(item.Type));
                    }
                }
            }
            if (isAddLog)
            {
                if (logFixedCost.EndsWith("，"))
                {
                    logFixedCost = logFixedCost.Remove(logFixedCost.Length - 1);
                }
                log += ("固定费用：{" + logFixedCost + "}");
            }

            if (!string.IsNullOrWhiteSpace(log))
            {
                log = (entity == null ? "增加" : "修改") + "长包房设置  " + string.Format("账号：{0}，", model.Regid.Replace(hid, "")) + log;
                if (log.EndsWith("，"))
                {
                    log = log.Remove(log.Length - 1);
                }
                _pmsContext.OpLogs.Add(new OpLog
                {
                    CDate = DateTime.Now,
                    Hid = currentInfo.HotelId,
                    CUser = currentInfo.UserName,
                    Ip = Gemstar.BSPMS.Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress(),
                    XType = Gemstar.BSPMS.Common.Services.Enums.OpLogType.长包房设置.ToString(),
                    CText = (log.Length > 4000 ? log.Substring(0, 4000) : log),
                    Keys = model.Regid,
                });
            }

            //保存
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 根据过租方式获取名称
        /// </summary>
        /// <param name="rentType"></param>
        /// <returns></returns>
        private string GetRentTypeName(byte rentType)
        {
            string result = "";
            switch (rentType)
            {
                case 1:
                    result = "每天过租";
                    break;
                case 2:
                    result = "满一个月过租";
                    break;
                case 3:
                    result = "月底过租";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取固定费用类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetCostSetTypeName(byte? type)
        {
            if(type == 1)
            {
                return "加收";
            }
            else if(type == 2)
            {
                return "包费";
            }
            else
            {
                return (type != null || type.HasValue) ? type.ToString() : "";
            }
        }

        /// <summary>
        /// 获取所有长包房在住订单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        public List<KeyValuePairModel<string, string>> GetAllPermanentRoomIOrder(string hid)
        {
            var rateCodeIsMonth = _pmsContext.Rates.Where(c => c.Hid == hid && c.isMonth == true).Select(c => c.Id).ToList();
            if(rateCodeIsMonth == null || rateCodeIsMonth.Count <= 0)
            {
                return new List<KeyValuePairModel<string, string>>();
            }

            var resDetails = _pmsContext.ResDetails.Where(c => c.Hid == hid && rateCodeIsMonth.Contains(c.RateCode) && c.Status == "I").Select(c => new KeyValuePairModel<string, string> { Key = c.Regid, Value = c.RoomNo, Data = c.Regid.Substring(c.Hid.Length) }).OrderBy(c => c.Value).AsNoTracking().ToList();
            if(resDetails == null || resDetails.Count <= 0)
            {
                return new List<KeyValuePairModel<string, string>>();
            }
            return resDetails;
        }

        /// <summary>
        /// 验证导入长包房账务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public JsonResultData CheckImportPermanentRoomFolio(ICurrentInfo currentInfo, string itemId, List<PermanentRoomInfo.PermanentRoomImportFolioPara> list)
        {
            //验证酒店ID
            string hid = currentInfo.HotelId;
            if (string.IsNullOrWhiteSpace(hid))
            {
                return JsonResultData.Failure("获取不到当前登录用户信息！");
            }
            //验证消费项目
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return JsonResultData.Failure("请选择消费项目！");
            }
            var itemDEntity = _pmsContext.Items.Where(c => c.Hid == hid && c.Id == itemId && c.DcFlag == "D").AsNoTracking().FirstOrDefault();
            if(itemDEntity == null)
            {
                return JsonResultData.Failure("请选择消费项目！");
            }
            //验证参数
            if(list == null  || list.Count <= 0)
            {
                return JsonResultData.Failure("请填写表单！");
            }
            foreach(var item in list)
            {
                if (string.IsNullOrWhiteSpace(item.Regid))
                {
                    return JsonResultData.Failure("账号不能为空！");
                }
                if (string.IsNullOrWhiteSpace(item.RoomNo))
                {
                    return JsonResultData.Failure("房号不能为空！");
                }

                if(item.AmountD == null || item.AmountD.HasValue == false || item.AmountD.Value < 0)
                {
                    return JsonResultData.Failure("请输入金额！");
                }
                if (itemDEntity.IsQuantity == true)
                {
                    if(item.Quantity == null || item.Quantity.HasValue == false)
                    {
                        item.Quantity = 1;
                    }
                    if(item.Quantity.Value <= 0)
                    {
                        return JsonResultData.Failure("请输入正确的数量！");
                    }
                }
                if(itemDEntity.Action != "51" && itemDEntity.Action != "52" && itemDEntity.Action != "53")
                {
                    item.LastTimeMeterReading = null;
                    item.ThisTimeMeterReading = null;
                }
            }

            var resDetails = GetAllPermanentRoomIOrder(hid);
            foreach(var item in list)
            {
                if(!resDetails.Where(c => c.Key == item.Regid && c.Value == item.RoomNo).Any())
                {
                    return JsonResultData.Failure(string.Format("账号：{0}，房号：{1}，找不到此在住长包房订单！", item.Regid.Substring(hid.Length), item.RoomNo));
                }
            }

            if(list.Select(c => c.RoomNo).GroupBy(c => c.ToString()).Count()  != list.Count)
            {
                return JsonResultData.Failure("请去除重复的房号！");
            }

            if (list.Select(c => c.Regid).GroupBy(c => c.ToString()).Count() != list.Count)
            {
                return JsonResultData.Failure("请去除重复的账号！");
            }

            return JsonResultData.Successed();
        }

        /// <summary>
        /// 获取最后抄表数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regidAndRoomNos">账号ID，房号</param>
        /// <param name="action">51水费，52电费，53燃气</param>
        public List<KeyValuePairModel<string, string>> GetLastTimeMeterReading(string hid, List<KeyValuePairModel<string, string>> regidAndRoomNos, string action)
        {
            if (string.IsNullOrWhiteSpace(hid) || regidAndRoomNos == null || regidAndRoomNos.Count <= 0)
            {
                return null;
            }
            if(action != "51" && action != "52" && action != "53")
            {
                return null;
            }
            //获取付款方式
            var itemids  = _pmsContext.Items.Where(c => c.Hid == hid && c.DcFlag == "D" && c.Action == action).AsNoTracking().Select(c => c.Id).ToList();
            if(itemids == null || itemids.Count <= 0)
            {
                return null;
            }
            //获取账务
            var regids = regidAndRoomNos.Select(c => c.Key).ToList();
            var transids = _pmsContext.ResFolios.Where(c => c.Hid == hid && regids.Contains(c.Regid) && itemids.Contains(c.Itemid) && c.Dcflag == "D" && (c.Status == 1 || c.Status ==2)).AsNoTracking().Select(c => c.Transid).ToList();
            
            //获取抄表数
            var lastTimeMeterReading = _pmsContext.ResFolioLogs.Where(c => c.Hid == hid && c.XType == 2 && transids.Contains(c.Transid) && regids.Contains(c.Other1)).AsNoTracking().ToList();
            
            //分析
            foreach(var item in regidAndRoomNos)
            {
                bool isTrue = false;
                int meterReading = 0;
                string value = lastTimeMeterReading.Where(c => c.Hid == hid && c.XType == 2 && c.Other1 == item.Key && c.Other2 == item.Value && !string.IsNullOrWhiteSpace(c.Value2)).OrderByDescending(c => c.CDate).Select(c => c.Value2).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if(Int32.TryParse(value,out meterReading))
                    {
                        isTrue = true;
                    }
                }
                if (!isTrue)
                {
                    var setEntity = _pmsContext.PermanentRoomSets.Where(c => c.Regid == item.Key && c.Hid == hid).FirstOrDefault();
                    if(setEntity != null)
                    {
                        if(action == "51")
                        {
                            meterReading = setEntity.WaterMeter;
                            isTrue = true;
                        }
                        else if(action == "52")
                        {
                            meterReading = setEntity.WattMeter;
                            isTrue = true;
                        }
                        else if (action == "53")
                        {
                            meterReading = setEntity.NaturalGas;
                            isTrue = true;
                        }
                    }
                }
                if (isTrue)
                {
                    item.Data = meterReading;
                }
            }
            return regidAndRoomNos;
        }

        /// <summary>
        /// 删除当天导入
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="shiftid">班次ID</param>
        /// <returns></returns>
        public JsonResultData DeleteCurrentDayImport(string hid, string shiftid, string itemId)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                return JsonResultData.Failure("请指定酒店id");
            }
            if (string.IsNullOrWhiteSpace(shiftid))
            {
                return JsonResultData.Failure("请指定班次id");
            }
            try
            {
                var result = _pmsContext.Database.SqlQuery<string>("exec up_resFolio_cancelPermanentRoomImport @hid=@hid,@shiftid=@shiftid,@itemid=@itemid"
                    , new SqlParameter("@hid", hid)
                    , new SqlParameter("@shiftid", shiftid)
                    , new SqlParameter("@itemid", string.IsNullOrWhiteSpace(itemId) ? "" : itemId)
                ).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return JsonResultData.Successed(result);
                }
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 获取长包房订单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">订单ID</param>
        /// <returns></returns>
        public ResDetail GetPermanentRoomOrder(string hid, string regid)
        {
            return _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid).AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        /// 获取长包房 房号最后读数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomNo">房号</param>
        /// <returns></returns>
        public PermanentRoomInfo.LastMeter UpGetPermanentRoomLastWaterAndElectricity(string hid, string roomNo)
        {
            try
            {
                return _pmsContext.Database.SqlQuery<PermanentRoomInfo.LastMeter>("exec up_get_PermanentRoomLastWaterAndElectricity @h99hid=@h99hid,@roomNo=@roomNo"
                    , new SqlParameter("@h99hid", hid)
                    , new SqlParameter("@roomNo", roomNo)
                ).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

    }
}
