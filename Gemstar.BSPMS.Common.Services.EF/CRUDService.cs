using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Common.Services.EF
{
    /// <summary>
    /// 通用的增删改查的实现
    /// </summary>
    /// <typeparam name="T">增删改查的类型</typeparam>
    public abstract class CRUDService<T> : ICRUDService<T> where T : class
    {
        /// <summary>
        /// 由子类来实现，根据指定的id值来实例化类型实例
        /// 主要是批量删除时会调用此方法
        /// </summary>
        /// <param name="id">id值</param>
        /// <returns>对应的实例</returns>
        protected abstract T GetTById(string id);
        /// <summary>
        /// 可以由子类来重写，实现一些在删除之前的业务处理逻辑，默认为空
        /// </summary>
        /// <param name="obj">需要删除的业务实体类</param>
        protected virtual void BeforeDelete(T obj) { }
        /// <summary>
        /// 可以由子类来重写，实现一些在更新之后的业务处理逻辑，默认为空
        /// </summary>
        /// <param name="obj">要更新的对象，要求所有字段都有值</param>
        /// <param name="originObj">要更新的对象的原始对象，一般是从客户端传递回来的，传给这边以便只update更改了的列而不是全部列进行update</param>
        /// <param name="needUpdateFieldNames">要更新的字段名称列表，一般是根据从客户端传递回来的值自动计算出来的一个字段名称列表</param>
        protected virtual void AfterUpdate(T obj, T originObj, List<string> needUpdateFieldNames = null) { }


        /// <summary>
        /// 创建通用的增删改查实现的实例
        /// </summary>
        /// <param name="db">EF实例</param>
        /// <param name="tset">增删改查类型的集合</param>
         protected CRUDService(DbContext db, DbSet<T> tset)
        {
            dbContext = db;
            dbSetForT = tset;
        }
        /// <summary>
        /// 增加对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public virtual void Add(T obj)
        {
            dbSetForT.Add(obj);
        }

        /// <summary>
        /// 增加集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        public virtual List<T> AddAndCopy(T groupModel, string groupId, string dataControlCode, List<string> selectedResortHids)
        {            
            throw new NotImplementedException("增加集团记录并且分发需要由具体的子服务类来实现");
        }
        /// <summary>
        /// 增加数据增删改的日志记录
        /// </summary>
        /// <param name="opType">当前操作类型</param>
        public void AddDataChangeLog(OpLogType opType)
        {
            var logWriter = dbContext as IDataChangeLog;
            if(logWriter != null)
            {
                logWriter.AddDataChangeLogs(opType);
            }
        }
        public void ChangeStatus(T model,EntityStatus status)
        {
            if(model is IEntityEnable)
            {
                var change = (IEntityEnable)model;
                change.Status = status;
                dbContext.Entry(model).State = EntityState.Modified;
            }else
            {
                throw new ApplicationException("更改状态时实体类型必须实现接口IEntityEnable");
            }
        }
        public virtual List<T> ChangeStatusAndCopy(T model, EntityStatus status)
        {
            throw new NotImplementedException("启用或禁用集团并分发到集团分店的记录需要由具体的子服务类来实现");
        }

        /// <summary>
        /// 批量启用禁用
        /// </summary>
        /// <param name="ids">以逗号分隔的实体主键</param>
        /// <param name="status">新的状态</param>
        /// <param name="logType">操作类型</param>
        /// <returns>批量启用禁用结果</returns>
        public virtual JsonResultData BatchUpdateStatus(string ids, EntityStatus status, OpLogType logType)
        {
            throw new NotImplementedException("批量启用禁用需要由具体的子服务类来实现");
        }
        /// <summary>
        /// 提交所有修改
        /// </summary>
        public virtual void Commit()
        {
            dbContext.SaveChanges();
        }
        /// <summary>
        /// 删除对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public virtual void Delete(T obj)
        {
            BeforeDelete(obj);
            try
            {
                dbSetForT.Remove(obj);
            }
            catch
            {
                dbContext.Entry(obj).State = EntityState.Deleted;
            }
        }
        /// <summary>
        /// 删除集团和分发到集团分店的记录
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        public virtual List<T> DeleteGroupAndHotelCopied(T groupModel)
        {
            throw new NotImplementedException("删除集团和分发到集团分店的记录需要由具体的子服务类来实现");
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">要删除的用户id，多项之间以逗号分隔</param>
        /// <returns>删除结果</returns>
        public JsonResultData BatchDelete(string ids, OpLogType opType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return JsonResultData.Failure("请指定要删除的记录id，多项之间以逗号分隔");
                }
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    T delete = GetTById(id);
                    Delete(delete);
                }
                AddDataChangeLog(opType);
                Commit();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }


        /// <summary>
        /// 根据主键值获取对应的实例对象
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>对应的实例对象</returns>
        public virtual T Get(object key)
        {
            return dbSetForT.Find(key);
        }

        //public virtual void Update(T obj)
        //{
        //    try
        //    {
        //        dbContext.Entry(obj).State = EntityState.Modified;
        //    }
        //    catch
        //    {
        //        UpdateLoaded(obj);
        //    }
        //}
        public static void Update<U>(DbContext dbContext, U obj, U originObj, List<string> needUpdateFieldNames = null) where U:class
        {
            DbEntityEntry<U> entry;
            try
            {
                if (originObj == null)
                {
                    dbContext.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    entry = dbContext.Entry(obj);
                    entry.State = EntityState.Unchanged;
                    //对比obj与originObj对象的所有属性，只有值不相同时，才设置为已修改
                    Type tType = typeof(T);
                    var properties = tType.GetProperties();
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(obj);
                        var originValue = prop.GetValue(originObj);
                        var needUpdate = needUpdateFieldNames == null ? true : needUpdateFieldNames.Contains(prop.Name);
                        if (needUpdate && !object.Equals(value, originValue))
                        {
                            entry.Property(prop.Name).IsModified = true;
                        }
                    }
                }
            }
            catch
            {
                UpdateLoaded(dbContext,obj);
            }
        }
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
        public virtual List<T> EditAndCopy(T groupModel, T originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties)
        {
            throw new NotImplementedException("修改集团记录并且分发需要由具体的子服务类来实现");
        }

        private static void UpdateLoaded<U>(DbContext dbContext,U obj) where U:class
        {
            ObjectContext objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, obj);


            object origin;
            if (objContext.TryGetObjectByKey(entityKey, out origin))
            {
                objContext.ApplyCurrentValues(objSet.EntitySet.Name, obj);
            }
            else
            {
                dbContext.Entry(obj).State = EntityState.Modified;
            }
        }
        /// <summary>
        /// 要更新的对象
        /// 使用此方法时注意，不能只是新创建一个对象，然后修改几个字段的值后调用此方法，如果这样的话，会导致那些没有被赋值的字段的值全部变成null值
        /// 要么是从ef中获取来的对象进行修改，要么是新建的对象，但是赋全部字段的值
        /// </summary>
        /// <param name="obj">要更新的对象，要求所有字段都有值</param>
        /// <param name="originObj">要更新的对象的原始对象，一般是从客户端传递回来的，传给这边以便只update更改了的列而不是全部列进行update</param>
        /// <param name="needUpdateFieldNames">要更新的字段名称列表，一般是根据从客户端传递回来的值自动计算出来的一个字段名称列表</param>
        public void Update(T obj, T originObj, List<string> needUpdateFieldNames = null)
        {
            Update(dbContext, obj, originObj, needUpdateFieldNames);
            AfterUpdate(obj, originObj, needUpdateFieldNames);
        }

        private void UpdateLoaded(T obj)
        {
            UpdateLoaded(dbContext, obj);
        }

        private DbContext dbContext;
        private DbSet<T> dbSetForT;
    }
}
