//打印账单的js
//打印账单按钮点击事件
function folioPrintButton_clicked(e, selectedRegIdsValue, selectedTransIdsValue, addTransIds) {
    debugger;
    if (e) {
        e.preventDefault();
    }
    //判断是否有选中至少一条客账明细
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIdsValue != null && selectedRegIdsValue != undefined && selectedRegIdsValue.length > 0) {
        selectedRegIds = selectedRegIdsValue;
    }
    if (selectedRegIds.length == 0) {
        jAlert("请选择要打印账单的房间");
        return;
    }
    //判断是否有选中至少一条账务明细
    var selectedFolioIds = folioGetSelectedFolioIdArray();
    if (selectedTransIdsValue != null && selectedTransIdsValue != undefined && selectedTransIdsValue.length > 0) {
        selectedFolioIds = selectedTransIdsValue;
    }
    if (addTransIds != null && addTransIds != undefined && addTransIds.length > 0) {
        selectedFolioIds.push(addTransIds);
    }
    if (selectedFolioIds.length == 0) {
        jAlert("请选择要打印账单的明细");
        return;
    }
    var isSignature = $("#IsSignature").val();
    var printIndex = isSignature == 1 ? 10 : 1;
    var parameterValues = "@inputUser=" + FolioCommonValues.CurrentUserName + "&@regids={regids}&@transids={transids}&@addTransids={addTransids}";
    parameterValues = parameterValues.replace("{regids}", selectedRegIds.join(',')).replace("{transids}", selectedFolioIds.join(',')).replace("{addTransids}", ((addTransIds != null && addTransIds != undefined && addTransIds.length > 0) ? addTransIds: ""));
    $.post(FolioCommonValues.AddQueryParaTemp, { ReportCode: "up_print_bill", ParameterValues: parameterValues, ChineseName: "账单", print: printIndex }, function (result) {
        if (result.Success) {
            if (printIndex == 10) {
                try {
                    var name = $("#folioAddRegId").data("kendoDropDownList").dataItem().RegName;
                    var names = name.split("　");
                    var roomNo = names[0].split(":")[1];
                    var regid = names[1].split(":")[1];
                    window.open(result.Data + "&sType=2&regid=" + regid + "&roomNo=" + roomNo);
                } catch (e) {
                    var select = $("#gridFolioGuest").data("kendoGrid").select();
                    var gridData = $("#gridFolioGuest").data("kendoGrid").dataItem(select);
                    if (gridData.RegId.length > 6)
                        window.open(result.Data + "&sType=2&regid=" + gridData.RegId.substring(6) + "&roomNo=" + gridData.RoomNo);
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
function checkFolioPrintButton_clicked(e) {
    checkRentCharge("folioPrint");
    //folioPrintButton_clicked(e);
}