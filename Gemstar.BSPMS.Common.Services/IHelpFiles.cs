using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Common.Services
{
    public interface IHelpFiles: ICRUDService<HelpFiles>
    {
        /// <summary>
        /// 获取所有文档
        /// </summary>
        /// <returns></returns>
        List<HelpFiles> GetHelpFiles();

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="helpid"></param>
        /// <returns></returns>
        List<HelpFilesImg> GetHelpFilesImg(int? helpid);
        /// <summary>
        /// hotel保存
        /// </summary>
        /// <param name="help"></param>
        /// <returns></returns>
        JsonResultData SaveHelpFiles(HelpFiles help);
        /// <summary>
        /// admin保存
        /// </summary>
        /// <param name="help"></param>
        /// <returns></returns>
        JsonResultData SaveHelpFilesAdmin(HelpFiles help);
    }
}
