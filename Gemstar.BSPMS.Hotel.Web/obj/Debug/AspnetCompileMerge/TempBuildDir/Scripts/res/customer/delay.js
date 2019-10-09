//延期
var DelayWindow = {
    //初始化
    Initialization: function () {
        $("#delayWindow").kendoWindow({
            width: "900px",
            title: "延期",
            visible: false,
            modal: true,
            actions: ["Close"],
            activate: function () {
                var checkObj = $("input#checkAllDelay")[0];
                checkObj.checked = false;
                checkObj.click();
                document.getElementById("submitDelayButton").focus();
                var $checked = $("#delayTbody").find("input[name='checkDelay']");
                var editDate = new Date();editDate.setFullYear(1970);
                $checked.each(function (index, item) {
                    if (!item.disabled) {
                        var tr = $(item).parents("tr");
                        var depDate = tr.find("input[name='txtDelay']").data("kendoDateTimePicker").value();
                        if (depDate != null && depDate != undefined && editDate < depDate) {
                            editDate = depDate;
                        }
                    }
                });
                if (editDate.getFullYear() == 1970) {
                    editDate = new Date(new Date().valueOf() + 1 * 24 * 60 * 60 * 1000);
                }
                $("#editedDetDate").data("kendoDateTimePicker").value(editDate);
                $("#editedDetDateHidden").val(new Date(editDate.valueOf() - 1 * 24 * 60 * 60 * 1000).ToDateTimeString());
                $("#editedDetDays").data("kendoNumericTextBox").value(1);
                DelayWindow.EditDetDate.Init();
            },
        });
        $("#editDetDateButton").unbind("click").click(function (e) { DelayWindow.Edit(e); });
        $("#submitDelayButton").unbind("click").click(function (e) { DelayWindow.Save(e); });
        $("#cancelDelayButton").unbind("click").click(function (e) { DelayWindow.Cancel(e); });
        $("#checkAllDelay").unbind("change").change(function (e) { DelayWindow.SelectAll(); });
    },
    //延期弹框
    Open: function () {
        if (!$("#delayWindow").data("kendoWindow")) { DelayWindow.Initialization(); }
        var tBody = $("#delayTbody");
        tBody.empty();
        var list = OrderList.Get();
        if (list == null || list == undefined) { return; }
        var length = list.length;

        //组装表格
        var trListHtml = [];
        var ii = 0;
        for (var i = 0; i < length; i++) {
            if (list[i].Status == "I") {
                var isUseScore = list[i].RateCodeEntity.IsUseScore;
                var disabled = (isUseScore) ? " disabled=\"disabled\" " : " ";

                var trHtml = [];
                trHtml.push("<tr " + (ii % 2 == 0 ? "" : "class='k-alt'") + ">"); ii++;
                var checkId = "checkDelay_" + i;
                trHtml.push("<td><input style=\"padding-left:0px;\" type=\"checkbox\" class=\"k-checkbox\" id=\"" + checkId + "\" name=\"checkDelay\" " + disabled + "/><label class=\"k-checkbox-label\" for=\"" + checkId + "\"></label></td>");//全选
                trHtml.push("<td data-regid=\"" + list[i].Regid + "\">" + list[i].Regid.replace(list[i].Hid, '') + "</td>");//账号
                trHtml.push("<td>" + list[i].RoomNo + "</td>");//房号
                var priceStr = "";//房价
                if (list[i].OrderDetailPlans != null && list[i].OrderDetailPlans != undefined && list[i].OrderDetailPlans.length > 0) {
                    var prices = [];
                    var priceLength = list[i].OrderDetailPlans.length;
                    for (var j = 0; j < priceLength; j++) { if (list[i].OrderDetailPlans[j]["Ratedate"] == CustomerCommonValues.BusinessDate) { prices.push(list[i].OrderDetailPlans[j]["Price"]); break; } }
                    priceStr = prices.join(",");
                }
                if (priceStr == "") {
                    try {
                        var rate = JSON.parse(list[i].OriginResDetailInfo).Rate;
                        if (rate != null && rate != undefined && !isNaN(rate) && rate >= 0) { priceStr = rate; }
                    } catch (e) { }
                }
                trHtml.push("<td title=\"" + priceStr + "\">" + priceStr + "</td>");//房价
                trHtml.push("<td>" + list[i].Guestname + "</td>");//客人名
                trHtml.push("<td>" + list[i].ArrDate.substring(0, 16) + "</td>");//抵店时间
                trHtml.push("<td>" + list[i].DepDate.substring(0, 16) + "</td>");//离店时间
                if (isUseScore) {
                    trHtml.push("<td>积分换房不能延期</td>");//延期后的离店时间
                } else {
                    trHtml.push("<td data-depdate=\"" + list[i].DepDate.substring(0, 16) + "\"><input name=\"txtDelay\" /></td>");//延期后的离店时间
                }
                trHtml.push("</tr>");
                trListHtml.push(trHtml.join(""));
            }
        }
        tBody.html(trListHtml.join(""));
        //初始化日期控件
        $("[name='txtDelay']").each(function () {
            var dataDepdate = $(this).parents("[data-depdate]").attr("data-depdate");
            if (!(dataDepdate != null && dataDepdate != undefined && dataDepdate.length > 0 && dataDepdate != "null")) {
                dataDepdate = new Date().valueOf();
            }
            var depDate = new Date(dataDepdate);
            $(this).kendoDateTimePicker({
                value: new Date(depDate.valueOf() + 1 * 24 * 60 * 60 * 1000),
                format: "yyyy-MM-dd HH:mm"
            }).data("kendoDateTimePicker").min(new Date());
            $(this).css("color", "#e62722");
        });
        //显示弹窗
        $("#delayWindow").data("kendoWindow").center().open();
    },
    //保存
    Save: function () {
        var $checked = $("#delayTbody").find("input[name='checkDelay']:checked");
        if ($checked.length <= 0) { jAlert("请先选择要延期的记录", "前往选择"); return; }
        //获取参数并验证
        var data = [];
        $checked.each(function (index, item) {
            var tr = $(item).parents("tr");
            var regid = tr.find("td[data-regid]").attr("data-regid");
            if (regid == null || regid == undefined || regid.length <= 0) {
                return;
            }
            var depDate = tr.find("input[name='txtDelay']").data("kendoDateTimePicker").value();
            if (depDate == null || depDate == undefined) {
                jAlert("请选择延期时间");
                return;
            }
            var txtDelay = depDate.ToDateTimeWithoutSecondString();
            if (txtDelay == null || txtDelay == undefined || txtDelay.length <= 0) {
                jAlert("请选择延期时间");
                return;
            }
            data.push({ Key: regid, Value: txtDelay });
        });
        //延期
        $.post(CustomerCommonValues.DelayDepDate, { data: data, saveContinue: $("#SaveContinue").val(), delayContinue: $("#delayContinue").val() }, function (result) {
            if (result.Success) {
                $("#delayTableDiv").find("tbody").empty();
                $("#delayWindow").data("kendoWindow").close();
                OrderCustomer.RefreshData(result, "DelayWindow.Save");
                //提示弹出门锁卡
                var lockType = CustomerCommonValues.lockType;
                var lockCode = CustomerCommonValues.lockCode;
                if (lockType != null && lockType != undefined && lockType.length > 0 && lockCode != null && lockCode != undefined && lockCode.length > 0) {
                    jConfirm("延期成功！\n" + result.Msg + "是否制作门锁卡？", "  是  ", "  否  ", function (confirmed) {
                        if (confirmed) { document.getElementById("btnLock").click(); }
                    });
                } else {
                    jAlert("延期成功！\n" + result.Msg, "知道了");
                }
            }
            else {
                if (result.ErrorCode == 3) {
                    jConfirm(result.Data, "确定", "取消", function (confirmed) {
                        if (confirmed) {
                            $("#delayContinue").val("1");
                            DelayWindow.Save();
                            return;
                        }
                    });
                } else {
                    ajaxErrorHandle(result);
                    OrderCustomer.RefreshData(result, "DelayWindow.Save");
                }
            }
            $("#delayContinue").val("0");
        }, 'json');
    },
    //取消
    Cancel: function () {
        $("#delayTbody").find("tbody").empty();
        $("#delayWindow").data("kendoWindow").close();
    },
    //全选
    SelectAll: function () {
        var checked = $("input#checkAllDelay")[0].checked;
        $("#delayTbody").find("input[name='checkDelay']").each(function (index, item) {
            if (!item.disabled) {
                item.checked = checked;
            }
        });
    },
    //调整日期
    Edit: function () {
        var $checked = $("#delayTbody").find("input[name='checkDelay']:checked");
        if ($checked.length == 0) { jAlert("请先选择要延期的记录", "前往选择"); return; }
        var editedDepDateValue = $("#editedDetDate").data("kendoDateTimePicker").value();
        if (editedDepDateValue == null) { jAlert("请先输入要调整为的延期", "前往输入"); return; }

        $checked.each(function (index, item) {
            $(item).parents("tr").find("input[name='txtDelay']").data("kendoDateTimePicker").value(editedDepDateValue);
        });
    },
    //调整
    EditDetDate: {
        Init: function () {
            $("#editedDetDate").data("kendoDateTimePicker").bind("change", function () {
                DelayWindow.EditDetDate.Change("date");
            });
            $("#editedDetDays").data("kendoNumericTextBox").bind("change", function() {
                DelayWindow.EditDetDate.Change("day");
            });
        },
        Change: function (type) {
            var editedDetDateHidden = $("#editedDetDateHidden").val();
            var startDate = new Date(editedDetDateHidden);
            if (startDate == null || startDate == undefined) { return; }
            if (type == "date") {
                var depDate = $("#editedDetDate").data("kendoDateTimePicker").value();
                if (depDate == null || depDate == undefined) { depDate = startDate; }
                var num = parseInt((kendo.date.getDate(depDate) - kendo.date.getDate(startDate)) / (1000 * 60 * 60 * 24));
                $("#editedDetDays").data("kendoNumericTextBox").value(num < 0 ? 0 : num);

            } else if (type == "day") {
                var days = $("#editedDetDays").data("kendoNumericTextBox").value();
                if (days == null || days == undefined || days < 0) { days = 0; }
                var newDate = kendo.date.addDays(kendo.date.getDate(startDate), days);

                var depDate = $("#editedDetDate").data("kendoDateTimePicker").value();
                if (depDate == null || depDate == undefined) { depDate = startDate; }
                $("#editedDetDate").data("kendoDateTimePicker").value(kendo.date.getDate(newDate).ToDateString() + " " + depDate.ToTimeString());
            }
        }
    }
};