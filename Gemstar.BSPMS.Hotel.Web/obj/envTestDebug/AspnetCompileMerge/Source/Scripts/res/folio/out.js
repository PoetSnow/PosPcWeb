//迟付的js
function initfolioDelayPayReasonWindow() {
    //初始化迟付原因窗口
    $("#folioDelayPayReasonWindow").kendoWindow({
        width: "320px",
        title: "迟付",
        visible: false,
        modal: true,
        actions: [
            "Close"
        ]
    });
    //注册迟付确定按钮点击事件
    $("#folioDelayPayConfirmButton").click(function (e) {
        folioDelayPayConfirmButton_clicked(e);
    });
    //注册迟付取消按钮点击事件
    $("#folioDelayPayCancelButton").click(function (e) {
        folioDelayPayCancelButton_clicked(e);
    });
}
//迟付按钮点击事件
function folioDelayPayButton_clicked(e) {
    if (e) {
        e.preventDefault();
    }
    //判断是否有选中至少一条客账明细
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds.length == 0) {
        jAlert("请选择要迟付的房间");
        return;
    }
    //迟付时，长包房提示收取最后一个月房租
    if ($("#folioDelayPayButton").attr("operationContinue") != "true") {
        //var IsMonthRegids = [];
        //var tempOrderList = OrderList.Get();
        //$.each(selectedRegIds, function (index, item) {
        //    $.each(tempOrderList, function (orderIndex, orderItem) {
        //        if (item == orderItem.Regid && orderItem.RateCodeEntity != null && orderItem.RateCodeEntity.isMonth == true) {
        //            IsMonthRegids.push(replaceHid(orderItem.Regid, orderItem.Hid));
        //        }
        //    });
        //});
        //if (IsMonthRegids.length > 0) {
        //    var msg = "账号：" + IsMonthRegids.join(",") + "是长包房，请入账收取最后一个月的房租！";
        //    jConfirm(msg, "去收取", "继续迟付", function (result) {
        //        if (result == false) {
        //            $("#folioDelayPayButton").attr("operationContinue", "true");
        //            $("#folioDelayPayButton")[0].click();
        //        }
        //    });
        //    return;
        //}
        if ($("#PermanentRoomOrderAdd").length > 0) {
            var msg = "<div id=\"permanentRoomCheckOut_InAdvanceCheckout_Div\"></div>长包房迟付，请收取最后一个月的房租！";
            jConfirm(msg, "去收取", "继续迟付", function (result) {
                if (result == false) {
                    $("#folioDelayPayButton").attr("operationContinue", "true");
                    $("#folioDelayPayButton")[0].click();
                } else {
                    folioInAdvanceCheckoutButton_click("out");
                }
            });
            inAdvanceCheckout_GetEndDate();
            return;
        }
    }
    //显示迟付原因窗口，并且根据选中的房号等资料更新迟付窗口中的内容
    var win = $("#folioDelayPayReasonWindow").data("kendoWindow");
    if (!win) {
        initfolioDelayPayReasonWindow();
        win = $("#folioDelayPayReasonWindow").data("kendoWindow");
    }
    var grid = $("#gridFolioGuest").data("kendoGrid");
    var selectedRoomNos = [];
    var isRemoveChecked = false;
    $("input.folioGuestRowCheck:checked").each(function (index, obj) {
        var $tr = $(obj).parents("tr");
        var dataitem = grid.dataItem($tr);
        if (dataitem.StatuName == "在住") {
            selectedRoomNos.push(dataitem.RoomNo);
        } else {
            obj.checked = false;
            isRemoveChecked = true;
        }
    });
    if (isRemoveChecked) { folioQueryButton_clicked(); }
    if (selectedRoomNos.length == 0) {
        jAlert("请选择已经入住的房间");
        return;
    }
    $("#folioDelayPayTotalRoomQty").text(selectedRoomNos.length);
    $("#folioDelayPayRoomNos").text(selectedRoomNos.join(','));
    $("#folioDelayPayReason").val("");
    win.center().open();
}
function folioDelayPayButton_clicked_after() {
    //判断是否有选中至少一条客账明细
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds.length == 0) {
        jAlert("请选择要迟付的房间");
        return;
    }
    var reason = $("#folioDelayPayReason").val();
    if (!reason) {
        jAlert("请输入迟付原因");
        return;
    }
    var regIdsStr = selectedRegIds.join(',');
    $.post(FolioCommonValues.Out, { regIds: regIdsStr,outReason:reason }, function (data) {
        if (data.Success) {
            //设置迟付成功的标记
            FolioCommonValues.FolioStatus = "out";
            //重新加载账务明细
            jAlert("迟付成功");
            folioGuestGrid_refresh();
            folioQueryButton_clicked();
        }
        else {
            folioDayChargeRemove();
            //jAlert("迟付失败，原因："+data.Data);
            ajaxErrorHandle(data);
        }
    }, 'json');
}
//迟付原因确定按钮点击
function folioDelayPayConfirmButton_clicked(e) {
    if (e) { e.preventDefault(); }
    var reason = $("#folioDelayPayReason").val();
    if (!reason) {
        jAlert("请输入迟付原因");
        return;
    }
    var win = $("#folioDelayPayReasonWindow").data("kendoWindow");
    win.close();
    checkRentCharge("out");
}
//迟付原因取消按钮点击事件
function folioDelayPayCancelButton_clicked(e) {
    if (e) { e.preventDefault(); }
    var win = $("#folioDelayPayReasonWindow").data("kendoWindow");
    win.close();
}