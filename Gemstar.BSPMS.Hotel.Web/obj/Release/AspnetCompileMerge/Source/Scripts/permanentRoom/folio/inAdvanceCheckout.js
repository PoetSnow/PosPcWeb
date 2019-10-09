//预结JS
function folioInAdvanceCheckoutButton_click(e) {

    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds == null || selectedRegIds == undefined  || selectedRegIds.length <= 0) {
        jAlert("请选择要预结的房间");
        return;
    }

    var regids = [];
    $.each(selectedRegIds, function (index, item) {
        regids.push(replaceHid(item, FolioCommonValues.HotelId));
    });
    
    var obj = $("#folioInAdvanceCheckoutWindow");
    var html = [];
    html.push("<table class=\"editFormTable\">");
    html.push("<tbody>");
    html.push("<tr><td class=\"textright\">账号</td><td style=\"height:30px;\">" + regids.join(",") + "</td></tr>");
    html.push("<tr><td class=\"textright\">预结至日期</td><td><input id=\"folioInAdvanceCheckoutDate\" /></td></tr>");

    if (e == "checkout" || e == "out") {
        html.push("<tr><td class=\"textright\">本次读数</td><td style=\"padding-top:8px;\">" + waterAndElectricityAddFolio.GetHtmlForCheckout() + "</td></tr>");
    }

    html.push("<tr><td class=\"textright\"></td><td style=\"padding-top:8px;\"><button type=\"button\" class=\"k-button\" id=\"folioInAdvanceCheckoutWindow_btn\" onclick=\"folioInAdvanceCheckoutWindow_btn_click()\">确定</button></td></tr>");
    html.push("</tbody>");
    html.push("</table>");

    obj.html(html.join(""));

    $("#folioInAdvanceCheckoutDate").kendoDatePicker({format:"yyyy-MM-dd"});

    var folioInAdvanceCheckoutWindow = obj.data("kendoWindow");
    if (folioInAdvanceCheckoutWindow == null || folioInAdvanceCheckoutWindow == undefined) {
        obj.kendoWindow({
            width: "470px",
            height: "auto",
            title: "预结",
            visible: false,
            modal: true,
            actions: ["Close"],
            close: function () {
                $("#folioInAdvanceCheckoutWindow").empty();
            },
        }).data("kendoWindow").center().open();
    } else {
        folioInAdvanceCheckoutWindow.center().open();
    }
};
function folioInAdvanceCheckoutWindow_btn_click(e) {
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds == null || selectedRegIds == undefined || selectedRegIds.length <= 0) {
        jAlert("请选择要预结的房间");
        return;
    }
    var regIdsStr = selectedRegIds.join(',');

    var endDate = $("#folioInAdvanceCheckoutDate").data("kendoDatePicker").value();
    if (endDate == null || endDate == undefined) {
        jAlert("请选择要预结的日期");
        return;
    }

    if (!waterAndElectricityAddFolio.Submit()) { return;}

    $.post(FolioCommonValues.InAdvanceCheckout, { regIds: regIdsStr, endDate: endDate.ToDateString() }, function (data) {
        if (data.Success) {
            jAlert("预结成功", "知道了", function () {
                $("#folioInAdvanceCheckoutWindow").data("kendoWindow").close();
                folioMoreDivClose();
            });
            folioGuestGrid_refresh();
            folioQueryButton_clicked();
        }
        else {
            ajaxErrorHandle(data);
        }
    }, 'json');
}
function inAdvanceCheckout_GetEndDate() {
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds == null || selectedRegIds == undefined || selectedRegIds.length <= 0) {
        jAlert("请选择要预结的房间");
        return;
    }
    var regIdsStr = selectedRegIds.join(',');
    $.post(FolioCommonValues.InAdvanceCheckout_GetEndDate, { regIds: regIdsStr }, function (result) {
        if (result.Success) {
            var msg = '<p>订单情况:</p><div class="k-widget k-grid"><table class="k-selectable"><thead class="k-grid-header"><tr><th class="k-header">账号</th><th class="k-header">最后日期（租金已生成）</th></tr></thead>';
            for (var i = 0; i < result.Data.length; i++) {
                var info = result.Data[i];
                msg += '<tr><td>' + (info["shortRegid"] == null ? "" : info["shortRegid"]) + '</td><td>' + (info["endDate"] == null ? "" : info["endDate"]) + '</td></tr>';
            }
            msg += '</table></div>';
            $("#permanentRoomCheckOut_InAdvanceCheckout_Div").html(msg);
        }
        else {
            ajaxErrorHandle(data);
        }
    }, 'json');
}