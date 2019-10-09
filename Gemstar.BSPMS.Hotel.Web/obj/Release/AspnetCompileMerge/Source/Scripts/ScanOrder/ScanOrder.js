//添加消费项目
function AddlocalStorageItem(model) {


    var arr = new Array();
    var list = localStorage.getItem("billDetailList");
    var jsonObj = JSON.parse(list);//转换为json对象
    if (jsonObj == null || jsonObj.length <= 0) {
        //添加
        model.OrderId = 1;

        var newModel = addBillDetail(model);
        arr.push(newModel);
        localStorage.setItem("billDetailList", JSON.stringify(arr));
    }
    else {
        //处理作法
        var actionList = localStorage.getItem("ActionList");
        var jsonAction = JSON.parse(actionList);//转换为json对象


        var actionArr = new Array();        //消费项目对应作法

        var result = true;  //本次操作的数据已经存在 修改
        for (var i = 0; i < jsonObj.length; i++) {
            if (model.Itemid == jsonObj[i].Itemid) {
                if (model.Quantity != 0) {
                    jsonObj[i].Quantity = model.Quantity;
                    jsonObj[i].Amount = model.Quantity * model.Price;
                    //修改做法
                    if (jsonAction != null && jsonAction.length > 0) {
                        for (var j = 0; j < jsonAction.length; j++) {

                            if (jsonObj[i].OrderId == jsonAction[j].OrderId) {
                                actionArr.push(jsonAction[j]);
                            }
                            else {
                                actionArr.push(jsonAction[j]);
                            }
                        }
                    }

                    arr.push(jsonObj[i]);
                }
                else {
                    if (jsonAction != null && jsonAction.length > 0) {
                        for (var j = 0; j < jsonAction.length; j++) {
                            if (jsonAction[j].OrderId != jsonObj[i].OrderId) {
                                actionArr.push(jsonAction[j]);
                            }
                        }
                    }

                }
                result = false;
            }
            else {
                arr.push(jsonObj[i]);
            }

        }

        if (result) {
            //添加
            model.OrderId = parseInt(getBillDetailListMaxNum()) + 1;
            var newModel = addBillDetail(model);
            arr.push(newModel);

            if (jsonAction != null && jsonAction.length > 0) {
                for (var i = 0; i < jsonAction.length; i++) {
                    actionArr.push(jsonAction[i]);

                }
            }

        }

        localStorage.setItem("billDetailList", JSON.stringify(arr));
        localStorage.setItem("ActionList", JSON.stringify(actionArr));
    }
    subClassStyle();
    var list = JSON.parse(localStorage.getItem("billDetailList"));
    if (list == null || list.length <= 0) {
        $("#segmentedControlContents .mui-control-content").each(function () {

            if ($(this).hasClass("mui-active")) {
                $(this).find("span").each(function () {
                    $(this).html("");
                    $(this).css("display", "none");
                    $(this).prev().css("display", "none");
                });
            }

        });
    }
    else {
        $("#segmentedControlContents .mui-control-content").each(function () {
            if ($(this).hasClass("mui-active")) {
                $(this).find("span").each(function () {
                    if (model.Itemid == $(this).attr("data-id") && model.Quantity>0) {
                        $(this).html(model.Quantity);
                        $(this).css("display", "block");
                        $(this).prev().css("display", "block");
                    }
                });
            }
        });
    }
}



//添加数据
function addBillDetail(model) {

    billDetail.OrderId = model.OrderId;
    billDetail.hid = model.hid;
    billDetail.Itemid = model.Itemid;
    billDetail.ItemCode = model.ItemCode;
    billDetail.ItemName = model.ItemName;
    billDetail.IsDiscount = model.IsDiscount;
    billDetail.IsService = model.IsService;
    billDetail.Unitid = model.Unitid;
    billDetail.UnitCode = model.UnitCode;
    billDetail.UnitName = model.UnitName;
    billDetail.Price = model.Price;
    billDetail.Quantity = model.Quantity;
    billDetail.Tabid = model.Tabid;
    billDetail.Amount = model.Price * model.Quantity;
    billDetail.itemClassId = model.itemClassId;
    return billDetail;
}

var billDetail = {


    OrderId: 0,  /// 排序字段。用于本地保存数据记录行号与作法进行匹配

    TopOrderId: 0,  /// 上级行号。用于套餐明细与套餐匹配

    Hid: "",  /// 酒店代码

    Itemid: "",    /// 项目id

    ItemCode: "", /// 项目代码

    ItemName: "",  /// 项目名称

    Unitid: "",  /// 单位id

    UnitCode: "",   /// 单位代码

    UnitName: "",   /// 单位名称

    Quantity: 0.0,  /// 数量

    Piece: 0.0,  /// 称重条只

    Multiple: 0.0, //扣钝倍数

    Price: 0.0, // 单价

    AddPrice: 0.0, // 作法加价

    Dueamount: 0.0, // 折前金额

    Amount: 0.0,  // 金额

    Service: 0.0,       /// 服务费

    Tax: 0.0,    //税额

    Place: "",  // 客位

    Action: "", // 作法

    Request: "", //要求

    Isauto: 0, //自动标志

    Status: 0,//计费状态

    Tabid: "",  //餐台

    Sale: "",   //业务员

    ModiUser: "",   //修改操作员

    ModiDate: "",    //修改时间

    PriceOri: 0.0,   //原价

    PriceClub: 0.0,     //会员价

    DiscountClub: 0.0,  //会员折扣

    Approver: "",   //批准人

    CanReason: "",  //取消原因

    Memo: "",    //Memo

    Acbillno: "",  //手工单号

    OriQuan: 0.0,    //称重原数量

    IsDishDisc: "",   //是否单道折扣

    IsService: "", //是否收取服务费

    itemClassId: ""  //消费项目大类
};

//计算总金额
function refreshTotalPrice() {
    var list = localStorage.getItem("billDetailList");
    var jsonObj = JSON.parse(list);//转换为json对象
    if (jsonObj != null && jsonObj.length > 0) {

        var amount = 0.0;
        for (var i = 0; i < jsonObj.length; i++) {
            amount += parseFloat(jsonObj[i].Amount);
        }
        $("#showShopCart").html('<i class="fa fa-shopping-cart" aria-hidden="true"></i> 购物车 ￥' + amount.toFixed(2) + ' ');

    }
    else {

        $("#showShopCart").html('<i class="fa fa-shopping-cart" aria-hidden="true"></i> 购物车 ￥' + 0.00 + ' ');
    }

}

//滚动菜式添加图片
function addQuantityImg(obj) {
    var model = {
        hid: localStorage.getItem("hid"),
        Itemid: $(obj).attr("data-id"),
        ItemCode: $(obj).attr("data-code"),
        ItemName: $(obj).attr("data-name"),
        IsDiscount: $(obj).attr("data-isDiscount"),
        IsService: $(obj).attr("data-isService"),
        Unitid: $(obj).attr("data-unitId"),
        UnitCode: $(obj).attr("data-unitCode"),
        UnitName: $(obj).attr("data-UnitName"),
        Price: $(obj).attr("data-price"),
        Quantity: 1,
        Tabid: localStorage.getItem("tabid"),
        itemClassId: $(obj).attr("data-subclassid")
    };
    var list = JSON.parse(localStorage.getItem("billDetailList"));
    if (list != null && list.length > 0) {
        for (var i = 0; i < list.length; i++) {
            if (model.Itemid == list[i].Itemid) {
                model.Quantity = list[i].Quantity + 1;
            }
        }
    }
    AddlocalStorageItem(model);
    refreshItemPrice($(obj).attr("data-id"));
    refreshTotalPrice();
}

//添加数量
function addQuantity(obj) {
    $("#itemid").val($(obj).attr("data-id"));
    var quantity = $(obj).parent().find('span').text();
    if (quantity == "" || parseInt(quantity) == 0) {
        quantity = 0;
    }
    var model = {
        hid: localStorage.getItem("hid"),
        Itemid: $(obj).attr("data-id"),
        ItemCode: $(obj).attr("data-code"),
        ItemName: $(obj).attr("data-name"),
        IsDiscount: $(obj).attr("data-isDiscount"),
        IsService: $(obj).attr("data-isService"),
        Unitid: $(obj).attr("data-unitId"),
        UnitCode: $(obj).attr("data-unitCode"),
        UnitName: $(obj).attr("data-UnitName"),
        Price: $(obj).attr("data-price"),
        Quantity: parseInt(quantity) + 1,
        Tabid: localStorage.getItem("tabid"),
        itemClassId: $(obj).attr("data-subclassid")
    };
    if (model.Quantity > 0) {
        $(obj).parent().find('span').html(model.Quantity)
        $(obj).parent().find('a').first().css("display", "block");
        $(obj).parent().find('span').css("display", "block");
    }
    else {

        $(obj).parent().find('span').text("");
        $(obj).parent().find('a').first().css("display", "none");
        $(obj).parent().find('span').css("display", "none");
    }

    AddlocalStorageItem(model);
    refreshItemPrice($(obj).attr("data-id"));
    refreshTotalPrice();

}

/* 减少 */
function reductionQuantity(obj) {
    var quantity = $(obj).parent().find('span').text();


    if (quantity == "" || parseInt(quantity) == 0) {
        quantity = 0;
    }
    var model = {
        Itemid: $(obj).attr("data-id"),
        ItemCode: $(obj).attr("data-code"),
        ItemName: $(obj).attr("data-name"),
        IsDiscount: $(obj).attr("data-isDiscount"),
        IsService: $(obj).attr("data-isService"),
        Unitid: $(obj).attr("data-unitId"),
        UnitCode: $(obj).attr("data-unitCode"),
        UnitName: $(obj).attr("data-UnitName"),
        Price: $(obj).attr("data-price"),
        Quantity: parseInt(quantity) - 1,
        Tabid: localStorage.getItem("tabid"),
        itemClassId: $(obj).attr("data-subclassid")
    };
    
    if (model.Quantity > 0) {
        $(obj).parent().find('span').html(model.Quantity)
        $(obj).parent().find('a').first().css("display", "block");
        $(obj).parent().find('span').css("display", "block");
    }
    else {
        var shopcart = $(obj).parents().find(".shopCart").attr("class");
        if (shopcart == "shopCart") {
            $(obj).parents("li").remove();
        }
        $(obj).parent().find('span').html("");
        $(obj).parent().find('a').first().css("display", "none");
        $(obj).parent().find('span').css("display", "none");
    }
    AddlocalStorageItem(model); //添加数据
    //$("#item_" + $(obj).attr("data-id")).val();
    refreshItemPrice($(obj).attr("data-id"));
    refreshTotalPrice();

}

function subClassStyle() {
    var subClassList = localStorage.getItem("billDetailList");
    var jsonObj = JSON.parse(subClassList);//转换为json对象
    if (jsonObj != null && jsonObj.length > 0) {
        var subclass = groupSubClass(jsonObj, "itemClassId");
        $(".mui-control-item").each(function () {
            $(this).find("span").first().hide();
        });
        for (var i = 0; i < subclass.length; i++) {
            if (parseInt(subclass[i].data.num) > 0) {
                $("#subClass_" + subclass[i].data.subClassId).find("span").first().text(subclass[i].data.num);
                $("#subClass_" + subclass[i].data.subClassId).find("span").first().show();
            }
            else {
                $("#subClass_" + subclass[i].data.subClassId).find("span").first().hide();
            }

        }
    }
    else {
        $(".mui-control-item").each(function () {
            $(this).find("span").first().hide();
        });
    }
}


//数字变化
function numberChange(obj) {
    $("#itemid").val($(obj).attr("data-id"));
    var model = {
        hid: localStorage.getItem("hid"),
        Itemid: $(obj).attr("data-id"),
        ItemCode: $(obj).attr("data-code"),
        ItemName: $(obj).attr("data-name"),
        IsDiscount: $(obj).attr("data-isDiscount"),
        IsService: $(obj).attr("data-isService"),
        Unitid: $(obj).attr("data-unitId"),
        UnitCode: $(obj).attr("data-unitCode"),
        UnitName: $(obj).attr("data-UnitName"),
        Price: $(obj).attr("data-price"),
        Quantity: parseInt($(obj).val()),
        Tabid: localStorage.getItem("tabid"),
        itemClassId: $(obj).attr("data-subclassid")
    };
    AddlocalStorageItem(model);
    refreshItemPrice($(obj).attr("data-id"));
    refreshTotalPrice();

}

function switchTab(index, item, title) {
    var html1 = '<div id="scroll' + index + '" class="mui-scroll-wrapper">'
        + '<div class="mui-scroll">'
        + '<div class="mui-loading">'
        + '<div class="mui-spinner">'
        + '</div>'
        + '</div>'
        + '</div>'
        + '</div>';

    $(item).html(html1);

    $(item).find('.mui-scroll').html("<div style='width: 100%; text-align: center; padding: 20px;'>" + title + "功能开发中，敬请期待！</div>");
}

//清空购物车
function clearShopCart() {
    localStorage.removeItem("billDetailList");
    localStorage.removeItem("ActionList");
    localStorage.removeItem("GroupList");
    showShopCart();
    refreshTotalPrice();
    subClassStyle();
    var list = JSON.parse(localStorage.getItem("billDetailList"));
    if (list == null || list.length <= 0) {
        $("#segmentedControlContents .mui-control-content").each(function () {

            if ($(this).hasClass("mui-active")) {
                $(this).find("span").each(function () {
                    $(this).html("");
                    $(this).css("display", "none");
                    $(this).prev().css("display", "none");
                });
            }

        });
    }
}

//获取账单明细中最大的行号
function getBillDetailListMaxNum() {
    var list = localStorage.getItem("billDetailList");
    var jsonObj = JSON.parse(list);//转换为json对象
    var arr = new Array();
    if (list != null) {
        for (var i = 0; i < jsonObj.length; i++) {
            arr.push(jsonObj[i].OrderId);
        }

    }
    return Math.max.apply(null, arr);//最大值。
}


/**
     * 将对象集合进行分组。
     * @param source 分组的集合。
     * @param key 分组的属性。
     * @returns 分组结果。
     */
function grouplist(source, key) {
    var map = {},
        dest = [];
    for (var i = 0; i < source.length; i++) {
        var ai = source[i];
        if (!ai[key])
            throw new Error("需要分組的 key 不存在。");

        if (!map[ai[key]]) {
            var model = {
                Igroupid: ai.Igroupid,
                ActionIds: ai.ActionNo,
                ActionNames: ai.ActionName,
                OrderId: ai.OrderId
            };
            dest.push({
                key: ai[key],
                data: model
            });
            map[ai[key]] = ai;
        }
        else {
            for (var j = 0; j < dest.length; j++) {
                var dj = dest[j];
                if (dj.data[key] == ai[key]) {
                    dj.key = ai[key];
                    dj.data.ActionIds += "," + ai.ActionNo;
                    dj.data.ActionNames += "," + ai.ActionName;
                    //dj.data.push(ai);
                    break;
                }

            }
        }
    }

    return dest;
}


function groupSubClass(source, key) {
    var map = {},
        dest = [];
    for (var i = 0; i < source.length; i++) {
        var ai = source[i];
        if (!ai[key])
            throw new Error("需要分組的 key 不存在。");

        if (!map[ai[key]]) {
            var model = {
                subClassId: ai.itemClassId,
                num: Number(ai.Quantity)
            };
            dest.push({
                key: ai[key],
                data: model
            });
            map[ai[key]] = ai;
        }
        else {
            for (var j = 0; j < dest.length; j++) {
                var dj = dest[j];
                if (dj.data[key] == ai[key]) {
                    dj.key = ai[key];
                    dj.data.num += Number(ai.Quantity);
                    //dj.data.push(ai);
                    break;
                }

            }
        }
    }

    return dest;
}

//获取Url参数

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return r[2]; return '';
}

