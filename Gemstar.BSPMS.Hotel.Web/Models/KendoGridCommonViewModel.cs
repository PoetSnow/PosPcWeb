namespace Gemstar.BSPMS.Hotel.Web.Models
{
    public class KendoGridCommonViewModel: KendoGridBaseViewModel {
        public KendoGridCommonViewModel()
        {
            RowTemplate = "";
            HtmlAttributes = null;
        }
        /// <summary>
        /// 行模板
        /// </summary>
        public string RowTemplate { get; set; }
        
        /// <summary>
        /// 表格属性
        /// </summary>
        public object HtmlAttributes { get; set; }
    }
}
