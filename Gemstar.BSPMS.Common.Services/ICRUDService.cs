using Gemstar.BSPMS.Common;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 增删改的通用接口
    /// </summary>
    /// <typeparam name="T">要进行增删改的实体类型</typeparam>
    public interface ICRUDService<T> where T : class
    {
        /// <summary>
        /// 根据主键值获取对应的实例对象
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>对应的实例对象</returns>
        T Get(object key);
        /// <summary>
        /// 增加对象
        /// </summary>
        /// <param name="obj">要增加的对象</param>
        void Add(T obj);

        /// <summary>
        /// 增加集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        List<T> AddAndCopy(T groupModel, string groupId, string dataControlCode, List<string> selectedResortHids);
        /// <summary>
        /// 更改实体状态
        /// </summary>
        /// <param name="model">要更改状态的实体</param>
        /// <param name="status">更改到的实体状态</param>
        void ChangeStatus(T model, EntityStatus status);
        /// <summary>
        /// 启用禁用集团记录并且同时启用禁用分发到分店的记录
        /// </summary>
        /// <param name="model">要启用禁用的集团记录</param>
        /// <param name="status">新的状态</param>
        /// <returns>启用禁用的实体列表</returns>
        List<T> ChangeStatusAndCopy(T model, EntityStatus status);
        /// <summary>
        /// 批量启用禁用
        /// </summary>
        /// <param name="ids">以逗号分隔的实体主键</param>
        /// <param name="status">新的状态</param>
        /// <param name="logType">操作类型</param>
        /// <returns>批量启用禁用结果</returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status, OpLogType logType);
        ///// <summary>
        ///// 要更新的对象
        ///// 使用此方法时注意，不能只是新创建一个对象，然后修改几个字段的值后调用此方法，如果这样的话，会导致那些没有被赋值的字段的值全部变成null值
        ///// 要么是从ef中获取来的对象进行修改，要么是新建的对象，但是赋全部字段的值
        ///// </summary>
        ///// <param name="obj">要更新的对象，要求所有字段都有值</param>
        //void Update(T obj);
        /// <summary>
        /// 要更新的对象
        /// 使用此方法时注意，不能只是新创建一个对象，然后修改几个字段的值后调用此方法，如果这样的话，会导致那些没有被赋值的字段的值全部变成null值
        /// 要么是从ef中获取来的对象进行修改，要么是新建的对象，但是赋全部字段的值
        /// </summary>
        /// <param name="obj">要更新的对象，要求所有字段都有值</param>
        /// <param name="originObj">要更新的对象的原始对象，一般是从客户端传递回来的，传给这边以便只update更改了的列而不是全部列进行update</param>
        /// <param name="needUpdateFieldNames">要更新的字段名称列表，一般是根据从客户端传递回来的值自动计算出来的一个字段名称列表，如果不传递，则直接比较两个对象的所有不匹配字段</param>
        void Update(T obj, T originObj, List<string> needUpdateFieldNames = null);

        /// <summary>
        /// 修改集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="originModel">原始集团记录</param>
        /// <param name="fieldNames">原始集团记录修改字段列表</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        /// <param name="updateProperties">分发属性列表</param>
        List<T> EditAndCopy(T groupModel, T originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties);
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="obj">要删除的对象</param>
        void Delete(T obj);
        /// <summary>
        /// 删除集团和分发到集团分店的记录
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        List<T> DeleteGroupAndHotelCopied(T groupModel);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">要删除的用户id，多项之间以逗号分隔</param>
        /// <returns>删除结果</returns>
        JsonResultData BatchDelete(string ids, OpLogType opType);
        /// <summary>
        /// 增加数据增删改的日志记录
        /// </summary>
        /// <param name="opType">当前操作类型</param>
        void AddDataChangeLog(OpLogType opType);
        /// <summary>
        /// 保存更改
        /// </summary>
        void Commit();
    }
}
