using Gemstar.BSPMS.Common.Services.Entities;
using System.Data.Entity;

namespace Gemstar.BSPMS.Common.Services.EF
{
    /// <summary>
    /// 系统日志上下文数据库
    /// </summary>
    public class DbCommonContext : DbContext
    {
        static DbCommonContext()
        {
            Database.SetInitializer<DbCommonContext>(null);
        }

        public DbCommonContext(string connStr) : base(connStr)
        {
            //设置一些默认值，以优化一些速度
            Configuration.ValidateOnSaveEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;//在需要修改和删除时，需要手动的打开此开关
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<SysLog> SysLogs { get; set; }

        public DbSet<PayLog> PayLogs { get; set; }

        public DbSet<SysPara> SysParas { get; set; }

        public DbSet<CenterHotel> Hotels { get; set; }

        public DbSet<HelpFiles> HelpFiles { get; set; }

        public DbSet<Notice> Notice { get; set; }

        public DbSet<HelpFilesImg> HelpFilesImg { get; set; }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<CityMaster> Citys { get; set; }

        public DbSet<StarLevel> StarLevels { get; }

        public DbSet<SysCheckCode> SysCheckCodes { get; set; }

        public DbSet<AdSet> AdSets { get; set; }
        public DbSet<HotelChannel> HotelChannels { get; set; }
        public DbSet<TryInfo> TryInfos { get; set; }

        public DbSet<M_v_channelCode> M_v_channelCodes { get; set; }    
        public DbSet<M_v_codeListPub> M_v_codeListPubs { get; set; }
        public DbSet<M_v_channelPara> M_v_channelParas { get; set; }
        public DbSet<M_v_payPara> M_v_payParas { get; set; }
        public DbSet<M_v_products> M_v_products { get; set; }
        public DbSet<HotelProducts> HotelProducts { get; set; }
        public DbSet<HotelPos> HotelPos { get; set; }

        public DbSet<ServicesOperator> ServicesOperators { get; set; }
        public DbSet<ServicesAuthorize> ServicesAuthorizes { get; set; }
        public DbSet<HotelFunctions> HotelFunctionses { get; set; }
        public DbSet<WeixinQrcodes> WeixinQrcodes { get; set; }
        public DbSet<WeixinOwnerHotelMapping> WeixinOwnerHotelMappings { get; set; }
        public DbSet<WeixinOperatorHotelMapping> WeixinOperatorHotelMappings { get; set; }
        public DbSet<WeixinTemplateMessage> WeixinTemplateMessages { get; set; }
        public DbSet<WeixinQrcodeLogin> WeixinQrcodeLogins { get; set; }

        public DbSet<DataBaseList> DbLists { get; set; }

        public DbSet<posSmMappingHid> posSmMappingHids { get; set; }
    }
}
