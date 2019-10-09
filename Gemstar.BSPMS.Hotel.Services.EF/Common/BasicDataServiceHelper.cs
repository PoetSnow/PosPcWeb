using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Services.EF.Common
{
    /// <summary>
    /// 基础数据服务辅助类，用于处理基础数据分发规则
    /// </summary>
    public static class BasicDataServiceHelper
    {
        #region 增加集团记录并且按指定方式进行分发
        /// <summary>
        /// 增加集团记录并且按指定方式进行分发
        /// </summary>
        /// <typeparam name="T">基础数据类型</typeparam>
        /// <param name="addPara">增加并且分发参数实例</param>
        /// <returns>增加的记录列表</returns>
        public static List<T> AddAndCopy<T>(BasicDataAddAndCopyModel<T> addPara) where T : class
        {
            var result = new List<T>();
            addPara.CRUDService.Add(addPara.GroupModel);
            result.Add(addPara.GroupModel);
            //根据分发方式，对分店进行分发
            if (addPara.DataControlCode == DataControlType.AllResorts.Code)
            {
                //分发时与酒店权限无关，直接取集团下的所有分店
                var allHotels = addPara.DB.PmsHotels.Where(w => w.Grpid == addPara.GroupId && w.Hid != addPara.GroupId).ToList();
                foreach (var hotel in allHotels)
                {
                    var hotelModel = AddHotelCopyed(addPara, hotel.Hid);
                    if (hotelModel != null)
                    {
                        result.Add(hotelModel);
                    }
                }
            }
            else if (addPara.DataControlCode == DataControlType.SelectedResorts.Code)
            {
                //根据选中的酒店id进行分发
                foreach (var hotelId in addPara.SelectedResortHids)
                {
                    if (!string.IsNullOrWhiteSpace(hotelId))
                    {
                        var hotelModel = AddHotelCopyed(addPara, hotelId);
                        if (hotelModel != null)
                        {
                            result.Add(hotelModel);
                        }
                    }
                }
                //将选中的酒店id保存到基础资料控制表中，以便下次默认这些酒店
                addPara.DB.Database.ExecuteSqlCommand("update BasicDataResortControl set SelectedHids = @selectedHids where BasicDataCode = @code and grpid = @grpid",
                new SqlParameter("@selectedHids", string.Join(",", addPara.SelectedResortHids) ?? "")
                , new SqlParameter("@code", addPara.BasicDataCode ?? "")
                , new SqlParameter("@grpid", addPara.GroupId ?? "")
                );
            }
            else
            {
                //是根据分店属性来进行分发
                var manageTypeHotels = addPara.DB.PmsHotels.Where(w => w.Grpid == addPara.GroupId && w.Hid != addPara.GroupId && w.ManageType == addPara.DataControlCode).ToList();
                foreach (var hotel in manageTypeHotels)
                {
                    var hotelModel = AddHotelCopyed(addPara, hotel.Hid);
                    if (hotelModel != null)
                    {
                        result.Add(hotelModel);
                    }
                }
            }
            return result;
        }
        private static T AddHotelCopyed<T>(BasicDataAddAndCopyModel<T> addPara, string hid) where T : class
        {
            var hotelModel = addPara.BasicDataService.GetNewHotelBasicData(hid, addPara.GroupModel);
            if (hotelModel != null)
            {
                addPara.CRUDService.Add(hotelModel);
            } 
            return hotelModel;
        }
        #endregion

        #region 修改并且分发集团记录
        /// <summary>
        /// 修改并且分发集团记录
        /// </summary>
        /// <typeparam name="T">基础数据类型</typeparam>
        /// <param name="editPara">修改并且分发参数实例</param>
        /// <returns>修改后的记录列表</returns>
        public static List<T> EditAndCopy<T>(BasicDataEditAndCopyModel<T> editPara) where T : class
        {
            var result = new List<T>();
            editPara.CRUDService.Update(editPara.GroupModel, editPara.OriginGroupModel, editPara.GroupModelUpdateFieldNames);
            result.Add(editPara.GroupModel);

            //根据分发方式，对分店进行分发
            if (editPara.DataControlCode == DataControlType.AllResorts.Code)
            {
                //分发时与酒店权限无关，直接取集团下的所有分店
                var allHotels = editPara.DB.PmsHotels.Where(w => w.Grpid == editPara.GroupId && w.Hid != editPara.GroupId).OrderBy(w => w.Seqid).ToList();
                foreach (var hotel in allHotels)
                {
                    var hotelModel = EditHotelCopyed(editPara, hotel.Hid);
                    result.Add(hotelModel);
                }
            }
            else if (editPara.DataControlCode == DataControlType.SelectedResorts.Code)
            {
                //根据选中的酒店id进行分发
                foreach (var hotelId in editPara.SelectedResortHids)
                {
                    if (!string.IsNullOrWhiteSpace(hotelId))
                    {
                        var hotelModel = EditHotelCopyed(editPara, hotelId);
                        result.Add(hotelModel);
                    }
                }
                //将选中的酒店id保存到基础资料控制表中，以便下次默认这些酒店
                editPara.DB.Database.ExecuteSqlCommand("update BasicDataResortControl set SelectedHids = @selectedHids where BasicDataCode = @code and grpid = @grpid",
                 new SqlParameter("@selectedHids", string.Join(",", editPara.SelectedResortHids) ?? "")
                 , new SqlParameter("@code", editPara.BasicDataCode ?? "")
                 , new SqlParameter("@grpid", editPara.GroupId ?? "")
                 );
            }
            else
            {
                //是根据分店属性来进行分发  
                var manageTypeHotels = editPara.DB.PmsHotels.Where(w => w.Grpid == editPara.GroupId && w.Hid != editPara.GroupId && w.ManageType == editPara.DataControlCode).OrderBy(w => w.Seqid).ToList();
                foreach (var hotel in manageTypeHotels)
                {
                    var hotelModel = EditHotelCopyed(editPara, hotel.Hid);
                    result.Add(hotelModel);
                }
            }
            return result;
        }
        private static T EditHotelCopyed<T>(BasicDataEditAndCopyModel<T> editPara, string hid) where T : class
        {
            var hotelModel = editPara.BasicDataService.GetCopyedHotelBasicData(hid, editPara.GroupModel,false);
            if (hotelModel == null)
            {
                hotelModel = editPara.BasicDataService.GetNewHotelBasicData(hid, editPara.GroupModel);
                if (hotelModel != null)
                {
                    editPara.CRUDService.Add(hotelModel);
                }
            }
            else
            {
                hotelModel = editPara.BasicDataService.GetCopyedHotelBasicData(hid, editPara.GroupModel, true);
                if (hotelModel != null)
                {
                    AutoSetValueHelper.SetValues(editPara.GroupModel, hotelModel, editPara.CopyedUpdateProperties);
                    editPara.CRUDService.Update(hotelModel, editPara.OriginGroupModel, editPara.CopyedUpdateProperties);
                }
            }
            return hotelModel;
        }
        #endregion

        #region 启用禁用并且同时分发集团记录
        /// <summary>
        /// 启用禁用并且同时分发集团记录
        /// </summary>
        /// <typeparam name="T">要启用禁用的集团记录</typeparam>
        /// <param name="changePara">更改到的目标状态</param>
        /// <returns>更改状态的实体列表</returns>
        public static List<T> ChangeGroupAndHotelCopiedStatus<T>(BasicDataStatusChangeAndCopyModel<T> changePara) where T : class, IEntityEnable
        {
            var result = new List<T>();
            changePara.CRUDService.ChangeStatus(changePara.GroupModel, changePara.Status);
            result.Add(changePara.GroupModel);
            var hotelCopied = changePara.BasicDataService.GetCopyedHotelBasicDatas(changePara.GroupModel);
            foreach (var copy in hotelCopied)
            {
                changePara.CRUDService.ChangeStatus(copy,changePara.Status);
                result.Add(copy);
            }
            return result;
        }
        #endregion

        #region 删除集团记录并且同时删除分发的所有分店记录
        /// <summary>
        /// 删除集团记录并且同时删除分发的所有分店记录
        /// </summary>
        /// <typeparam name="T">基础数据类型</typeparam>
        /// <param name="deletePara">删除参数</param>
        public static List<T> DeleteGroupAndHotelCopied<T>(BasicDataDeleteGroupAndHotelCopiedModel<T> deletePara) where T : class
        {
            var result = new List<T>();
            deletePara.CRUDService.Delete(deletePara.GroupModel);
            result.Add(deletePara.GroupModel);
            var hotelCopied = deletePara.BasicDataService.GetCopyedHotelBasicDatas(deletePara.GroupModel);
            foreach(var copy in hotelCopied)
            {
                deletePara.CRUDService.Delete(copy);
                result.Add(copy);
            }
            return result;
        }
        #endregion
    }
}
