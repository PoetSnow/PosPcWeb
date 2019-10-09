using Autofac;
using Autofac.Integration.Mvc;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.BreakfastManage;
using Gemstar.BSPMS.Hotel.Services.CRMManage;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.BreakfastManage;
using Gemstar.BSPMS.Hotel.Services.EF.CRMManage;
using Gemstar.BSPMS.Hotel.Services.EF.MarketingManage;
using Gemstar.BSPMS.Hotel.Services.EF.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Services.EF.NotifyManage;
using Gemstar.BSPMS.Hotel.Services.EF.OnlineInterfaceManage;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;
using Gemstar.BSPMS.Hotel.Services.EF.Percentages;
using Gemstar.BSPMS.Hotel.Services.EF.PosManage;
using Gemstar.BSPMS.Hotel.Services.EF.ReportManage;
using Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage;
using Gemstar.BSPMS.Hotel.Services.EF.ResInvoiceManage;
using Gemstar.BSPMS.Hotel.Services.EF.ResManage;
using Gemstar.BSPMS.Hotel.Services.EF.RoomStatusManage;
using Gemstar.BSPMS.Hotel.Services.EF.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.EF.SystemManage;
using Gemstar.BSPMS.Hotel.Services.EF.WeixinManage;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.Percentages;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Services.ResFolioManage;
using Gemstar.BSPMS.Hotel.Services.ResInvoiceManage;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Services.WeixinManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage;
using Gemstar.BSPMS.Hotel.Web.Models;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.App_Start
{
    /// <summary>
    /// autofac容器注册
    /// </summary>
    public class AutofacConfig
    {
        public static void Config(MvcApplication mvcApplication)
        {
            /*
             说明：创建服务接口和接口实现后，都需要在此处进行注册，然后在需要使用的地方调用baseController中的GetService<Interface>来获取对应接口的实现实例
             注册时为了防止每次合并都出现冲突，请按模块进行注册，下方使用注释的形式列出了相关模块区域
             */
            var builder = new ContainerBuilder();
            builder.Register(c => { return DependencyResolver.Current; }).As<IDependencyResolver>();
            //通用的mvc组件注册
            //builder.RegisterType<CurrentInfoSession>().As<ICurrentInfo>().InstancePerRequest();
            builder.Register(c =>
            {
                var currentInfo = new CurrentInfoSession();
                currentInfo.LoadValues();
                return currentInfo;
            }).As<ICurrentInfo>().InstancePerRequest();
            builder.RegisterType<MvcApplication>().As<ISettingProvider>();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModule<AutofacWebTypesModule>();
            //EF数据实体注册
            builder.Register(c =>
            {
                var currentInfo = c.Resolve<ICurrentInfo>();
                var request = c.Resolve<HttpRequestBase>();
                return new DbHotelPmsContext(MvcApplication.GetHotelDbConnStr(), currentInfo.HotelId, currentInfo.UserName, request);
            }).InstancePerRequest().AsSelf().As<DbContext>();
            builder.Register(c => new DbCommonContext(MvcApplication.GetCenterDBConnStr())).InstancePerRequest();

            //系统管理(资料设置)相关服务注册
            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<BasicDataResortControlService>().As<IBasicDataResortControlService>();
            builder.RegisterType<HotelStatusService>().As<IHotelStatusService>();
            builder.RegisterType<SysLogService>().As<ISysLogService>();
            builder.RegisterType<AuthCheckViaMemoryCache>().As<IAuthCheck>();
            builder.RegisterType<CommonQueryService>().As<ICommonQueryService>();
            builder.RegisterType<HotelInfoService>().As<IHotelInfoService>();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<PmsHotelService>().As<IPmsHotelService>();
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<PmsUserService>().As<IPmsUserService>();
            builder.RegisterType<UserRoleSingleService>().As<IUserRoleSingleService>();
            builder.RegisterType<UserRoleGroupService>().As<IUserRoleGroupService>();
            builder.RegisterType<AuthListService>().As<IAuthListService>();
            builder.RegisterType<ShiftService>().As<IShiftService>();
            builder.RegisterType<RoomTypeService>().As<IRoomTypeService>();
            builder.RegisterType<RoomService>().As<IRoomService>();
            builder.RegisterType<RoomStatusService>().As<IRoomStatusService>();
            builder.RegisterType<CodeListService>().As<ICodeListService>();
            builder.RegisterType<MbrCardTypeService>().As<IMbrCardTypeService>();
            builder.RegisterType<MbrCardService>().As<IMbrCardService>();
            builder.RegisterType<MasterService>().As<IMasterService>();
            builder.RegisterType<SysParaService>().As<ISysParaService>();
            builder.RegisterType<SysCheckCodeService>().As<ISysCheckCodeService>();
            builder.RegisterType<ItemService>().As<IItemService>();
            builder.RegisterInstance<IBeforeLoginService>(new BeforeLoginService(MvcApplication.GetCenterDBConnStr(), mvcApplication.SettingInfo.ApplicationName));
            builder.RegisterType<PmsParaService>().As<IPmsParaService>();
            builder.RegisterInstance<ISmsLogService>(new SmsLogService(DependencyResolver.Current.GetService<ICurrentInfo>() == null ? "" : DependencyResolver.Current.GetService<ICurrentInfo>().HotelId, MvcApplication.GetCenterDBConnStr(), DependencyResolver.Current.GetService<ICurrentInfo>() == null ? "" : DependencyResolver.Current.GetService<ICurrentInfo>().UserCode));
            builder.RegisterType<ItemScoreService>().As<IItemScoreService>();
            builder.RegisterType<ScoreUseRuleService>().As<IScoreUseRuleService>();
            builder.RegisterType<ChargeFreeService>().As<IChargeFreeService>();
            builder.RegisterType<SalesService>().As<ISalesService>();
            builder.RegisterType<HotelDayService>().As<IHotelDayService>();
            builder.RegisterType<HotelDayDetailService>().As<IHotelDayDetailService>();
            builder.RegisterType<RateService>().As<IRateService>();
            builder.RegisterType<RateDetailService>().As<IRateDetailService>();
            builder.RegisterType<ImportExcelService>().As<IImportExcelService>();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>();

            builder.RegisterType<ItemService>().As<IItemService>();
            builder.RegisterType<GuestService>().As<IGuestService>();
            builder.RegisterType<GuestTransService>().As<IGuestTransService>();
            builder.RegisterType<CompanyService>().As<ICompanyService>();
            builder.RegisterType<CompanyLogService>().As<ICompanyLogService>();
            builder.RegisterType<CompanyCaService>().As<ICompanyCaService>();
            builder.RegisterType<CompanyTransService>().As<ICompanyTransService>();
            builder.RegisterType<BookingNotesService>().As<IBookingNotesService>();
            builder.RegisterType<OperationLogService>().As<IoperationLog>();
            builder.RegisterType<TryInfoService>().As<ITryInfoService>();
            builder.RegisterType<ServiceAuthorizeService>().As<IServiceAuthorizeService>();
            builder.RegisterType<CouponService>().As<ICouponService>();
            //房态管理相关服务设置
            builder.RegisterType<RoomStatusService>().As<IRoomStatusService>();
            builder.RegisterType<WakeCallService>().As<IWakeCallService>();
            //保留房设置
            builder.RegisterType<RoomHoldService>().As<IRoomHoldService>();
            //渠道信息
            builder.RegisterType<ChannelService>().As<IChannelService>();
            //预订管理相关服务设置
            builder.RegisterType<ResService>().As<IResService>();
            builder.RegisterType<Services.EF.PermanentRoomManage.ResService>().As<Services.PermanentRoomManage.IResService>();
            builder.RegisterType<Services.EF.PermanentRoomManage.PermanentRoomGoodsService>().As<Services.PermanentRoomManage.IPermanentRoomGoodsService>();
            builder.RegisterType<Services.EF.PermanentRoomManage.PermanentRoomPricePlanService>().As<Services.PermanentRoomManage.IPermanentRoomPricePlanService>();
            //报表角色权限设置
            builder.RegisterType<RoleAuthReportService>().As<IRoleAuthReportService>();
            //消费录入权限设置
            builder.RegisterType<RoleAuthItemConsumeService>().As<IRoleAuthItemConsumeService>();
            //接待管理相关服务设置，预订客账服务设置
            builder.RegisterType<ResFolioService>().As<IResFolioService>();
            builder.RegisterType<ResInvoiceService>().As<IResInvoiceService>();
            builder.RegisterType<HelpFilesServices>().As<IHelpFilesService>();
            builder.RegisterType<NoticeService>().As<INoticeService>();
            builder.RegisterType<BreakfastService>().As<IBreakfastService>();
            builder.RegisterType<ResBillSettingService>().As<IResBillSettingService>();
            builder.RegisterType<PermanentRoomService>().As<IPermanentRoomService>();
            //客户关系管理相关服务设置
            builder.RegisterType<CompanySignerService>().As<ICompanySignerService>();
            builder.RegisterType<PlanTaskService>().As<IPlanTaskService>();
            //会员相关服务设置

            //报表相关服务设置
            builder.RegisterType<ReportService>().As<IReportService>();

            //集团管理相关服务设置

            //支付相关服务设置
            builder.RegisterType<PayServiceBuilder>().As<IPayServiceBuilder>();
            builder.RegisterType<PayLogRecordInDbService>().As<IPayLogService>();
            builder.RegisterType<WaitPayListService>().As<IWaitPayListService>();
            builder.RegisterType<ResFolioPayInfoService>().As<IResFolioPayInfoService>();

            //通知相关服务设置
            builder.RegisterType<NotifyService>().As<INotifyService>();
            //短信相关服务设置
            builder.RegisterType<SMSSendService>().As<ISMSSendService>();
            //微信相关服务设置
            builder.RegisterType<QrCodeService>().As<IQrCodeService>();
            builder.RegisterType<QrcodeLoginService>().As<IQrcodeLoginService>();
            //业主分成参数定义
            builder.RegisterType<RoomOwnerCalcParaDefineService>().As<IRoomOwnerCalcParaDefineService>();
            //分成类型定义
            builder.RegisterType<RoomOwnerCalcTypeService>().As<IRoomOwnerCalcTypeService>();
            //分成展示项目定义
            builder.RegisterType<RoomOwnerCalcDispParaService>().As<IRoomOwnerCalcDispParaService>();
            //业主房间委托表
            builder.RegisterType<RoomOwnerRoomInfosService>().As<IRoomOwnerRoomInfosService>();
            //业主费用记录表
            builder.RegisterType<RoomOwnerFeeService>().As<IRoomOwnerFeeService>();
            //业主报表数据表
            builder.RegisterType<RoomOwnerCalcResultService>().As<IRoomOwnerCalcResultService>();
            builder.RegisterType<percentagesPlanService>().As<IpercentagesPlanService>();
            //业务员提成
            builder.RegisterType<Gemstar.BSPMS.Hotel.Services.EF.Percentages.SalesmanPlanService>().As<Gemstar.BSPMS.Hotel.Services.Percentages.ISalesmanPlanService>();
            builder.RegisterType<Gemstar.BSPMS.Hotel.Services.EF.Percentages.SalesmanPolicyService>().As<Gemstar.BSPMS.Hotel.Services.Percentages.ISalesmanPolicyService>();
            //操作员提成
            builder.RegisterType<Gemstar.BSPMS.Hotel.Services.EF.Percentages.OperatorPlanService>().As<Gemstar.BSPMS.Hotel.Services.Percentages.IOperatorPlanService>();
            builder.RegisterType<Gemstar.BSPMS.Hotel.Services.EF.Percentages.OperatorPolicyService>().As<Gemstar.BSPMS.Hotel.Services.Percentages.IOperatorPolicyService>();
            //打扫房间提成
            builder.RegisterType<Gemstar.BSPMS.Hotel.Services.EF.Percentages.CleanRoomPolicyService>().As<Gemstar.BSPMS.Hotel.Services.Percentages.ICleanRoomPolicyService>();

            //Pos
            builder.RegisterType<PosPosService>().As<IPosPosService>();
            builder.RegisterType<PosShiftService>().As<IPosShiftService>();
            builder.RegisterType<PosRefeService>().As<IPosRefeService>();
            builder.RegisterType<PosTabtypeService>().As<IPosTabtypeService>();
            builder.RegisterType<Gemstar.BSPMS.Hotel.Services.EF.PosManage.PosTabService>().As<IPosTabService>();
            builder.RegisterType<PosActionTypeService>().As<IPosActionTypeService>();
            builder.RegisterType<PosActionService>().As<IPosActionService>();
            builder.RegisterType<PosUnitService>().As<IPosUnitService>();
            builder.RegisterType<PosShuffleService>().As<IPosShuffleService>();
            builder.RegisterType<PosRequestService>().As<IPosRequestService>();
            builder.RegisterType<PosAcTypeService>().As<IPosAcTypeService>();
            builder.RegisterType<PosDeptClassService>().As<IPosDeptClassService>();
            builder.RegisterType<PosDepartService>().As<IPosDepartService>();
            builder.RegisterType<PosRequestService>().As<IPosRequestService>();
            builder.RegisterType<PosShuffleService>().As<IPosShuffleService>();
            builder.RegisterType<PosItemClassService>().As<IPosItemClassService>();
            builder.RegisterType<PosItemService>().As<IPosItemService>();
            builder.RegisterType<PosItemPriceService>().As<IPosItemPriceService>();
            builder.RegisterType<PosItemMultiClassService>().As<IPosItemMultiClassService>();
            builder.RegisterType<PosItemRefeService>().As<IPosItemRefeService>();
            builder.RegisterType<PosTabServiceService>().As<IPosTabServiceService>();
            builder.RegisterType<PosTabOpenItemService>().As<IPosTabOpenItemService>();
            builder.RegisterType<PosProdPrinterService>().As<IPosProdPrinterService>();
            builder.RegisterType<PosTabStatusService>().As<IPosTabStatusService>();
            builder.RegisterType<PosBillService>().As<IPosBillService>();
            builder.RegisterType<PosBillDetailService>().As<IPosBillDetailService>();
            builder.RegisterType<PosTabLogService>().As<IPosTabLogService>();
            builder.RegisterType<PosCustomerTypeService>().As<IPosCustomerTypeService>();
            builder.RegisterType<PosReasonService>().As<IPosReasonService>();
            builder.RegisterType<PosDiscTypeService>().As<IPosDiscTypeService>();
            builder.RegisterType<PosBillDetailActionService>().As<IPosBillDetailActionService>();
            builder.RegisterType<PosAdvanceFuncService>().As<IPosAdvanceFuncService>();
            builder.RegisterType<PosSellOutService>().As<IPosSellOutService>();
            builder.RegisterType<PosItemSuitService>().As<IPosItemSuitService>();
            builder.RegisterType<PosBillChangeService>().As<IPosBillChangeService>();
            builder.RegisterType<PosItemActionService>().As<IPosItemActionService>();
            builder.RegisterType<PosActionMultisubService>().As<IPosActionMultisubService>();
            builder.RegisterType<PosItemClassDiscountService>().As<IPosItemClassDiscountService>();

            //注册买单相关的服务，由于买单可能会用到其他业务服务实例，所以放在后面来进行注册
            builder.RegisterType<PaymentServicesPos>().As<IPaymentServices>();
            builder.RegisterType<PosBillDetailService>().As<IPayActionXmlOperate>();
            builder.RegisterType<PayActionXmlBuilder>().AsSelf();
            builder.RegisterType<PayActionXmlBase>().AsSelf();
            builder.RegisterType<PayActionXmlHouse>().AsSelf();
            builder.RegisterType<PayActionXmlMbrcard>().AsSelf();
            builder.RegisterType<PayActionXmlMbrLargess>().AsSelf();
            builder.RegisterType<PayActionXmlMbrCardAndLargess>().AsSelf();
            builder.RegisterType<PayActionXmlCorp>().AsSelf();
            builder.RegisterType<PayActionXmlOnlineBarCode>().AsSelf();
            builder.RegisterType<PayActionXmlPrePay>().AsSelf();

            builder.RegisterType<PosCommon>().AsSelf();
            builder.RegisterType<PosHolidayService>().As<IPosHolidayService>();
            builder.RegisterType<PosOnSaleService>().As<IPosOnSaleService>();
            //库存物品
            builder.RegisterType<PosCostItemService>().As<IPosCostItemService>();
            //扫码点餐
            builder.RegisterType<PosMBannerService>().As<IPosMBannerService>();
            builder.RegisterType<PosMScrollService>().As<IPosMScrollService>();

            builder.RegisterType<PosPrintBillService>().As<IPosPrintBillService>();
            //海鲜池
            builder.RegisterType<PosSeaFoodPoolService>().As<IPosSeaFoodPoolService>();

            //消费项目仓库耗用表
            builder.RegisterType<PosBillCostService>().As<IPosBillCostService>();
            //二级仓库列表
            builder.RegisterType<PosDepotService>().As<IPosDepotService>();
            //公用市别
            builder.RegisterType<PosShuffleNewsService>().As<IPosShuffleNewsService>();

            //操作折扣设置
            builder.RegisterType<PosOperDiscountService>().As<IPosOperDiscountService>();

            //发票接口
            builder.RegisterType<InvoiceService>().As<IInvoiceService>();

            //凭证设置
            builder.RegisterType<PosVoucherSetService>().As<IPosVoucherSetService>();
            builder.RegisterType<PosWhVoucherService>().As<IPosWhVoucherService>();
            builder.RegisterType<PosWhVoucherdetailService>().As<IPosWhVoucherdetailService>();


            //预定
            builder.RegisterType<PosReserveService>().As<IPosReserveService>();

            //预定详细信息
            builder.RegisterType<PosBillOrderService>().As<IPosBillOrderService>();

            //定金
            builder.RegisterType<YtPrepayService>().As<IYtPrepayService>();
            //注册到mvc中
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }
    }
}