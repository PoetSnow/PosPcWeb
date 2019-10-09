using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.Common;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    public class BaseEditIncellController<T, I> : BaseController where T : class where I : ICRUDService<T>
    {
        #region 执行ajax数据查询
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult AjaxQuery(CommonQueryModel query, [DataSourceRequest]DataSourceRequest request)
        {
            var queryService = GetService<ICommonQueryService>();
            var procedure = query.QueryProcedureName;
            var queryParameters = new CommonQueryHelper(query.QueryParameterValues);
            queryParameters.SetHiddleParaValuesFromCurrentInfo(CurrentInfo, queryService.GetProcedureParameters(query.QueryProcedureName));
            var paraValues = queryParameters.GetParameters();
            var _queryService = GetService<ICommonQueryService>();
            var result = _queryService.ExecuteQuery<T>(procedure, paraValues);
            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region 执行grid的批量增加功能
        /// <summary>
        /// 执行grid的批量增加
        /// </summary>
        /// <param name="request">数据源请求对象</param>
        /// <param name="addModels">要增加的数据库实体列表</param>
        /// <param name="actionsBeforeAdd">在执行add之前，需要执行的方法，一般是给主键赋值</param>
        /// <returns>批量增加结果</returns>
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult _Add(DataSourceRequest request, IEnumerable<T> addModels, Action<T> actionsBeforeAdd, OpLogType opType)
        {
            try
            {
                var service = GetService<I>();
                foreach (var add in addModels)
                {
                    actionsBeforeAdd(add);
                    service.Add(add);
                }
                service.AddDataChangeLog(opType);
                service.Commit();
                return Json(addModels.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {                
                //友好提示
                ModelState.AddModelError("Name", JsonResultData.FriendlyMessage(ex));
                return Json(addModels.ToDataSourceResult(request, ModelState));
            }
        }
        #endregion

        #region 执行grid的批量修改功能
        /// <summary>
        /// 执行grid的批量修改
        /// </summary>
        /// <param name="request">数据源请求对象</param>
        /// <param name="updateModels">要更新的数据库实体实例集合</param>
        /// <param name="originModels">要更新的数据库实体的原始实例集合</param>
        /// <param name="getOriginModelForCurrentUpdate">从原始实例集合中找出对应当前修改的实例的原始实例，一般是直接根据主键值进行查找</param>
        /// <param name="afterSaveCommit">主要是用于一些主键会在保存后由触发器自动修改的情况，在此方法中重新获取一次主键值或者是根据相同规则重新给主键字段赋值</param>
        /// <returns>批量修改后的结果</returns>
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult _Update(DataSourceRequest request, IEnumerable<T> updateModels, IEnumerable<T> originModels, Func<IEnumerable<T>, T, T> getOriginModelForCurrentUpdate, OpLogType opType,Action<T> afterSaveCommit = null)
        {
            try
            {
                var service = GetService<I>();
                foreach (var update in updateModels)
                {
                    var origin = getOriginModelForCurrentUpdate(originModels, update);
                    service.Update(update, origin);
                }
                service.AddDataChangeLog(opType);
                service.Commit();
                if(afterSaveCommit != null)
                {
                    foreach(var update in updateModels)
                    {
                        afterSaveCommit(update);
                    }
                }
                return Json(updateModels.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Name", JsonResultData.FriendlyMessage(ex));
                return Json(updateModels.ToDataSourceResult(request, ModelState));
            }
        }
        #endregion

    }
}
