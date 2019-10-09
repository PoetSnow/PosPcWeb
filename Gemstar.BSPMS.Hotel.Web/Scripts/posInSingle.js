

//大类翻页
function classPage() {
    var listWidth = $(".right-top-depart ul").width() - 1;
    var timeWidth = $(".mainRight-time").outerWidth(true);
    var liWidth = $(".right-top-depart li").first().outerWidth(true);
    var size = parseInt((listWidth - liWidth) / liWidth);

    var index = parseInt($("#pageIndexClass").val());
    var total = parseInt($("#pageTotalClass").val());
    var number = (total % size) > 0 ? parseInt(total / size) + 1 : parseInt(total / size);

    if (index < number) {
        index += 1;
        $("#pageIndexClass").val(index);
    } else {
        $("#pageIndexClass").val(1);
    }
    var model = {
        Tabid: $("#tabid").val(),
        Refeid: $("#refeid").val(),
        PageIndex: $("#pageIndexClass").val(),
        PageSize: size
    };
    queryItemClass(model);
}

//根据关键词查询消费项目
function keywordQuery() {
    $(this).keydown(function (e) {
        var key = window.event ? e.keyCode : e.which;
        if (key.toString() == "13") {
            return false;
        }
    });
    model = {
        Refeid: $("#refeid").val(),
        Keyword: $("#txtKeyword").val(),
        PageIndex: $("#pageIndexItem").val(),
        PageSize: getPagingSize() - 2
    };
    queryItem(model);
}

//消费项目翻页
function pageTurningItem(operation) {
    var index = parseInt($("#pageIndexItem").val());
    var total = parseInt($("#pageTotalItem").val());
    var size = getPagingSize() - 2;
    var number = (total % size) > 0 ? parseInt(total / size) + 1 : parseInt(total / size);

    if (Number(operation) === 1) {
        index -= 1;
    }
    else {
        index += 1;
    }

    if (index < 1 || index > number) {
        return false;
    }

    var model = {
        Itemid: $("#itemClassid").val(),
        Refeid: $("#refeid").val(),
        PageIndex: index,
        PageSize: size
    };

    queryItem(model);
}

//是否是分类
function isSubclass(obj) {
    if (tabFlag == 0 && $("#billid").val() === "") {
        //jAlert("请选择要点餐的台号");
        layer.alert("请选择要点餐的台号", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }



    $("#unitid").val("");
    var id = $(obj).attr("data-id");
    $("#rowIndex").val(id);
    var isClass = $(obj).attr("data-isclass");
    if (Boolean(isClass) === true) {
        $("#itemClassid").val(id);
        var model = {
            Itemid: id,
            Refeid: $("#refeid").val(),
            PageIndex: 1,
            PageSize: getPagingSize() - 2
        };

        $("#itemLevel").val($("#itemLevel").val() + "|" + JSON.stringify(model));
        queryItem(model);
        setItemStyle();
    }
    else {
        if (Flag == "2") {  //餐饮模式直接进入入单界面不能操作
            return false;
        }
        if (isHandle == true) {
            //加载层
            var index = layer.msg('处理中...', {
                icon: 0
                , shade: 0.01
            });
            $("#isHandleIndex").val(index);
            setTimeout(function () { layer.close(index); }, 200);
            return false;
        }
        else {
            $("#isHandle").val("true");
            isHandle = true;
        }
        SetlocalStorage();  //清空localStorage的值

        //验证打单之后是否可以修改
        $.ajax({
            url: '/PosManage/PosInSingle/CheckEditForBillRefe ',
            type: "post",
            data: { BillId: $("#billid").val() },
            dataType: "json",
            success: function (data) {
                if (data.Success) {

                    localStorage.setItem("itemCode", $(obj).attr("data-code"));         //项目编码
                    localStorage.setItem("itemName", $(obj).attr("data-name"));         //项目名称    
                    localStorage.setItem("isDiscount", $(obj).attr("data-isDiscount"));  //是否折扣
                    localStorage.setItem("isService", $(obj).attr("data-isService"));       //是否收服务费
                    localStorage.setItem("isHandWrite", $(obj).attr("data-isHandWrite"));   //是否手写
                    localStorage.setItem("isInput", $(obj).attr("data-isInput"));           //是否手工录入
                    localStorage.setItem("iscurrent", $(obj).attr("data-iscurrent"));   //时价菜
                    localStorage.setItem("isSeaFood", $(obj).attr("data-isSeaFood"));   //是否海鲜
                    localStorage.setItem("isWeight", $(obj).attr("data-isWeight"));     //是否称重
                    localStorage.setItem("isQuan", $(obj).attr("data-isquan"));     //是否输入数量

                    $("#itemid").val(id);
                    initItemAction(id);
                    initItemPrice(id);
                    setItemStyle();
                    $("#itagperiod").val("");
                }
                else {
                    if (isHandle == true) {
                        isHandle = false;
                    }
                    layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                }
            },
            error: function (data) {
                layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
            }
        });
        $("#txtKeyword").val("");
        $("#txtKeyword").focus();
    }
}

function ensure() {
    if ($("#cbxCode").is(":checked")) {
        var liList = $(".top-content-list ul").first().find("li a");
        $(liList).each(function () {
            if ($(this).attr("data-code") === $("#txtKeyword").val()) {
                $(this).click();
            }
        });
    }
    else {
        keywordQuery();
    }
}

//返回
function returnParent() {
    var current = $("#itemLevel").val();
    $("#pageIndexItem").val(parseInt($("#pageIndexItem").val()) - 1);
    var levels = current.split('|');

    if (levels.length > 2) {
        var model = JSON.parse(levels[levels.length - 2]);
        queryItem(model);

        var value = "{}";
        for (var i = 1; i < levels.length - 1; i++) {
            value += "|" + levels[i];
        }
        current = $("#itemLevel").val(value);
    }
    else {
        initItemClass();
    }
}

//增加消费项目
function addBillDetail(obj) {
    if (tabFlag == 0 && $("#billid").val() === "") {
        //jAlert("请选择要操作的台号");
        layer.alert("请选择要操作的台号", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }
    // $("#unitid").val($(obj).attr("data-id"));
    localStorage.setItem("unitCode", $(obj).attr("data-code"));
    localStorage.setItem("unitName", $(obj).attr("data-name"));

    var price = parseFloat($(obj).attr("data-price"));
    $("#detailTotal").text(parseFloat($("#detailTotal").text()) + price);

    var model = {
        MBillid: $("#mBillid").val(),
        Billid: $("#billid").val(),
        Itemid: $("#itemid").val(),
        ItemCode: localStorage.getItem("itemCode"),
        ItemName: localStorage.getItem("itemName"),
        IsDiscount: localStorage.getItem("isDiscount"),
        IsService: localStorage.getItem("isService"),
        Unitid: $("#unitid").val(),

        UnitCode: localStorage.getItem("unitCode"),
        UnitName: localStorage.getItem("unitName"),
        Price: $(obj).attr("data-price"),
        Quantity: 1,
        Tabid: $("#tabid").val(),
        itagperiod: $("#itagperiod").val()
    };

    //自定义套餐功能
    var isHandWrite = localStorage.getItem("isHandWrite");
    var isInput = localStorage.getItem("isInput");

    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
        var isUpdateSuite = $("#isUpdateSuite").val();
        if ((data["SP"] == true && data["isUserDefined"] == true) || (isUpdateSuite == "1" && data["SD"] == true) || (isUpdateSuite == "1" && $("#upid").val() != "")) {
            var flag = true;
            if (data["SD"] == true) {
                var allRows = grid.items();
                allRows.each(function () {
                    var itemData = grid.dataItem($(this));
                    if (itemData["SP"] == true && itemData["Upid"] == data["Upid"]) {
                        if (itemData["isUserDefined"] == false) {
                            flag = false;
                        }
                    }
                });
            }
            if ($("#isUpdateSuite").val() == "0") {
                flag = false;
            }
            if (flag) {
                //拿到系统设置的是否显示提示框系统参数
                var isshow = $("#IsShowSetMealMessagebox").val();
                if (isshow == "0") {
                    model.Upid = data["Upid"];
                    addCustomSuiteDetail(model);
                    $("#grid").data("kendoGrid").dataSource.read();
                }
                else {
                    //询问框
                    layer.confirm('是否替换套餐明细？', {
                        btn: ['是', '否'] //按钮
                    }, function () { //是
                        if (data["SP"] == true) {
                            layer.alert("请选择套餐明细进行替换");
                            return false;
                        }
                        model.Id = data["Id"];
                        model.Upid = data["Upid"];
                        layer.closeAll();
                        updateCustomSuiteDetail(model, obj);
                        $("#grid").data("kendoGrid").dataSource.read();
                    }, function () { //否     
                        model.Upid = data["Upid"];
                        addCustomSuiteDetail(model);
                        $("#grid").data("kendoGrid").dataSource.read();
                    });
                }
            }
            else {
                insBillDetailDataBase(model);
            }
            return false;
        }
    }
    /*判断当前选择的消费项目是否是时价菜以及设置的价格是否大于0
        是时价菜以及没有设置价格的直接弹出输入价格的窗口
        */

    if (localStorage.getItem("iscurrent") == "True" && $(obj).attr("data-price") <= 0) {
        $.ajax({
            url: '/PosManage/PosInSingle/_PriceNumberByAddItem',
            type: "post",
            data: model,
            datatype: "html",
            success: function (dataResult) {
                var boolJson = isJson(dataResult);//判断是否为json格式
                if (boolJson) { //如果是json 格式
                    if (dataResult.Success == false) {
                        if (isHandle == true) {
                            isHandle = false;
                        }
                        layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }
                }
                layer.open({
                    type: 1,
                    title: "输入价格",
                    closeBtn: 0, //不显示关闭按钮
                    area: ['300px', '320px'], //宽高
                    content: dataResult
                });
            },
            error: function (dataResult) {
                if (isHandle == true) {
                    isHandle = false;
                }
                layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: 'err' });
            }
        });
    }
    else if (localStorage.getItem("isSeaFood") == "True" && localStorage.getItem("isWeight") == "True") {
        model.WeighFlag = "A";  //用于区分是入单界面还是海鲜池界面
        //如果是海鲜并且称重 弹出称重界面
        $.ajax({
            url: '/PosManage/PosInSingle/_WeighInput',
            type: "post",
            data: model,
            datatype: "html",
            success: function (dataResult) {
                var boolJson = isJson(dataResult);//判断是否为json格式
                if (boolJson) { //如果是json 格式
                    if (dataResult.Success == false) {
                        if (isHandle == true) {
                            isHandle = false;
                        }
                        layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }
                }
                layer.open({
                    type: 1,
                    title: "称重",
                    closeBtn: 0, //不显示关闭按钮
                    area: ['auto', '230px'], //宽高
                    content: dataResult
                });
            },
            error: function (dataResult) {
                if (isHandle == true) {
                    isHandle = false;
                }
                layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: 'err' });
            }
        });
    }
    else if (localStorage.getItem("isQuan") == "True") {
        var jsonStr = JSON.stringify(model);
        var modelA = {
            Title: "数量：",
            Message: "请输入数量",
            ID: -1,
            Value: -1,
            Callback: "AddBillDetailQuan('" + jsonStr + "', $('#txtNumberValue').val())",
        };
        //是否输入数量
        $.ajax({
            url: '/PosManage/PosInSingle/_NumberInput',
            type: "post",
            data: modelA,
            datatype: "html",
            success: function (dataResult) {
                var boolJson = isJson(dataResult);//判断是否为json格式
                if (boolJson) { //如果是json 格式
                    if (dataResult.Success == false) {
                        if (isHandle == true) {
                            isHandle = false;
                        }
                        layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }
                }
                layer.open({
                    type: 1,
                    title: "",
                    closeBtn: 0, //不显示关闭按钮
                    area: ['300px', '390px'], //宽高
                    content: dataResult
                });
            },
            error: function (dataResult) {
                if (isHandle == true) {
                    isHandle = false;
                }
                layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: 'err' });
            }
        });

    }
    else {
        insBillDetailDataBase(model);
    }



}
//输入数量添加账单
function AddBillDetailQuan(jsonStr, num) {
    if (num == "") {
        num = 1;
    }
    var model = JSON.parse(jsonStr);
    model.Quantity = num;
    insBillDetailDataBase(model);
}
//数据库写入数据
function insBillDetailDataBase(model) {
    $.ajax({
        url: '/PosManage/PosInSingle/AddBillDetail',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                if (data.Data != "") {
                    var entity = data.Data;
                    $("#mBillid").val(entity.MBillid);
                    $("#billid").val(entity.Billid);
                    $("#tabid").val(entity.Tabid);
                    $("#iGuest").val(entity.IGuest);
                    getBillInfo();
                }
                $("#grid").data("kendoGrid").dataSource.read();
                getStatistics(1);

            } else {

                if (isHandle == true) {
                    isHandle = false;
                }
                if (data.ErrorCode == "1") {
                    var layindex = layer.confirm(data.Data, {
                        btn: ['确定', '取消'] //按钮
                        , title: '快点云Pos提示'
                        , shade: 'rgba(0,0,0,0)'
                    }, function () {
                        $.ajax({
                            url: '/PosManage/PosInSingle/AddBillDetailB',
                            type: "post",
                            data: model,
                            dataType: "json",
                            success: function (dataB) {
                                if (dataB.Success) {
                                    layer.close(layindex);
                                    $("#grid").data("kendoGrid").dataSource.read();
                                    getStatistics(1);
                                }
                                else {
                                    layer.alert(dataB.Data, { title: "快点云Pos提示", skin: "err" });
                                }

                            },
                            error: function (dataB) {
                                layer.alert(dataB.responseText, { title: "快点云Pos提示", skin: "err" });
                            }
                        });
                    }, function () {

                    });
                    return false;
                }
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            if (isHandle == true) {
                isHandle = false;
            }
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//单位价格翻页
function pricePage() {
    var listWidth = $(".top-content-list ul:nth-child(2)").width() - 1;
    var liWidth = $(".top-content-list ul:nth-child(2) li").first().outerWidth(true);
    var size = parseInt(listWidth / liWidth) - 1;

    var index = parseInt($("#pageIndexPrice").val());
    var total = parseInt($("#pageTotalPrice").val());

    if (parseInt(size) < parseInt(total)) {
        var number = (total % size) > 0 ? parseInt(total / size) + 1 : parseInt(total / size);

        if (index < number) {
            index += 1;
            $("#pageIndexPrice").val(index);
        } else {
            $("#pageIndexPrice").val(1);
        }

        queryItemPrice($("#itemid").val());
    }
}

//选择单位
function selectUnit(obj) {
    //设置单位
    $("#unitid").val($(obj).data('id'));
    addBillDetail(obj);
    setItemPriceStyle($(obj).attr("data-id"));
}

//更新数量
function UpdateQuantity(operation, quantity) {
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJsonCheck) {
            if (dataJsonCheck.Success) {
                var grid = $("#grid").data("kendoGrid");
                var selectedRows = grid.select();
                if (selectedRows.length > 0) {
                    var row = selectedRows[0];
                    var data = grid.dataItem(row);

                    //是否套餐明细
                    if (data["SD"] == true && data["Status"] != "4") {
                        layer.alert("套餐明细不支持修改数量，请修改套餐数量。", { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }
                    if (data["isUserDefined"] == false && data["SD"] == true) {
                        layer.alert("非自定义套餐不能修改套餐明细数量", { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }

                    if (Number(data["Status"]) == 51 || Number(data["Status"]) == 52 || Number(data["Status"]) == 54) {
                        return false;
                    }
                    if (Number(data["Status"]) === 4) {
                        if (Number(operation) === 1) {
                            quantity = parseInt(data["Quantity"]) + quantity;
                        }
                        else if (Number(operation) === 2) {
                            quantity = parseInt(data["Quantity"]) - quantity;
                        }

                        var model = {
                            Id: data["Id"],
                            Quantity: quantity,
                        };
                        $.ajax({
                            url: '/PosManage/PosInSingle/UpdateQuantity',
                            type: "post",
                            data: model,
                            dataType: "json",
                            success: function (dataJson) {
                                if (dataJson.Success) {
                                    $("#grid").data("kendoGrid").dataSource.read();
                                    getStatistics(1);
                                    if (Number(quantity) === 0) {
                                        $("#rowIndex").val(data["Id"]);
                                        var aTag = $(".top-content-list a");
                                        for (var i = 0; i < aTag.length - 2; i++) {
                                            if ($(aTag[i]).attr("data-id") === data["Itemid"]) {
                                                $(aTag[i]).find("span:nth-child(4)").hide();
                                            }
                                        }
                                    }
                                } else {
                                    if (dataJson.ErrorCode == "1") {
                                        var layindex = layer.confirm(dataJson.Data, {
                                            btn: ['确定', '取消'] //按钮
                                            , title: '快点云Pos提示'
                                            , shade: 'rgba(0,0,0,0)'
                                        }, function () {
                                            $.ajax({
                                                url: '/PosManage/PosInSingle/UpdateQuantity',
                                                type: "post",
                                                data: { model: model, Flag: "1" },
                                                dataType: "json",
                                                success: function (dataB) {
                                                    if (dataB.Success) {
                                                        layer.close(layindex);
                                                        $("#grid").data("kendoGrid").dataSource.read();
                                                        getStatistics(1);
                                                        if (Number(quantity) === 0) {
                                                            $("#rowIndex").val(data["Id"]);
                                                            var aTag = $(".top-content-list a");
                                                            for (var i = 0; i < aTag.length - 2; i++) {
                                                                if ($(aTag[i]).attr("data-id") === data["Itemid"]) {
                                                                    $(aTag[i]).find("span:nth-child(4)").hide();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        layer.alert(dataB.Data, { title: "快点云Pos提示", skin: "err" });
                                                    }

                                                },
                                                error: function (dataB) {
                                                    layer.alert(dataB.responseText, { title: "快点云Pos提示", skin: "err" });
                                                }
                                            });
                                        }, function () {

                                        });
                                    }
                                    else {
                                        layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: 'err' });
                                    }

                                }
                            },
                            error: function (dataJson) {
                                layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: 'err' });
                            }
                        });
                    }
                    else {
                        layer.alert("落单的消费项目不支持修改", { title: "快点云Pos提示", skin: 'err' });
                    }
                }
                else {
                    layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
                }
            }
            else {
                layer.alert(dataJsonCheck.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (dataJsonCheck) {
            layer.alert(dataJsonCheck.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//修改数量
function quantityChange(obj) {
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                var grid = $("#grid").data("kendoGrid");
                var selectedRows = grid.select();
                if (selectedRows.length > 0) {
                    var row = selectedRows[0];
                    var data = grid.dataItem(row);

                    //是否套餐明细
                    if (data["SD"] == true && data["Status"] != "4") {
                        layer.alert("套餐明细不支持修改数量，请修改套餐数量。", { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }
                    if (data["isUserDefined"] == false && data["SD"] == true) {
                        layer.alert("非自定义套餐不能修改套餐明细数量", { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }

                    if (Number(data["Status"]) === 4) {
                        var model = {
                            Title: "数量：",
                            Message: "请输入数量",
                            ID: $("#billDetailId").val(),
                            Value: $("#quantity").val(),
                            Callback: "UpdateQuantity(3, $('#txtNumberValue').val());",
                        };

                        $.ajax({
                            url: '/PosManage/PosInSingle/_NumberInput',
                            type: "post",
                            data: model,
                            dataType: "html",
                            success: function (data) {
                                var boolJson = isJson(data);    //判断是否为json 格式
                                if (!boolJson) {
                                    layer.open({
                                        type: 4,
                                        title: false,
                                        closeBtn: 0,
                                        scrollbar: false,
                                        shadeClose: true,
                                        tips: [2, '#00000000'],
                                        content: [data, $(obj)]
                                    });
                                    $('#txtNumberValue').focus();
                                    $(".wrap").css({ "margin": 0 });
                                    $(".layui-layer-tips .layui-layer-content").css({ "overflow": "hidden", "margin": "0", "padding": "0", "background": "rgba(150, 150, 150, 0.9)" });
                                }
                                else {
                                    var jsonData = JSON.parse(data);
                                    if (jsonData.Success == false) {
                                        layer.alert(jsonData.Data, { title: "快点云Pos提示", skin: 'err' });
                                        return false;
                                    }
                                }
                            },
                            error: function (data) {
                                layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                            }
                        });
                    }
                    else {
                        layer.alert("落单的消费项目不支持修改", { title: "快点云Pos提示", skin: 'err' });
                    }
                } else {
                    layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
                }
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//获取原因
function getReasonList(obj, listUrl, totalUrl) {

    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {

                //将此回调方法注册到window对象,方便刷卡界面调用
                window.callBackFun = function (Istagtype, listUrl, totalUrl) {//获取当前data属性0：删除，1：赠送
                    var grid = $("#grid").data("kendoGrid");
                    var selectedRows = grid.select();
                    if (Istagtype == "1") {   //赠送
                        if (selectedRows.length > 0) {
                            var row = selectedRows[0];
                            var data = grid.dataItem(row);
                            //是否套餐明细

                            if (data["SD"] == true && data["SP"] == false) {
                                layer.alert("套餐明细不支持赠送，请取消套餐。", { title: "快点云Pos提示" });
                                return false;
                            }
                        }

                        var size = getPagingSize() - 2;

                        var model = {
                            Istagtype: Istagtype,
                            PageIndex: 1,
                            PageSize: size
                        };

                        queryList(model, listUrl, totalUrl);
                    }
                    else {
                        if (selectedRows.length > 0) {
                            var row = selectedRows[0];
                            var data = grid.dataItem(row);

                            //是否套餐明细
                            if (data["SD"] == true && data["SP"] == false) {
                                if (data["Status"] != 4) {
                                    layer.alert("套餐明细不支持取消，请取消套餐。", { title: "快点云Pos提示", skin: "err" });
                                    return false;
                                }

                            }
                            //状态为保存或者赠送的时候
                            if (data["Status"] == 4) {
                                var model = {
                                    Id: data["Id"],
                                    status: data["Status"],
                                    CanReason: "",
                                    istagtype: Istagtype,
                                };

                                $.ajax({
                                    url: '/PosManage/PosInSingle/CancelItemB',
                                    type: "post",
                                    data: model,
                                    dataType: "json",
                                    success: function (dataJson) {
                                        if (dataJson.Success) {
                                            $("#grid").data("kendoGrid").dataSource.read();
                                            getStatistics(1);
                                            var aTag = $(".top-content-list a");
                                            for (var i = 0; i < aTag.length - 2; i++) {
                                                if ($(aTag[i]).attr("data-id") === data["Itemid"]) {
                                                    $(aTag[i]).find("span:nth-child(4)").hide();
                                                }
                                            }
                                        } else {
                                            layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: 'err' });
                                        }
                                    },
                                    error: function (dataJson) {
                                        layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: 'err' });
                                    }
                                });
                            }
                            else {
                                var size = getPagingSize() - 2;
                                var model = {
                                    Istagtype: Istagtype,
                                    PageIndex: 1,
                                    PageSize: size
                                };

                                queryList(model, listUrl, totalUrl);
                            }
                        }
                        else {
                            layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
                        }
                    };
                }

                if ($(obj).attr("data") == "1" || data.Data == "1") {//有取消权限直接调用回调方法
                    window.callBackFun($(obj).attr("data"), listUrl, totalUrl);
                } else {
                    var cardModel = {
                        ReturnType: 1,
                        Message: "请刷卡...",
                        Callback: "window['callBackFun'](\"" + $(obj).attr("data") + "\",\"" + listUrl + "\",\"" + totalUrl + "\")"//调用全局的callBackFun方法
                    };

                    //刷卡取消消费项目
                    $.ajax({
                        url: '/PosManage/Shared/_PayByCardForCancel',
                        type: "post",
                        data: cardModel,
                        dataType: "html",
                        success: function (cardData) {
                            layer.open({
                                type: 1,
                                title: "当前操作员权限不足",
                                skin: 'layui-layer-demo', //样式类名
                                closeBtn: 0, //不显示关闭按钮
                                area: ['auto', 'auto'], //宽高
                                content: cardData
                            });
                        },
                        error: function (cardData) {
                            layer.alert(cardData.responseText, { title: "快点云Pos提示" });
                        }
                    });
                }
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });

    //  queryItemPrice(0);
}

//手写原因
function handReason(url) {
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
        //如果项目取消，不做任何操作
        if (data["Status"] > 50) {
            return false;
        }
        localStorage.setItem("Status", data["Status"]);

        localStorage.setItem("Quantity", data["Quantity"]);
        localStorage.setItem("billDetailId", data["Id"]);
        var Quantity = data["Quantity"];    //数量
        if (Quantity != 1) {
            var model = {
                Title: "手写原因：",
                Message: "请输入原因",
                ID: $("#billDetailId").val(),
                Callback: "cancelHandItemByItemNum($('#txtInputValue').val());",
            };
            $.ajax({
                url: url,
                type: "post",
                data: model,
                dataType: "html",
                success: function (dataHtml) {
                    layer.open({
                        type: 4,
                        title: false,
                        closeBtn: 0,
                        scrollbar: false,
                        shadeClose: true,
                        area: ['auto', 'auto'],
                        tips: [2, '#000000'],
                        content: [dataHtml, $("#AddHandWrite")]
                    });
                },
                error: function (data) {
                    layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                }
            });
        }
        else {
            var model = {
                Title: "手写原因：",
                Message: "请输入原因",
                ID: $("#billDetailId").val(),
                Callback: "cancelHandItem($('#txtInputValue').val());",
            };
            $.ajax({
                url: url,
                type: "post",
                data: model,
                dataType: "html",
                success: function (dataHtml) {
                    layer.open({
                        type: 4,
                        title: false,
                        closeBtn: 0,
                        scrollbar: false,
                        shadeClose: true,
                        area: ['auto', 'auto'],
                        tips: [2, '#fff'],
                        content: [dataHtml, $("#AddHandWrite")]
                    });
                },
                error: function (data) {
                    layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                }
            });
        }
    }
}

function cancelHandItemByItemNum(val) {
    localStorage.setItem("canReason", val);
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
    }
    var model = {
        id: data["Id"],
        quantity: data["Quantity"],
        itemName: data["ItemName"],
        Callback: "CancelItemByNumber( $('#txtNumberValueByCancelItem').val())"
    };
    $.ajax({
        url: '/PosManage/PosInSingle/_NumberInputByPosIn',
        type: "post",
        data: model,
        dataType: "html",
        success: function (data) {
            layer.open({
                type: 1,
                scrollbar: false,//禁止滚动条
                closeBtn: 0, //不显示关闭按钮
                shadeClose: true, //开启遮罩关闭
                area: ['20rem', '31rem'], //宽高
                maxWidth: "75%",
                maxHeight: "75%",
                content: data
            });
            $('#txtNumberValueByCancelItem').focus();
            $(".wrap").css({ "margin": 0 });
            $(".layui-layer-tips .layui-layer-content").css({ "overflow": "hidden", "margin": "0", "padding": "0", "background": "rgba(150, 150, 150, 0.9)" });
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//更新手写原因
function cancelHandItem(obj) {
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);

        var model = {
            Id: data["Id"],
            CanReason: obj,
            istagtype: 0,
        };

        $.ajax({
            url: '/PosManage/PosInSingle/CancelItem',
            type: "post",
            data: model,
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    $("#grid").data("kendoGrid").dataSource.read();
                    getStatistics(1);
                } else {
                    layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                }
            },
            error: function (data) {
                layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
            }
        });
    }
    else {
        // jAlert("请选择要操作的消费项目");
        layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
    }
}

//更新原因
function cancelItem(obj) {
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);

        //如果项目取消，不做任何操作
        if (data["Status"] > 50) {
            return false;
        }
        //点击是赠送原因，并且项目不可以赠送
        if (data["isLargess"] == false && $(obj).attr("data-istagtype") == "1") {
            layer.alert(data["ItemName"] + "不可以赠送", { title: "快点云Pos提示", skin: 'err' });
            return false;
        }
        localStorage.setItem("Status", data["Status"]);
        localStorage.setItem("canReason", $(obj).attr("data-name"));
        localStorage.setItem("istagtype", $(obj).attr("data-istagtype"));
        localStorage.setItem("isreuse", $(obj).attr("data-isreuse"));
        localStorage.setItem("Quantity", data["Quantity"]);
        localStorage.setItem("billDetailId", data["Id"]);
        var Quantity = data["Quantity"];    //数量
        //数量不等于1
        if (Quantity != 1) {
            //已经是赠送的项目再次点击同样的取消原因则取消赠送状态
            if (data["Status"] == "2" && $(obj).attr("data-istagtype") == "1" && data["CanReason"] == $(obj).attr("data-name")) {
                var model = {
                    id: data["Id"],
                    status: data["Status"],
                    canReason: $(obj).attr("data-name"),
                    istagtype: $(obj).attr("data-istagtype"),
                    isreuse: $(obj).attr("data-isreuse"),
                };
                $.ajax({
                    url: '/PosManage/PosInSingle/CancelItemA',
                    type: "post",
                    data: model,
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {
                            $("#grid").data("kendoGrid").dataSource.read();
                            getStatistics(1);
                            setCanReasonStyle();
                        } else {
                            layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                        }
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
                return false;
            }
            var model = {
                id: data["Id"],
                quantity: data["Quantity"],
                itemName: data["ItemName"],
                Callback: "CancelItemByNumber( $('#txtNumberValueByCancelItem').val())"
            };
            $.ajax({
                url: '/PosManage/PosInSingle/_NumberInputByPosIn',
                type: "post",
                data: model,
                dataType: "html",
                success: function (data) {
                    layer.open({
                        type: 1,
                        scrollbar: false,//禁止滚动条
                        closeBtn: 0, //不显示关闭按钮
                        shadeClose: true, //开启遮罩关闭
                        area: ['20rem', '31rem'], //宽高
                        maxWidth: "75%",
                        maxHeight: "75%",
                        content: data
                    });
                    $('#txtNumberValueByCancelItem').focus();
                    $(".wrap").css({ "margin": 0 });
                    $(".layui-layer-tips .layui-layer-content").css({ "overflow": "hidden", "margin": "0", "padding": "0", "background": "rgba(150, 150, 150, 0.9)" });
                },
                error: function (data) {
                    layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                }
            });
        } else {
            var model = {
                id: data["Id"],
                status: data["Status"],
                canReason: $(obj).attr("data-name"),
                istagtype: $(obj).attr("data-istagtype"),
                isreuse: $(obj).attr("data-isreuse"),
            };
            var url = model.istagtype == "0" ? "/PosManage/PosInSingle/CancelItemB" : "/PosManage/PosInSingle/CancelItemA";
            $.ajax({
                url: url,
                type: "post",
                data: model,
                dataType: "json",
                success: function (data) {
                    if (data.Success) {
                        $("#grid").data("kendoGrid").dataSource.read();
                        getStatistics(1);

                        setCanReasonStyle();
                    } else {
                        layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                    }
                },
                error: function (data) {
                    layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                }
            });
        }
    }
    else {
        //  jAlert("请选择要操作的消费项目");
        layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
    }
}

//取消，赠送输入数量的回调函数
function CancelItemByNumber(val) {
    var Quantity = localStorage.getItem("Quantity");
    if (Number(val) <= 0 || isNaN(val)) {
        layer.alert("输入的数量不能为空或者小于等于0", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }
    if (Number(val) > Number(Quantity)) {
        layer.alert("输入的数量大于项目数量", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }

    var model = {
        status: localStorage.getItem("Status"),
        canReason: localStorage.getItem("canReason"),
        istagtype: localStorage.getItem("istagtype"),
        isreuse: localStorage.getItem("isreuse"),
        CancelNum: val,
        Id: localStorage.getItem("billDetailId")
    };
    $.ajax({
        url: '/PosManage/PosInSingle/CancelItemByNumber',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                layer.closeAll();
                $("#grid").data("kendoGrid").dataSource.read();
                getStatistics(1);

                setCanReasonStyle();
            } else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//获取要求
function getRequestList(obj, listUrl, totalUrl) {
    var size = getPagingSize() - 3;

    var model = {
        PageIndex: 1,
        PageSize: size,
        Refeid: $("#refeid").val()
    };
    queryList(model, listUrl, totalUrl);
}

//更新要求
function updateRequest(obj) {
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                var grid = $("#grid").data("kendoGrid");
                var selectedRows = grid.select();
                if (Number($(obj).attr("data-iTagOperator")) === 0) {
                    if (selectedRows.length === 0) {
                        layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
                        return false;
                    }
                    var row = selectedRows[0];
                    var data = grid.dataItem(row);
                    //添加判断。取消状态不做任何操作
                    if (data["Status"] > 50) {
                        return false;
                    }
                    var model = {
                        Id: data["Id"],
                        Billid: $("#billid").val(),
                        Request: $(obj).attr("data-name"),
                    };
                }
                else {
                    model = {
                        Id: "",
                        Billid: $("#billid").val(),
                        Request: $(obj).attr("data-name"),
                    };
                }
                $.ajax({
                    url: '/PosManage/PosInSingle/UpdateRequest?requestid=' + $(obj).attr("data-id"),
                    type: "post",
                    data: model,
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {
                            $("#grid").data("kendoGrid").dataSource.read();
                        } else {
                            layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                        }
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//获取作法类型
function getActionTypeList(obj, listUrl, totalUrl) {
    var size = getPagingSize() - 4;

    var model = {
        Itemid: $("#itemid").val(),
        Istagtype: $(obj).attr("data"),
        PageIndex: 1,
        PageSize: size,
        Refeid: $("#refeid").val()
    };

    queryList(model, listUrl, totalUrl);
}

//获取作法
function getActionList(obj, listUrl, totalUrl) {
    var size = getPagingSize() - 2;

    var model = {
        ActionTypeId: $(obj).attr("data-id"),
        PageIndex: 1,
        PageSize: size
    };

    queryList(model, listUrl, totalUrl);
}

//更新账单明细作法
function updateBillDetailAction(obj, url) {
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                var grid = $("#grid").data("kendoGrid");
                var selectedRows = grid.select();

                if (selectedRows.length === 0) {
                    // jAlert("请选择要操作的消费项目");
                    layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
                    return false;
                }

                var row = selectedRows[0];
                var dataRow = grid.dataItem(row);
                //如果是取消的项目，不做任何操作
                if (dataRow["Status"] > 50) {
                    return false;
                }
                $("#rowIndex").val(dataRow["Id"]);
                var model = {
                    ActionId: $(obj).attr("data-id"),
                    MBillid: $("#mBillid").val(),
                    Mid: dataRow["Id"],
                    Quan: dataRow["Quantity"],
                    DeptClassid: dataRow["DeptClassid"],
                    IGuest: $("#iGuest").val(),
                    Igroupid: $("#igroupid").val(),
                    ActionType: $(obj).attr("data-type")
                };

                $.ajax({
                    url: url,
                    type: "post",
                    data: model,
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {
                            $("#grid").data("kendoGrid").dataSource.read();
                            $("#igroupid").val(data.Data);
                            openActionGroup();
                            getStatistics(1);

                            initActionMultisub($(obj).attr("data-id"));
                            //getActionGroup('/PosManage/PosInSingle/GetActionPageIndex', '/PosManage/PosInSingle/_ActionList', '/PosManage/PosInSingle/GetActionTotal')
                        } else {
                            layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                        }
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}
//修改消费项目对应的做法
function UpdateBillActionForItem(obj) {
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                var grid = $("#grid").data("kendoGrid");
                var selectedRows = grid.select();

                if (selectedRows.length === 0) {
                    // jAlert("请选择要操作的消费项目");
                    layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", skin: 'err' });
                    return false;
                }

                var row = selectedRows[0];
                var dataRow = grid.dataItem(row);
                //如果是取消的项目，不做任何操作
                if (dataRow["Status"] > 50) {
                    return false;
                }
                $("#rowIndex").val(dataRow["Id"]);
                var model = {
                    ActionId: $(obj).attr("data-id"),
                    MBillid: $("#mBillid").val(),
                    Mid: dataRow["Id"],
                    Quan: dataRow["Quantity"],
                    DeptClassid: dataRow["DeptClassid"],
                    IGuest: $("#iGuest").val(),
                    Igroupid: $("#igroupid").val(),
                    // ActionType: $(obj).attr("data-type")
                };

                $.ajax({
                    url: '/PosManage/PosInSingle/UpdateBillActionForItem',
                    type: "post",
                    data: model,
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {

                            $("#grid").data("kendoGrid").dataSource.read();
                            $("#igroupid").val(data.Data);
                            openActionGroup();
                            getStatistics(1);

                            //    initActionMultisub($(obj).attr("data-id"));
                            //getActionGroup('/PosManage/PosInSingle/GetActionPageIndex', '/PosManage/PosInSingle/_ActionList', '/PosManage/PosInSingle/GetActionTotal')
                        } else {
                            layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                        }
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}



//获取作法分组
function getActionGroup(url, listUrl, totalUrl, obj) {
    $("#itemids").val("");

    selRow();
    var grid = $("#grid").data("kendoGrid");

    //获取当前点击对象的tr 的data-uid 选中对应的tr
    var uid = $(obj).parent().parent().attr("data-uid");

    var foundrow = grid.table.find("tr[data-uid='" + uid + "']");
    grid.select(foundrow);

    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
        $("#rowIndex").val(data["Id"]);
        if (listUrl.indexOf("_ActionTypeList") > -1) {
            var size = getPagingSize() - 3;
        }
        else {
            var size = getPagingSize() - 2;
        }
        // var size = getPagingSize() - 3;
        var model = {
            mid: data["Id"],
            PageSize: size,
            MBillid: $("#mBillid").val(),
        };

        $.ajax({
            url: url,
            type: "post",
            data: model,
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    $("#igroupid").val(data.Data.igroupid);
                    queryList(data.Data, listUrl, totalUrl);
                    openActionGroup();
                } else {
                    layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                }
            },
            error: function (data) {
                layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
            }
        });
    }
}

//获取所选作法分组
function getSelActionGroup(obj, url, listUrl, totalUrl) {
    $("#itemids").val("");
    var size = getPagingSize() - 2;
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
        var model = {
            Mid: data["Id"],
            MBillid: $("#mBillid").val(),
            PageSize: size,
            ActionId: $(obj).attr("data-ids"),
        };
        $.ajax({
            url: url,
            type: "post",
            data: model,
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    queryList(data.Data, listUrl, totalUrl);
                    openActionGroup();
                    selActionGroup(obj);
                } else {
                    layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                }
            },
            error: function (data) {
                layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
            }
        });
    }
}

//选择作法分组
function selActionGroup(obj) {
    $("#itemids").val($(obj).attr("data-ids"));
    $("#igroupid").val($(obj).attr("data-igroupid"));
    $("#openActionGroup .content-list-detail").attr("style", "");
    $(obj).parents("li").first().css({ "background-color": "#ffc500" });
    setItemStyle();
}

//新建作法分组
function newActionGroup(obj) {
    $("#igroupid").val("");
    $("#openActionGroup .content-list-detail").attr("style", "");
    // $("#openActionGroup .content-list-detail").css({ "background-color": "rgba(200, 235, 255, 0.1);" });
    $(obj).parents("li").first().css({ "background-color": "#ffc500" });
    setItemStyle();
}

//获取折扣类型
function getDiscTypeList(listUrl, totalUrl, discType) {
    $("#discType").val(discType);//给折扣类型赋值
    var size = getPagingSize() - 2;

    var model = {
        PageIndex: 1,
        PageSize: size,
        discType: discType
    };
    queryList(model, listUrl, totalUrl);
}

//更新折扣类型
function updateDiscType(obj, url) {
    if ($("#billid").val() === "") {
        layer.alert("请选择要操作的台号", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                //全单折，照单折
                var model = {
                    Billid: $("#billid").val(),
                    Discount: $(obj).attr("data-discount"),
                    IsForce: $(obj).attr("data-type") //折扣类型
                };
                $.ajax({
                    url: url,
                    type: "post",
                    data: model,
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {
                            $("#grid").data("kendoGrid").dataSource.read();
                            getStatistics(1);
                        } else {
                            layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                        }
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//编辑开台信息
function editOpenTab(url, callback) {
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                if ($("#billid").val() === "") {
                    layer.alert("请选择要操作的台号", { title: "快点云Pos提示", skin: 'err' });
                    return false;
                }

                var model = {
                    Billid: $("#billid").val(),
                    Refeid: $("#refeid").val(),
                };
                $.ajax({
                    url: url,
                    type: "post",
                    data: model,
                    dataType: "html",
                    success: function (data) {
                        var boolJson = isJson(data);//验证返回的值是否是json格式
                        if (boolJson) {
                            var obj = JSON.parse(data);
                            if (obj.Success == false) {
                                layer.alert(obj.Data, { title: "快点云Pos提示", skin: 'err' });
                                return false;
                            }
                        }

                        layer.open({
                            type: 1,
                            title: "编辑开台信息",
                            scrollbar: false,//禁止滚动条
                            closeBtn: 0, //不显示关闭按钮
                            shadeClose: true, //开启遮罩关闭
                            area: ['auto', 'auto'], //宽高
                            content: data
                        });

                        $("#saveEditFormButton").click(function (e) {
                            e.preventDefault();
                            saveFormData($("#saveEditFormButton"), 'eidtOpenTab');
                            callback();
                        });
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示" });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//获取餐台列表
function getTabStatusList(listUrl, totalUrl) {
    //封装程序 无餐台 快餐模式
    //台号设置成未买单的单据
    if (getQueryString("mode") == "B") {
        BmodeBillList(0)
    }
    else {
        if (Flag == "2") {
            return false;
        }
        if (Boolean($("#isoutsell").val()) === true) {
            var size = getPagingSize() - 3;
        }
        else {
            size = getPagingSize() - 2;
        }

        var model = {
            Refeid: $("#refeid").val(),
            PageIndex: 1,
            PageSize: size
        };
        if ($("#billid").val().length === 0) {
            model.Refeid = "";
        }
        queryList(model, listUrl, totalUrl);
    }
}

//设置餐台样式
function setTabBackClass() {
    var list = $(".top-content-list ul:first").find("li");
    list.each(function () {
        var status = $(this).attr("data-staus");
        if (Number(status) === 1) {
            $(this).addClass("staus-clean");
        }
        else if (Number(status) === 2) {
            $(this).addClass("staus-intimidate");
        }
        else if (Number(status) === 3) {
            $(this).addClass("staus-beAlone");
        }
        else if (Number(status) === 4) {
            $(this).addClass("staus-sit");
        }
        else if (Number(status) === 5) {
            $(this).addClass("staus-reserve");
        }
        else if (Number(status) === 6) {
            $(this).addClass("staus-repair");
        }
        else if (Number(status) === 7) {
            $(this).addClass("staus-empty");
        }
    });
}

//选择行
function selRow() {
    var grid = $("#grid").data("kendoGrid");
    var dataRows = grid.items();
    for (var i = 0; i < dataRows.length; i++) {
        var row = grid.tbody.find(">tr:not(.k-grouping-row)").eq(i);
        var dataRow = grid.dataItem(dataRows[i]);

        row.removeClass();
        //根据状态修改背景色
        if (Number(dataRow["Status"]) === 0) {
            row.addClass("normal-state");
        }
        else if (Number(dataRow["Status"]) === 1) {
            row.addClass("case-state");
        }
        else if (Number(dataRow["Status"]) === 2) {
            row.addClass("give-state");
        }
        else if (Number(dataRow["Status"]) === 4) {
            row.addClass("neworder-state");
        }
        else if (Number(dataRow["Status"]) === 51 || Number(dataRow["Status"]) === 52) {
            row.addClass("cancel-state");
        }

        //是否套餐明细
        if (dataRow["SD"] == true) {
            row.removeClass();
            row.addClass("package-state");
            if (dataRow["isUserDefined"] == true) {
                if ($("#isUpdateSuite").val() == 0) {
                    $("#initSuite").hide();
                    $("#finishSuite").hide();
                    $("#updateSuite").show();
                }
            }


            row.find(".tdrow").css("padding-left", "17px")
        }

        if (Number(dataRow["Isauto"]) === 2) {
            row.addClass("original-state");
        }

        if (Number($("#rowIndex").val()) === Number(dataRow["Id"]) || $("#rowIndex").val() === dataRow["Itemid"]) {
            grid.select(row);
        }

        if (Number(dataRow["Isauto"]) === 2) {
            row.addClass("original-state");
        }

        if (Number(dataRow["sentStatus"]) === 1) {
            row.addClass("dish-state");
        }

        //样式调整
        $.ajax({
            url: '/PosManage/PosInSingle/GetBillDetailLisrForBillID',
            type: "post",
            data: { BillId: $("#billid").val() },
            dataType: "json",
            success: function (data) {
                $("#itemids").val(data.Data);
                if (isHandle == true) {
                    isHandle = false;
                }
            },
            error: function (data) {
                if (isHandle == true) {
                    isHandle = false;
                }
                layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
            }
        });
    }

    var selectedRows = grid.select();
    if (selectedRows.length == 0) {
        var row = grid.tbody.find(">tr:not(.k-grouping-row)").eq(dataRows.length - 1);
        grid.select(row);
        row.trigger("click");
    }
    //双击单据明细，弹出是否上菜 zk 20190430
    var grid = $("#grid").data("kendoGrid");
    grid.tbody.find(">tr:not(.k-grouping-row)").on('dblclick', function (e) {
        var thisrow = grid.dataItem($(this));
        if (thisrow.Status == 4) {
            layer.alert("请先落单", { title: "快点云Pos提示", btn: ['确定', '关闭'] });
            return false;
        }
        if (!thisrow.sentStatus || thisrow.sentStatus == 0) {
            layer.confirm('是否已上菜', {
                btn: ['是', '否'] //按钮
                , title: '快点云Pos提示'
                , shade: 'rgba(0,0,0,0)'
            }, function () {
                //验证打单之后是否可以修改
                $.ajax({
                    url: '/PosManage/PosInSingle/CheckEditForBillRefe',
                    type: "post",
                    data: { BillId: $("#billid").val() },
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {
                            //成功之后的操作
                            //更新明细状态
                            $.ajax({
                                url: '/PosManage/PosInSingle/UpdatePosBillDetailStatus',
                                type: "post",
                                data: { detailid: thisrow.Id, sentstatus: 1 },
                                dataType: "json",
                                success: function (data) {
                                    layer.alert('已上菜', { title: "快点云Pos提示" }, function () { $("#grid").data("kendoGrid").dataSource.read(); layer.closeAll(); });
                                },
                                error: function (data) {
                                    layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                                }
                            });
                        }
                        else { layer.alert(data.Data, { title: "快点云Pos提示" }); }
                    },
                    error: function (data) {
                        layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
            }, function () {
                layer.closeAll();
            });
        }
        else {
            layer.confirm('是否取消已上菜', {
                btn: ['是', '否'] //按钮
                , title: '快点云Pos提示'
                , shade: 'rgba(0,0,0,0)'
            }, function () {
                //验证打单之后是否可以修改
                $.ajax({
                    url: '/PosManage/PosInSingle/CheckEditForBillRefe',
                    type: "post",
                    data: { BillId: $("#billid").val() },
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {
                            //成功之后的操作
                            //更新明细状态
                            $.ajax({
                                url: '/PosManage/PosInSingle/UpdatePosBillDetailStatus',
                                type: "post",
                                data: { detailid: thisrow.Id, sentstatus: 0 },
                                dataType: "json",
                                success: function (data) {
                                    layer.alert('已取消上菜', { title: "快点云Pos提示" }, function () { $("#grid").data("kendoGrid").dataSource.read(); layer.closeAll(); });
                                },
                                error: function (data) {
                                    layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                                }
                            });
                        }
                        else { layer.alert(data.Data, { title: "快点云Pos提示" }); }
                    },
                    error: function (data) {
                        layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
            }, function () {
                layer.closeAll();
            });
        }
    });
}
function setlocalStorageValue() {
    layer.closeAll();
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();

    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
        localStorage.setItem("itemCode", data["ItemCode"]);
        localStorage.setItem("itemName", data["ItemName"]);
        localStorage.setItem("isDiscount", data["isDiscount"]);
        localStorage.setItem("isService", data["isService"]);
        initItemAction(data["Itemid"]);
    }
    //点击菜式设置赠送样式
    setCanReasonStyle();
}

//打开先落选择窗口
function openItemSelect() {
    //if (Flag=="2") {
    //    return false;
    //}
    if ($("#billid").val() === "" && Flag == "2") {
        layer.alert("请选择要操作的台号", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }

    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                var model = {
                    Billid: $("#billid").val(),
                };

                $.ajax({
                    url: '/PosManage/PosInSingle/_ItemListSelect',
                    type: "post",
                    data: model,
                    dataType: "html",
                    success: function (data) {
                        var boolJson = isJson(data);
                        if (boolJson) {
                            var obj = JSON.parse(data);
                            if (obj.Success == false) {
                                layer.alert(obj.Data, { title: "快点云Pos提示", skin: "err" });
                                return false;
                            }
                        }
                        layer.open({
                            type: 1,
                            title: "选择先落的消费项目",
                            scrollbar: false,//禁止滚动条
                            closeBtn: 0, //不显示关闭按钮
                            shadeClose: true, //开启遮罩关闭
                            area: ['600px', 'auto'], //宽高
                            maxWidth: "75%",
                            maxHeight: "75%",
                            content: data
                        });
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//先落全选
function checkedRowAll(obj) {
    var ids = "";
    $("#selIds").val("");
    var grid = $("#gridSelect").data("kendoGrid");

    if ($(obj).is(":checked")) {
        grid.tbody.find(">tr:not(.k-grouping-row) input[type='checkbox']").prop("checked", true);

        var dataRows = grid.items();
        for (var i = 0; i < dataRows.length; i++) {
            var row = grid.tbody.find(">tr:not(.k-grouping-row)").eq(i);
            var data = grid.dataItem(dataRows[i]);
            ids += "|" + data["Id"];
        }
        $("#selIds").val(ids);
    }
    else {
        grid.tbody.find(">tr:not(.k-grouping-row) input[type='checkbox']").prop("checked", false);
    }
}

//先落单选
function checkedRow(obj) {
    var ids = $("#selIds").val();
    var grid = $("#gridSelect").data("kendoGrid");

    if ($(obj).is(":checked")) {
        $("#selIds").val(ids + "|" + $(obj).val());

        var dataRows = grid.items();
        for (var i = 0; i < dataRows.length; i++) {
            var row = grid.tbody.find(">tr:not(.k-grouping-row)").eq(i);
            var data = grid.dataItem(dataRows[i]);
        }
    }
    else {
        ids = "";
        var arr = $.grep(ids.split('|'), function (value) {
            return value !== $(obj).val();
        });

        dataRows = grid.items();
        for (i = 0; i < dataRows.length; i++) {
            row = grid.tbody.find(">tr:not(.k-grouping-row)").eq(i);
            data = grid.dataItem(dataRows[i]);
            for (var j = 0; j < arr.length; j++) {
                if (data["Id"] === arr[j]) {
                    ids += "|" + data["Id"];
                }
            }
        }
        $("#selIds").val(ids);
    }
}

//落单
function beAlone() {
    //if (Flag=="2") {
    //    return false;
    //}
    if ($("#billid").val() === "" && Flag == "2") {
        layer.alert("请选择要操作的台号", { title: "快点云Pos提示", btn: ['确定', '关闭'] });
        return false;
    }
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                $.ajax({
                    url: '/PosManage/PosInSingle/beAlone' + "?billid=" + $("#billid").val(),
                    type: "post",
                    dataType: "json",
                    success: function (data) {
                        if (data.Success) {
                            getStatistics(2);
                            $("#grid").data("kendoGrid").dataSource.read();

                            //落单之后 不能修改套餐
                            $("#updateSuite").hide();
                            $("#finishSuite").hide();
                            $("#initSuite").show();

                            $("#upid").val("");
                            $("#isUpdateSuite").val("0");

                            if ("undefined" != typeof jsObject) {   //封装程序
                                if (data.Data.order.length > 0 && data.Data.order != "") {
                                    jsObject.UserName = $("#userName").val();
                                    jsObject.isCanItemPrint = $("#isCanItemPrint").val();
                                    jsObject.PrintReport("PosTheMenu", "TheMenu", JSON.stringify(data.Data.order), false, false);
                                }
                            }
                            if (data.Data.CommitQuit == "1") {
                                //返回上一级界面
                                window.history.go(-1);
                            }
                        } else {
                            layer.alert(data.Data, { title: "快点云Pos提示" });
                        }
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示" });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//买单
function payBillInSingle(url, okUrl, editUrl) {
    if (Flag == "2") {
        return false;
    }
    var model = {
        Billid: $("#billid").val(),
        MBillid: $("#mBillid").val(),
    };
    var fourstatus = 0;
    var gridBillDetail = $("#grid").data("kendoGrid");
    var items = gridBillDetail.items();
    for (var i = 0; i < items.length; i++) {
        var row = gridBillDetail.tbody.find(">tr:not(.k-grouping-row)").eq(i);
        var data = gridBillDetail.dataItem(row);
        if (data != null && data["Status"] == 4) {
            //layer.alert("当前存在未落单的项目，请确认后再进行买单！", { title: "快点云Pos提示", skin: 'err' });
            //return false;
            fourstatus++;
        }
    }
    if (fourstatus > 0) {
        beAlone()
    }
    $.ajax({
        url: '/PosManage/PosInSingle/CheckRefeITagPrintBill',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                $("#isPrintBill").val(data.Data);
                if (Number($("#tabFlag").val()) === 0) {
                    normalPayBill(url, okUrl);
                } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("I") > -1) {
                    virtualPayBill(url, okUrl);
                } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("J") > -1) {
                    manualPayBill(url, okUrl, editUrl);
                } else if (Number($("#tabFlag").val()) === 2) { //外卖台买单
                    normalPayBill(url, okUrl);
                } else if (Number($("#tabFlag").val()) === 1 && getQueryString("mode") == "B" && "undefined" != typeof jsObject) {//快餐模式买单
                    payBillWindow(url, okUrl);
                }
            }
            else {
                if (data.ErrorCode == 1) {
                    layer.confirm(data.Data, {
                        btn: ['继续', '取消'] //按钮
                        , title: '快点云Pos提示'
                        , shade: 'rgba(0,0,0,0)'
                    }, function () {
                        if (Number($("#tabFlag").val()) === 0) {
                            normalPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("I") > -1) {
                            virtualPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("J") > -1) {
                            manualPayBill(url, okUrl, editUrl);
                        } else if (Number($("#tabFlag").val()) === 2) {//外卖台买单
                            normalPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && getQueryString("mode") == "B" && "undefined" != typeof jsObject) {//快餐模式买单
                            payBillWindow(url, okUrl);
                        }
                    }, function () {
                        layer.closeAll();
                    });
                }
                else if (data.ErrorCode == 2) {
                    layer.confirm(data.Data, {
                        btn: ['是', '否'] //按钮
                        , title: '快点云Pos提示'
                        , shade: 'rgba(0,0,0,0)'
                    }, function () {
                        $("#isPrintBill").val(1);//打印账单
                        if (Number($("#tabFlag").val()) === 0) {
                            normalPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("I") > -1) {
                            virtualPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("J") > -1) {
                            manualPayBill(url, okUrl, editUrl);
                        } else if (Number($("#tabFlag").val()) === 2) {//外卖台买单
                            normalPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && getQueryString("mode") == "B" && "undefined" != typeof jsObject) {//快餐模式买单
                            payBillWindow(url, okUrl);
                        }
                    }, function () {
                        $("#isPrintBill").val(0);   //不打印账单
                        if (Number($("#tabFlag").val()) === 0) {
                            normalPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("I") > -1) {
                            virtualPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && $("#openInfo").val().indexOf("J") > -1) {
                            manualPayBill(url, okUrl, editUrl);
                        } else if (Number($("#tabFlag").val()) === 2) {//外卖台买单
                            normalPayBill(url, okUrl);
                        } else if (Number($("#tabFlag").val()) === 1 && getQueryString("mode") == "B" && "undefined" != typeof jsObject) {//快餐模式买单
                            payBillWindow(url, okUrl);
                        }
                    });
                }
                else {
                    layer.alert(data.Data, { title: "快点云Pos提示" });
                }
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}

//正常买单
function normalPayBill(url, okUrl) {
    payBillWindow(url);
}

//自动虚拟台买单
function virtualPayBill(url, okUrl) {
    $.ajax({
        url: '/PosManage/PosInSingle/beAlone' + "?billid=" + $("#billid").val(),
        type: "post",
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                $("#grid").data("kendoGrid").dataSource.read();
                getStatistics(1);

                payBillWindow(url);
            } else {
                layer.alert(data.Data, { title: "快点云Pos提示" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}

//手工指定台买单
function manualPayBill(url, okUrl, editUrl) {
    editOpenTab(editUrl, function () {
        $.ajax({
            url: '/PosManage/PosInSingle/beAlone' + "?billid=" + $("#billid").val(),
            type: "post",
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    $("#grid").data("kendoGrid").dataSource.read();
                    getStatistics(1);

                    payBillWindow(url);
                } else {
                    layer.alert(data.Data, { title: "快点云Pos提示" });
                }
            },
            error: function (data) {
                layer.alert(data.responseText, { title: "快点云Pos提示" });
            }
        });
    });
}

//买单
function payBillWindow(url) {
    var model = {
        Billid: $("#billid").val(),
        MBillid: $("#mBillid").val(),
        Callback: "payBillSuccess('" + $("#billid").val() + "','" + $("#mBillid").val() + "');",
        InputCallback: "payBillSuccess('" + $("#billid").val() + "','" + $("#mBillid").val() + "');",
        OpenFlag: openFlag
    };
    $.ajax({
        url: url,
        type: "post",
        data: model,
        dataType: "html",
        success: function (data) {
            var boolJson = isJson(data);    //判断是否为json 格式
            if (!boolJson) {
                layer.open({
                    type: 1,
                    title: "买单",
                    scrollbar: false,//禁止滚动条
                    closeBtn: 0, //不显示关闭按钮
                    shadeClose: false, //开启遮罩关闭
                    area: ['56rem', '34.5rem'], //宽高
                    maxWidth: "75%",
                    maxHeight: "75%",
                    content: data
                });
            }
            else {
                var jsonData = JSON.parse(data);
                if (jsonData.Success == false) {
                    layer.alert(jsonData.Data, { title: "快点云Pos提示" });
                    return false;
                }
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}

//买单成功
function payBillSuccess(billid, mBillid) {
    $.post("PosCashier/OtherPaymentSuccess", { billid: billid });

    if ($("#isPrintBill").val() != "1") {
        //不打单
        if (Number($("#tabFlag").val()) === 1) {
            if ($("#openInfo").val().indexOf("I") > -1 || $("#openInfo").val().indexOf("J") > -1) {
                //openVirtualTab();
                location.reload();
            }
        }
        if (openFlag == "A") {
            location.href = "/PosManage/PosTabStatus?rnd=" + Math.random();
        }
        else if (openFlag == "B") {
            location.href = "/PosManage/PosCashier/Index";
        }
        else {
            if (window.history.length > 1) {
                window.history.back();
            }
            else {
                location.href = "../Account/Index";
            }
        }
    }
    else {
        var model = {
            ReportCode: "PosBillPrint",
            ProcedureName: "up_pos_print_billDetail",
            ParameterValues: "@h99billid=" + billid + "&@h99mBillid=" + mBillid + "",
            IsOpenSearchWindow: false,
            ChineseName: "账单打印预览",
            StyleName: ""
        };

        $.post('/PosManage/PosInSingle/AddQueryParaTempByPaySusses?print=1&Flag=A&isPrintBill=' + $("#isPrintBill").val(), model, function (result) {
            if (result.Success) {
                if ("undefined" != typeof jsObject) {   //封装程序
                    if (result.Data.ListDetail.length > 0 && result.Data.ListDetail != "") {
                        console.log($("#CurrUserName").val());
                        jsObject.UserName = $("#CurrUserName").val();
                        jsObject.HotelName = $("#CurrHotelName").val();
                        jsObject.PrintReportByPaySusses("PosBillPrintByPay", "up_pos_print_billDetail", result.Data.ListDetail, false, false);
                    }
                }
                else {
                    $("#printFrame").attr("src", result.Data.url);
                }
            } else {
                layer.alert(result.Data, {
                    title: "快点云Pos提示", btn: ['重新打印账单', '关闭'], function() {
                        $.post('/PosManage/PosInSingle/AddQueryParaTempByPaySusses?print=1&Flag=A&isPrintBill=' + $("#isPrintBill").val(), model, function (result) {
                            if (result.Success) {
                                if ("undefined" != typeof jsObject) {   //封装程序
                                    if (result.Data.ListDetail.length > 0 && result.Data.ListDetail != "") {
                                        jsObject.UserName = $("#CurrUserName").val();
                                        jsObject.HotelName = $("#CurrHotelName").val();
                                        jsObject.PrintReportByPaySusses("PosBillPrintByPay", "up_pos_print_billDetail", result.Data.ListDetail, false, false);
                                    }
                                }
                                else {
                                    $("#printFrame").attr("src", result.Data.url);
                                }
                            } else {
                                layer.alert("重打失败，请在[客账查询]进行[打单]", { title: "快点云Pos提示", btn: ['确定', '关闭'] });
                            }
                        }, 'json');
                    }
                });
            }
        }, 'json');
    }
}

//退出
function exitInSingle(openFlag, Flag) {
    if (Flag == "2") {
        if (openFlag == "A") {
            location.href = "/PosManage/PosTabStatus?rnd=" + Math.random();
        }
        else if (openFlag == "B" || openFlag == "C") {
            location.href = "/PosManage/PosCashier/Index";
        }
        else {
            if (window.history.length > 1) {
                window.history.back();
            }
            else {
                location.href = "../Account/Index";
            }
        }
    }
    if (openFlag == "D") {  //预订菜谱
        location.href = "/PosManage/PosReserve?rnd=" + Math.random() + "&posId=" + $("#posId").val();
    }
    var model = {
        Tabid: $("#tabid").val(),
        Refeid: $("#refeid").val(),
        Billid: $("#billid").val()
    };

    $.ajax({
        url: '/PosManage/PosInSingle/CancelLockTab',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                if (data.Data == "1") {
                    location.href = "../Account/Index";
                }
                else {
                    if (openFlag == "A") {
                        location.href = "/PosManage/PosTabStatus?rnd=" + Math.random();
                    }
                    else if (openFlag == "B" || openFlag == "C") {
                        location.href = "/PosManage/PosCashier/Index";
                    }
                    else {
                        if (window.history.length > 1) {
                            window.history.back();
                        }
                        else {
                            location.href = "../Account/Index";
                        }
                    }
                }
            } else {
                if (data.ErrorCode == "2") {
                    layer.confirm(data.Data.msg, {
                        btn: ['确定', '取消'] //按钮
                        , title: '快点云Pos提示'
                        , shade: 'rgba(0,0,0,0)'
                    }, function () {
                        //再次落单，并且退出
                        beAlone();
                        if (openFlag == "A") {
                            location.href = "/PosManage/PosTabStatus?rnd=" + Math.random();
                        }
                        else if (openFlag == "B" || openFlag == "C") {
                            location.href = "/PosManage/PosCashier/Index";
                        }
                        else {
                            if (window.history.length > 1) {
                                window.history.back();
                            }
                            else {
                                location.href = "../Account/Index";
                            }
                        }
                    }, function () {
                        //打印保存点菜单
                        if ("undefined" != typeof jsObject) {   //封装程序
                            if (data.Data.jsonOrder != null && data.Data.jsonOrder.length > 0 && data.Data.jsonOrder != "") {
                                jsObject.UserName = $("#userName").val();
                                jsObject.isCanItemPrint = $("#isCanItemPrint").val();
                                jsObject.PrintReport("PosTheMenu", "TheMenu", JSON.stringify(data.Data.jsonOrder), false, false);
                            }
                        }

                        if (openFlag == "A") {
                            location.href = "/PosManage/PosTabStatus?rnd=" + Math.random();
                        }
                        else if (openFlag == "B" || openFlag == "C") {
                            location.href = "/PosManage/PosCashier/Index";
                        }
                        else {
                            if (window.history.length > 1) {
                                window.history.back();
                            }
                            else {
                                location.href = "../Account/Index";
                            }
                        }
                    });
                }
                else {
                    layer.alert(data.Data, { title: "快点云Pos提示", skin: "err" });
                }
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}

//输入
function inputSingle(value) {
    var val = $("#txtKeyword").val();
    if (value.indexOf("退格") === -1 && value.indexOf("清空") === -1) {
        $("#txtKeyword").val(val + value);
    }
    else if (value.indexOf("退格") > -1) {
        $("#txtKeyword").val(val.replace(/.$/, ''));
    }
    else if (value.indexOf("清空") > -1) {
        $("#txtKeyword").val("");
    }
    $("#txtKeyword").trigger("input");
}

//显示提示
function tipShow(obj) {
    layer.tips($(obj).attr("data-msg"), $(obj), {
        tips: [$(obj).attr("data-position"), "#111"], //还可配置颜色
        time: $("#tipsTime").val(),
    });
}

//隐藏提示
function tipHide(obj) {
    layer.closeAll('tips');
}

//提示消息
function tipMsg(obj) {
    layer.msg($(obj).attr("data-msg"), {
        time: $("#tipsTime").val(),
    });
}

//打开窗口
function openWindow(window) {
    if ($("#" + window).data("kendoWindow") != undefined) {
        $("#" + window).data("kendoWindow").center().open();
    }
}

//关闭窗口
function closeWindow(window) {
    if ($("#" + window).data("kendoWindow") != undefined) {
        $("#" + window).data("kendoWindow").close();
    } else {
        layer.closeAll();
    }
}

//换台
function ChangeTable(openFlag) {
    //验证打单之后是否可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                //成功之后的操作
                var model = {
                    Tabid: $("#tabid").val(),
                    Refeid: $("#refeid").val(),
                    Billid: $("#billid").val(),
                    openFlag: openFlag
                }
                $.ajax({
                    url: '/PosManage/PosInSingle/_ChangeTable',
                    type: "post",
                    data: model,
                    dataType: "html",
                    success: function (data) {
                        var boolJson = isJson(data);
                        if (boolJson) {
                            var obj = JSON.parse(data);
                            if (obj.Success == false) {
                                layer.alert(obj.Data, { title: "快点云Pos提示" });
                                return false;
                            }
                        }
                        var windowWidth = $(window).width();
                        var left = windowWidth - 660 - 300;
                        layer.open({
                            type: 1,
                            title: "换台",
                            scrollbar: false,//禁止滚动条
                            closeBtn: 0, //不显示关闭按钮
                            shadeClose: false, //开启遮罩关闭
                            area: ['300px', 'auto'], //宽高
                            content: data,
                            offset: ['auto', left + "px"],
                            id: "changeTab"
                        });
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示" });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}

//添加折扣
function AddDiscTypeA(discType) {
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataCheck) {
            if (dataCheck.Success) {
                var detailIdList = 0;
                if (discType == "4" || discType == "5") {
                    //单道折扣判断是否选择了项目
                    var grid = $("#grid").data("kendoGrid");
                    var selectedRows = grid.select();
                    if (selectedRows.length > 0) {
                        var row = selectedRows[0];
                        var data = grid.dataItem(row);

                        //是否套餐明细
                        if (data["SD"] == true) {
                            layer.alert("套餐明细不支持单道折扣，请设置套餐折扣。", { title: "快点云Pos提示" });
                            return false;
                        }

                        //取消的或者赠送的不计算单道折扣
                        if (Number(data["Status"]) == 51 || Number(data["Status"]) == 52 || Number(data["Status"]) == 54) {
                            return false;
                        }
                        else {
                            var isIsDiscount = data["isIsDiscountStringForItem"];
                            if (isIsDiscount == "否") {
                                layer.alert("选择的消费项目不可打折", { title: "快点云Pos提示" });
                                return false;
                            }
                            detailIdList = data["Id"];
                        }
                    }
                    else {
                        layer.alert("请选择消费项目", { title: "快点云Pos提示" });
                        return false;
                    }
                }
                var ulr = "";
                switch (discType) {
                    case "-1":
                    case "0":
                        ulr = "/PosManage/PosInSingleDiscount/_DiscountNumBerA";
                        break;
                    case "1":
                        ulr = "/PosManage/PosInSingleDiscount/_DiscountNumBerB";
                        break;
                    case "2":
                        ulr = "/PosManage/PosInSingleDiscount/_DiscountNumBerC";
                        break;
                    case "3":
                        ulr = "/PosManage/PosInSingleDiscount/_DiscountNumBerD";
                        break;
                    case "4":
                        ulr = "/PosManage/PosInSingleDiscount/_DiscountNumBerE";
                        break;
                    case "5":
                        ulr = "/PosManage/PosInSingleDiscount/_DiscountNumBerF";
                        break;
                    default:
                        ulr = "/PosManage/PosInSingleDiscount/_DiscountNumBerA";
                        break;
                }
                $.ajax({
                    url: ulr,
                    type: "post",
                    data: { discType: discType, BillId: $("#billid").val(), detailIdList: detailIdList },
                    dataType: "html",
                    success: function (data) {
                        var boolJson = isJson(data);
                        if (boolJson) {
                            var obj = JSON.parse(data);
                            if (obj.Success == false) {
                                layer.alert(obj.Data, { title: "快点云Pos提示", skin: "err" });
                                return false;
                            }
                        }
                        layer.open({
                            type: 1,
                            title: false,
                            closeBtn: 0, //不显示关闭按钮
                            area: ['320px', '400px'], //宽高
                            content: data,
                            shadeClose: true
                        });
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
                    }
                });
            }
            else {
                layer.alert(dataCheck.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (dataCheck) {
            layer.alert(dataCheck.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}

//添加作法
function AddPractice(url) {
    //先验证是否打单可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                var grid = $("#grid").data("kendoGrid");
                var selectedRows = grid.select();
                if (selectedRows.length > 0) {
                    var row = selectedRows[0];
                    var data = grid.dataItem(row);
                    var model =
                    {
                        mid: data["Id"],
                        ItemName: data["ItemName"]
                    };
                    $.ajax({
                        url: url,
                        type: "post",
                        data: model,
                        dataType: "html",
                        success: function (dataHtml) {
                            layer.open({
                                type: 1,
                                title: "手写作法",
                                closeBtn: 0, //不显示关闭按钮
                                area: ['390px', '360px'], //宽高
                                content: dataHtml,
                                shadeClose: false
                            });
                        },
                        error: function (dataHtml) {
                            layer.alert(dataHtml.responseText, { title: "快点云Pos提示", skin: 'err' });
                        }
                    });
                }
                else {
                    layer.alert("请选择消费项目", { title: "快点云Pos提示", skin: 'err' });
                }

            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: 'err' });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}
//添加作法
function AddPracticeItem(val) {
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var dataRow = grid.dataItem(row);
        var model = {
            MBillid: $("#mBillid").val(),
            Mid: dataRow["Id"],
            Quan: dataRow["Quantity"],
            DeptClassid: dataRow["DeptClassid"],
            IGuest: $("#iGuest").val(),
            Igroupid: $("#igroupid").val(),
            HandActionName: val
        };

        $.ajax({
            url: '/PosManage/PosInSingle/UpdateBillDetailHandAction',
            type: "post",
            data: model,
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $("#igroupid").val(data.Data);
                    openActionGroup();
                    getStatistics(1);
                } else {
                    layer.alert(data.Data, { title: "快点云Pos提示" });
                }
            },
            error: function (data) {
                layer.alert(data.responseText, { title: "快点云Pos提示" });
            }
        });
    }
}

//添加要求
function AddRequest(url) {
    //先验证是否打单可以修改
    $.ajax({
        url: '/PosManage/PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                var model = {
                    Title: "手写要求：",
                    Message: "请输入要求",
                    ID: $("#billDetailId").val(),
                    Callback: "AddRequestItem($('#txtInputValue').val());",
                };

                $.ajax({
                    url: url,
                    type: "post",
                    data: model,
                    dataType: "html",
                    success: function (dataHtml) {
                        layer.open({
                            type: 4,
                            title: false,
                            closeBtn: 0,
                            scrollbar: false,
                            shadeClose: true,
                            area: ['auto', 'auto'],
                            tips: [2, '#000000'],
                            content: [dataHtml, $("#AddHandWrite")]
                        });
                    },
                    error: function (dataHtml) {
                        layer.alert(dataHtml.responseText, { title: "快点云Pos提示" });
                    }
                });
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}

//添加要求
function AddRequestItem(val) {
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();

    if (selectedRows.length === 0) {
        layer.alert("请选择要操作的消费项目", { title: "快点云Pos提示", btn: ['确定', '关闭'] });
        return false;
    }
    var row = selectedRows[0];
    var data = grid.dataItem(row);
    //添加判断。取消状态不做任何操作
    if (data["Status"] > 50) {
        return false;
    }
    var model = {
        Id: data["Id"],
        Billid: $("#billid").val(),
        Request: val,
    };
    $.ajax({
        url: '/PosManage/PosInSingle/UpdateBillDetailHandRequest',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                $("#grid").data("kendoGrid").dataSource.read();
            } else {
                layer.alert(data.Data, { title: "快点云Pos提示" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}

//高级功能列表
function GetAdvanceFuncList(listUrl, totalUrl) {
    var size = getPagingSize() - 2;
    var model = {
        Refeid: $("#refeid").val(),
        PageIndex: 1,
        PageSize: size
    };

    queryList(model, listUrl, totalUrl);
}

//清空localStorage存储的值
function SetlocalStorage() {
    localStorage.setItem("itemCode", "");         //项目编码
    localStorage.setItem("itemName", "");         //项目名称    
    localStorage.setItem("isDiscount", "");  //是否折扣
    localStorage.setItem("isService", "");       //是否收服务费
    localStorage.setItem("isHandWrite", "");   //是否手写
    localStorage.setItem("isInput", "");           //是否手工录入
    localStorage.setItem("iscurrent", "");   //时价菜
    localStorage.setItem("isSeaFood", "");   //是否海鲜
    localStorage.setItem("isWeight", "");     //是否称重
    localStorage.setItem("isQuan", "");     //是否输入数量
    $("#unitid").val("");
    $("#itemid").val("");
    localStorage.setItem("unitCode", "");
    localStorage.setItem("unitName", "");
}

// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
Date.prototype.Format = function (fmt) { //author: meizz
    var o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "H+": this.getHours(), //小时
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        "S": this.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

//快餐模式--“台号”按钮事件
function BmodeBillList(operation) {
    $.ajax({
        url: '/PosManage/PosInSingle/GetBModeBillListCount',
        type: "post",
        data: { refeid: $("#refeid").val() },
        dataType: "text",
        success: function (data) {
            $("#pageTotalItem").val(data);
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
    var total = parseInt($("#pageTotalItem").val());
    var size = getPagingSize() - 3;
    var index = parseInt($("#pageIndexItem").val());
    var number = (total % size) > 0 ? parseInt(total / size) + 1 : parseInt(total / size);
    console.log("总数：" + total, "页码:" + index, "数量：" + size, "number：" + number)
    if (Number(operation) == 0) {
        index = 1;
    }
    else if (Number(operation) == 1) {
        index -= 1;
    }
    else {
        index += 1;
    }
    if (index < 1 || index > number) {
        return false;
    }
    console.log("页码:" + index, "数量：" + size)
    $.ajax({
        url: '/PosManage/PosInSingle/_BModeBillList',
        type: "post",
        data: { refeid: $("#refeid").val(), pageIndex: index, pageSize: size },
        dataType: "html",
        success: function (data) {
            var boolJson = isJson(data);
            if (boolJson) {
                var obj = JSON.parse(data);
                if (obj.Success == false) {
                    layer.alert(obj.Data, { title: "快点云Pos提示" });
                    return false;
                }
            }
            $("#pageIndexItem").val(index);
            $(".top-content-list ul").first().remove();
            $(".top-content-list ul").first().before(data);

            var liCount = $(".top-content-list ul").first().find("li").length - 2;

            if ($(".tabStatusList").length > 0) {
                var liCount = $(".top-content-list ul").first().find("li").length - 3;
            }

            var html = "";  //填充空白部分
            if (parseInt(liCount) >= 0 && parseInt(liCount) < parseInt(size)) {
                count = Number(size) - Number(liCount);
                for (var i = 0; i < count; i++) {
                    html += '<li class="content-list-detail" style="background-color: rgba(200, 235, 255, 0.1);background-image: none;"></li>';
                }
                if ($(".tabStatusList").length > 0) {
                    $(".top-content-list ul:first-child li:nth-last-child(2)").before(html);
                }
                else {
                    $(".top-content-list ul:first-child li:nth-child(" + liCount + ")").after(html);
                }
            }
            else if (parseInt(liCount) <= 0) {
                var count = parseInt(size) - liCount;
                for (var i = 0; i < count; i++) {
                    html += '<li class="content-list-detail" style="background-color: rgba(200, 235, 255, 0.1);background-image: none;"></li>';
                }
                if (liCount <= 0) {
                    $(".top-content-list ul:first-child li:nth-child(1)").before(html);
                }
                else {
                    $(".top-content-list ul:first-child li:nth-child(" + liCount + ")").after(html);
                }
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: 'err' });
        }
    });
}
//快餐模式--"+"号，添加新的单据
function AddNewBModeBill() {
    $.ajax({
        url: '/PosManage/PosInSingle/AddNewBModeBill',
        type: "post",
        data: { refeid: $("#refeid").val() },
        dataType: "json",
        success: function (data) {
            $("#tabNo").click();
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示" });
        }
    });
}
//快餐模式--点击对应台号跳转到入单界面
function OpenBModeBill(billid) {
    window.location.href = "http://pos.gshis.com/PosManage/PosInSingle/Index?mode=B&billid=" + billid;
}