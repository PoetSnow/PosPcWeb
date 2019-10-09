//调账
function folioResBillButton_clicked(e) {
    if (e) { e.preventDefault(); }
    //判断是否有选中并且只能选一条账务明细
    var selectedFolioIds = [];
    $("input.folioFolioRowCheck:checked").each(function (index, obj) {
        selectedFolioIds.push($(obj).val());
    });
    if (selectedFolioIds.length <= 0) {
        jAlert("请先勾选要调账的账务记录！");
        folioMoreDivClose();
        return;
    }

    var folioInfoText = "数量：{0}笔，消费金额：{1}，支付金额：{2}，余额：{3}";
    folioInfoText = folioInfoText.replace("{0}", selectedFolioIds.length).replace("{1}", $("#folioDebitSumSpan").text()).replace("{2}", $('#folioCreditSumSpan').text()).replace("{3}", $("#folioBalanceSumSpan").text());    
    $("#adjustFolioWindow_FolioInfo").text(folioInfoText);
    $("#folioQueryResBillIdAll").data("kendoDropDownList").dataSource.read();

    var adjustFolioWindow = $("#adjustFolioWindow").data("kendoWindow");
    if (adjustFolioWindow == null || adjustFolioWindow == undefined) {
        $("#adjustFolioWindow").kendoWindow({
            width: "490px",
            title: "调账",
            visible: false,
            modal: true,
            actions: ["Close"],
            close: function () {
                $("#adjustFolioWindow_FolioInfo").text("");
                $("#folioQueryResBillIdAll").data("kendoDropDownList").value("");
                folioMoreDivClose();
            }
        }).data("kendoWindow").center().open();
    } else {
        adjustFolioWindow.center().open();
    }
}
//关闭
function adjustFolioWindow_closeEditFormButton_clicked(e) {
    adjustFolioWindowClose();
}
//确定
function adjustFolioWindow_saveEditFormButton_clicked(e) {
    //判断是否有选中并且只能选一条账务明细
    var selectedFolioIds = [];
    $("input.folioFolioRowCheck:checked").each(function (index, obj) {
        selectedFolioIds.push($(obj).val());
    });
    if (selectedFolioIds.length <= 0) {
        jAlert("请先勾选要调账的账务记录！");
        adjustFolioWindowClose();
        return;
    }

    var resBillId = $("#folioQueryResBillIdAll").data("kendoDropDownList").value();
    if (resBillId == null || resBillId == undefined || $.trim(resBillId) == "") {
        jAlert("请选择目标账单！");
        return;
    }

    $.post(FolioCommonValues.AdjustFolio, { resId: $("#Resid").val(), folioIds: selectedFolioIds, toResBillCode: resBillId }, function (data) {
        if (data.Success) {
            jAlert("调账成功！", "知道了");
            //刷新客账明细
            folioQueryButton_clicked();
            folioQueryResBillIdRefresh();
        } else {
            ajaxErrorHandle(data);
        }
        adjustFolioWindowClose();
    }, 'json');
}
function folioQueryResBillIdRefresh() {
    $("#folioQueryResBillId").data("kendoDropDownList").dataSource.read();
}
function adjustFolioWindowClose() {
    $("#adjustFolioWindow").data("kendoWindow").close();
}