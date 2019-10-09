//转账的js
//转账按钮点击事件
function folioTransferButton_clicked(e) {
    if (e) {
        e.preventDefault();
    }
    //判断当前选择的状态是否是未结，如果不是，则提示并且不允许进行操作
    var selectedState = $("input[name='folioStatu']:checked").val();
    if (selectedState != "1") {
        jAlert("只有在未结状态下才可以进行转账操作");
        return;
    }
    //判断是否有选中至少一条账务明细
    $("#folioCheckoutAddedTransIds").val("");
    var selectedFolioIds = folioGetSelectedFolioIdArray();
    if (selectedFolioIds.length == 0) {
        jAlert("请选择要转账的明细账");
        return;
    }
    var notRegIds = "";
    var selectedRegIds = folioGetSelectedRegIdArray();
    if (selectedRegIds.length > 0) {
        notRegIds = selectedRegIds.join(",");
    }
    //显示通用的选择客账窗口来选择转账到的目标客账，并且在选择后的回调中继续处理
    showCommonSelectRegidWindow({ isSettle: 0, status: "I", callback: folioTransferAfterTargetRegidSelected, notRegIds: notRegIds, filterStatus: "I,O,R" });
}
function folioTransferAfterTargetRegidSelected(selectedRegId, roomNo, guestName) {
    if (!selectedRegId) {
        jAlert("请选择要转到的目标客账");
        return;
    }
    //判断是否有选中至少一条账务明细
    var selectedFolioIds = folioGetSelectedFolioIdArray();
    if (selectedFolioIds.length == 0) {
        jAlert("请选择要转账的明细账");
        return;
    }
    var msg = "";
    try{
        var regids =[];
        var gridFolioFolio = $("#gridFolioFolio").data("kendoGrid");
        $.each(selectedFolioIds, function (index, item) {
            var dataItem = gridFolioFolio.dataSource.get(item);
            if (dataItem != null && dataItem != undefined && dataItem.RegId != null && dataItem.RegId != undefined && $.trim(dataItem.RegId).length > 0) {
                if (!arrayContains(regids, $.trim(dataItem.RegId))) {
                    regids.push($.trim(dataItem.RegId));
                }
            }
        });
        var isTrue = false;
        var msgTemp = "&#12288;&#12288;账号:{0},房号:{1},客人名:{2}\n";
        if (regids.length > 0) {
            msg += "转出：\n";
            var gridFolioGuest = $("#gridFolioGuest").data("kendoGrid");
            $.each(regids, function (index, item) {
                var dataItem = gridFolioGuest.dataSource.get(item);
                if (dataItem != null && dataItem != undefined) {
                    msg += msgTemp.replace("{0}", dataItem.RegId.replace(FolioCommonValues.HotelId, "")).replace("{1}", (dataItem.RoomNo == null || dataItem.RoomNo == undefined ? "" : dataItem.RoomNo)).replace("{2}", (dataItem.GuestName == null || dataItem.GuestName == undefined ? "" : dataItem.GuestName));
                    isTrue = true;
                }
            }); 
        }
        if (isTrue) {
            msg += "转入：\n" + msgTemp.replace("{0}", selectedRegId.replace(FolioCommonValues.HotelId, "")).replace("{1}", (roomNo == null || roomNo == undefined ? "" : roomNo)).replace("{2}", (guestName == null || guestName == undefined ? "" : guestName));
            msg += "转账金额：\n" + ("&#12288;&#12288;消费金额:{0},付款金额:{1}\n").replace("{0}", $("#folioDebitSumSpan").text()).replace("{1}", $("#folioCreditSumSpan").text());
        } else {
            msg = "";
        }
    } catch (e) { msg = ""; }
    jConfirm(msg + "确定要转账吗?", "确定", "返回", function (confirmed) {
        if (confirmed) {
            $.post(FolioCommonValues.Transfer, { transIds: selectedFolioIds.join(','), regId: selectedRegId }, function (data) {
                if (data.Success) {
                    jAlert("转账成功");
                    closeCommonSelectRegidWindow();

                    folioGuestGrid_refresh();
                    folioQueryButton_clicked();
                } else {
                    //jAlert("转账失败，原因：" + data.Data);
                    ajaxErrorHandle(data);
                    return;
            }
            }, 'json');
        } else {
            closeCommonSelectRegidWindow();
    }
});
}
function arrayContains(arr, obj) {
    var i = arr.length;  
    while (i--) {  
        if (arr[i] === obj) {  
            return true;  
        }  
    }  
    return false;  
}