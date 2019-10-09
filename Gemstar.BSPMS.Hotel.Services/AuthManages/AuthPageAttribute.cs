using System;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 用于匹配controller中的action方法与数据库中对应的权限项的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AuthPageAttribute:Attribute
    {
        /// <summary>
        /// 以指定的权限项id初始化权限项，产品类型默认为pms
        /// </summary>
        /// <param name="authCode">权限项id</param>
        public AuthPageAttribute(string authCode):this(ProductType.Pms,authCode)
        {
        }
        /// <summary>
        /// 以指定的产品类型，权限项id初始化权限项
        /// </summary>
        /// <param name="productType">产品类型</param>
        /// <param name="authCode">权限项id</param>
        public AuthPageAttribute(ProductType productType,string authCode) {
            ProductTypeInstance = productType;
            AuthCode = authCode;
        }
        /// <summary>
        /// 对应的权限项id
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public ProductType ProductTypeInstance { get; set; }
    }
}
