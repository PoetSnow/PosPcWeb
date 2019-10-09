using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Common.Services.EF
{
    /// <summary>
    /// 产品服务实例
    /// </summary>
    public class ProductService : IProductService
    {
        private DbCommonContext _db;

        public ProductService(DbCommonContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取所有产品列表
        /// </summary>
        /// <returns>所有产品列表</returns>
        public List<M_v_products> GetAllProducts()
        {
            return _db.M_v_products.OrderBy(w => w.SeqId).ToList();
        }

        /// <summary>
        /// 获取指定酒店的产品列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的产品列表</returns>
        public List<string> GetHotelProducts(string hid)
        {
            return _db.HotelProducts.Where(w => w.Hid == hid).Select(w => w.ProductCode).ToList();
        }

        /// <summary>
        /// 获取已开通指定产品的集团分店列表
        /// </summary>
        /// <param name="grpid">集团ID</param>
        /// <param name="productCode">产品代码</param>
        /// <returns></returns>
        public List<string> GetHotels(string grpid, string productCode)
        {
            return _db.Database.SqlQuery<string>("exec up_list_GroupHotelByProduct @grpid=@grpid,@productCode=@productCode", new SqlParameter("@grpid", grpid), new SqlParameter("@productCode", productCode)).ToList();
        }

        public M_v_products GetProductByCode(string code)
        {
            var product = _db.M_v_products.SingleOrDefault(w => w.Code == code);
            return product ?? GetDefault();
        }

        public M_v_products GetProductByDomain(string domain) {
            var product = _db.M_v_products.SingleOrDefault(w => w.Domain == domain || w.Domain2 == domain);
            return product??GetDefault();
        }

        private M_v_products GetDefault()
        {
            return _db.M_v_products.Single(w => w.Code == "pos");
        }

        /// <summary>
        /// 设置指定酒店的产品模块
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="productCodes">产品模块代码列表</param>
        /// <returns>设置结果</returns>
        public JsonResultData SetHotelProducts(string hid, List<string> productCodes)
        {
            try
            {
                var oldProducts = _db.HotelProducts.Where(w => w.Hid == hid).ToList();
                _db.HotelProducts.RemoveRange(oldProducts);
                foreach (var code in productCodes)
                {
                    //check if the code is exist
                    var exist = oldProducts.SingleOrDefault(w => w.ProductCode == code);
                    if (exist != null)
                    {
                        _db.Entry(exist).State = System.Data.Entity.EntityState.Unchanged;
                    }
                    else
                    {
                        //not exist,need add one
                        var hotelProduct = new HotelProducts
                        {
                            Hid = hid,
                            ProductCode = code
                        };
                        _db.HotelProducts.Add(hotelProduct);
                    }
                }
                _db.SaveChanges();
                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
    }
}
