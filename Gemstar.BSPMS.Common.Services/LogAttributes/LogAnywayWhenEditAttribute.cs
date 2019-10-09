using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 指定某个属性无论如何都要记录日志
    /// 主要用于系统参数修改时，同时记录参数名称
    /// 因为系统参数修改时，只能修改值，名称没有改变，所以默认不会记录
    /// </summary>
    public class LogAnywayWhenEditAttribute : Attribute
    {
    }
}
