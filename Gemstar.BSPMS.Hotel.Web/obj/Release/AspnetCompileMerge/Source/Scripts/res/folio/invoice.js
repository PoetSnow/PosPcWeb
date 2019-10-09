//开票
//初始化事件
function folioInvoiceWindow_Initialization() {
    //初始化窗口
    $("#folioInvoiceWindow").kendoWindow({
        width: "1200px",
        height : "485px",
        title: "发票管理",
        content: "",
        iframe: true,
        modal: true,
        visible: false,
        resizable: true,
        scrollable: true,
        actions: [
            "Close"
        ],
        close: function () {
            $("#folioInvoiceWindow").empty();
        },
        refresh: function () {
            $("#folioInvoiceWindow").data("kendoWindow").center().open();
        },
        activate: function () {
            $("#folioInvoiceWindow").css("overflow-y", "hidden");
        }
    });
}
//发票管理按钮点击事件
function folioInvoiceButton_clicked(e) {
    if (e) { e.preventDefault(); }
    if (!$("#folioInvoiceWindow").data("kendoWindow")) { folioInvoiceWindow_Initialization(); }
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来开票", "确定");
        return;
    }
    $("#folioInvoiceWindow").data("kendoWindow").refresh({
        url: FolioCommonValues.Invoice + "?type=0&id=" + resIdValue,
        type: "GET",
        iframe: true,
        cache: false,
    });
}