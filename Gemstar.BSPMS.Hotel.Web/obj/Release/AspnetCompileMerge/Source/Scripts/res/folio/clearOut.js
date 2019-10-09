//取消离店js
//取消结账按钮点击事件
function folioCancelOutButton_clicked(e) {
    if (e) { e.preventDefault(); }

    //判断是否有选中至少一条客账明细
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds.length == 0) {
        jAlert("请选择要取消离店的房间");
        return;
    }
    //取出选中的房号等资料，以便提示
    var win = $("#folioDelayPayReasonWindow").data("kendoWindow");
    var grid = $("#gridFolioGuest").data("kendoGrid");
    var selectedRoomNos = [];
    $("input.folioGuestRowCheck:checked").each(function (index, obj) {
        var $tr = $(obj).parents("tr");
        var dataitem = grid.dataItem($tr);
        if (dataitem.StatuName == "迟付" || dataitem.StatuName == "已结") {
            selectedRoomNos.push(dataitem.RoomNo);
        }
    });
    if (selectedRoomNos.length == 0) {
        jAlert("请选择已经迟付或已结的房间");
        return;
    }
    var confirmMsg = "确定要将"+selectedRoomNos.join(',')+" 共"+selectedRoomNos.length+"间房取消离店吗?";

    jConfirm(confirmMsg, "确定", "返回", function (confirmed) {
        if (confirmed) {
            //判断是否有选中至少一条客账明细
            var selectedRegIds = folioGetSelectedRegIdArray();
            if (selectedRegIds.length == 0) {
                jAlert("请选择要取消离店的房间");
                return;
            }
            //检查是否有可以取消结账的已结批次号
            $.post(FolioCommonValues.CancelOut, { regIds: selectedRegIds.join(',') }, function (data) {
                if (data.Success) {
                    jAlert("取消离店成功！\r\n续住需<延期>。", "知道了");

                    folioGuestGrid_refresh();
                    folioQueryButton_clicked();
                } else {
                    //jAlert("取消离店失败，原因：" + data.Data);
                    ajaxErrorHandle(data);
                }
            }, 'json');
        }
    });
}