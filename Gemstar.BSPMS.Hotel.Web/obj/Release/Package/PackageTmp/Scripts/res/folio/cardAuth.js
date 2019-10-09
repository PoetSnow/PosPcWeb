//预授权的js代码
//初始化事件
function folioCardAuthWindow_Initialization() {
    //初始化窗口
    $("#folioCardAuthWindow").kendoWindow({
        width: "800px",
        title: "预授权",
        visible: false,
        modal: true,
        actions: [
            "Close"
        ],
        close: function () { folioMoreDivClose(); }
    });
    //初始化预授权表格组件
    $("#folioCardAuthGrid").kendoGrid({
        columns: [
            { field: "Id", title: "Id", hidden: true, attributes: { "class": "cardauthid"} },
            { field: "RoomNo", title: "房间号", template: "#: $.trim(RoomNo).length > 0 ? RoomNo : '_'+Regid.substring(6) #" },
            { field: "GuestName", title: "客人名" },
            { field: "ItemName", title: "项目" },
            { field: "CardNo", title: "信用卡号" },
            { field: "AuthAmount", title: "授权金额" },
            { field: "BillAmount", title: "扣款金额" },
            { field: "StatuName", title: "状态" }
        ],
        height: 200,
        autoBind: false,
        scrollable: true,
        selectable: "row",
        messages: { "noRecords": "没有可用的记录。" },
        dataSource: {
            type: (function () { if (kendo.data.transports['aspnetmvc-ajax']) { return 'aspnetmvc-ajax'; } else { throw new Error('The kendo.aspnetmvc.min.js script is not included.'); } })(),
            transport: {
                read: { url: FolioCommonValues.AjaxCardAuths, data: function () { return { resId: $("#Resid").val() }; } }
            },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            serverGrouping: true,
            serverAggregates: true,
            schema: {
                data: "Data",
                total: "Total",
                errors: "Errors",
                model: {
                    fields: {
                        "Id": { "type": "string" },
                        "RoomNo": { "type": "string" },
                        "GuestName": { "type": "string" },
                        "ItemName": { "type": "string" },
                        "ItemId": { "type": "string" },
                        "CardNo": { "type": "string" },
                        "AuthNo": { "type": "string" },
                        "AuthAmount": { "type": "number", "defaultValue": null },
                        "BillAmount": { "type": "number", "defaultValue": null },
                        "ValidDate": { "type": "string" },
                        "Remark": { "type": "string" },
                        "StatuName": { "type": "string" },
                        "Status": { "type": "string" },
                        "CreateUser": { "type": "string" },
                        "CreateDate": { "type": "string" },
                        "CompleteUse": { "type": "string" },
                        "CompleteDate": { "type": "string" }
                    }
                }
            }
        },
        change: function (e) {
            var selectedRows = this.select();
            var dataItem = this.dataItem(selectedRows[0]);
            folioCardAuth_tableSelected(dataItem);
        },
        dataBound: function () {
            var isSelect = false;
            var idValue = $("#folioCardAuthId").val();
            if (idValue != null && idValue != undefined && $.trim(idValue).length > 0) {
                var row = $("#folioCardAuthGrid td.cardauthid:contains('" + idValue + "')").parents("tr");
                if (row != null && row != undefined && row.length > 0) {
                    var grid = $("#folioCardAuthGrid").data("kendoGrid");
                    if (grid != null && grid != undefined) {
                        grid.clearSelection();
                        grid.select(row);
                        isSelect = true;
                    }
                }
            }
            if (!isSelect) {
                folioCardAuthAdd_clicked();
            }
        }
    });
    //默认增加一条空记录给用户输入
    folioCardAuthAdd_clicked();
    $("#folioCardAuthAdd").unbind("click").click(function (e) { folioCardAuthAdd_clicked(e); });
    $("#folioCardAuthSave").unbind("click").click(function (e) { folioCardAuthSave_clicked(e); });
    $("#folioCardAuthComplete").unbind("click").click(function (e) { folioCardAuthComplete_clicked(e); });
    $("#folioCardAuthCancel").unbind("click").click(function (e) { folioCardAuthCancel_clicked(e); });
    $("#folioCardAuthClose").unbind("click").click(function (e) { folioCardAuthClose_clicked(e); });
    $("#folioCardAuthItem").data("kendoDropDownList").dataSource.read();
}
//预授权
function folioCardAuthButton_clicked(e) {
    if (e) { e.preventDefault(); }
    if (!$("#folioCardAuthWindow").data("kendoWindow")) { folioCardAuthWindow_Initialization(); }
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来操作预授权");
        return;
    }
    $("#folioCardAuthWindow").data("kendoWindow").center().open();
    folioCardAuthTableRefresh();
    folioCardAuthAdd_clicked();
    $("#folioCardAuthWindow").data("kendoWindow").bind("close", function () { folioCardAuthWindowCloseToRefreshOrderDetail() });
}
//设置加载客账下拉列表数据的参数
function folioResDetailGuestSetPara(e) {
    return {
        resId: $("#Resid").val(),
        rnd: Math.random()
    };
}
//刷新预授权表格数据
function folioCardAuthTableRefresh() {
    $("#folioCardAuthGrid").data("kendoGrid").dataSource.read();
}
//增加按钮点击
function folioCardAuthAdd_clicked(e) {
    if (e) { e.preventDefault(); }
    var currentDate = new Date();
    currentDate = currentDate.ToDateTimeString();
    $("#folioCardAuthId").val("");
    $("#folioCardAuthCardNo").val("");
    $("#folioCardAuthValidDate").data("kendoDatePicker").value("");
    $("#folioCardAuthAuthNo").val("");
    $("#folioCardAuthAuthAmount").data("kendoNumericTextBox").value("");
    $("#folioCardAuthCreateDate").val(currentDate);
    $("#folioCardAuthCreateUser").val(FolioCommonValues.CurrentUserName);
    $("#folioCardAuthBillAmount").data("kendoNumericTextBox").value("");
    $("#folioCardAuthBillAmount").data("kendoNumericTextBox").enable(false);
    $("#folioCardAuthCompleteDate").val("");
    $("#folioCardAuthCompleteUser").val("");
    $("#folioCardAuthStatus").val("授权");
    $("#folioCardAuthRemark").val("");
    $("#folioCardAuthOriginJsonStr").val("");
    $("#folioCardAuthStatuValue").val("1");
    var selectedRegId = folioGetTableSelectedRegId();
    if (selectedRegId != null && selectedRegId != undefined && selectedRegId.length > 0) {
        $("#folioCardAuthRegId").data("kendoDropDownList").value(selectedRegId);
    }
    folioCardAuth_setButtonEnableStatus(true);
}
//表格选中后刷新下方数据显示
function folioCardAuth_tableSelected(dataItem) {
    if (dataItem == null || dataItem == undefined) { return;}
    //给控件赋值
    $("#folioCardAuthId").val(dataItem.Id);
    $("#folioCardAuthRegId").data("kendoDropDownList").value(dataItem.Regid);
    $("#folioCardAuthItem").data("kendoDropDownList").value(dataItem.ItemId);
    $("#folioCardAuthCardNo").val(dataItem.CardNo);
    $("#folioCardAuthValidDate").data("kendoDatePicker").value((dataItem.ValidDate != null && dataItem.ValidDate != undefined && dataItem.ValidDate.length > 0) ? new Date(dataItem.ValidDate) : dataItem.ValidDate);
    $("#folioCardAuthAuthNo").val(dataItem.AuthNo);
    $("#folioCardAuthAuthAmount").data("kendoNumericTextBox").value(dataItem.AuthAmount);
    $("#folioCardAuthCreateDate").val(dataItem.CreateDate);
    $("#folioCardAuthCreateUser").val(dataItem.CreateUser);
    $("#folioCardAuthBillAmount").data("kendoNumericTextBox").value(dataItem.BillAmount);
    $("#folioCardAuthBillAmount").data("kendoNumericTextBox").enable(true);
    $("#folioCardAuthCompleteDate").val(dataItem.CompleteDate);
    $("#folioCardAuthCompleteUser").val(dataItem.CompleteUse);
    $("#folioCardAuthRemark").val(dataItem.Remark);
    $("#folioCardAuthStatus").val(dataItem.StatuName);
    $("#folioCardAuthStatuValue").val(dataItem.Status);

    var clonedData = clone(dataItem, true, true);
    $("#folioCardAuthOriginJsonStr").val(JSON.stringify(clonedData));
    //根据状态禁用或启用保存，完成，取消按钮
    if (dataItem.Status == 1) {
        folioCardAuth_setButtonEnableStatus(true);
    } else {
        folioCardAuth_setButtonEnableStatus(false);
        if (dataItem.Status == 2 && $("#folioCardAuthCompleteStatus").attr("refreshFatherPage") != "false") { $("#folioCardAuthCancel").removeClass("k-state-disabled").removeAttr("disabled"); }
    }
    autoOpenAuthCard_Selected_SetValue(dataItem);
}
//设置按钮的可用状态，参数必须是true或者false
function folioCardAuth_setButtonEnableStatus(enableStatu) {
    var folioCardAuthSave = $("#folioCardAuthSave");
    var folioCardAuthComplete = $("#folioCardAuthComplete");
    var folioCardAuthCancel = $("#folioCardAuthCancel");
    if (enableStatu) {
        folioCardAuthSave.removeClass("k-state-disabled").removeAttr("disabled");
        folioCardAuthComplete.removeClass("k-state-disabled").removeAttr("disabled");
        folioCardAuthCancel.removeClass("k-state-disabled").removeAttr("disabled");
    }
    else {
        folioCardAuthSave.addClass("k-state-disabled").attr("disabled", "disabled");
        folioCardAuthComplete.addClass("k-state-disabled").attr("disabled", "disabled");
        folioCardAuthCancel.addClass("k-state-disabled").attr("disabled", "disabled");
    }
}
//保存按钮点击
function folioCardAuthSave_clicked(e) {
    if (e) { e.preventDefault(); }
    //取出授权id的值，看是否有值来决定是调用增加保存还是修改保存
    var idValue = $("#folioCardAuthId").val();
    if (idValue) {
        folioCardAuthSaveForUpdated();
    } else {
        folioCardAuthSaveForAdded();
    }
}
//保存前的通用数据验证
function folioCardAuthDataValidateBeforeSave() {
    var temp = $("#folioCardAuthRegId").data("kendoDropDownList").value();
    if (!temp) {
        jAlert("请选择账单");
        return false;
    }
    temp = $("#folioCardAuthItem").data("kendoDropDownList").value();
    if (!temp) {
        jAlert("请选择项目");
        return false;
    }
    temp = $("#folioCardAuthCardNo").val();
    if (!temp) {
        jAlert("请输入信用卡卡号");
        return false;
    }
    temp = $("#folioCardAuthValidDate").data("kendoDatePicker").value();
    if (!temp) {
        jAlert("请选择信用卡有效期");
        return false;
    }
    temp = $("#folioCardAuthAuthNo").val();
    if (!temp) {
        jAlert("请输入授权号");
        return false;
    }
    temp = $("#folioCardAuthAuthAmount").data("kendoNumericTextBox").value();
    if (!temp || temp <= 0) {
        jAlert("请输入大于0的授权金额");
        return false;
    }
    return true;
}
//增加保存
function folioCardAuthSaveForAdded() {
    if (folioCardAuthDataValidateBeforeSave()) {
        var validateDate = $("#folioCardAuthValidDate").data("kendoDatePicker").value();
        validateDate = validateDate.ToDateString();
        validateDate = validateDate.substring(0, 7);
        $.post(FolioCommonValues.AddCardAuth,
    {
        RegId: $("#folioCardAuthRegId").data("kendoDropDownList").value(),
        ItemId: $("#folioCardAuthItem").data("kendoDropDownList").value(),
        CardNo: $("#folioCardAuthCardNo").val(),
        ValidDate: validateDate,
        AuthNo: $("#folioCardAuthAuthNo").val(),
        AuthAmount: $("#folioCardAuthAuthAmount").data("kendoNumericTextBox").value(),
        Remark: $("#folioCardAuthRemark").val()
    },
    function (data) {
        if (data.Success) {
            jAlert("保存预授权成功");
            folioCardAuthTableRefresh();
        } else {
            //jAlert("保存预授权失败，原因:" + data.Data);
            ajaxErrorHandle(data);
            return;
        }
    },
        'json');
    }
}
//修改保存
function folioCardAuthSaveForUpdated(callback) {
    if (folioCardAuthDataValidateBeforeSave()) {
        var statuValue = $("#folioCardAuthStatuValue").val();
        var billAmountValue = $("#folioCardAuthBillAmount").data("kendoNumericTextBox").value();
        if (statuValue == 2) {
            if (!billAmountValue || billAmountValue <= 0) {
                jAlert("预授权完成时，扣款金额必须大于0");
                return;
            }
        }
        var validateDate = $("#folioCardAuthValidDate").data("kendoDatePicker").value();
        validateDate = validateDate.ToDateString();
        validateDate = validateDate.substring(0, 7);
        $.post(FolioCommonValues.UpdateCardAuth,
    {
        originJsonStr: $("#folioCardAuthOriginJsonStr").val(),
        Id: $("#folioCardAuthId").val(),
        Regid: $("#folioCardAuthRegId").data("kendoDropDownList").value(),
        Itemid: $("#folioCardAuthItem").data("kendoDropDownList").value(),
        CardNo: $("#folioCardAuthCardNo").val(),
        ValidDate: validateDate,
        AuthNo: $("#folioCardAuthAuthNo").val(),
        AuthAmount: $("#folioCardAuthAuthAmount").data("kendoNumericTextBox").value(),
        BillAmount: billAmountValue,
        Remark: $("#folioCardAuthRemark").val(),
        Status: statuValue,
        isCheckout: $("#folioCardAuthIsCheckout").val(),
    },
    function (data) {
        if (callback) {
            callback(data);
        } else {
            if (data.Success) {
                jAlert("保存预授权成功");
                folioCardAuthTableRefresh();
            } else {
                //jAlert("保存预授权失败，原因:" + data.Data);
                ajaxErrorHandle(data);
                return;
            }
        }
    }, 'json');
    }
}
//完成按钮点击事件
function folioCardAuthComplete_clicked(e) {
    if (e) { e.preventDefault(); }
    //取出授权id的值，看是否有值来决定是调用增加保存还是修改保存
    var idValue = $("#folioCardAuthId").val();
    if (!idValue) {
        jAlert("刚增加的记录不能使用完成功能，请使用保存功能");
        return;
    }
    $("#folioCardAuthStatuValue").val("2");
    folioCardAuthSaveForUpdated(function (data) {
        if (data.Success) {
            jAlert("完成预授权成功");
            folioCardAuthTableRefresh();
            if ($("#folioCardAuthCompleteStatus").attr("refreshFatherPage") == "false") {
                var folioCheckoutAddedTransIds = $("#folioCheckoutAddedTransIds");
                if (folioCheckoutAddedTransIds.val().length <= 0) {
                    folioCheckoutAddedTransIds.val(data.Data);
                } else {
                    folioCheckoutAddedTransIds.val(folioCheckoutAddedTransIds.val() + "," + data.Data);
                }
            } else {
                folioQueryButton_clicked();
            }
        } else {
            jAlert("完成预授权失败，原因:" + data.Data);
            return;
        }
    });
}
//取消按钮点击事件
function folioCardAuthCancel_clicked(e) {
    if (e) { e.preventDefault(); }
    //取出授权id的值，看是否有值来决定是调用增加保存还是修改保存
    var idValue = $("#folioCardAuthId").val();
    if (!idValue) {
        jAlert("刚增加的记录不能使用取消功能，请使用保存功能");
        return;
    }
    $("#folioCardAuthStatuValue").val("51");
    folioCardAuthSaveForUpdated(function (data) {
        if (data.Success) {
            jAlert("取消预授权成功");
            folioCardAuthTableRefresh();
            if ($("#folioCardAuthCompleteStatus").attr("refreshFatherPage") != "false") { folioQueryButton_clicked(); }
        } else {
            jAlert("取消预授权失败，原因:" + data.Data);
            return;
        }
    });
}
//关闭按钮点击
function folioCardAuthClose_clicked(e) {
    if (e) { e.preventDefault(); }
    $("#folioCardAuthWindow").data("kendoWindow").close();
}
//自动弹框预授权
function autoOpenAuthCard(billAmount, cardAuthCloseWindowCallback, notCardAuthCallback, isCheckout) {
    $.post(FolioCommonValues.GetCardAuthIds, { regids: folioGetSelectedRegIdArray().join(',') }, function (result) {
        if (result.Success && result.Data != null && result.Data != undefined && result.Data.length > 0) {
            //有预授权 弹出预授权
            $("#folioCardAuthCompleteStatus").attr("refreshFatherPage", "false");
            $("#folioCardAuthIsCheckout").val(isCheckout);
            var folioCardAuthWindow = $("#folioCardAuthWindow").data("kendoWindow");
            if (!folioCardAuthWindow) {
                folioCardAuthWindow_Initialization();
                folioCardAuthWindow = $("#folioCardAuthWindow").data("kendoWindow");
            }
            folioCardAuthWindow.center().open();
            folioCardAuthWindow.bind("close", function () {
                if (typeof (cardAuthCloseWindowCallback) == "function") {
                    cardAuthCloseWindowCallback();
                }
                $("#folioCardAuthCompleteStatus").attr("refreshFatherPage", "true");
                $("#folioCardAuthIsCheckout").val("");
                $("#folioCardAuthAutoOpenObj").removeAttr("data-ids"); $("#folioCardAuthAutoOpenObj").removeAttr("data-billAmount");
                folioCardAuthWindow.unbind("close");
            });
            $("#folioCardAuthAutoOpenObj").attr("data-ids", result.Data.join(",")); $("#folioCardAuthAutoOpenObj").attr("data-billAmount", billAmount);
            folioCardAuthAdd_clicked();
            var folioCardAuthGrid = $("#folioCardAuthGrid").data("kendoGrid");
            folioCardAuthGrid.bind("dataBound", dataBoundSetValue(result.Data[0], billAmount));
            folioCardAuthGrid.dataSource.read();
        } else {
            //没有预授权 弹出入账
            if (typeof (notCardAuthCallback) == "function") {
                notCardAuthCallback();
            }
        }
    });
}
function dataBoundSetValue(id, billAmount) {
    setTimeout(function () {
        var folioCardAuthGrid = $("#folioCardAuthGrid").data("kendoGrid");
        folioCardAuthGrid.select($("#folioCardAuthGrid td.cardauthid:contains('" + id.toUpperCase() + "')").parents("tr"));
        folioCardAuthGrid.unbind("dataBound", dataBoundSetValue);
    }, 500);
}
function autoSetFolioCardAuthBillAmount(billAmount) {
    var itemObj = $("#folioCardAuthItem").data("kendoDropDownList");
    var itemObjDataItem = itemObj.dataItem(itemObj.select());
    if (itemObjDataItem != null && itemObjDataItem != undefined && itemObjDataItem.Rate != null && itemObjDataItem.Rate != undefined) {
        var rate = parseFloat(itemObjDataItem.Rate);
        if (isNaN(rate) || rate <= 0) { rate = 1.0; }
        billAmount = (billAmount / rate).toFixed(2);
    }
    $("#folioCardAuthBillAmount").data("kendoNumericTextBox").value(billAmount);
}
function autoOpenAuthCard_Selected_SetValue(dataItem) {
    if (dataItem == null || dataItem == undefined) { return; }
    var obj = $("#folioCardAuthAutoOpenObj");
    var ids = obj.attr("data-ids");
    var billAmount = parseFloat(obj.attr("data-billAmount"));
    if (!(ids != null && ids != undefined && ids.length > 0 && !isNaN(billAmount) && billAmount > 0)) { return; }
    if (ids.toLowerCase().indexOf(dataItem.Id.toLowerCase()) != -1 && dataItem.Status == 1) {
        autoSetFolioCardAuthBillAmount(billAmount);
    }
}
//关闭预授权窗口，刷新左侧订单列表
function folioCardAuthWindowCloseToRefreshOrderDetail() {
    folioGuestGrid_refresh();
    $("#folioCardAuthWindow").data("kendoWindow").unbind("close", "folioCardAuthWindowCloseToRefreshOrderDetail");
}