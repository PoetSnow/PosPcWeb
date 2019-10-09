//批量修改订单状态
//初始化事件
function batchChangeOrderStatusWindow_Initialization() {
    //初始化窗口
    $("#batchChangeOrderStatusWindow").kendoWindow({
        width: "650px",
        height: "432px",
        title: "批量修改订单状态",
        content: "",
        iframe: true,
        modal: true,
        visible: false,
        resizable: false,
        scrollable: false,
        actions: ["Close"],
        close: function () {
            $("#batchChangeOrderStatusWindow").empty();
            try { OrderCustomer.GetRemote(); } catch (e) { }
        },
        refresh: function () {
            $("#batchChangeOrderStatusWindow").data("kendoWindow").center().open();
        },
        activate: function () {
            $("#batchChangeOrderStatusWindow").css("overflow-y", "hidden");
        }
    });
}
//按钮点击事件
function batchChangeOrderStatusButton_clicked(e) {
    if (e) { e.preventDefault(); }
    if (!$("#batchChangeOrderStatusWindow").data("kendoWindow")) { batchChangeOrderStatusWindow_Initialization(); }
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来批量更改", "确定");
        return;
    }
    $("#batchChangeOrderStatusWindow").data("kendoWindow").refresh({
        url: CustomerCommonValues.BatchChangeOrderStatus + "?resId=" + resIdValue,
        type: "GET",
        iframe: true,
        cache: false,
    });
}