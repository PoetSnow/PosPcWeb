using System;
using System.Collections.Generic;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services.Entities;
using System.Data;
using System.Linq;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class HelpFilesServices : CRUDService<HelpFiles>, IHelpFilesService
    {
        private DbCommonContext _DbCommonContext;
        public HelpFilesServices(DbCommonContext db) : base(db, db.HelpFiles)
        {
            _DbCommonContext = db;
        }

        /// <summary>
        /// 增加指定问题的阅读次数
        /// </summary>
        /// <param name="helpId">问题id</param>
        public void AddReadQty(int helpId)
        {
            try
            {
                _DbCommonContext.Configuration.AutoDetectChangesEnabled = true;
                var help = _DbCommonContext.HelpFiles.SingleOrDefault(w => w.Id == helpId);
                help.ReadNumber++;
                _DbCommonContext.SaveChanges();
                _DbCommonContext.Configuration.AutoDetectChangesEnabled = false;
            }
            catch 
            {

            }
        }
        /// <summary>
        /// 获取帮助文档
        /// </summary>
        /// <param name="menuId">菜单模块id</param>
        /// <param name="checkStatu">审核状态</param>
        /// <returns></returns>
        public List<HelpFiles> GetHelpFiles(string menuId)
        {
            var query = _DbCommonContext.HelpFiles as IQueryable<HelpFiles>;
            if (!string.IsNullOrWhiteSpace(menuId))
            {
                query = query.Where(w => w.MenuId.Contains(menuId)).OrderByDescending(o=>o.UpdateDate);
            }
            return query.ToList();
        }

        public List<HelpFilesImg> GetHelpFilesImg(int? helpid, string name)
        {
            var query = _DbCommonContext.HelpFilesImg as IQueryable<HelpFilesImg>;
            if (helpid.HasValue)
            {
                query = query.Where(w => w.HelpId == helpid.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(w => w.Title.Contains(name));
            }
            return query.ToList();
        }

        protected override HelpFiles GetTById(string id)
        {
            var helpFiles = new HelpFiles();
            helpFiles.Id =int.Parse(id);
            return helpFiles;
        }
        public JsonResultData SaveHelpFiles(HelpFiles help)
        {
            try
            {
                if (help.Id == 0)
                {
                    _DbCommonContext.HelpFiles.Add(help);
                }
                else
                {
                    var data = _DbCommonContext.HelpFiles.Where(w => w.Id == help.Id).FirstOrDefault();
                    data.UpdateDate = DateTime.Now;
                    data.UpdateUser = help.UpdateUser;
                    data.CheckStatus = false;
                    data.Content = help.Content;
                    data.Title = help.Title;
                    data.MenuId = help.MenuId;
                    data.MenuName = help.MenuName;
                    _DbCommonContext.Entry(data).State = EntityState.Modified;
                }
                _DbCommonContext.SaveChanges();
                return new JsonResultData() { Success = true };
            }
            catch (Exception)
            {
                return new JsonResultData() { Success = false, Data = "保存失败" };
                
            }



        }
        /// <summary>
        /// 增加上传到七牛后的帮助图片记录
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="url">图片在七牛的访问地址</param>
        /// <param name="helpId">当前帮助记录id</param>
        /// <returns>增加是否成功的结果</returns>
        public JsonResultData AddHelpFileImage(string name, string url, int? helpId)
        {
            try
            {
                var img = new HelpFilesImg
                {
                    Title = name,
                    ImgAddress = url,
                    HelpId = helpId
                };
                _DbCommonContext.HelpFilesImg.Add(img);
                _DbCommonContext.SaveChanges();

                return JsonResultData.Successed("增加帮助图片成功");
            }catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        public JsonResultData SaveHelpFilesAdmin(HelpFiles help)
        {
            try
            {
                var data = _DbCommonContext.HelpFiles.Where(w => w.Id == help.Id).FirstOrDefault();
                data.CheckStatus=help.CheckStatus;
                data.CheckUser = help.CheckUser;
                data.CheckDate = help.CheckDate;
                _DbCommonContext.Entry(data).State = EntityState.Modified;
                _DbCommonContext.SaveChanges();
                return new JsonResultData() { Success = true };
            }
            catch (Exception)
            {
                return new JsonResultData() { Success = false, Data = "保存失败" };
            }
        }
        public JsonResultData DeleteFileImage(int id)
        {
            try
            {
              var data=  _DbCommonContext.HelpFilesImg.Where(w => w.Id == id).FirstOrDefault();
              _DbCommonContext.HelpFilesImg.Remove(data);
              _DbCommonContext.SaveChanges();
              return new JsonResultData() { Success = true };
            }
            catch (Exception)
            {
                return new JsonResultData() { Success = false, Data = "删除失败" };
            }
        }
    }
}
