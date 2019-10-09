//清账的js
//清账按钮点击事件
function folioSettleButton_clicked(e) {
    if (e) {
        e.preventDefault();
    }
    //判断当前选择的状态是否是未结，如果不是，则提示并且不允许进行结账操作
    var selectedState = $("input[name='folioStatu']:checked").val();
    if (selectedState != "1") {
        jAlert("只有在未结状态下才可以进行清账操作");
        return;
    }
    //判断是否有选中至少一条账务明细
    var selectedFolioIds = folioGetSelectedFolioIdArray();
    if (selectedFolioIds.length == 0) {
        jAlert("请选择要清账的明细账");
        return;
    }
    //设置在结账过程中的入账transid为空
    $("#folioCheckoutAddedTransIds").val("");

    var transIdsStr = selectedFolioIds.join(',');
    $.post(FolioCommonValues.ClearCheck, { transIds: transIdsStr }, function (data) {
        if (data.Success) {
            if (data.Data) {
                var billAmount = parseFloat(data.Data["Balance"]);
                if ((!isNaN(billAmount)) && billAmount > 0) {
                    autoOpenAuthCard(billAmount, function () { folioClearCheckAfterCheck_Check(); }, function () { folioClearCheckAfterCheck_ShowAddFolioWindow(data); }); return;
                }
                folioClearCheckAfterCheck_ShowAddFolioWindow(data); return;
            } else {
                folioClearAfterPayment("");
            }
        } else {
            //jAlert("结账检查失败，原因:" + data.Data);
            ajaxErrorHandle(data);
            return;
        }
    }, 'json');
}
//检查账务
function folioClearCheckAfterCheck_Check() {
    $.post(FolioCommonValues.ClearCheck, { transIds: folioGetSelectedFolioIdArray().join(',') }, function (result) {
        if (result.Success && result.Data) { folioClearCheckAfterCheck_ShowAddFolioWindow(result); } else { folioClearAfterPayment("", function () { try { $("#folioAddFolioDiv").data("kendoWindow").close(); } catch (e) { } }); }
    }, 'json');
}
//显示入账弹框
function folioClearCheckAfterCheck_ShowAddFolioWindow(data) {
    //显示入账中的付款，让操作员来操作
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来清账");
        return;
    }
    folioAddSavedCallback = folioClearAfterPayment;
    folioAddContinueSavedCallback = function () { folioClearCheckAfterCheck_Check(); };
    var addFolioWindow = $("#folioAddFolioDiv").data("kendoWindow");
    var selectedRegId = folioGetTableSelectedRegId();
    addFolioWindow.refresh({
        url: FolioCommonValues.AddFolioClear,
        data: { resId: resIdValue, selectedRegId: selectedRegId, isCheckout: 1, rnd: Math.random() },//isCheckout:是否是入账
        type: "get",
        iframe: false
    }).center().open();
    addFolioWindow.bind("refresh", function () {
        //默认选中付款
        $("input#folioAddTypeDCFlag_c").prop("checked", "checked").trigger("change");
        $("#folioAddItemAmountC").data("kendoNumericTextBox").value(data.Data["Balance"]);
        $("#folioAddItemAmountOriC").data("kendoNumericTextBox").value(data.Data["Balance"]);
        if (data.Data["Id"]) {
            $("#folioAddItem").data("kendoDropDownList").value(data.Data["Id"]);
            $("#folioAddItemId").val(data.Data["Id"]);
            $("#folioAddItemAction").val(data.Data["Action"])
            $("#folioAddItemRate").val(data.Data["Rate"]);
            $("#hidIsCheckout").val("1");
            folioAddItem_payActionHandle(data.Data["Action"]);
            //按照汇率改变金额
            folioAddItem_calcRateAmount(1);
        }
        var folioAddFolioDivWindow = $("#folioAddFolioDiv").data("kendoWindow");
        folioAddFolioDivWindow.center();
        folioAddFolioDivWindow.unbind("refresh");
    });
}
function folioClearAfterPayment(transId, postCompleteCallback) {
    //将参数中回收的日租半日租的folioid与选中的folioid拼在一起，提交后台进行结账判断，是否有需要退款或收款的项目
    var selectedRegIds = folioGetSelectedRegIdArray();
    var selectedTransIds = folioGetSelectedFolioIdArray();
    if (transId) {
        selectedTransIds.push(transId);
    }
    var regIdsStr = selectedRegIds.join(',');
    var transIdsStr = selectedTransIds.join(',');
    $.post(FolioCommonValues.Clear, { transIds: transIdsStr }, function (data) {
        if (data.Success) {
            //重新加载账务明细
            jAlert("清账成功", null, function () { folioPrintButton_clicked(false, selectedRegIds, selectedTransIds); });
            //folioGuestGrid_refresh();
            folioQueryButton_clicked();
        }
        else {
            //jAlert("清账失败，原因："+data.Data);
            ajaxErrorHandle(data);
        }
        if (typeof (postCompleteCallback) == "function") {
            postCompleteCallback();
        }
    }, 'json');
}