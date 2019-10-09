using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services
{
   public interface IGuestTransService : ICRUDService<GuestTrans>
    {
        /// <summary>
        /// 获取指定酒店下的记账项目列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<GuestTrans> GetGuestTrans(string id );
    }
}
