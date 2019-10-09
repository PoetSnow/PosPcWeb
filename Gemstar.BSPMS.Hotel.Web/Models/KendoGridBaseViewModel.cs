namespace Gemstar.BSPMS.Hotel.Web.Models
{
    public class KendoGridBaseViewModel
    {
        public KendoGridBaseViewModel()
        {
            GridControlId = "grid";
            EnableFunctionForAdd = true;
            EnableFunctionForDelete = true;
            EnableFunctionForEdit = true;
            EnableFunctionForQuery = true;
            EnableFunctionForDetail = false;
            EnableFunctionForEnable = false;
            EnableFunctionForDisable = false;
            EnableFunctionForPage = true;
            EnableCustomToolbarForFirst = false;
            JsFuncForGetAjaxQueryPara = "getCommonQueryParas";
            EnableFunctionForSave = true;
            EnableScrollable = false;
            onDataBound = null;
            ServerOperation = false;
        }
        /// <summary>
        /// grid控件id，默认为grid
        /// </summary>
        public string GridControlId { get; set; }
        /// <summary>
        /// 主键值对应的列名称，用于从数据行中获取记录的主键值
        /// </summary>
        public string KeyColumnName { get; set; }
        
        /// <summary>
        /// 查看详情可用开关
        /// </summary>
        public bool EnableFunctionForDetail { get; set; }


        /// <summary>
        /// 新增功能可用开关
        /// </summary>
        public bool EnableFunctionForAdd { get; set; }

        /// <summary>
        /// 删除功能可用开关
        /// </summary>
        public bool EnableFunctionForDelete { get; set; }

        /// <summary>
        /// 编辑功能可用开关
        /// </summary>
        public bool EnableFunctionForEdit { get; set; }

        /// <summary>
        /// 查询功能可用开关
        /// </summary>
        public bool EnableFunctionForQuery { get; set; }

        /// <summary>
        /// 启用功能可用开关
        /// </summary>
        public bool EnableFunctionForEnable { get; set; }

        /// <summary>
        /// 禁用功能可用开关
        /// </summary>
        public bool EnableFunctionForDisable { get; set; }

        /// <summary>
        /// 分页开关
        /// </summary>
        public bool EnableFunctionForPage { get; set; }

        /// <summary>
        /// 按钮位置开关 是否在第一位
        /// </summary>
        public bool EnableCustomToolbarForFirst { get; set; }
        /// <summary>
        /// ajax查询时，组装查询用的参数的js方法，默认为getCommonQueryParas
        /// </summary>
        public string JsFuncForGetAjaxQueryPara { get; set; }
        /// <summary>
        /// 保存按钮开关
        /// </summary>
        public bool EnableFunctionForSave { get; set; }

        /// <summary>
        /// 延迟会员有效期
        /// </summary>
        public bool DelayValidDate { get; set; }
         
        /// <summary>
        /// 添加修改业务员
        /// </summary>
        public bool UpdateSales { get; set; }
        
        /// <summary>
        /// 添加修改业务员
        /// </summary>
        public bool UpdateScores { get; set; }
        
        /// <summary>
        /// 是否显示滚动条
        /// </summary>
        public bool EnableScrollable { get; set; }

        /// <summary>
        /// 加载事件，默认null
        /// </summary>
        public string onDataBound { get; set; }
        /// <summary>
        /// 是否服务器进行分页等处理,默认为全部返回到前端，由客户端处理
        /// 遇到数据量较多时，返回太多数据会导致很慢并且长度超长，需要修改为服务端处理
        /// </summary>
        public bool ServerOperation { get; set; }
    }
}
