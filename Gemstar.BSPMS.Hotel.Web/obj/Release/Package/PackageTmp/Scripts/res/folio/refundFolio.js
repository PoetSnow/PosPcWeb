//退款账务js
function folioRefundButton_clicked(e) {
    folioRefundWindow.Open();
    return;

    if (e) { e.preventDefault(); }
    //判断当前选择的状态是否是未结，如果不是，则提示并且不允许进行操作
    var selectedState = $("input[name='folioStatu']:checked").val();
    if (selectedState != "1" && selectedState != "51") {
        jAlert("只有在未结或作废状态下才可以进行操作");
        folioMoreDivClose();
        return;
    }
    //判断是否有选中并且只能选一条账务明细
    var selectedFolioIds = [];
    $("input.folioFolioRowCheck:checked").each(function (index, obj) {
        selectedFolioIds.push($(obj).val());
    });
    if (selectedFolioIds.length != 1) {
        jAlert("只能选择一条账务进行操作");
        folioMoreDivClose();
        return;
    }

    $.post(FolioCommonValues.RefundCheck, { transId: selectedFolioIds.join(',') }, function (data) {
        if (data.Success) {
            folioRefundFunc(selectedFolioIds);
        } else {
            ajaxErrorHandle(data); folioMoreDivClose(); return;
        }
    }, 'json');
}
function folioRefundFunc(selectedFolioIds) {
    var msg = "账号：{0}，房号：{1}，项目：{2}，金额：{3}。\n请填写退款金额：";
    var dataItem = $("#gridFolioFolio").data("kendoGrid").dataItem($("#gridFolioFolio input.folioFolioRowCheck:checked[type='checkbox'][value='" + selectedFolioIds[0] + "']").parents("tr"));
    if (dataItem != null && dataItem != undefined) {
        msg = msg.replace("{0}", replaceHid(dataItem.RegId, FolioCommonValues.HotelId)).replace("{1}", dataItem.RoomNo).replace("{2}", dataItem.ItemName).replace("{3}", (dataItem.DebitAmount == null ? dataItem.CreditAmount : dataItem.DebitAmount));
    }

    jPrompt(msg, "", "退款", "取消", function (result) {
        var money = parseFloat(result);
        if (isNaN(money) || money == undefined || money == null || money <= 0) { jAlert("请输入正确的退款金额！"); return; }
        if (result != null) {
            $.post(FolioCommonValues.Refund, { transId: selectedFolioIds.join(','), amount: result }, function (data) {
                if (data.Success) {
                    jAlert("入账成功！", "确定");
                    folioGuestGrid_refresh();
                } else {
                    ajaxErrorHandle(data);
                }
                folioMoreDivClose();
            }, 'json');
        } else {
            folioMoreDivClose();
        }
    }, "账务退款");
}
var folioRefundWindow = {
    Init: function () {
        var windowObj = $("#refundfolioWindow");
        var windowObjKendo = windowObj.data("kendoWindow");
        if (windowObjKendo == null || windowObjKendo == undefined) {
            windowObj.kendoWindow({
                title: "退款",
                width: "800px",
                visible: false,
                modal: true,
                actions: ["Close"],
                close: function () {
                    folioRefundWindow.Clear();
                    try {
                        folioMoreDivClose();
                        folioQueryButton_clicked();
                    } catch (e) {}
                }
            });
            windowObjKendo = windowObj.data("kendoWindow");
            $("#refundfolioWindow_Amount").kendoNumericTextBox({ min: 0, decimals: 2 });
            $("#refundfolioWindow_submit").click(function (e) { folioRefundWindow.Save(e); });
            $("#refundfolioWindow_close").click(function (e) { folioRefundWindow.Close(e); });
        }
        return windowObjKendo;
    },
    Open: function (refundAmount, closeWindowCallback, notCallback) {
        var isAutoOpen = false;
        if (refundAmount != null && refundAmount != undefined && typeof (closeWindowCallback) == "function" && typeof (notCallback) == "function") {
            isAutoOpen = true;
        }
        folioRefundWindow.Clear();
        //判断是否有选中至少一条客账明细
        var selectedRegIds = folioGetSelectedRegIdArray();
        if (selectedRegIds.length == 0) {
            if (isAutoOpen) {
                notCallback();
            }
            else {
                jAlert("请选择房间！");
            }
            return;
        }
        $.post(FolioCommonValues.GetRefundFolios, { regids: selectedRegIds.join(",") }, function (result) {
            if (result.Success) {
                if (result.Data != null && result.Data != undefined && result.Data.length > 0) {
                    var amountKendo = $("#refundfolioWindow_Amount").data("kendoNumericTextBox");
                    if (amountKendo == null || amountKendo == undefined) {
                        $("#refundfolioWindow_Amount").kendoNumericTextBox({ min: 0, decimals: 2 });
                    }
                    var selectedTransId = null;
                    var trHtml = [];
                    $.each(result.Data, function (index, item) {
                        var tr = ("<tr data-transid='{0}' data-amount='{1}'>").replace("{0}", item.Transid).replace("{1}", item.Amount);
                        tr += ("<td>{0}</td>").replace("{0}", replaceHid(item.Regid, item.Hid));
                        tr += ("<td>{0}</td>").replace("{0}", item.RoomNo);
                        tr += ("<td>{0}</td>").replace("{0}", item.ItemName);
                        tr += ("<td>{0}</td>").replace("{0}", item.Amount);
                        tr += ("<td>{0}</td>").replace("{0}", convertJsonDate(item.TransDate, "ToDateTimeString"));
                        tr += ("<td>{0}</td>").replace("{0}", item.Remark);
                        tr += ("<td>{0}</td>").replace("{0}", item.InputUser);
                        tr += "</tr>";
                        trHtml.push(tr);
                        if (isAutoOpen) {
                            if (selectedTransId == null && item.Amount > refundAmount) {
                                selectedTransId = item.Transid;
                            }
                        }
                    });
                    $("#refundfolioWindow_Tbody").html(trHtml.join(""));
                    $("#refundfolioWindow_Tbody tr").click(function () {
                        $("#refundfolioWindow_Tbody tr").removeClass("k-state-selected");
                        $(this).addClass("k-state-selected");
                        var amountKendo = $("#refundfolioWindow_Amount").data("kendoNumericTextBox");
                        if (amountKendo != null && amountKendo != undefined) {
                            amountKendo.value(null);
                            amountKendo.max($(this).attr("data-amount"));
                        }
                    });
                    //弹框
                    var windowObjKendo = folioRefundWindow.Init();
                    windowObjKendo.center().open();
                    //自动选择
                    if (isAutoOpen) {
                        if (selectedTransId != null) {
                            $("#refundfolioWindow_Tbody tr[data-transid='" + selectedTransId + "']").click();
                        }
                        $("#refundfolioWindow_Amount").data("kendoNumericTextBox").value(refundAmount);
                        $("#refundfolioWindow").data("kendoWindow").bind("deactivate", function () {
                            closeWindowCallback();
                            $("#refundfolioWindow").data("kendoWindow").unbind("deactivate");
                        });
                    }
                    else {
                        $("#refundfolioWindow_Tbody tr:eq(0)").click();
                    }
                } else {
                    if (isAutoOpen) {
                        notCallback();
                    }
                    else {
                        jAlert("没有可退款的账务！\n仅支持（支付宝刷卡支付、支付宝扫码支付、微信刷卡支付、微信扫码支付）退款");
                    }
                    return;
                }
            } else {
                if (isAutoOpen) {
                    notCallback();
                }
                else {
                    ajaxErrorHandle(result);
                }
                return;
            }
        }, 'json');
    },
    Close: function () {
        var windowObjKendo = $("#refundfolioWindow").data("kendoWindow");
        if (windowObjKendo != null && windowObjKendo != undefined) {
            windowObjKendo.close();
        }
    },
    Clear: function () {
        $("#refundfolioWindow_Tbody").empty();
        var amountKendo = $("#refundfolioWindow_Amount").data("kendoNumericTextBox");
        if (amountKendo != null && amountKendo != undefined) {
            amountKendo.value(null);
        }
    },
    Save: function () {
        var selectedTr = $("#refundfolioWindow_Tbody tr.k-state-selected");
        if (!(selectedTr != null && selectedTr != undefined && selectedTr.length == 1)) {
            jAlert("请选择要退款的账务！");
            return;
        }
        var transid = selectedTr.attr("data-transid");
        var amountStr = selectedTr.attr("data-amount");
        var amount = $("#refundfolioWindow_Amount").data("kendoNumericTextBox").value();
        if (amount <= 0) {
            jAlert("请输入退款金额！");
            return;
        }
        if (amount > parseFloat(amountStr)) {
            jAlert("退款金额不能大于" + amountStr + "！");
            return;
        }
        jConfirm("确定退款？", "确定", "返回", function (confirmed) {
            if (confirmed) {
                $.post(FolioCommonValues.Refund, { transId: transid, amount: amount }, function (data) {
                    if (data.Success) {
                        jAlert("入账成功！", "确定");
                        folioQueryButton_clicked();
                    } else {
                        ajaxErrorHandle(data);
                    }
                    folioRefundWindow.Open();
                }, 'json');
            }
        });
    },
    AutoOpen: function (refundAmount, closeWindowCallback, notCallback) {
        if (!($.trim(refundAmount).length > 0 && refundAmount != null && refundAmount != undefined && typeof (closeWindowCallback) == "function" && typeof (notCallback) == "function")) {
            if (typeof (notCallback) == "function") {
                notCallback();
            }
            else {
                jAlert("检查退款信息时错误！");
                return;
            }
        }
        folioRefundWindow.Open(Math.abs(refundAmount), closeWindowCallback, notCallback);
    },
    CloseCallBackEvents:function(callback){
        callback();
        $("#refundfolioWindow").data("kendoWindow").unbind("close", "folioRefundWindow.CloseCallBackEvents");
    },
};