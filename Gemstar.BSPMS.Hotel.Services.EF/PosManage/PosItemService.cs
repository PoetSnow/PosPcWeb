using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos消费项目服务实现
    /// </summary>
    public class PosItemService : CRUDService<PosItem>, IPosItemService
    {
        private DbHotelPmsContext _db;

        public PosItemService(DbHotelPmsContext db) : base(db, db.PosItems)
        {
            _db = db;
        }

        protected override PosItem GetTById(string id)
        {
            return new PosItem { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的消费项目是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目代码</param>
        /// <param name="name">消费项目名称</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string dcFlag)
        {
            return _db.PosItems.Any(w => w.Hid == hid && w.DcFlag == dcFlag && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的消费项目是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目代码</param>
        /// <param name="name">消费项目名称</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <param name="exceptId">要排队的消费项目id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string dcFlag, string exceptId)
        {
            return _db.PosItems.Any(w => w.Hid == hid && w.DcFlag == dcFlag && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 根据酒店、付款方式获取处理方式
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">付款方式id</param>
        /// <returns></returns>
        public PosItem GetEntity(string hid, string id)
        {
            return _db.PosItems.Where(w => w.Hid == hid && w.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定酒店和模块下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        public List<PosItem> GetPosItem(string hid, string dcFlag)
        {
            return _db.PosItems.Where(w => w.Hid == hid && w.DcFlag == dcFlag && (w.Status == (byte)EntityStatus.启用 || w.Status == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店下的消费项目总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns></returns>
        public int GetPosItemTotalByDcFlag(string hid, string dcFlag)
        {
            return _db.PosItems.Count(w => w.Hid == hid && w.DcFlag == dcFlag && (w.Status == (byte)EntityStatus.启用 || w.Status == null));
        }

        public List<PosItem> GetPosItemByDcFlag(string hid, string dcFlag, int pageIndex, int pageSize, string refeId)
        {
            var list = _db.PosItems.Where(w => w.Hid == hid && w.DcFlag == dcFlag && (w.Status == (byte)EntityStatus.启用 || w.Status == null)).OrderBy(o => o.Seqid).ToList();
            //判断付款方式项目是否有设置营业点
            //if (list.Where(d => d.Refeid != null).Count() > 0)
            //{
            list = list.Where(d => string.IsNullOrEmpty(d.Refeid) || d.Refeid.Contains(refeId)).ToList();
            //  }
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        public List<PosItem> GetPosItemByModule(string hid, string moduleCode, string dcFlag)
        {
            return _db.PosItems.Where(w => w.Hid == hid && w.DcFlag == dcFlag && w.Module == moduleCode && (w.Status == (byte)EntityStatus.启用 || w.Status == null)).ToList();
        }

        /// <summary>
        /// 根据指定酒店、模块、付款标识、是否开台项目获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="dcFlag">付款标识（D：消费，C：付款）</param>
        /// <param name="isOpenItem">是否开台项目</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        public List<PosItem> GetPosOpenItemByModule(string hid, string moduleCode, string dcFlag, bool? isOpenItem)
        {
            return _db.PosItems.Where(w => w.Hid == hid && w.DcFlag == dcFlag && w.Module == moduleCode && w.IsOpenItem == isOpenItem && (w.Status == (byte)EntityStatus.启用 || w.Status == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店、模块和是否分类下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemClassid">项目大类ID</param>
        /// <param name="isSubClass">是否是分类</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店、模块和是否分类下的消费项目列表</returns>
        public List<PosItem> GetPosItemByItemAndIsSubClass(string hid, string itemClassid, bool isSubClass, string dcFlag)
        {
            return _db.PosItems.Where(w => w.Hid == hid && w.DcFlag == dcFlag && w.ItemClassid == itemClassid && w.IsSubClass == isSubClass && (w.Status == (byte)EntityStatus.启用 || w.Status == null)).ToList();
        }

        /// <summary>
        /// 获取指定项目大类、分类下的项目代码(自增1)
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemClassid">项目大类ID</param>
        /// <param name="subClassid">项目分类ID</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店、模块和是否分类下的消费项目列表</returns>
        public string GetItemCodeByClassid(string hid, string itemClassid, string subClassid, string dcFlag, bool isSubClass = false)
        {
            string itemCode = "001";
            string itemClassCode = "";
            string subClassCode = "";

            PosItemClass itemClassList = _db.PosItemClasss.Where(w => w.Hid == hid && w.Id == itemClassid).FirstOrDefault();
            if (itemClassList != null)
            {
                itemClassCode = itemClassList.Code;
            }

            PosItem subClassList = _db.PosItems.Where(w => w.Hid == hid && w.Id == subClassid).FirstOrDefault();
            if (subClassList != null)
            {
                subClassCode = subClassList.Code;
            }

            int count = _db.PosItems.Count(w => w.Hid == hid && w.DcFlag == dcFlag && w.IsSubClass == isSubClass && w.ItemClassid == itemClassid && (string.IsNullOrEmpty(w.SubClassid) || w.SubClassid == subClassid));

            if (count == 0)
            {
                if (string.IsNullOrWhiteSpace(subClassCode))
                {
                    itemCode = itemClassCode + itemCode;
                }
                else
                {
                    itemCode = subClassCode + itemCode;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(subClassCode))
                {
                    itemCode = itemClassCode + (count + 1).ToString().PadLeft(3, '0');
                }
                else
                {
                    itemCode = subClassCode + (count + 1).ToString().PadLeft(3, '0');
                }
            }

            return itemCode;
        }


        /// <summary>
        /// 获取指定项目大类、分类下的项目代码
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemClassid">项目大类ID</param>
        /// <param name="subClassid">项目分类ID</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>代码</returns>
        public string GetNewItemCodeByClassid(string hid, string itemClassid, string subClassid, string dcFlag, bool isSubClass = false)
        {
            string itemCode = "001";
            string itemClassCode = "";
            string subClassCode = "";
            string classcode = "";

            PosItemClass itemClass = _db.PosItemClasss.Where(w => w.Hid == hid && w.Id == itemClassid).FirstOrDefault();
            if (itemClass != null)
            {
                itemClassCode = itemClass.Code;
                classcode = itemClassCode.Trim();
            }

            PosItem subClass = _db.PosItems.Where(w => w.Hid == hid && w.Id == subClassid).FirstOrDefault();
            if (subClass != null)
            {
                subClassCode = subClass.Code;
                classcode = subClassCode.Trim();
            }

            //var sql = "exec up_pos_GetNewPositemCode @hid,@itemclassid,@isSubClass,@subClassid,@dcFlag,@ClassCode";
            //var sqlpara = new SqlParameter [6]
            //{
            //    new SqlParameter("@hid",hid),
            //    new SqlParameter("@itemclassid", itemClassid),
            //    new SqlParameter("@isSubClass", isSubClass),
            //    new SqlParameter("@subClassid", subClassid ?? ""),
            //    new SqlParameter("@dcFlag", dcFlag),
            //    new SqlParameter("@ClassCode", classcode)
            //};
            //var code = _db.Database.SqlQuery<string>(sql,sqlpara).FirstOrDefault();
            //itemCode = classcode + newnum.ToString().PadLeft(3, '0');
            //return itemCode;

            var maxnum = _db.PosItems.Where(t => t.Hid == hid && t.Code.StartsWith(classcode)).Max(t => t.Code);
            if (maxnum != classcode && !string.IsNullOrEmpty(maxnum))
            {
                var newnum = long.Parse(maxnum.Substring(maxnum.Length - (maxnum.Length - classcode.Length))) + 1;
                itemCode = classcode + newnum.ToString().PadLeft(3, '0');
                return itemCode;
            }
            else
            {
                var code = "1";
                itemCode = classcode + code.PadLeft(3, '0');
                return itemCode;
            }
        }



        /// <summary>
        /// 根据酒店、营业点、项目大类、市别、获取消费项目总记录数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="itemClassid">项目id</param>
        /// <param name="shuffleid">市别id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public int GetPosItemTotal(string hid, string refeid, string itemClassid, string shuffleid, string keyword)
        {
            var results = _db.Database.SqlQuery<up_pos_list_itemByItemClassidResult>("exec up_pos_list_itemByItemClassid @hid=@hid,@refeid=@refeid,@itemClassid=@itemClassid,@shuffleid=@shuffleid,@keyword=@keyword", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@itemClassid", itemClassid), new SqlParameter("@shuffleid", shuffleid), new SqlParameter("@keyword", keyword)).Count();
            return results;
        }

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="itemClassid">项目id</param>
        /// <param name="shuffleid">市别id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public List<up_pos_list_itemByItemClassidResult> GetPosItemByItemClassid(string hid, string refeid, string itemClassid, string shuffleid, string keyword)
        {
            var results = _db.Database.SqlQuery<up_pos_list_itemByItemClassidResult>("exec up_pos_list_itemByItemClassid @hid=@hid,@refeid=@refeid,@itemClassid=@itemClassid,@shuffleid=@shuffleid,@keyword=@keyword", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@itemClassid", itemClassid), new SqlParameter("@shuffleid", shuffleid), new SqlParameter("@keyword", keyword)).OrderBy(o => o.Seqid).ToList();
            return results;
        }

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="itemClassid">项目id</param>
        /// <param name="shuffleid">市别id</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public List<up_pos_list_itemByItemClassidResult> GetPosItemByItemClassid(string hid, string refeid, string itemClassid, string shuffleid, int pageIndex, int pageSize, string keyword)
        {
            var results = _db.Database.SqlQuery<up_pos_list_itemByItemClassidResult>("exec up_pos_list_itemByItemClassid @hid=@hid,@refeid=@refeid,@itemClassid=@itemClassid,@shuffleid=@shuffleid,@keyword=@keyword", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@itemClassid", itemClassid), new SqlParameter("@shuffleid", shuffleid), new SqlParameter("@keyword", keyword)).OrderBy(o => o.Seqid).ToList();
            return results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }




        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public List<up_pos_list_suitItemByRefeidResult> GetSuitItemByRefeid(string hid, string refeid, string keyword)
        {
            var results = _db.Database.SqlQuery<up_pos_list_suitItemByRefeidResult>("exec up_pos_list_suitItemByRefeid @hid=@hid,@refeid=@refeid,@keyword=@keyword", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@keyword", keyword)).OrderBy(o => o.Seqid).ToList();
            return results;
        }

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public List<up_pos_list_suitItemByRefeidResult> GetSuitItemByRefeid(string hid, string refeid, int pageIndex, int pageSize, string keyword)
        {
            var results = _db.Database.SqlQuery<up_pos_list_suitItemByRefeidResult>("exec up_pos_list_suitItemByRefeid @hid=@hid,@refeid=@refeid,@keyword=@keyword", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@keyword", keyword)).OrderBy(o => o.Seqid).ToList();
            return results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 根据酒店ID得到最大的Code
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public string GetMaxtemCode(string hid)
        {
            var result = _db.PosItems.Where(m => m.Hid == hid).Max(m => m.Code);
            return result;
        }

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public override JsonResultData BatchUpdateStatus(string ids, EntityStatus status, OpLogType opType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔");
                }
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    var update = _db.PosItems.Find(id);

                    if (update.Status != (byte)status)
                    {
                        update.Status = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(opType);
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 验证消费项目是否有使用
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        public bool IsExistsBillByItemId(string hid, string itemId)
        {
            bool result = false;

            //判断消费项目是否在账单中被占用
            if (_db.PosBillDetails.Any(w => w.Hid == hid && w.Itemid == itemId))
            {
                result = true;
            }
            //消费项目是分类 是否有子级项目
            if (_db.PosItems.Any(w => w.Hid == hid && w.SubClassid == itemId) ||
                _db.PosItemMultiClasss.Any(w => w.Hid == hid && w.ItemClassid == itemId && w.Itemid != itemId))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 指定酒店和营业点是否拥有套餐项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeid">营业点ID</param>
        /// <returns></returns>
        public bool isItemSuit(string hid, string refeid)
        {
            return _db.PosItems.Any(a => a.Hid == hid && (a.Refeid == refeid || a.Refeid == null) && (a.IsSuite == true || a.IsFeast == true || a.IsUserDefined == true));
        }

        /// <summary>
        /// 获取酒店下的所有库存
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<PosItem> GetPosCostItem(string hid)
        {
            return _db.PosItems.Where(x => x.Hid == hid && x.IsCostItem == true).ToList();
        }

        /// <summary>
        /// 根据酒店和处理方式获取付款方式
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="payType"></param>
        /// <returns></returns>
        public PosItem GetItem(string hid, string payType)
        {
            var item = _db.PosItems.Where(m => m.Hid == hid && m.PayType == payType).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// 通过 lambda表达式获取 消费项目
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="wherefunc">lambda表达式</param>
        /// <returns></returns>
        public List<PosItem> GetItems(string hid, Func<PosItem, bool> wherefunc)
        {
            return _db.PosItems.Where(m => m.Hid == hid).Where(wherefunc).ToList();
        }


        /// <summary>
        /// 根据酒店和显示设置获取消费项目
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="showSet"></param>
        /// <returns></returns>
        public List<up_pos_scan_list_PosItemByShowSetResult> GetPosItemByShowSet(string hid, string showSet)
        {
            return _db.Database.SqlQuery<up_pos_scan_list_PosItemByShowSetResult>("exec up_pos_scan_list_PosItemByShowSet @hid=@hid,@showSet=@showSet", new SqlParameter("@hid", hid), new SqlParameter("@showSet", showSet)).ToList();
        }

        /// <summary>
        /// 获取指定酒店和显示设置下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="showSet">显示设置</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        public List<PosItem> GetPosItemByShowSet(string hid, string moduleCode, string showSet, string dcFlag)
        {
            return _db.PosItems.Where(w => w.Hid == hid && w.Module == moduleCode && w.ShowSet.Contains(showSet) && w.DcFlag == dcFlag && (w.Status == (byte)EntityStatus.启用 || w.Status == null)).ToList();
        }


        /// <summary>
        /// 根据消费项目删除相关联的数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="module"></param>
        /// <param name="itemid"></param>
        public void DeletePosItemOther(string hid, string itemid)
        {
            _db.Database.ExecuteSqlCommand(" exec uo_pos_DeletePosItem @hid=@hid,@itemid=@itemid", new SqlParameter("@hid", hid), new SqlParameter("@itemid", itemid));
        }
        /// <summary>
        /// 查询消费项目大类
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="wherefunc"></param>
        /// <returns></returns>
        public List<PosItem> GetPosItems(string hid, Func<PosItem, bool> wherefunc)
        {
            return _db.PosItems.Where(u => u.Hid == hid).Where(wherefunc).ToList();
        }


        //获取Pos支付方式
        /// <summary>
        /// 获取Pos支付方式
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Entities.V_codeListPub GetPosPayType(string code)
        {
            var service = DependencyResolver.Current.GetService<ICodeListService>();
            var currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
            //取出当前酒店运营后台中设置的已开通的付款处理动作
            var hid = currentInfo.HotelId;
            var MasterService = DependencyResolver.Current.GetService<IMasterService>();
            var ac = MasterService.GetHotelItemAction(hid);
            var actionList = new List<string>();
            actionList.Add("no");
            actionList.Add("PrePay");
            if (ac != null)
            {
                actionList.AddRange(ac.Split(','));
            }
            //取出当前产品适用的所有付款处理动作
            var datas = service.GetPosPayType(currentInfo.ProductType);
            //合并取出当前酒店已经开通的付款处理动作
            var listItems = datas.Where(w => actionList.Contains(w.code)).ToList();
            return listItems.Where(u => u.code == code).FirstOrDefault();
        }

        /// <summary>
        /// 扫码点餐的项目分类
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeId"></param>
        /// <returns></returns>
        public List<PosItem> GetScanPosSubClassList(string hid, string refeId)
        {
            return _db.Database.SqlQuery<PosItem>("exec up_pos_scan_subClassList @hid=@hid,@refeId=@refeId", new SqlParameter("@hid", hid), new SqlParameter("@refeId", refeId)).ToList();
        }

        public string PosItemHz2other(string name, int type)
        {
            return _db.Database.SqlQuery<string>($"SELECT dbo.uf_Hz2other('{name}',{type})").FirstOrDefault();
        }

        public bool IsExistsPosCostByItemId(string hid, string itemId)
        {
            bool result = false;

            //判断消费项目是否在库存中被占用
            if (_db.PostCosts.Any(w => w.Hid == hid && w.CostItemid == itemId))
            {
                result = true;
            }

            return result;
        }
    }
}