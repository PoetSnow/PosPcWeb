using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Controllers {
    [NotAuth]
    public class GridColumnsManageController : Controller {        
        public ActionResult Index(GridColumnsManageViewModel model) {
            return View(model);
        }
        [HttpPost]
        public ActionResult Save(GridColumnsManageViewModel model, FormCollection formCollection) {
            var db = DependencyResolver.Current.GetService<DbHotelPmsContext>();
            var currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
            //删除表中的原来记录
            db.Database.ExecuteSqlCommand("delete GridColumnsSettings where hid={0} and area={1} and controller={2} and action={3}",currentInfo.HotelId, model.setArea,
                model.setController, model.setAction);

            if (string.Equals(formCollection["actionKey"], "0")) {
                var columns = model.ColumnSettings;
                foreach (var column in columns) {
                    string hiddenValue = formCollection[string.Format("hidden{0}", column.Name)];
                    string orderValue = formCollection[string.Format("order{0}", column.Name)];
                    string widthValue = formCollection[string.Format("width{0}", column.Name)];
                    column.Hidden = hiddenValue == "0";
                    int temp;
                    if (Int32.TryParse(orderValue, out temp)) {
                        column.Order = temp;
                    }
                    if (Int32.TryParse(widthValue, out temp)) {
                        column.Width = temp;
                    }
                }
                columns = columns.OrderBy(c => c.Order).ToList();
                var series = new JavaScriptSerializer();
                model.columns = series.Serialize(columns);

                //插入修改后的记录
                var setting = new Gemstar.BSPMS.Hotel.Services.Entities.GridColumnsSettings {
                    Action = model.setAction
                    , Area = model.setArea
                    , Controller = model.setController
                    , Hid = currentInfo.HotelId
                    , ColumnSettings = model.columns
                };
                db.GridColumnsSettings.Add(setting);
                db.SaveChanges();
            }

            return RedirectToAction(model.setAction, model.setController, new { area = model.setArea });
        }

    }
}
