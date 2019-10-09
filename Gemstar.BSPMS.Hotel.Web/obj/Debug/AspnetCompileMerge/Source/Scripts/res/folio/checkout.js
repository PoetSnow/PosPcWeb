//结账的js
//初始化事件
function folioCheckoutDayChargeWindow_Initialization() {
    //初始化入账窗口
    if (!$("#folioCheckoutDayChargeWindow").data("kendoWindow")) {
        $("#folioCheckoutDayChargeWindow").kendoWindow({
            width: "520px",
            title: "日租/半日租/钟点房租收取",
            visible: false,
            modal: true,
            actions: [
                "Close"
            ]
        });
    }
    //注册加收日租半日租按钮点击事件
    $("#folioDayChargeAddButton").unbind("click").click(function (e) {
        folioDayChargeAddButton_clicked(e);
    });
    //注册免收日租半日租按钮点击事件
    $("#folioDayChargeFreeButton").unbind("click").click(function (e) {
        folioDayChargeFreeButton_clicked(e);
    });
}
//检查是否收取日租、半日租、钟点房租
function checkRentCharge(origin) {
    if (origin != "checkout" && origin != "out" && origin != "folioPrint") { return; }
    $("#folioCheckoutAddedOrigin").val(origin);
    //判断是否有选中至少一条客账明细
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds.length == 0) {
        if (origin == "checkout") {
            jAlert("请选择要结的房间");
        } else if (origin == "out") {
            jAlert("请选择要迟付的房间");
        } else if (origin == "folioPrint") {
            jAlert("请选择房间");
        }
        return;
    }
    //检查结束，有效，调用后台检查是否收到半日租，如果要则弹出收取半日租界面，否则则继续结账
    $.post(FolioCommonValues.CheckDayCharge, { regIds: selectedRegIds.join(',') }, function (data) {
        if (data.Success) {
            var chargeInfos = data.Data;
            if (chargeInfos.length == 0) {
                //没有需要收取的日租或半日租，则直接进行下一步
                if (origin == "checkout") {
                    folioCheckoutAfterCheck("", 1);
                } else if (origin == "out") {
                    $("#folioCheckoutDayChargeWindow").data("kendoWindow").close();
                    folioDelayPayButton_clicked_after();
                } else if (origin == "folioPrint") {
                    folioPrintButton_clicked(false);
                }
            } else {
                //显示收取日租半日租界面，让操作员操作后再进行下一步
                var trHtml = "";
                for (var i = 0; i < chargeInfos.length; i++) {
                    var rowClass = i % 2 == 0 ? "" : "k-alt";
                    var info = chargeInfos[i];
                    var options = info.Type == "钟点房租" ? ('<option value="钟点房租" selected="selected">钟点房租</option>') : ('<option value="日租" ' + (info.Type == "日租" ? "selected=\"selected\"": "") + '>日租</option><option value="半日租" ' + (info.Type == "半日租" ? "selected=\"selected\"": "") + '>半日租</option>');
                    trHtml += '<tr role="row" class="' + rowClass + '"><td>' + info.RoomNo + '</td><td style="white-space:nowrap;">' + info.GuestName + '</td><td>' + info.RoomTypeName + '</td><td><select class="folioDayChargeType" style="max-width:90px;">' + options + '</select><input type="hidden" class="folioDayChargeRegId" value="' + info.RegId + '" /><input type="hidden" class="folioDayChargeRoomNo" value="' + info.RoomNo + '" /><input type="hidden" class="folioDayChargeRate" value="' + info.Rate + '" /></td><td><input type="text" style="width:100px;" class="folioDayChargeAmount k-textbox" data-type="' + info.Type + '" data-amount="' + info.Amount + '" value="' + info.Amount + '" /></td></tr>';
                }
                $("#folioCheckoutDayChargeWindow").find("tbody").empty().html(trHtml);
                $("select.folioDayChargeType").kendoDropDownList({ change: folioDayChargeTypeSelect_changed });
                $("#folioCheckoutDayChargeWindow").data("kendoWindow").center().open();
            }
        } else {
            //jAlert(data.Data);
            ajaxErrorHandle(data);
            return;
        }
    }, 'json');
}
//加收日租半日租下拉框改变事件
function folioDayChargeTypeSelect_changed(e) {
    var inputAmount = $(this.element).parents("tr").find("input.folioDayChargeAmount");
    var originType = inputAmount.attr("data-type");
    var originAmount = parseFloat(inputAmount.attr("data-amount"));
    var type = this.value();
    var amount = inputAmount.val();
    if (!isNaN(originAmount) && originAmount >= 0) {
        if (originType == "日租") {
            if (type == "半日租") {
                inputAmount.val((originAmount / 2.00).toFixed(2));
            } else if (type == "日租") {
                inputAmount.val(originAmount.toFixed(2));
            }
        } else if (originType == "半日租") {
            if (type == "日租") {
                inputAmount.val((originAmount * 2.00).toFixed(2));
            } else if (type == "半日租") {
                inputAmount.val(originAmount.toFixed(2));
            }
        }
    }
}
//加收日租半日租按钮单击事件
function folioDayChargeAddButton_clicked(e) {
    var origin = $("#folioCheckoutAddedOrigin").val();
    if (origin != "checkout" && origin != "out" && origin != "folioPrint") { return; }
    if (e) {
        e.preventDefault();
    }
    var chargeInfos = [];
    $("#folioCheckoutDayChargeWindow").find("tbody").find("tr").each(function (index, obj) {
        var $tr = $(obj);
        var info = {
            RegId: $tr.find("input.folioDayChargeRegId").val(),
            Type: $tr.find("select.folioDayChargeType").data("kendoDropDownList").value(),
            Amount: $tr.find("input.folioDayChargeAmount").val(),
            RoomNo: $tr.find("input.folioDayChargeRoomNo").val()
        };
        chargeInfos.push(info);
    });
    if (chargeInfos.length == 0) {
        jAlert("没有需要收取的日租或半日租或钟点房租");
        return;
    }
    $.post(FolioCommonValues.AddDayCharge, { chargeInfos: chargeInfos, isTemp: (origin == "folioPrint" ? true : false), AuthorizationSaveContinue: $("#authorizationSaveContinue").val() }, function (data) {
        if (data.Success) {
            //设置加收日租半日租标记
            FolioCommonValues.FolioStatus = "addDayCharge";
            //继续结账或者迟付
            if (origin == "checkout") {
                var transIds = data.Data;
                folioCheckoutAfterCheck(transIds, 1);
            } else if (origin == "out") {
                $("#folioCheckoutDayChargeWindow").data("kendoWindow").close();
                folioDelayPayButton_clicked_after();
            } else if (origin == "folioPrint") {
                $("#folioCheckoutDayChargeWindow").data("kendoWindow").close();
                folioPrintButton_clicked(false, null, null, data.Data);
            }
        } else {
            if (data.ErrorCode == 4) { authorizationWindow.Open(4, data.Data, "folioDayChargeAddButton_clicked"); return; }
            jAlert("收取日租半日租钟点房租失败，原因：" + data.Data);
            return;
        }
    }, 'json');
}
//免收日租半日租按钮单击事件
function folioDayChargeFreeButton_clicked(e) {
    var origin = $("#folioCheckoutAddedOrigin").val();
    if (origin != "checkout" && origin != "out" && origin != "folioPrint") { return; }
    if (origin == "folioPrint") {
        $("#folioCheckoutDayChargeWindow").data("kendoWindow").close();
        folioPrintButton_clicked(false);
        return;
    }
    if (e) {
        e.preventDefault();
    }
    var reason = $("#folioDayChargeFreeReason").val();
    if (!reason) {
        jAlert("请输入免收原因");
        return;
    }
    var chargeInfos = [];
    $("#folioCheckoutDayChargeWindow").find("tbody").find("tr").each(function (index, obj) {
        var $tr = $(obj);
        chargeInfos.push($tr.find("input.folioDayChargeRegId").val());
    });
    if (chargeInfos.length == 0) {
        jAlert("没有需要免收的日租或半日租或钟点房租");
        return;
    }
    var regIds = chargeInfos.join(',');
    $.post(FolioCommonValues.FreeDayCharge, { regIds: regIds, freeReason: reason, AuthorizationSaveContinue: $("#authorizationSaveContinue").val() }, function (data) {
        if (data.Success) {
            //设置加收日租半日租标记，撤销操作时去除免收原因
            FolioCommonValues.FolioStatus = "addDayCharge";
            if (origin == "checkout") {
                folioCheckoutAfterCheck("", 1);
            } else if (origin == "out") {
                $("#folioCheckoutDayChargeWindow").data("kendoWindow").close();
                folioDelayPayButton_clicked_after();
            }
        } else {
            if (data.ErrorCode == 4) { authorizationWindow.Open(4, data.Data, "folioDayChargeFreeButton_clicked"); return; }
            jAlert("免收失败，原因：" + data.Data);
            return;
        }
    }, 'json');
}
//移除之前加收的日租和半日租，此处只是定义，在收取成功并且结账或者迟付不成功时，或者是收取成功并且关闭了后续的结账或者迟付时调用
function folioDayChargeRemove() {
    var regIds = folioGetSelectedRegIdArray();    
    if (regIds.length > 0 && FolioCommonValues.FolioStatus == "addDayCharge") {
        $.post(FolioCommonValues.RemoveDayCharge, { regIds: regIds.join(',') }, function (data) {
            if (data.Success) {
                FolioCommonValues.FolioStatus = "noChange";
            } else {
                jAlert(data.Data);
            }
        }, 'json');
    }
}
//获取选中的regid数组
function folioGetSelectedRegIdArray() {
    var selectedRegIds = [];
    $("input.folioGuestRowCheck:checked").each(function (index, obj) {
        selectedRegIds.push($(obj).val());
    });
    return selectedRegIds;
}
//获取选中的folioid数组
function folioGetSelectedFolioIdArray() {
    var selectedFolioIds = [];
    $("input.folioFolioRowCheck:checked").each(function (index, obj) {
        selectedFolioIds.push($(obj).val());
    });
    var addedTransIds = $("#folioCheckoutAddedTransIds").val();
    if (addedTransIds) {
        selectedFolioIds.push(addedTransIds);
    }
    return selectedFolioIds;
}
//结账按钮点击事件
function folioCheckoutButton_clicked(e) {
    if (e) {
        e.preventDefault();
    }
    //判断当前选择的状态是否是未结，如果不是，则提示并且不允许进行结账操作
    var selectedState = $("input[name='folioStatu']:checked").val();
    if (selectedState != "1") {
        jAlert("只有在未结状态下才可以进行结账操作");
        return;
    }
    var selectedBill = $("#folioQueryResBillId").data("kendoDropDownList").value();
    if ($.trim(selectedBill) != "") {
        jAlert("只有选择“全部账单”才可以进行结账操作！");
        return;
    }
    //判断是否有选中至少一条客账明细
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds.length == 0) {
        jAlert("请选择要结的房间");
        return;
    }
    //结账时，长包房提示收取最后一个月房租
    if ($("#folioCheckoutButton").attr("operationContinue") != "true") {
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
        //    jConfirm(msg, "去收取", "继续结账", function (result) {
        //        if (result == false) {
        //            $("#folioCheckoutButton").attr("operationContinue", "true");
        //            $("#folioCheckoutButton")[0].click();
        //        }
        //    });
        //    return;
        //}
        if ($("#PermanentRoomOrderAdd").length > 0) {
            var msg = "<div id=\"permanentRoomCheckOut_InAdvanceCheckout_Div\"></div>长包房结账，请收取最后一个月的房租！";
            jConfirm(msg, "去收取", "继续结账", function (result) {
                if (result == false) {
                    $("#folioCheckoutButton").attr("operationContinue", "true");
                    $("#folioCheckoutButton")[0].click();
                } else {
                    folioInAdvanceCheckoutButton_click("checkout");
                }
            });
            inAdvanceCheckout_GetEndDate();
            return;
        }
    }
    //判断是否有选中至少一条账务明细
    var selectedFolioIds = folioGetSelectedFolioIdArray();
    //if (selectedFolioIds.length == 0) {
    //    jAlert("请选择要结的明细");
    //    return;
    //}
    //设置在结账过程中的入账transid为空
    $("#folioCheckoutAddedTransIds").val("");
    //检查结束，有效，调用后台检查是否收到半日租，如果要则弹出收取半日租界面，否则则继续结账
    checkRentCharge("checkout");
}
function folioCheckoutAfterCheck(transIds, isCheckout) {//isCheckout:为了判断这个是从哪调用的 0:默认(不需要这个状态的地址调用) 1：结账
    if (transIds) {
        $("#folioCheckoutAddedTransIds").val(transIds);
    }
    $("#folioCheckoutDayChargeWindow").data("kendoWindow").close();
    var selectedRegIds = folioGetSelectedRegIdArray();
    var selectedTransIds = folioGetSelectedFolioIdArray();
    var regIdsStr = selectedRegIds.join(',');
    var transIdsStr = selectedTransIds.join(',');
    $.post(FolioCommonValues.CheckoutCheck, { regIds: regIdsStr, transIds: transIdsStr }, function (data) {
        if (data.Success) {
            if (data.Data) {
                var billAmount = parseFloat(data.Data["Balance"]);
                if ((!isNaN(billAmount)) && billAmount > 0) {
                    autoOpenAuthCard(billAmount, function () { folioCheckoutAfterCheck_Check(isCheckout); }, function () { folioCheckoutAfterCheck_ShowAddFolioWindow(data, isCheckout); }, isCheckout); return;
                }
                if ((!isNaN(billAmount)) && billAmount < 0) {
                    folioRefundWindow.AutoOpen(billAmount, function () { folioCheckoutAfterCheck_Check(isCheckout); }, function () { folioCheckoutAfterCheck_ShowAddFolioWindow(data, isCheckout); }); return;
                }
                folioCheckoutAfterCheck_ShowAddFolioWindow(data, isCheckout); return;
            } else {
                folioCheckoutAfterPayment("");
            }
        } else {
            folioDayChargeRemove();
            //jAlert("结账检查失败，原因:" + data.Data);
            ajaxErrorHandle(data);
            return;
        }
    }, 'json');
}
//检查账务
function folioCheckoutAfterCheck_Check(isCheckout) {
    $.post(FolioCommonValues.CheckoutCheck, { regIds: folioGetSelectedRegIdArray().join(','), transIds: folioGetSelectedFolioIdArray().join(',') }, function (result) {
        if (result.Success && result.Data) { folioCheckoutAfterCheck_ShowAddFolioWindow(result, isCheckout); } else { folioCheckoutAfterPayment("", function () { try{$("#folioAddFolioDiv").data("kendoWindow").close();}catch(e){} }); }
    }, 'json');
}
//显示入账弹框
function folioCheckoutAfterCheck_ShowAddFolioWindow(data, isCheckout) {
    //显示入账中的付款，让操作员来操作
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来结账");
        return;
    }
    folioAddSavedCallback = folioCheckoutAfterPayment;
    folioAddContinueSavedCallback = function () { folioCheckoutAfterCheck_Check(isCheckout); };
    var selectedRegId = folioGetTableSelectedRegId();
    var checkoutRegIds = folioGetSelectedRegIdArray().join(',');
    var addFolioWindow = $("#folioAddFolioDiv").data("kendoWindow");
    addFolioWindow.refresh({
        url: FolioCommonValues.AddFolioCheckOut,
        data: { resId: resIdValue, selectedRegId: selectedRegId, isCheckout: isCheckout, checkoutRegIds: checkoutRegIds, rnd: Math.random() },
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
            $("#folioAddItemAction").val(data.Data["Action"]);
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
function folioCheckoutAfterPayment(transId, postCompleteCallback) {
    //将参数中回收的日租半日租的folioid与选中的folioid拼在一起，提交后台进行结账判断，是否有需要退款或收款的项目
    var selectedRegIds = [];
    var grid = $("#gridFolioGuest").data("kendoGrid");
    $("input.folioGuestRowCheck:checked").each(function (index, obj) {
        var $tr = $(obj).parents("tr");
        var dataitem = grid.dataItem($tr);
        if (dataitem.StatuName != "已结") {
            selectedRegIds.push(dataitem.RegId);
        }
    });
    if (selectedRegIds.length == 0) {
        jAlert("请选择未结的房间");
        return;
    }
    var selectedTransIds = folioGetSelectedFolioIdArray();
    if (transId) {
        selectedTransIds.push(transId);
    }
    var regIdsStr = selectedRegIds.join(',');
    var transIdsStr = selectedTransIds.join(',');
    $.post(FolioCommonValues.Checkout, { regIds: regIdsStr, transIds: transIdsStr }, function (data) {
        if (data.Success) {
            //设置结账成功的标记
            FolioCommonValues.FolioStatus = "checkout";
            //重新加载账务明细
            jAlert("结账成功", null, function () {
                if (selectedTransIds.length > 0) {
                    folioPrintButton_clicked(false, selectedRegIds, selectedTransIds);
                    //return;
                }
                LockWindow.CheckLockInfoByRegIds(regIdsStr);
                //刷新客账
                folioGuestGrid_refresh();
                folioQueryButton_clicked();
            });
        }
        else {
            folioDayChargeRemove();
            //jAlert("结账失败，原因："+data.Data);
            ajaxErrorHandle(data);
        }
        if (typeof (postCompleteCallback) == "function") {
            postCompleteCallback();
        }
    }, 'json');
}