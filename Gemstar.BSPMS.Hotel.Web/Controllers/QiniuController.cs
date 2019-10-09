using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    [NotAuth]
    public class QiniuController : BaseController
    {
        // GET: Qiniu
        [AuthButton(AuthFlag.Update)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取上传凭证
        /// IsView:是否是前端上传
        /// </summary>
        /// <returns></returns> 
        [AuthButton(AuthFlag.Update)]
        public string GetUptoken(bool IsHelpFiles = true, bool IsView = true)
        {
            var _sysParaService = GetService<ISysParaService>();
            var qiniuPara = _sysParaService.GetQiniuPara();
            string bucket = IsHelpFiles ? "jxd-pmshelpfiles" : (qiniuPara.ContainsKey("bucket") ? qiniuPara["bucket"] : "jxd-res");
            string access_key = qiniuPara.ContainsKey("access_key") ? qiniuPara["access_key"] : "7TVp7dAC9xHLtd8VHPnHjAJOy9YLh7rhwbzZe7s2";
            string secret_key = qiniuPara.ContainsKey("secret_key") ? qiniuPara["secret_key"] : "Ic96Wia-MQ4T2ma1wQfeqG_zlj1aRMhnZTeIsMGg";
            return QiniuHelper.GetUpToken(bucket, access_key, secret_key, IsView);
        }

        /// <summary>
        /// 删除单个文件
        /// </summary>
        /// <param name="bucket">文件所在的空间名</param>
        /// <param name="path">七牛文件完整路径</param>
        /// IsHelpFiles :true:jxd-pmshelpfiles   false:jxd-res
        [AuthButton(AuthFlag.Update)]
        public bool QiniuDelete(string path, bool IsHelpFiles = true)
        {
            var _sysParaService = GetService<ISysParaService>();
            var qiniuPara = _sysParaService.GetQiniuPara();
            string bucket = IsHelpFiles ? "jxd-pmshelpfiles" : (qiniuPara.ContainsKey("bucket") ? qiniuPara["bucket"] : "jxd-res");
            string access_key = qiniuPara.ContainsKey("access_key") ? qiniuPara["access_key"] : "7TVp7dAC9xHLtd8VHPnHjAJOy9YLh7rhwbzZe7s2";
            string secret_key = qiniuPara.ContainsKey("secret_key") ? qiniuPara["secret_key"] : "Ic96Wia-MQ4T2ma1wQfeqG_zlj1aRMhnZTeIsMGg";
            var filename = path.Split('/').Last();
            var IsSuccess = QiniuHelper.ImgDelete(bucket, filename, access_key, secret_key);
            //if (IsSuccess)
            //    AddOperationLog(OpLogType.删除七牛图片, string.Format("图片地址：{0}，存储资源：{1}", path, bucket));
            return IsSuccess;
        }
        [AuthButton(AuthFlag.None)]
        public void UploadFile(Stream stream, string key)
        {
            var token = GetUptoken(true, false);
            QiniuHelper.UploadFile(stream, key, token);
        }
    }
}