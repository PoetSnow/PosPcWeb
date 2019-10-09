//客账的账务列表相关js代码开始
//注册相关事件
function gridFolioFolio_Initialization() {
    //注册客人列表的全选更改事件
    $("input.folioFolioAllCheck").unbind("change").change(function (e) {
        e.preventDefault();
        gridFolioFolioAllCheck_changed();
    });
    //注册状态更改事件
    $("input[name='folioStatu']").unbind("change").change(function (e) {
        e.preventDefault();
        folioQueryButton_clicked();
    });
    //房号回车事件
    $("#txtRoomNo").unbind("keydown").keydown(function (e) {
        if (e.keyCode == 13) {
            folioQueryButton_clicked();
        }
    });
    //客账记录双击事件
    $("#folioRowInfoWindow").kendoWindow({
        width: "690px",
        title: "账务详情",
        visible: false,
        modal: true,
        actions: ["Close"],
        maxHeight: "599px",
    });
    $("#gridFolioFolio .k-grid-content tbody").on("dblclick", "tr.k-state-selected", function (e) {
        e.preventDefault();
        gridFolioFolio_editGridItem(this);
    });
}
//设置异步查询时的查询参数
function setQueryFolioPara() {
    var regIds = [];
    var gridFolioGuest = $("#gridFolioGuest").find("tr input.folioGuestRowCheck").each(function (index, item) {
        if (item.checked) {
            regIds.push(item.value);
        }
    });
    var itemTypeIds = $("#folioQueryItemTypes").data("kendoMultiSelect").value();
    if (!itemTypeIds) {
        itemTypeIds = [];
    }
    var beginDateValue = $("#folioQueryDateBegin").data("kendoDatePicker").value();
    if (!beginDateValue) {
        beginDateValue = "";
    } else {
        beginDateValue = beginDateValue.ToDateString();
    }
    var endDateValue = $("#folioQueryDateEnd").data("kendoDatePicker").value();
    if (!endDateValue) {
        endDateValue = "";
    } else {
        endDateValue = endDateValue.ToDateString();
    }
    return {
        RegIds: regIds.join(','),
        Status: $("input[name='folioStatu']:checked").val(),
        TransDateBegin: beginDateValue,
        TransDateEnd: endDateValue,
        RoomNO: $("#txtRoomNo").val(),
        ItemTypeIds: itemTypeIds.join(','),
        ResBillCode: $("#folioQueryResBillId").data("kendoDropDownList").value()
    };
}
var resLoad = {};
resLoad.loadNum = 0;
//全选复选框的更改事件
function gridFolioFolioAllCheck_changed() {
    var checked = $("input.folioFolioAllCheck")[0].checked;
    $("input.folioFolioRowCheck").each(function (index, obj) {
        obj.checked = checked;
    });
    folioCalcFolioAmountsSum();
}
//客账账务信息数据绑定后的事件，用于注册其中行的复选框更改事件
function folioFolioTable_dataBound() {
    var roomStatus = $("#hidRoomStatus").val();
    if (roomStatus == "0" && resLoad.loadNum == 0) {
        $("#folioCheckoutButton").trigger("click");
    }
    resLoad.loadNum++;
    //注册客人列表的复选框更改事件，以便在更改时刷新账务明细
    $("input.folioFolioRowCheck").change(function (e) {
        e.preventDefault();
        folioCalcFolioAmountsSum();
    });
    //注册打印押金单按钮点击事件
    $("span.printDeposit").click(function (e) {
        e.preventDefault();
        printDeposit_clicked(e);
    });
    var folioBalanceSumSpan = $("#folioBalanceSumSpan").text();
    if (!isNaN(folioBalanceSumSpan) && folioBalanceSumSpan < 0) {
        $("#folioBalanceSumSpan").css("color", "red");

    }
    //合并td,右边账务明细
    var td = $("#folioBalanceSumSpan").parent();
    $(td).next().remove();
    $(td).attr("colspan", "2");
    SumCardAuthAmountBySelectRegIds();
}
//重新计算余额消费和支付的小计值
function folioCalcFolioAmountsSum() {
    var csum = 0.0;
    var dsum = 0.0;
    var grid = $("#gridFolioFolio").data("kendoGrid");
    $("input.folioFolioRowCheck:checked").each(function (i, o) {
        var $tr = $(o).parents("tr");
        var data = grid.dataItem($tr);
        var c = parseFloat(data.CreditAmount);
        var d = parseFloat(data.DebitAmount);
        if (!isNaN(c)) {
            csum += c;
        }
        if (!isNaN(d)) {
            dsum += d;
        }
    });
    var bsum = csum - dsum;
    $("#folioBalanceSumSpan").text(bsum.toFixed(2));
    $("#folioDebitSumSpan").text(dsum.toFixed(2));
    $('#folioCreditSumSpan').text(csum.toFixed(2));
}
//查询事件
function folioQueryButton_clicked(e) {
    if (e) {
        e.preventDefault();
    }
    var grid = $("#gridFolioFolio").data("kendoGrid");
    grid.dataSource.read();

}
//打印押金单
function printDeposit_clicked(e) {
    var target = e.target;
    var $tr = $(target).parents("tr");
    var grid = $("#gridFolioFolio").data("kendoGrid");
    var dataItem = grid.dataItem($tr);
    var isSignature = $("#IsSignature").val();
    var printIndex = isSignature == 1 ? 10 : 1;
    var parameterValues = "@transId={transids}";
    parameterValues = parameterValues.replace("{transids}", dataItem.Transid);
    $.post(FolioCommonValues.AddQueryParaTemp, { ReportCode: "up_print_deposit", ParameterValues: parameterValues, ChineseName: "押金单",print:printIndex }, function (result) {
        if (result.Success) {
            if (printIndex == 10) {
                if (dataItem.RegId.length > 6)
                    window.open(result.Data + "&sType=3&regid=" + dataItem.RegId.substring(6) + "&roomNo=" + dataItem.RoomNo);
                else
                    window.open(result.Data);
            }
            else
              window.open(result.Data);
        } else {
            //jAlert(data.Data, "知道了");
            ajaxErrorHandle(result);
        }
    }, 'json');
}
//双击客账记录打开详情
function gridFolioFolio_editGridItem(row) {
    var infoTable = $("#gridFolioFolioRowInfo_info");
    var folioTable = $("#gridFolioFolioRowInfo_folio");
    var settleTable = $("#gridFolioFolioRowInfo_settle");
    var settleFieldset = $("#gridFolioFolioRowInfo_settle_fieldset");
    var folioLegend = $("#gridFolioFolioRowInfo_folio_legend");
    infoTable.empty();
    folioTable.empty();
    settleTable.empty();
    settleFieldset.css("display", "none");
    folioLegend.text("消费付款");
    var cancelAndRecoveryFolioFieldset = $("#gridFolioFolioRowInfo_CancelAndRecoveryFolio_fieldset");
    var cancelAndRecoveryFolioTable = $("#gridFolioFolioRowInfo_CancelAndRecoveryFolio");
    cancelAndRecoveryFolioFieldset.css("display", "none");
    cancelAndRecoveryFolioTable.empty();

    var grid = $("#gridFolioFolio").data("kendoGrid");
    if (grid == null || grid == undefined) { return;}
    var data = grid.dataItem($(row));
    if (data == null || data == undefined) { return; }
    if (data["Transid"] == null || data["Transid"] == undefined) { return; }

    $.post(FolioCommonValues.GetFolioByTransid, { transid: data["Transid"] }, function (result) {
        if (result != null && result != undefined && result.Success && result.Data != null && result.Data != undefined) {
            if (result.Data.Entity == null || result.Data.Entity == undefined) { return;}
            var entity = result.Data.Entity;
            var elementStr = "<td class=\"textright\">{0}</td><td class=\"folio_textright_value\">{1}</td>";
            var content = [];
            //1.账务信息
            content.push("<tr>");
            content.push(elementStr.replace("{0}", "账务名：").replace("{1}", replaceNull(entity.FolioName)));
            content.push(elementStr.replace("{0}", "").replace("{1}", ""));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "发生时间：").replace("{1}", convertJsonDate(entity.TransDate, "ToDateTimeString")));
            content.push(elementStr.replace("{0}", "发生营业日：").replace("{1}", convertJsonDate(entity.TransBsnsDate)));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "发生班次：").replace("{1}", replaceNull(result.Data.TransShiftName)));
            content.push(elementStr.replace("{0}", "操作员：").replace("{1}", replaceNull(entity.InputUser)));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "账号：").replace("{1}", replaceHid(entity.Regid, entity.Hid)));
            content.push(elementStr.replace("{0}", "原账号：").replace("{1}", replaceHid(entity.RegidFrom, entity.Hid)));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "房号：").replace("{1}", replaceNull(entity.RoomNo)));
            content.push(elementStr.replace("{0}", "账单：").replace("{1}", replaceNull(entity.resBillCode)));
            content.push("</tr>");
            infoTable.html(content.join("")); content = [];
            //2.消费付款信息
            var dcflagStr = replaceDcflag(entity.Dcflag);
            if (dcflagStr.length > 0) { folioLegend.text(dcflagStr + "信息"); }
            content.push("<tr>");
            content.push(elementStr.replace("{0}", "类型：").replace("{1}", dcflagStr));
            if (dcflagStr == "付款") {content.push(elementStr.replace("{0}", "付款类型：").replace("{1}", replacePaymentdesc(entity.Paymentdesc)));} else {content.push(elementStr.replace("{0}", "").replace("{1}", ""));}
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "项目：").replace("{1}", replaceNull(result.Data.ItemName)));
            content.push(elementStr.replace("{0}", "数量：").replace("{1}", replaceNull(entity.Quantity)));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "原币金额：").replace("{1}", replaceNull(entity.OriAmount)));
            content.push(elementStr.replace("{0}", "金额：").replace("{1}", replaceNull(entity.Amount)));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "状态：").replace("{1}", replaceStatus(entity.Status)));
            var cancelAndRecoveryDateStr = convertJsonDate(entity.CancelAndRecoveryDate, "ToDateTimeString");
            if (cancelAndRecoveryDateStr.length > 0) { var cancelAndRecoveryDateTitle = "作废恢复时间："; if (entity.Status == 51 || entity.Status == 53) { cancelAndRecoveryDateTitle = "作废时间："; } content.push(elementStr.replace("{0}", cancelAndRecoveryDateTitle).replace("{1}", cancelAndRecoveryDateStr)); } else { content.push(elementStr.replace("{0}", "").replace("{1}", "")); }
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "自动标记：").replace("{1}", replaceIsauto(entity.Isauto)));
            content.push(elementStr.replace("{0}", "间夜数：").replace("{1}", replaceNull(entity.Nights)));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "押金单号：").replace("{1}", replaceNull(entity.SeqNo)));
            content.push(elementStr.replace("{0}", "单号：").replace("{1}", replaceNull(entity.InvNo)));
            content.push("</tr><tr>");
            content.push(elementStr.replace("{0}", "参考号：").replace("{1}", replaceNull(entity.RefNo)));
            content.push(elementStr.replace("{0}", "备注：").replace("{1}", replaceNull(entity.Remark)));
            content.push("</tr>");
            folioTable.html(content.join("")); content = [];
            //3.结账信息
            var billidStr = replaceNull(entity.Billid);
            if (billidStr.length > 0) {
                content.push("<tr>");
                content.push(elementStr.replace("{0}", "结账时间：").replace("{1}", convertJsonDate(entity.SettleDate, "ToDateTimeString")));
                content.push(elementStr.replace("{0}", "结账营业日：").replace("{1}", convertJsonDate(entity.SettleBsnsdate)));
                content.push("</tr><tr>");
                content.push(elementStr.replace("{0}", "结账班次：").replace("{1}", replaceNull(result.Data.SettleShiftName)));
                content.push(elementStr.replace("{0}", "结账操作员：").replace("{1}", replaceNull(entity.SettleUser)));
                content.push("</tr>");
                settleTable.html(content.join("")); content = [];
                settleFieldset.css("display", "block");
            }
            else {
                settleTable.empty();
                settleFieldset.css("display", "none");
            }
            //4.账务作废与恢复
            if (result.Data.CancelAndRecoveryFolioLog != null && result.Data.CancelAndRecoveryFolioLog != undefined && result.Data.CancelAndRecoveryFolioLog.length > 0) {
                $.each(result.Data.CancelAndRecoveryFolioLog, function (index, item) {
                    var xType = "";
                    if (item.Value1 == "1" && item.Value2 == "51") {
                        xType = "作废";
                    } else if (item.Value1 == "51" && item.Value2 == "1") {
                        xType = "恢复";
                    }
                    if (xType.length > 0) {
                        content.push("<tr>");
                        content.push(elementStr.replace("{0}", xType + "记录时间：").replace("{1}", convertJsonDate(item.CDate, "ToDateTimeString")));
                        content.push(elementStr.replace("{0}", xType + "原因：").replace("{1}", replaceNull(item.Remark)));
                        content.push("</tr>");
                    }
                });
                cancelAndRecoveryFolioTable.html(content.join("")); content = [];
                cancelAndRecoveryFolioFieldset.css("display", "block");
            } else {
                cancelAndRecoveryFolioFieldset.css("display", "none");
                cancelAndRecoveryFolioTable.empty();
            }
            $("#folioRowInfoWindow").data("kendoWindow").center().open();
        }
    }, 'json');
}
function replaceNull(value) {
    if (value == null || value == undefined) {
        return "";
    }
    return value;
}
function replaceHid(value, hid) {
    if (value != null && value != undefined && value.length > 0 && hid != null && hid != undefined && hid.length > 0) {
        var index = value.indexOf(hid);
        if (index == 0) {
            return value.substring(hid.length);
        }
    }
    if (value == null || value == undefined) { value = ""; }
    return value;
}
function replaceDcflag(value) {//D:消费C: 付款
    var result = "";
    if (value == null || value == undefined) { return result; }
    value = value.toUpperCase();
    if (value == "D") {
        result = "消费";
    }
    else if (value == "C") {
        result = "付款";
    }
    else {
        result = value;
    }
    return result;
}
function replacePaymentdesc(value) {//A收押金 B退押金 C结帐收款 D结帐退款
    var result = "";
    if (value == null || value == undefined) { return result; }
    value = value.toUpperCase();
    switch (value) {
        case "A":
            result = "收押金";
            break;
        case "B":
            result = "退押金";
            break;
        case "C":
            result = "结帐收款";
            break;
        case "D":
            result = "结帐退款";
            break;
        case "E":
            result = "收订金";
            break;
        case "F":
            result = "退订金";
            break;
        default:
            result = value;
    }
    return result;
}
function replaceStatus(value) {//1:有效未结 2:已结 51:作废 52:待支付 53:反结作废
    var result = "";
    if (value == null || value == undefined) { return result; }
    switch (value) {
        case 1:
            result = "未结";
            break;
        case 2:
            result = "已结";
            break;
        case 51:
            result = "作废";
            break;
        case 52:
            result = "待支付";
            break;
        case 53:
            result = "反结作废";
            break;
        case 54:
            result = "临时账务";
            break;
        default:
            result = value;
    }
    return result;
}
function replaceIsauto(value) {//1夜租 2半日租 3日租 5钟点房租
    var result = "";
    if (value == null || value == undefined) { return result; }
    switch (value) {
        case 0:
            result = "无";
            break;
        case 1:
            result = "夜租";
            break;
        case 2:
            result = "半日租";
            break;
        case 3:
            result = "日租";
            break;
        case 5:
            result = "钟点房租";
            break;
        default:
            result = value;
    }
    return result;
}
function setQueryResBillIdPara() {
    return {
        resId: $("#Resid").val()
    };
}
//右侧客账列表右下角，预授权金额显示
function SumCardAuthAmountBySelectRegIds() {
    var sumCardAuthAmount = 0;
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds != null && selectedRegIds != undefined && selectedRegIds.length > 0) {
        var list = $("#gridFolioGuest").data("kendoGrid").dataSource.data();
        $.each(list, function (index, item) {
            if (item.CardAuthAmount != null && item.CardAuthAmount != undefined) {
                $.each(selectedRegIds, function (regid_index, regid) {
                    if (item.RegId == regid) {
                        sumCardAuthAmount += item.CardAuthAmount;
                    }
                });
            }
        });
    }
    $("#cardAuthTd").html("授权金额：" + sumCardAuthAmount.toFixed(2));
}