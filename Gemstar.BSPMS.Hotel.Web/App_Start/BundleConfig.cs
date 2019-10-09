using System.Reflection;
using System.Web;
using System.Web.Optimization;

namespace Gemstar.BSPMS.Hotel.Web
{
    public class BundleConfig
    {
        /// <summary>
        /// 通用的css文件
        /// </summary>
        public static string BundleHeaderCss {get { return string.Format("~/Content/headerCss{0}", GetVersion()); } }
        public static string BundleHeaderJs { get { return string.Format("~/Content/headerJs{0}", GetVersion()); } }
        /// <summary>
        /// 客情客账js脚本
        /// </summary>
        public static string BundleResScripts { get { return string.Format("~/Content/res{0}", GetVersion()); } }
        /// <summary>
        /// 长包房客情客账js脚本
        /// </summary>
        public static string BundlePermanentRoomResScripts { get { return string.Format("~/Content/permanentRoomRes{0}", GetVersion()); } }
        /// <summary>
        /// 批量预订批量入住js脚本
        /// </summary>
        public static string BundleResBatchScripts { get { return string.Format("~/Content/resBatch{0}", GetVersion()); } }
        public static string BundlePayScripts { get { return string.Format("~/Content/payScripts{0}", GetVersion()); } }

        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle(BundleHeaderCss).Include(
                "~/Content/jquery.alerts.css",
                "~/Content/kendo/2016.1.226/kendo.common-bootstrap.min.css",
                "~/Content/kendo/2016.1.226/kendo.mobile.all.min.css",
                "~/Content/kendo/2016.1.226/kendo.dataviz.min.css",
                "~/Content/kendo/2016.1.226/kendo.bootstrap.min.css",
                "~/Content/kendo/2016.1.226/kendo.dataviz.bootstrap.min.css",
                "~/Content/pace-theme-center-simple.css",
                "~/Content/Site.css",
                "~/Content/common.css",
                "~/Content/system.css"
                ));
            bundles.Add(new ScriptBundle(BundleHeaderJs).Include(
                "~/Scripts/jquery-1.12.3.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/kendo/2016.1.226/jszip.min.js",
                "~/Scripts/kendo/2016.1.226/kendo.all.min.js",
                "~/Scripts/kendo/2016.1.226/kendo.aspnetmvc.min.js",
                "~/Scripts/kendo.modernizr.custom.js",
                "~/Scripts/kendo/2016.1.226/cultures/kendo.culture.zh-CN.min.js",
                "~/Scripts/spin.min.js",
                "~/Scripts/jquery.alerts.js",
                "~/Scripts/dateExtension.js",
                "~/Scripts/json2.js",
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/commonOcx/AIP_MAIN.js",
                "~/Scripts/notice.js",
                "~/Scripts/keyboardNavigation.js"
                ));
            bundles.Add(new ScriptBundle(BundleResScripts).Include(
                "~/Scripts/res/batch/batchChangeOrderStatus.js",
                "~/Scripts/res/folio/splitFolio.js",
                "~/Scripts/res/customer/useScoreToCheckin.js",
                "~/Scripts/authorization/authorization.js",
                //"~/Scripts/res/customer/invoice.js",
                "~/Scripts/res/customer/ratePlanTemplate.js",
                "~/Scripts/res/customer/idScan.js",

                "~/Scripts/res/customer/setRoom.js",
                "~/Scripts/res/customer/lock.js",
                "~/Scripts/res/customer/changeRoom.js",
                "~/Scripts/res/customer/delay.js",
                "~/Scripts/res/customer/relation.js",

                "~/Scripts/res/customer/resOrderMain.js",
                "~/Scripts/res/customer/resOrderDetail.js",
                "~/Scripts/res/customer/resOrderDetailInfoCustomer.js",
                "~/Scripts/res/customer/resBillSetting.js",
                "~/Scripts/res/customer/permanentRoom.js",

                "~/Scripts/res/customer/orderCustomer.js",

                "~/Scripts/res/folio/resFolioGuesTable.js",
                "~/Scripts/res/folio/resFolioFolioTable.js",

                "~/Scripts/res/folio/printResBill.js",
                "~/Scripts/res/folio/addFolio.js",
                "~/Scripts/res/folio/cancelCheckout.js",
                "~/Scripts/res/folio/cancelClear.js",
                "~/Scripts/res/folio/cardAuth.js",
                "~/Scripts/res/folio/checkout.js",
                "~/Scripts/res/folio/clear.js",
                "~/Scripts/res/folio/clearOut.js",
                "~/Scripts/res/folio/out.js",
                "~/Scripts/res/folio/transfer.js",
                "~/Scripts/res/folio/invoice.js",
                "~/Scripts/res/folio/addItems.js",
                "~/Scripts/res/folio/cancelAndRecoveryFolio.js",
                "~/Scripts/res/folio/adjustFolio.js",
                "~/Scripts/res/folio/refundFolio.js",
                "~/Scripts/res/folio/orderFolio.js"
                ));

            bundles.Add(new ScriptBundle(BundleResBatchScripts).Include(
                "~/Scripts/res/customer/useScoreToCheckin.js",
                "~/Scripts/authorization/authorization.js",
                "~/Scripts/res/batch/batchEvents.js",
                "~/Scripts/res/batch/batchChangeRatePlan.js"
                ));
            bundles.Add(new ScriptBundle(BundlePayScripts).Include(
                "~/Scripts/payScripts/_PayBase.js",
                "~/Scripts/payScripts/PayAliBarcode.js",
                "~/Scripts/payScripts/PayAliQrcode.js",
                "~/Scripts/payScripts/PayBankCard.js",
                "~/Scripts/payScripts/PayCorp.js",
                "~/Scripts/payScripts/PayCredit.js",
                "~/Scripts/payScripts/PayHouse.js", 
                "~/Scripts/payScripts/PayMbrCard.js",
                "~/Scripts/payScripts/PayMbrCardAndLargess.js",
                "~/Scripts/payScripts/PayMbrCashTicket.js",
                "~/Scripts/payScripts/PayMbrLargess.js",
                "~/Scripts/payScripts/PayWxBarcode.js",
                "~/Scripts/payScripts/PayWxQrcode.js",
                "~/Scripts/payScripts/PayAliCredit.js",
                "~/Scripts/payScripts/PayRoomFolio.js",
                "~/Scripts/payScripts/PayPrePay.js"
                ));

            bundles.Add(new ScriptBundle(BundlePermanentRoomResScripts).Include(
                "~/Scripts/permanentRoom/customer/ratePlanTemplate.js",
                //"~/Scripts/permanentRoom/customer/idScan.js",

                "~/Scripts/permanentRoom/customer/setRoom.js",
                "~/Scripts/permanentRoom/customer/lock.js",
                "~/Scripts/permanentRoom/customer/changeRoom.js",
                "~/Scripts/permanentRoom/customer/delay.js",
                "~/Scripts/permanentRoom/customer/relation.js",

                "~/Scripts/permanentRoom/customer/resOrderMain.js",
                "~/Scripts/permanentRoom/customer/resOrderDetail.js",
                "~/Scripts/permanentRoom/customer/resOrderDetailInfoCustomer.js",
                //"~/Scripts/permanentRoom/customer/permanentRoom.js",

                "~/Scripts/permanentRoom/customer/orderCustomer.js",
                "~/Scripts/permanentRoom/folio/inAdvanceCheckout.js",
                "~/Scripts/permanentRoom/folio/waterAndElectricityAddFolio.js"
                ));
        }
        internal static string GetVersion()
        {
            var assembly = Assembly.GetAssembly(typeof(MvcApplication));
            var version = assembly.GetName().Version;
            return string.Format("{0}{1}", version.Revision, version.Build);
        }
    }
}