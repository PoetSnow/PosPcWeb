using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Common.Services
{
    public interface INoticeService : ICRUDService<Notice>
    {
        Notice GetNoticeFirst(DateTime time, string versionId = null);
        void UpdateNotice(Notice data);

       
    }
}
