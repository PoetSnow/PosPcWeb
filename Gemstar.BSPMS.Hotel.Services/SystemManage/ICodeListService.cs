using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface ICodeListService : ICRUDService<CodeList>
    {
        /// <summary>
        /// 获取代码类型列表
        /// </summary>
        /// <returns></returns>
        List<CodeType> CodeTypeList();

        /// <summary>
        /// 获取单个代码类型实体
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        CodeType GetCodeType(string code);

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);

        /// <summary>
        /// 是否已存在此记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="typeCode">代码类型</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        bool IsExists(string hid, string typeCode, string code);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="typeCode">类型代码</param>
        /// <returns></returns>
        List<CodeList> List(string hid, string typeCode);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="typeCode">类型代码</param>
        /// <returns></returns>
        List<V_codeListPub> List(string typeCode);
        /// <summary>
        /// 获取指定酒店的客人来源
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的客人来源</returns>
        List<CodeList> GetCustomerSource(string hid);
        /// <summary>
        /// 获取指定酒店的房间特色
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的房间特色</returns>
        List<CodeList> GetRoomFeatures(string hid);
        /// <summary>
        /// 获取指定酒店的市场分类
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的市场分类</returns>
        List<CodeList> GetMarketCategory(string hid);

        /// <summary>
        /// 获取指定酒店的合约单位类别
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的合约单位类别</returns>
        List<CodeList> GetCompanyType(string hid);
        /// <summary>
        /// 获取证件类型
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetCerType();
        /// <summary>
        /// 获取会员账户类型
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetAccountType();
        /// <summary>
        /// 获取pos模块列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosModules();
        /// <summary>
        /// 获取pos类别列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIstagclasss();
        /// <summary>
        /// 获取pos出品方式列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosProduceTypes();
        /// <summary>
        /// 获取pos微信点餐支付方式列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosWxPaytypes();
        /// 获取pos临时台标志列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIstagtemps();
        /// <summary>
        /// 获取pos餐台状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosStatnos();
        /// <summary>
        /// 获取pos财务类型列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosAcClasss();
        /// <summary>
        /// 获取pos币种
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosMontypeno();
        /// <summary>
        /// 获取pos处理方式
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosPayType(ProductType productType);
        /// <summary>
        /// 获取pos要求操作列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosiTagOperator();
        /// <summary>
        /// 获取pos联单打印列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsCombine();
        /// <summary>
        /// 获取pos要求属性列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsTagProperty();
        /// <summary>
        /// 获取pos出品状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsProduce();
        /// <summary>
        /// 获取pos显示临时台列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsHideTab();
        /// <summary>
        /// 获取pos出品名称列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsProdName();
        /// <summary>
        /// 获取pos状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosStatus();
        /// <summary>
        /// 获取pos点作法选项列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsOrderAction();
        /// <summary>
        /// 获取pos减库存列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsStock();
        /// <summary>
        /// 获取pos日期类型列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosITagperiod();
        /// <summary>
        /// 获取pos最低消费计法列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsByPerson();
        /// <summary>
        /// 获取pos数量方式列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosQuanMode();
        /// <summary>
        /// 获取pos收费状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsCharge();
        /// <summary>
        /// 获取pos串口号列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosComno();
        /// <summary>
        /// 获取开台录入信息列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosOpenInfo();
        /// <summary>
        /// 获取pos折扣类型列表
        /// </summary>
        /// <returns></returns>
         List<V_codeListPub> GetPosIsForce();
        /// <summary>
        /// 获取pos金额折类型列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosDaType();
        /// <summary>
        /// 获取pos账单状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosBillStatus();
        /// <summary>
        /// 获取posKTV开台类型列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIKtvStatus();
        /// <summary>
        /// 获取pos自动标志列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosIsauto();
        /// <summary>
        /// 获取pos账单明细状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosBillDetailStatus();
        /// <summary>
        /// 获取pos账单明细出品状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosBillDetailIsProduce();
        /// <summary>
        /// 获取pos客人类型状态列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosCustomerTypeStatus();
        /// <summary>
        /// 获取pos原因类型列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPosReasonIstagType();
        /// <summary>
        /// 获取楼层名
        /// </summary>
        /// <returns></returns>
        string GetFloorName(string id);
        /// <summary>
        /// 获取客账中的消费项目类型和付款项目类型
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns></returns>
        List<CodeList> GetFolioItemTypes(string hid);
        /// <summary>
        /// 获取发票开票项目
        /// </summary>
        /// <returns></returns>
        List<CodeList> GetInvoiceProjectType(string hid);
        /// <summary>
        /// 获取楼层
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<CodeList> GetFloorType(string hid);
        /// <summary>
        /// 获取业务员提成数据类型
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPercentagesSalesmanType();
        /// <summary>
        /// 获取操作员提成数据类型
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetPercentagesOperatorType();
        List<CodeList> GetItemsProducts(string hid);
        void updateGrpHotelCodelist(string grpid, string typecode);
        /// 通过ID获取合约单位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CodeList GetCodeListByID(string id);

        /// 判断这个数据是否存在其他表中
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string checkIsExistOtherTable(string hid, string id);

        /// <summary>
        /// 获取消费项目显示
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetShowSetList();

        /// <summary>
        /// 日期类型列表
        /// </summary>
        /// <returns></returns>
        List<V_codeListPub> GetiTagperiodList();
    }
}
