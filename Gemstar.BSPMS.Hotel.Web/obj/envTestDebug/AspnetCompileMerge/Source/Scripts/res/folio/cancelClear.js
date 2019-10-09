//取消清账js
//取消结账按钮点击事件
function folioCancelClearButton_clicked(e) {
    if (e) { e.preventDefault(); }
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来取消清账", "确定");
        return;
    }
    //检查是否有可以取消结账的已结批次号
    $.post(FolioCommonValues.AjaxCheckoutBatchNos, { resId: resIdValue }, function (data) {
        if (data.Success) {
            if (data.Data.length == 0) {
                jAlert("没有可以取消清账的已结批次，只能取消当前营业日的已结账");
            } else {
                var trHtml = "";
                for (var i = 0; i < data.Data.length; i++) {
                    var rowClass = i % 2 == 0 ? "" : "k-alt";
                    var info = data.Data[i];
                    trHtml += '<tr role="row" class="' + rowClass + '"><td>' + info.SettleDate + '</td><td>' + info.Amount + '</td><td>' + info.Name + '</td><td>' + info.SettleUser + '</td><td><button class="k-button" data-billid="' + info.BillId + '">取消清账</button></td></tr>';
                }
                $("#folioCancelCheckoutBatchNoSelectWindow").find("tbody").empty().html(trHtml);
                //注册取消结账按钮点击事件
                $("#folioCancelCheckoutBatchNoSelectWindow").find("tbody").find("button[data-billid]").click(function (e) {
                    e.preventDefault();
                    var billId = $(this).data("billid");
                    folioCancelClearCancelSpecificBatchNo(billId);
                });
                $("#folioCancelCheckoutBatchNoSelectWindow").data("kendoWindow").center().open();
            }
        } else {
            //jAlert("查询可取消清账的已结批次失败，原因：" + data.Data);
            ajaxErrorHandle(data);
        }
    }, 'json');
}
//取消清账窗口中的已结批次中的取消按钮点击事件,取消指定结账批次
function folioCancelClearCancelSpecificBatchNo(batchNo) {
    $.post(FolioCommonValues.CancelClear, { billId: batchNo }, function (data) {
        if (data.Success) {
            jAlert("取消清账成功");
            $("#folioCancelCheckoutBatchNoSelectWindow").data("kendoWindow").close();

            folioGuestGrid_refresh();
            folioQueryButton_clicked();
        } else {
            // jAlert("取消清账失败，原因：" + data.Data);
            ajaxErrorHandle(data);
        }
    }, 'json');
}