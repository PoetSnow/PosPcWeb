using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Models {
    public class GridColumnsManageViewModel {
        public string setArea { get; set; }
        public string setController { get; set; }
        public string setAction { get; set; }
        public string columns { get; set; }

        public List<GridColumnSetting> ColumnSettings {
            get {
                var series = new JavaScriptSerializer();
                return series.Deserialize<List<GridColumnSetting>>(columns);
            }
        }
    }
}