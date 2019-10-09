using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Common.Services
{
    public interface IHelpFilesService: ICRUDService<HelpFiles>
    {
        /// <summary>
        /// 增加指定问题的阅读次数
        /// </summary>
        /// <param name="helpId">问题id</param>
        void AddReadQty(int helpId);
        /// <summary>
        /// 获取帮助文档
        /// </summary>
        /// <param name="menuId">菜单模块id</param>
        /// <param name="checkStatu">审核状态</param>
        /// <returns></returns>
        List<HelpFiles> GetHelpFiles(string menuId);

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="helpid">帮助id</param>
        /// <param name="name">图片名称</param>
        /// <returns></returns>
        List<HelpFilesImg> GetHelpFilesImg(int? helpid,string name);
        JsonResultData SaveHelpFiles(HelpFiles help);
        JsonResultData SaveHelpFilesAdmin(HelpFiles help);
        /// <summary>
        /// 增加上传到七牛后的帮助图片记录
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="url">图片在七牛的访问地址</param>
        /// <param name="helpId">当前帮助记录id</param>
        /// <returns>增加是否成功的结果</returns>
        JsonResultData AddHelpFileImage(string name, string url, int? helpId);
        /// <summary>
        /// 删除图片(数据库)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonResultData DeleteFileImage(int id);
    }
}
