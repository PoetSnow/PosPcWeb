using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class DbHotelPmsContext : DbContext, IDataChangeLog
    {
        private string _hid, _userName;
        private HttpRequestBase _request;
        private ProductType _productType;

        static DbHotelPmsContext()
        {
            Database.SetInitializer<DbHotelPmsContext>(null);
        }

        public DbHotelPmsContext(string connStr, string hid, string userName, HttpRequestBase request, ProductType productType = ProductType.Pos) : base(connStr)
        {
            //设置一些默认值，以优化一些速度
            Configuration.ValidateOnSaveEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;//在需要修改和删除时，需要手动的打开此开关
            Configuration.LazyLoadingEnabled = false;
            _hid = hid;
            _userName = userName;
            _request = request;
            _productType = productType;
        }

        //通用的
        public DbSet<GridColumnsSettings> GridColumnsSettings { get; set; }

        //系统管理
        public DbSet<BasicDataResortControl> BasicDataResortControls { get; set; }

        public DbSet<PmsHotel> PmsHotels { get; set; }
        public DbSet<AuthList> AuthLists { get; set; }
        public DbSet<PmsUser> PmsUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleAuth> RoleAuths { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<CodeList> CodeLists { get; set; }
        public DbSet<PmsPara> PmsParas { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemScore> ItemScores { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<ChargeFree> ChargeFrees { get; set; }
        public DbSet<GuestTrans> GuestTranss { get; set; }
        public DbSet<ScoreUse> ScoreUses { get; set; }
        public DbSet<ScoreUseRule> ScoreUseRules { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<HotelDay> HotelDays { get; set; }
        public DbSet<HotelDayDetail> HotelDayDetails { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<RateDetail> RateDetails { get; set; }
        public DbSet<HotelStatus> HotelStatuses { get; set; }
        public DbSet<OpLog> OpLogs { get; set; }
        public DbSet<WeixinTemplateMessage> WeixinTemplateMessages { get; set; }
        public DbSet<AuthorizationRecord> AuthorizationRecords { get; set; }

        //房态中心
        public DbSet<RoomStatus> RoomStatuses { get; set; }

        public DbSet<RoomStatusLog> RoomStatusLogs { get; set; }
        public DbSet<WakeCall> WakeCalls { get; set; }
        public DbSet<WakeCallDetil> WakeCallDetils { get; set; }

        //会员中心
        public DbSet<MbrCardType> MbrCardTypes { get; set; }

        public DbSet<MbrCard> MbrCards { get; set; }
        public DbSet<MbrCardBalance> MbrCardBalances { get; set; }
        public DbSet<ProfileLog> ProfileLogs { get; set; }
        public DbSet<ProfileCa> ProfileCas { get; set; }
        public DbSet<ProfileTrans> ProfileTranses { get; set; }
        public DbSet<ProfileCard> ProfileCards { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        //客户关系
        public DbSet<Company> Companys { get; set; }

        public DbSet<CompanyCa> CompanyCas { get; set; }
        public DbSet<CompanyTrans> CompanyTranses { get; set; }
        public DbSet<CompanyLog> CompanyLogs { get; set; }
        public DbSet<CompanySigner> CompanySigners { get; set; }
        public DbSet<CompanySinImg> CompanySinImg { get; set; }

        //预订须知
        public DbSet<BookingNotes> BookingNotess { get; set; }

        //预留房设置
        public DbSet<RoomHold> RoomHolds { get; set; }

        public DbSet<Channel> Channels { get; set; }

        //计划任务
        public DbSet<PlanTask> PlanTasks { get; set; }

        //预订管理
        public DbSet<Res> Reses { get; set; }

        public DbSet<ResDetail> ResDetails { get; set; }
        public DbSet<ResDetailPlan> ResDetailPlans { get; set; }
        public DbSet<ResInvoiceInfo> ResInvoiceInfos { get; set; }
        public DbSet<LockLog> LockLogs { get; set; }
        public DbSet<RoomServiceLog> RoomServiceLogs { get; set; }
        public DbSet<ResBillSetting> ResBillSettings { get; set; }
        public DbSet<ResBillSettingTemplete> ResBillSettingTempletes { get; set; }

        public DbSet<RegInfo> RegInfos { get; set; }

        //账务
        public DbSet<ResFolio> ResFolios { get; set; }

        public DbSet<CardAuth> CardAuthes { get; set; }
        public DbSet<ResFolioPayInfo> ResFolioPayInfos { get; set; }
        public DbSet<WaitPayList> WaitPayLists { get; set; }
        public DbSet<ResFolioBreakfastInfo> ResFolioBreakfastInfos { get; set; }

        //发票
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<InvoiceInfo> InvoiceInfos { get; set; }

        //报表管理
        public DbSet<ReportFormat> ReportFormats { get; set; }

        public DbSet<SignatureLog> SignatureLog { get; set; }

        //报表权限
        public DbSet<RoleAuthReport> RoleAuthReports { get; set; }

        public DbSet<ReportQueryParaTemp> ReportQueryParaTemps { get; set; }
        public DbSet<ResLog> ResLogs { get; set; }
        public DbSet<ResFolioLog> ResFolioLogs { get; set; }

        //长包房
        public DbSet<PermanentRoomSet> PermanentRoomSets { get; set; }

        public DbSet<PermanentRoomFixedCostSet> PermanentRoomFixedCostSets { get; set; }
        public DbSet<PermanentRoomGoodsSet> PermanentRoomGoodsSets { get; set; }
        public DbSet<PermanentRoomGoodsSetTemplete> PermanentRoomGoodsSetTempletes { get; set; }
        public DbSet<PermanentRoomPricePlan> PermanentRoomPricePlans { get; set; }

        //分成参数名称定义
        public DbSet<RoomOwnerCalcParaDefine> RoomOwnerCalcParaDefines { get; set; }

        //分成类型定义
        public DbSet<RoomOwnerCalcType> RoomOwnerCalcTypes { get; set; }

        //分成类型定义
        public DbSet<RoomOwnerCalcDispPara> RoomOwnerCalcDispParas { get; set; }

        //业主房间委托表
        public DbSet<RoomOwnerRoomInfos> RoomOwnerRoomInfoss { get; set; }

        //业主费用记录表
        public DbSet<RoomOwnerFee> RoomOwnerFees { get; set; }

        //业主报表数据
        public DbSet<RoomOwnerCalcResult> RoomOwnerCalcResults { get; set; }

        public DbSet<SendXml> SendXmls { get; set; }

        //业务员提成
        public DbSet<PercentagesPlanSalesman> PercentagesPlanSalesmans { get; set; }

        //提成任务
        public DbSet<percentagesPlan> percentagesPlans { get; set; }

        public DbSet<PercentagesPolicySalesman> PercentagesPolicySalesmans { get; set; }

        //操作员提成
        public DbSet<PercentagesPlanOperator> PercentagesPlanOperators { get; set; }

        public DbSet<PercentagesPolicyOperator> PercentagesPolicyOperators { get; set; }

        //打扫提成
        public DbSet<PercentagesPolicyCleanRoom> PercentagesPolicyCleanRooms { get; set; }

        //消费入账权限
        public DbSet<RoleAuthItemConsume> RoleAuthItemConsumes { get; set; }

        public DbSet<RoomTypeEquipment> RoomTypeEquipments { get; set; }

        //pos
        /// <summary>
        /// 营业点对应收银点
        /// </summary>
        public DbSet<PosPos> PosPoses { get; set; }

        /// <summary>
        /// 班次表
        /// </summary>
        public DbSet<PosShift> PosShifts { get; set; }

        /// <summary>
        /// 营业点对应市别
        /// </summary>
        public DbSet<PosShuffle> PosShuffles { get; set; }

        /// <summary>
        /// 要求定义
        /// </summary>
        public DbSet<PosRequest> PosRequests { get; set; }

        /// <summary>
        /// 营业点定义
        /// </summary>
        public DbSet<PosRefe> PosRefes { get; set; }

        /// <summary>
        /// 餐台类型
        /// </summary>
        public DbSet<PosTabtype> PosTabtypes { get; set; }

        /// <summary>
        /// 餐台资料
        /// </summary>
        public DbSet<PosTab> PosTabs { get; set; }

        /// <summary>
        /// 作法分类
        /// </summary>
        public DbSet<PosActionType> PosActionTypes { get; set; }

        /// <summary>
        /// 作法基础资料
        /// </summary>
        public DbSet<PosAction> PosActions { get; set; }

        /// <summary>
        /// 单位定义
        /// </summary>
        public DbSet<PosUnit> PosUnits { get; set; }

        /// <summary>
        /// 财务分类
        /// </summary>
        public DbSet<PosAcType> PosAcTypes { get; set; }

        /// <summary>
        /// 部门类别
        /// </summary>
        public DbSet<PosDeptClass> PosDeptClasss { get; set; }

        /// <summary>
        /// 出品部门
        /// </summary>
        public DbSet<PosDepart> PosDeparts { get; set; }

        /// <summary>
        /// 消费项目大类
        /// </summary>
        public DbSet<PosItemClass> PosItemClasss { get; set; }

        /// <summary>
        /// 消费项目不同单位价格
        /// </summary>
        public DbSet<PosItemPrice> PosItemPrices { get; set; }

        /// <summary>
        /// 消费项目
        /// </summary>
        public DbSet<PosItem> PosItems { get; set; }

        /// <summary>
        /// 消费项目对应大类
        /// </summary>
        public DbSet<PosItemMultiClass> PosItemMultiClasss { get; set; }

        /// <summary>
        /// 消费项目对应营业点
        /// </summary>
        public DbSet<PosItemRefe> PosItemRefes { get; set; }

        /// <summary>
        /// 消费项目库存物品组成
        /// </summary>
        public DbSet<PostCost> PostCosts { get; set; }

        /// <summary>
        /// 服务费政策
        /// </summary>
        public DbSet<PosTabService> PosTabServices { get; set; }

        /// <summary>
        /// 开台项目
        /// </summary>
        public DbSet<PosTabOpenItem> PosTabOpenItems { get; set; }

        /// <summary>
        /// 出品打印机
        /// </summary>
        public DbSet<PosProdPrinter> PosProdPrinters { get; set; }

        /// <summary>
        /// 餐台状态
        /// </summary>
        public DbSet<PosTabStatus> PosTabStatuss { get; set; }

        /// <summary>
        /// 账单表
        /// </summary>
        public DbSet<PosBill> PosBills { get; set; }

        /// <summary>
        /// 账单明细表
        /// </summary>
        public DbSet<PosBillDetail> PosBillDetails { get; set; }

        /// <summary>
        /// 锁台列表
        /// </summary>
        public DbSet<PosTabLog> PosTabLogs { get; set; }

        /// <summary>
        /// 客人类型
        /// </summary>
        public DbSet<PosCustomerType> PosCustomerTypes { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public DbSet<PosReason> PosReasons { get; set; }

        /// <summary>
        /// 折扣类型
        /// </summary>
        public DbSet<PosDiscType> PosDiscTypes { get; set; }

        /// <summary>
        /// 账单作法明细表
        /// </summary>
        public DbSet<PosBillDetailAction> PosBillDetailActions { get; set; }

        /// <summary>
        /// 出品记录表
        /// </summary>
        public DbSet<PosProducelist> PosProducelists { get; set; }

        /// <summary>
        /// 出品记录备份表
        /// </summary>
        public DbSet<PosProducelistBak> PosProducelistBaks { get; set; }

        /// <summary>
        /// 消费项目对应作法
        /// </summary>
        public DbSet<PosItemAction> PosItemActions { get; set; }

        /// <summary>
        /// 同组作法
        /// </summary>
        public DbSet<PosActionMultisub> PosActionMultisubs { get; set; }

        /// <summary>
        /// 账单操作表
        /// </summary>
        public DbSet<PosBillChange> PosBillChanges { get; set; }

        /// <summary>
        /// 高级功能
        /// </summary>
        public DbSet<PosAdvanceFunc> PosAdvanceFuncs { get; set; }

        /// <summary>
        /// 沽清
        /// </summary>
        public DbSet<PosSellout> PosSellouts { get; set; }

        /// <summary>
        /// 套餐酒席
        /// </summary>
        public DbSet<PosItemSuit> PosItemSuits { get; set; }

        /// <summary>
        /// 用户报表收藏
        /// </summary>
        public DbSet<UserReportCollect> UserReportCollects { get; set; }

        /// <summary>
        /// 特价菜
        /// </summary>
        public DbSet<PosOnSale> PosOnSales { get; set; }

        /// <summary>
        /// 节假日设置
        /// </summary>
        public DbSet<PosHoliday> PosHolidays { get; set; }

        /// <summary>
        /// 账单打印表
        /// </summary>
        public DbSet<PosPrintBill> PosPrintBills { get; set; }

        /// <summary>
        /// 扫码点餐滚动菜式
        /// </summary>
        public DbSet<PosMScroll> PosMScrolls { get; set; }

        /// <summary>
        /// 扫码点餐Banner
        /// </summary>
        public DbSet<PosMBanner> PosMBanners { get; set; }

        /// <summary>
        /// 扫码点餐Banner
        /// </summary>
        public DbSet<PosMorderList> PosMorderLists { get; set; }

        /// <summary>
        /// 消费项目仓库耗用表
        /// </summary>
        public DbSet<PosBillCost> PosBillCosts { get; set; }

        /// <summary>
        /// 二级仓库列表
        /// </summary>
        public DbSet<PosDepot> PosDepots { get; set; }

        /// <summary>
        /// 操作员折扣设置
        /// </summary>
        public DbSet<PosOperDiscount> PosOperDiscounts { get; set; }

        /// <summary>
        /// 公用市别
        /// </summary>
        public DbSet<PosShuffleNews> PosShuffleNews { get; set; }

        /// <summary>
        /// 凭证
        /// </summary>
        public DbSet<WhVoucher> WhVoucher { get; set; }
        public DbSet<VoucherSet> VoucherSet { get; set; }
        public DbSet<WhVoucherdetail> WhVoucherdetail { get; set; }


        /// <summary>
        /// 预定详细信息
        /// </summary>
        public DbSet<PosBillOrder> PosBillOrders { get; set; }

        /// <summary>
        /// 定金表
        /// </summary>
        public DbSet<YtPrepay> YtPrepays { get; set; }

        #region 记录数据增删改日志

        /// <summary>
        /// 增加数据增删改查的日志
        /// 由ef自动检测本次savechange之前的所有增删改的记录进行记录
        /// </summary>
        /// <param name="opType">当前操作类型</param>
        public void AddDataChangeLogs(OpLogType opType, string entityName = null, bool addPrefixToEntityName = true)
        {
            var changeTracker = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);
            try
            {
                foreach (var entity in changeTracker)
                {
                    if (entity.Entity != null)
                    {
                        if (string.IsNullOrWhiteSpace(entityName))
                        {
                            entityName = GetEntityName(entity);
                        }
                        EntityState state = entity.State;
                        switch (entity.State)
                        {
                            case EntityState.Modified:
                                AddUpdateLog(entity, entityName, opType, addPrefixToEntityName);
                                break;

                            case EntityState.Added:
                                AddAddLog(entity, entityName, opType, addPrefixToEntityName);
                                break;

                            case EntityState.Deleted:
                                AddDeleteLog(entity, entityName, opType, addPrefixToEntityName);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetEntityName(DbEntityEntry entity)
        {
            var type = entity.Entity.GetType();
            //判断实体上是否有自定义日志中文名称属性，有则取中文名称属性
            var attributes = type.GetCustomAttributes(typeof(LogCNameAttribute), true);
            if (attributes.Length > 0)
            {
                var a = attributes[0] as LogCNameAttribute;
                return a.Name;
            }
            return ObjectContext.GetObjectType(type).Name;
        }

        private string PrimaryKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            if (null == objectStateEntry.EntityKey.EntityKeyValues)
            {
                //取实体中所有属性标记有key属性的值
                var keys = new List<string>();
                var type = entry.Entity.GetType();
                var propertys = type.GetProperties();
                foreach (var prop in propertys)
                {
                    if (prop.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0)
                    {
                        keys.Add(prop.GetValue(entry.Entity).ToString());
                    }
                }
                return string.Join(",", keys);
            }
            return string.Join(",", objectStateEntry.EntityKey.EntityKeyValues.Select(item => item.Value.ToString()).ToArray());
        }

        private string LogKeyValue(DbEntityEntry entity, DbPropertyValues dbValues, LogValueType valueType)
        {
            try
            {
                //取实体中所有属性标记有logkey属性的值
                var type = entity.Entity.GetType();
                var propertys = type.GetProperties();
                foreach (var prop in propertys)
                {
                    if (prop.GetCustomAttributes(typeof(LogKeyAttribute), true).Length > 0)
                    {
                        var objValue = GetValue(prop.Name, entity, dbValues, valueType);
                        if (objValue != null)
                        {
                            return objValue.ToString();
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return null;
        }

        private bool IsLogIgnored(string propertyName, DbEntityEntry entity)
        {
            var type = entity.Entity.GetType();
            var property = type.GetProperty(propertyName);
            var ignoredAttributes = property.GetCustomAttributes(typeof(LogIgnoreAttribute), true);
            bool isIgnored = ignoredAttributes.Length > 0;
            if (!isIgnored)
            {
                isIgnored = property.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0;
                if (isIgnored)
                {
                    isIgnored = (!(property.GetCustomAttributes(typeof(LogAnywayWhenEditAttribute), true).Length > 0));
                }
            }
            return isIgnored;
        }

        private bool IsLogAnywayWhenEdit(string propertyName, DbEntityEntry entity)
        {
            var type = entity.Entity.GetType();
            var property = type.GetProperty(propertyName);
            var attributes = property.GetCustomAttributes(typeof(LogAnywayWhenEditAttribute), true);
            bool hasCustomAttribute = attributes.Length > 0;
            return hasCustomAttribute;
        }

        private string PropertyCName(string propertyName, DbEntityEntry entity)
        {
            var type = entity.Entity.GetType();
            var property = type.GetProperty(propertyName);
            var attributes = property.GetCustomAttributes(typeof(LogCNameAttribute), true);
            if (attributes.Length > 0)
            {
                var a = attributes[0] as LogCNameAttribute;
                return a.Name;
            }
            return propertyName;
        }

        /// <summary>
        /// 获取实体的值，根据是否删除取不同来源的值
        /// 不是删除时，取当前值
        /// 删除时，优先取dbvalues中的值，其次取originvalues中的值
        /// </summary>
        /// <param name="isDelete">是否删除操作</param>
        /// <param name="dbValues">数据库中的值</param>
        /// <param name="originValues">实体原始值</param>
        /// <param name="currentValues">实体当前值</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>对应的值</returns>
        private object GetValue(string propertyName, DbEntityEntry entity, DbPropertyValues dbValues, LogValueType valueType)
        {
            object value = null;
            if (valueType == LogValueType.CurrentValue)
            {
                //增加状态取当前值
                value = entity.CurrentValues[propertyName];
            }
            else
            {
                if (dbValues != null)
                {
                    value = dbValues[propertyName];
                }
                else if (entity.OriginalValues != null)
                {
                    value = entity.OriginalValues[propertyName];
                }
            }
            return value;
        }

        private string PropertyValue(string propertyName, DbEntityEntry entity, DbPropertyValues dbValues, LogValueType valueType)
        {
            object currentValues = GetValue(propertyName, entity, dbValues, valueType);
            if (currentValues == null) { return null; }
            var type = entity.Entity.GetType();
            var property = type.GetProperty(propertyName);
            //判断是否有外键属性，如果有则根据属性中的语句和参数字段组成sql语句来取值
            var refrenceNameAttributes = property.GetCustomAttributes(typeof(LogRefrenceNameAttribute), true);
            if (refrenceNameAttributes.Length > 0)
            {
                var refrenceName = refrenceNameAttributes[0] as LogRefrenceNameAttribute;
                return getRefrenceName(refrenceName, propertyName, entity, valueType, dbValues);
            }
            //不具有外键属性，则直接取字段的值
            {
                //LogBoolAttribute
                var attributes = property.GetCustomAttributes(typeof(LogBoolAttribute), true);
                if (attributes.Length > 0)
                {
                    var a = attributes[0] as LogBoolAttribute;
                    var b = Convert.ToBoolean(currentValues);
                    return b ? a.TrueName : a.FalseName;
                }
            }

            {
                //LogDatetimeFormatAttribute
                var attributes = property.GetCustomAttributes(typeof(LogDatetimeFormatAttribute), true);
                if (attributes.Length > 0)
                {
                    var a = attributes[0] as LogDatetimeFormatAttribute;
                    return (currentValues as DateTime?).Value.ToString(a.Format);
                }
            }
            {
                //LogStartsWithHidAttribute
                {
                    var attributes = property.GetCustomAttributes(typeof(LogStartsWithHidAttribute), true);
                    if (attributes.Length > 0)
                    {
                        if (currentValues.ToString().StartsWith(_hid))
                        {
                            return currentValues.ToString().Substring(_hid.Length);
                        }
                    }
                }
                //LogEnumAttribute
                {
                    var attributes = property.GetCustomAttributes(typeof(LogEnumAttribute), true);
                    if (attributes.Length > 0)
                    {
                        var a = attributes[0] as LogEnumAttribute;
                        return EnumExtension.GetDescription(a.EnumType, currentValues.ToString());
                    }
                }
            }

            return currentValues.ToString();
        }

        private string getRefrenceName(LogRefrenceNameAttribute refrenceName, string propertyName, DbEntityEntry entity, LogValueType valueType, DbPropertyValues dbValues)
        {
            if (refrenceName == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(refrenceName.Sql))
            {
                return null;
            }
            //外键引用的值可能是以逗号分隔的值，所以需要拆分出来后，每个值进行取值
            var filedValue = GetValue(propertyName, entity, dbValues, valueType);
            if (filedValue == null)
            {
                return null;
            }
            var filedValueStr = filedValue.ToString();
            if (string.IsNullOrWhiteSpace(filedValueStr))
            {
                return null;
            }
            var name = new StringBuilder();
            var split = "";
            var values = filedValueStr.Split(',');
            foreach (var v in values)
            {
                if (!string.IsNullOrWhiteSpace(v))
                {
                    var paras = new List<object>();
                    paras.Add(v);
                    if (!string.IsNullOrWhiteSpace(refrenceName.OtherParaFieldNames))
                    {
                        var otherNames = refrenceName.OtherParaFieldNames.Split(',');
                        foreach (var pname in otherNames)
                        {
                            if (!string.IsNullOrWhiteSpace(pname))
                            {
                                paras.Add(GetValue(pname, entity, dbValues, valueType));
                            }
                        }
                    }
                    name.Append(split).Append(Database.SqlQuery<string>(refrenceName.Sql, paras.ToArray()).FirstOrDefault());
                    split = ",";
                }
            }
            return name.ToString();
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="content"></param>
        /// <param name="entity"></param>
        /// <param name="opType"></param>
        private void AddLog(string content, DbEntityEntry entity, OpLogType opType, string keys = "")
        {
            var maskTxt = "0000000000";
            var maskNum = (int)_productType;

            if (maskNum > 0 && maskNum <= 10)
            {
                maskNum--;
                maskTxt = maskTxt.Remove(maskNum, 1);
                maskTxt = maskTxt.Insert(maskNum, "1");
            }
            var log = new OpLog
            {
                Hid = _hid,
                CDate = DateTime.Now,
                CUser = _userName,
                Ip = UrlHelperExtension.GetRemoteClientIPAddress(_request),
                XType = opType.ToString(),
                CText = content,
                Keys = string.IsNullOrWhiteSpace(keys) ? PrimaryKeyValue(entity) : keys,
                Mask = maskTxt

            };
            OpLogs.Add(log);
        }

        private void AddUpdateLog(DbEntityEntry entity, string entityName, OpLogType opType, bool addPrefixToEntityName)
        {
            var dbValues = entity.GetDatabaseValues();
            var contentBuilder = new StringBuilder();
            foreach (string prop in entity.OriginalValues.PropertyNames)
            {
                if (IsLogIgnored(prop, entity))
                {
                    continue;
                }
                //单体酒店不显示酒店代码
                if (PropertyCName(prop, entity) == "酒店代码")
                {
                    PmsHotel hotel = PmsHotels.Where(w => w.Hid == _hid).FirstOrDefault();
                    if (hotel.Grpid.Trim() == null || hotel.Grpid.Trim() == "")
                    {
                        continue;
                    }
                }
                if (IsLogAnywayWhenEdit(prop, entity))
                {
                    //如果有属性无论如何都记录，则表示此字段肯定是不会进行修改的，所以只需要取当前值记录下来即可
                    contentBuilder.Insert(0, string.Format(" {0}：{1}", PropertyCName(prop, entity), PropertyValue(prop, entity, dbValues, LogValueType.CurrentValue)));
                }
                object currentValue = entity.CurrentValues[prop];
                object originalValue = dbValues[prop];//OriginalValues[prop];
                if (!Object.Equals(currentValue, originalValue) && entity.Property(prop).IsModified == true)
                {
                    contentBuilder.Append(string.Format(" {0}：{1}=>{2}", PropertyCName(prop, entity), PropertyValue(prop, entity, dbValues, LogValueType.OriginValue), PropertyValue(prop, entity, dbValues, LogValueType.CurrentValue)));
                }
            }
            if (contentBuilder.Length > 0)
            {
                contentBuilder.Insert(0, string.Format((addPrefixToEntityName ? "修改{0} " : "{0} "), entityName));
                string keys = LogKeyValue(entity, dbValues, LogValueType.CurrentValue);
                AddLog(contentBuilder.ToString(), entity, opType, keys);
            }
        }

        private void AddAddLog(DbEntityEntry entity, string entityName, OpLogType opType, bool addPrefixToEntityName)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat((addPrefixToEntityName ? "增加{0} " : "{0} "), entityName);
            foreach (string prop in entity.CurrentValues.PropertyNames)
            {
                if (IsLogIgnored(prop, entity))
                {
                    continue;
                }
                //单体酒店不显示酒店代码
                if (PropertyCName(prop, entity) == "酒店代码")
                {
                    PmsHotel hotel = PmsHotels.Where(w => w.Hid == _hid).FirstOrDefault();
                    if (hotel.Grpid.Trim() == null || hotel.Grpid.Trim() == "")
                    {
                        continue;
                    }
                }
                var value = PropertyValue(prop, entity, null, LogValueType.CurrentValue);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    contentBuilder.AppendFormat(" {0}：{1}", PropertyCName(prop, entity), value);
                }
            }
            string keys = LogKeyValue(entity, null, LogValueType.CurrentValue);
            AddLog(contentBuilder.ToString(), entity, opType, keys);
        }

        private void AddDeleteLog(DbEntityEntry entity, string entityName, OpLogType opType, bool addPrefixToEntityName)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat((addPrefixToEntityName ? "删除{0} " : "{0} "), entityName);
            var dbValues = entity.GetDatabaseValues();
            //entityName = ObjectContext.GetObjectType(entity.Entity.GetType()).Name;
            foreach (string prop in entity.OriginalValues.PropertyNames)
            {
                if (IsLogIgnored(prop, entity))
                {
                    continue;
                }
                //单体酒店不显示酒店代码
                if (PropertyCName(prop, entity) == "酒店代码")
                {
                    PmsHotel hotel = PmsHotels.Where(w => w.Hid == _hid).FirstOrDefault();
                    if (hotel.Grpid.Trim() == null || hotel.Grpid.Trim() == "")
                    {
                        continue;
                    }
                }
                var value = PropertyValue(prop, entity, dbValues, LogValueType.OriginValue);
                if (value != null)
                {
                    contentBuilder.AppendFormat(" {0}：{1}", PropertyCName(prop, entity), value);
                }
            }
            string keys = LogKeyValue(entity, dbValues, LogValueType.OriginValue);
            AddLog(contentBuilder.ToString(), entity, opType, keys);
        }

        #endregion 记录数据增删改日志
    }
}