//作废账务 与 恢复作废的账务 js
function folioCancelAndRecoveryButton_clicked(e) {
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

    $.post(FolioCommonValues.CancelAndRecoveryFolio, { transId: selectedFolioIds.join(','), reason: "", isCheck: true }, function (data) {
        if (data.Success) {
            if (data.Data != null && data.Data != undefined && data.Data.length > 0) {
                folioCancelAndRecoveryFunc(selectedState, selectedFolioIds);
            }
        } else {
            ajaxErrorHandle(data); folioMoreDivClose(); return;
        }
    }, 'json');

}
function folioCancelAndRecoveryFunc(selectedState, selectedFolioIds) {
    var action = (selectedState === "1" ? "作废" : "恢复");
    var msg = "账号：{0}，房号：{1}，项目：{2}，金额：{3}。\n请填写" + action + "原因：";
    var dataItem = $("#gridFolioFolio").data("kendoGrid").dataItem($("#gridFolioFolio input.folioFolioRowCheck:checked[type='checkbox'][value='" + selectedFolioIds[0] + "']").parents("tr"));
    if (dataItem != null && dataItem != undefined) {
        msg = msg.replace("{0}", replaceHid(dataItem.RegId, FolioCommonValues.HotelId)).replace("{1}", dataItem.RoomNo).replace("{2}", dataItem.ItemName).replace("{3}", (dataItem.DebitAmount == null ? dataItem.CreditAmount : dataItem.DebitAmount));
    }

    jPrompt(msg, "", (action + "账务"), "取消", function (result) {
        if (result != null) {
            $.post(FolioCommonValues.CancelAndRecoveryFolio, { transId: selectedFolioIds.join(','), reason: result, isCheck: false }, function (data) {
                if (data.Success) {
                    if (data.Data != null && data.Data != undefined && data.Data.length > 0) {
                        jAlert(data.Data);
                    }
                    folioGuestGrid_refresh();
                } else {
                    ajaxErrorHandle(data); return;
                }
                folioMoreDivClose();
            }, 'json');
        }else {
            folioMoreDivClose();
        }
    }, "账务作废与恢复");
}