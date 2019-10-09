//入账的js代码
var folioAddSavedCallback; var folioAddContinueSavedCallback;
//初始化事件
function folioAddFolioDiv_Initialization() {
    //初始化入账窗口
    if (!$("#folioAddFolioDiv").data("kendoWindow")) {
        $("#folioAddFolioDiv").kendoWindow({
            width: "490px",
            title: "入账",
            visible: false,
            modal: true,
            actions: [
                "Close"
            ],
            close: function () {
                folioAddSavedCallback = null; folioAddContinueSavedCallback = null;
                folioDayChargeRemove();
                //自动弹出门卡窗口
                var obj = document.getElementById("btnLock");
                if (obj != null && obj != undefined && obj.getAttribute("data-auto-open") == "true") {
                    obj.setAttribute("data-auto-open", "false");
                    var lockType = CustomerCommonValues.lockType;
                    var lockCode = CustomerCommonValues.lockCode;
                    if (lockType != null && lockType != undefined && lockType.length > 0 && lockCode != null && lockCode != undefined && lockCode.length > 0) {
                        $("#tabstripAuth").data("kendoTabStrip").activateTab($("[aria-controls='tabstripAuth-1']"));
                        obj.click();
                    }
                }
            }
        });
    }
}
//入账界面初始化控件值为空值
function folioAddSetAllControlsEmpty() {
    //删除所有动态增加的付款处理方式参数行
    $("#folioDynamicScript").empty();
    $("tr.folioAddPayment").remove();
    $("#folioPayResultDiv").empty();
    //隐藏所有动态变换的行，等后面根据值来显示对应的行
    $("tr.folioAddAmountForC,tr.folioAddAmountForD,tr.folioAddQtyInput").hide();
    //给相关字段赋空值，以避免将消费带到付款或者将付款带到消费里面
    $("#folioAddItemId").val("");
    $("#folioAddItem").data("kendoDropDownList").value("");
    $("#folioAddItemRate").val("");
    $("#folioAddItemQty").data("kendoNumericTextBox").value("");
    $("#folioAddItemTax").data("kendoNumericTextBox").value("");
    $("#folioAddItemAmountD").data("kendoNumericTextBox").value("");
    $("#folioAddItemAmountDNoTax").data("kendoNumericTextBox").value("");
    $("#folioAddItemAmountDTax").data("kendoNumericTextBox").value("");
    $("#folioAddItemAmountOriC").data("kendoNumericTextBox").value("");
    $("#folioAddItemAmountC").data("kendoNumericTextBox").value("");
    $("#folioAddInvNo").val("");
    $("#folioAddRemark").val("");
    $("#folioAddItemAction").val("");
    //根据消费或付款显示对应的控件给用户输入
    var dcFlagValue = $("input[name='folioAddTypeDCFlag']:checked").val();
    if (dcFlagValue == 'd') {
        $("tr.folioAddAmountForD").show();
    } else {
        $("tr.folioAddAmountForC").show();
    }
    $("#permanentRoomDepositTypeList").data("kendoDropDownList").value("");
}
//入账类型更改事件
function folioAddTypeDCFlag_changed() {
    if ($("input[name='folioAddTypeDCFlag']:checked").val() == "cardAuth") { document.getElementById("folioAuthButton").click(); $("input#folioAddTypeDCFlag_c").prop("checked", "checked").trigger("change"); return; }
    folioAddSetAllControlsEmpty();
    $("#folioAddItem").data("kendoDropDownList").dataSource.read();
    permanentRoomDepositTypeShow();
}
//入账
function folioAddButton_clicked(e, folioAddTypeDCFlag, isShowCardAuth) {
    if (e) { e.preventDefault(); }
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来入账", "确定");
        return;
    }
    folioAddSavedCallback = undefined;
    var selectedRegId = folioGetTableSelectedRegId();
    var addFolioWindow = $("#folioAddFolioDiv").data("kendoWindow");
    addFolioWindow.refresh({
        url: FolioCommonValues.AddFolio,
        data: { resId: resIdValue, selectedRegId: selectedRegId, rnd: Math.random() },
        type: "get",
        iframe: false
    }).center().open();
    addFolioWindow.bind("refresh", function () {
        var obj = $("#folioAddFolioDiv").data("kendoWindow");
        obj.center();
        if (folioAddTypeDCFlag == "c" || folioAddTypeDCFlag == "d") {
            $("input#folioAddTypeDCFlag_" + folioAddTypeDCFlag).prop("checked", "checked").trigger("change");
        }

        permanentRoomDepositTypeShow();
        if (isShowCardAuth == true) { $("#folioAddTypeDCFlag_cardAuth_radio").css("display", "inline"); }
        obj.unbind("refresh");
    });
}
function permanentRoomDepositTypeShow() {
    $("#permanentRoomDepositType").css("display", "none");
    var folioAddTypeDCFlag = $("input[name='folioAddTypeDCFlag']:checked").val();
    //长包房押金类型输入
    if (folioAddTypeDCFlag == "c") {
        if ($("#PermanentRoomOrderAdd").length > 0) {
            $("#permanentRoomDepositType").css("display", "table-row");
        }
    }
}
//入账项目自动完成设置查询参数
function folioAddItem_autoCompleteSetPara() {
    return {
        type: $("input[name='folioAddTypeDCFlag']:checked").val(),
        keyword: $("#folioAddItem").data("kendoDropDownList").filterInput.val()
    };
}
//入账项目自动完成选择项目
function folioAddItem_autoCompleteSelected(e) {
    //删除所有动态增加的付款处理方式参数行
    $("#folioDynamicScript").empty();
    $("tr.folioAddPayment").remove();

    var selectedLi = e.item[0];
    var index = -1;
    var ul_lis = e.sender.ul[0].childNodes;
    var count = ul_lis.length;
    for (var i = 0; i < count; i++) {
        var li = ul_lis[i];
        if (li == selectedLi) {
            index = i;
            break;
        }
    }
    var itemId = "";
    var $tr = $("tr.folioAddQtyInput");
    $tr.hide();
    if (index >= 0) {
        var dataItem = e.sender.dataItems()[index];
        itemId = dataItem["Id"];
        //如果是消费项目，并且设置为需要输入数量，则显示数量和单价行
        var dcFlagValue = $("input[name='folioAddTypeDCFlag']:checked").val();
        if (dcFlagValue == 'd') {
            $("#folioAddItemQty").data("kendoNumericTextBox").value(1);
            $("#folioAddItemPrice").data("kendoNumericTextBox").value(0);
            if (dataItem["IsQuantity"]) {
                $("#folioAddItemPrice").data("kendoNumericTextBox").value(dataItem["Price"]);
                $tr.show();
            }
            $("#folioAddItemTax").data("kendoNumericTextBox").value(dataItem["Taxrate"]);
            folioAddItem_calcItemAmount();
        }
        //如果是付款，则给汇率字段赋值
        if (dcFlagValue == 'c') {
            $("#folioAddItemRate").val(dataItem["Rate"]);
            //根据付款项目的处理方式，动态加载输入参数和相关js函数
            var actionValue = dataItem["Action"];
            folioAddItem_payActionHandle(actionValue);
            //按照汇率改变金额
            folioAddItem_calcRateAmount(1);
        }
    }
    $("#folioAddItemId").val(itemId);
    try { setTimeout(function () { autoSelectInputTextBox(); }, 500); } catch (e) { }
}
//根据入账项目的处理方式动态加载对应的界面和js方法
function folioAddItem_payActionHandle(actionValue) {
    if (actionValue == undefined)//第一次加载默认选中请选择项目，不需要加载
        return;
    $("#folioAddItemAction").val(actionValue);
    $.post(FolioCommonValues.PayCheck, { payAction: actionValue }, function (data) {
        if (data.Success) {
            var payAction = createPayByAction(actionValue);
            if (payAction) {
                payAction.PayInit();
            }
            if (actionValue == "corp") {
                //合约单位挂账 默认设置为客情页面的合约单位
                var cttid = $("#Cttid").data("kendoDropDownList").value();
                if (cttid != null && cttid != undefined && cttid.length > 0) {
                    var cttName = $("#Cttid").data("kendoDropDownList").text();
                    $("#folioCorpAutoComplete").val(cttName);
                    $("#lblProfileId").val(cttid);
                    //获取签单人
                    folioCorpAutoComplete_dataBound_folioCorpSignPerson(cttid, cttName);
                }
            } else if (actionValue == "mbrCard" || actionValue == 'mbrLargess' || actionValue == 'mbrCardAndLargess') {
                console.log("pay action:" + actionValue);
                var mbrCardNo = OrderList.GetOrderMbrCard(folioGetSelectedRegIdArray());
                console.log("mbr card no:" + mbrCardNo);
                if (mbrCardNo != null) {
                    $("#folioMbrCardNo").val(mbrCardNo);
                    console.log("#folioMbrCardNo value:" + $("#folioMbrCardNo").val());
                }
            }
        } else {
            //jAlert(data.Data);
            ajaxErrorHandle(data);
        }
    }, 'json');
}
//入账项目的数量和单价变化时，自动计算金额
function folioAddItem_calcItemAmount() {
    var qtyValue = parseFloat($("#folioAddItemQty").data("kendoNumericTextBox").value());
    var priceValue = parseFloat($("#folioAddItemPrice").data("kendoNumericTextBox").value());
    if (isNaN(qtyValue) || isNaN(priceValue)) {
        return;
    }
    var amountValue = qtyValue * priceValue;
    $("#folioAddItemAmountD").data("kendoNumericTextBox").value(amountValue);
    $("#folioAddItemAmountD").data("kendoNumericTextBox").trigger("change");
}
//项目金额变化时，自动计算税额，不含税金额
function folioAddItem_calcTaxAndNoTaxAmount() {
    var amountValue = parseFloat($("#folioAddItemAmountD").data("kendoNumericTextBox").value());
    if (isNaN(amountValue)) {
        return;
    }
    var taxRateValue = parseFloat($("#folioAddItemTax").data("kendoNumericTextBox").value());
    if (isNaN(taxRateValue)) {
        taxRateValue = 0;
    }
    taxRateValue = taxRateValue / 100.0;
    var noTaxAmount = (amountValue / (1 + taxRateValue)).toFixed(2);
    var taxValue = amountValue - noTaxAmount;
    $("#folioAddItemAmountDNoTax").data("kendoNumericTextBox").value(noTaxAmount);
    $("#folioAddItemAmountDTax").data("kendoNumericTextBox").value(taxValue);
}
//付款的原币金额变化时，自动根据汇率计算金额 默认值（当前对象）:金额发生改变事件 1:项目下拉框事件
function folioAddItem_calcRateAmount(isChange) {
    var oriAmuntValue = parseFloat($("#folioAddItemAmountOriC").data("kendoNumericTextBox").value());
    var isCheckout = $("#hidIsCheckout").val();//0:入账 1：结账或者清账
    if (isNaN(oriAmuntValue)) {
        return;
    }
    var rateValue = parseFloat($("#folioAddItemRate").val());
    if (isNaN(rateValue)) {
        rateValue = 1.0;
    }
    if (isCheckout == 0 || isChange != 1) {
        var amountValue = (oriAmuntValue * rateValue).toFixed(2);
        $("#folioAddItemAmountC").data("kendoNumericTextBox").value(amountValue);
    }
    else if (isCheckout == 1 && isChange == 1) {
        var folioAmount = $("#folioAddItemAmountC").data("kendoNumericTextBox").value();
        var amountValue = (folioAmount / rateValue).toFixed(2);
        $("#folioAddItemAmountOriC").data("kendoNumericTextBox").value(amountValue);
    }

}
//入账保存按钮点击事件，保存入账消费和付款
function folioAddSave_clicked(e) {
    if (e) {
        e.preventDefault();
    }
    var regIdValue = $("#folioAddRegId").data("kendoDropDownList").value();
    if (!regIdValue) {
        jAlert("请选择要入账的账单", "确定");
        return;
    }
    var itemIdValue = $("#folioAddItemId").val();
    if (!itemIdValue) {
        jAlert("请选择要入账的项目", "确定");
        return;
    }
    var isCheckout = $("#hidIsCheckout").val();
    var addModel = {
        FolioDCFlag: $("input[name='folioAddTypeDCFlag']:checked").val(),
        FolioRegId: regIdValue,
        FolioItemId: itemIdValue,
        FolioInvoNo: $("#folioAddInvNo").val(),
        IsCheckout: isCheckout,
        FolioRemark: $("#folioAddRemark").val(),
        AuthorizationSaveContinue: $("#authorizationSaveContinue").val(),
    };
    if (addModel.FolioDCFlag == 'd') {
        addModel.FolioItemQty = $("#folioAddItemQty").data("kendoNumericTextBox").value();
        addModel.FolioAmount = $("#folioAddItemAmountD").data("kendoNumericTextBox").value();
    } else {
        addModel.FolioOriAmount = $("#folioAddItemAmountOriC").data("kendoNumericTextBox").value();
        addModel.FolioAmount = $("#folioAddItemAmountC").data("kendoNumericTextBox").value();
        addModel.PayItemId = itemIdValue;
    }
    //检查支付处理方式检验是否成功
    var actionValue = $("#folioAddItemAction").val();
    if (actionValue) {
        addModel.FolioItemAction = actionValue;
        var payAction = createPayByAction(actionValue);
        var beforeFuncResult = true;

        if (payAction) {
            try {
                beforeFuncResult = payAction.PayCheck();
            } catch (e) { }
        }

        if (!beforeFuncResult) {
            return;//如果方法返回false，则不继续保存
        }
        //设置处理方式提交到后台的参数
        try {
            payAction.PaySetPara(addModel);
        } catch (e) { }        
    }
    $("#folioAddSave").attr("disabled", "disabled");

    //长包房押金类型输入
    if (addModel.FolioDCFlag == 'c' && $("#PermanentRoomOrderAdd").length > 0) {
        var permanentRoomDepositTypeValue = $("#permanentRoomDepositTypeList").data("kendoDropDownList").value();
        if ($.trim(permanentRoomDepositTypeValue).length > 0) {
            addModel.FolioDepositType = permanentRoomDepositTypeValue;
        }
    }

    $.post(FolioCommonValues.AddFolio, addModel, function (data) {
        if (data.Success) {
            //如果是待支付，并且有返回回调函数，则调用回调函数进行处理
            if (data.Data.Statu == "WaitPay" && data.Data.Callback !== "") {
                var payAction = createPayByAction(actionValue);
                try {
                    payAction.PayAfterSave(data);
                } catch (e) {

                }
            } else {
                if (typeof (folioAddContinueSavedCallback) == "function") { $("#folioAddContinue")[0].checked = true; }
                //如果是已经支付成功，则按支付成功的进行处理
                if ((typeof (folioAddSavedCallback) == "function") && ($("#folioAddContinue").prop("checked") == false)) {
                    folioAfterSave(data, isCheckout);
                } else {
                    jAlert("入账成功！", "确定", function () {
                        folioAfterSave(data, isCheckout);
                    });
                }
            }
        } else {
            if (data.ErrorCode == 1) {
                ajaxErrorHandle(data);
            } else {
                if (data.ErrorCode == 4) { authorizationWindow.Open(2, data.Data, "folioAddSave_clicked"); return; }
                jAlert(data.Data, "知道了", function () {
                    try {
                        if (typeof (folioAddContinueSavedCallback) == "function") { folioAddContinueSavedCallback(); } else { /*$("#folioAddFolioDiv").data("kendoWindow").close();*/ }
                    } catch (e) { }
                });
            }
        }
        $("#folioAddSave").removeAttr("disabled");
    }, 'json');
}
//增加客账明细成功的处理方法
function folioAfterSave(data, isCheckout) {
    //付款金额大于0执行打印
    var isPrint = false;
    var oriAmuntValue = parseFloat($("#folioAddItemAmountOriC").data("kendoNumericTextBox").value());
    if ((!isNaN(oriAmuntValue)) && oriAmuntValue > 0) {
        isPrint = true;
    }
    //根据是否继续录入进行不同的处理
    var continueAdd = $("#folioAddContinue").prop("checked");
    var isFolioAddSavedCallback = (typeof (folioAddSavedCallback) == "function");
    if (continueAdd) {
        if (isFolioAddSavedCallback) {
            var folioCheckoutAddedTransIds = $("#folioCheckoutAddedTransIds");
            if (folioCheckoutAddedTransIds.val().length <= 0) {
                folioCheckoutAddedTransIds.val(data.Data["FolioTransId"]);
            } else {
                folioCheckoutAddedTransIds.val(folioCheckoutAddedTransIds.val() + "," + data.Data["FolioTransId"]);
            }
            if (typeof (folioAddContinueSavedCallback) == "function") { folioAddContinueSavedCallback(); } else { folioAddSetAllControlsEmpty(); }
        } else {
            folioAddSetAllControlsEmpty();
        }
    } else {
        //调用入账成功后的回调
        if (isFolioAddSavedCallback) {
            folioAddSavedCallback(data.Data["FolioTransId"], function () { $("#folioAddFolioDiv").data("kendoWindow").close(); });
        } else {
            $("#folioAddFolioDiv").data("kendoWindow").close();
        }
        //刷新客账明细
        folioQueryButton_clicked();
        //刷新客账左侧列表
        folioGuestGrid_refresh();
        try { folioQueryResBillIdRefresh(); } catch (e) { }
    }
    //如果入账的是付款记录，则自动调用打印
    if (data.Data.DCFlag == "c" && isCheckout == 0 && isPrint && FolioCommonValues.IsPayPrintDeposit == "True") {
        var parameterValues = "@transId={transids}";
        var isSignature = $("#IsSignature").val();
        var printIndex = isSignature == 1 ? 10 : 1;
        parameterValues = parameterValues.replace("{transids}", data.Data.FolioTransId);
        $.post(FolioCommonValues.AddQueryParaTemp, { ReportCode: "up_print_deposit", ParameterValues: parameterValues, ChineseName: "押金单",print:printIndex }, function (result) {
            if (result.Success) {
                if (printIndex == 10) {
                    try {
                        var name = $("#folioAddRegId").data("kendoDropDownList").dataItem().RegName;
                        var names = name.split("　");
                        var roomNo = names[0].split(":")[1];
                        var regid = names[1].split(":")[1];
                        window.open(result.Data + "&sType=3&regid=" + regid + "&roomNo=" + roomNo);
                    } catch (e) {
                        var select = $("#gridFolioGuest").data("kendoGrid").select();
                        var gridData = $("#gridFolioGuest").data("kendoGrid").dataItem(select);
                        if (gridData.RegId.length > 6)
                            window.open(result.Data + "&sType=3&regid=" + gridData.RegId.substring(6) + "&roomNo=" + gridData.RoomNo);
                        else
                            window.open(result.Data);
                    }
                }
                else
                    window.open(result.Data);
            } else {
                //jAlert(data.Data, "知道了");
                ajaxErrorHandle(result);
            }
        }, 'json');
    }
}
//自动弹出入账窗口
function autoOpenAddFolioWindow() {
    var obj = document.getElementById("folioAddButton");
    if (obj != null && obj != undefined && obj.getAttribute("data-auto-open") == "true") {
        obj.setAttribute("data-auto-open", "false");
        folioAddButton_clicked(false, "c", true);
    }
}
//入账自动定位到文本框
function autoSelectInputTextBox() {
    var obj = null;
    if ($(".folioAddAmountForC").css("display") == "table-row") {
        obj = $("#folioAddItemAmountOriC");
    } else {
        if ($(".folioAddQtyInput").css("display") == "table-row") {
            obj = $("#folioAddItemQty");
        }
        else {
            if ($(".folioAddAmountForD").css("display") == "table-row") {
                obj = $("#folioAddItemAmountD");
            }
        }
    }
    if (obj == null || obj == undefined) { return; }

    var numerictextbox = obj.data("kendoNumericTextBox");
    numerictextbox.focus();
}
//显示合约单位签单人
function folioCorpAutoComplete_dataBound_folioCorpSignPerson(cttid, cttName) {
    if ($.trim(cttid).length <= 0 || $.trim(cttName).length <= 0){ return; }

    $.post("/PayManage/Pay/AutoCompleteCorp", { keyword: cttName }, function (result) {
        if (result != null && result != undefined && result.length > 0) {
            $.each(result, function (index, item) {
                if (item.CompanyId == cttid) {
                    PayCorpSigners((item.Signers != null && item.Signers != undefined && item.Signers.length > 0) ? item.Signers : []);
                    return false;
                }
            });
        }
    }, 'json');
}
//刷新付款
function folioCFlagToRefresh() {
    var folioAddTypeDCFlag = $("input[name='folioAddTypeDCFlag']:checked").val();
    if (folioAddTypeDCFlag == "c") {
        if (typeof (folioAddContinueSavedCallback) == "function") { folioAddContinueSavedCallback(); }
    }
}