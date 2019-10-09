//客账操作按钮相关js代码开始
function orderFolioInitialization() {
    gridFolioFolio_Initialization();
    gridFolioGuest_Initialization();
    folioAddFolioDiv_Initialization();
    folioCancelCheckoutBatchNoSelectWindow_Initialization();
    //folioCardAuthWindow_Initialization();
    folioCheckoutDayChargeWindow_Initialization();
    //folioInvoiceWindow_Initialization();
    //folioAddItemsDiv_Initialization();

    $("#folioAddButton").click(function (e) { folioAddButton_clicked(e); });
    $("#folioCheckoutButton").click(function (e) { folioCheckoutButton_clicked(e); });
    $("#folioDelayPayButton").click(function (e) { folioDelayPayButton_clicked(e); });
    $("#folioSettleButton").click(function (e) { folioSettleButton_clicked(e); });
    $("#folioAuthButton").click(function (e) { folioCardAuthButton_clicked(e); });
    $("#folioTransferButton").click(function (e) { folioTransferButton_clicked(e); });
    $("#folioPrintButton").click(function (e) { checkFolioPrintButton_clicked(e);});
    $("#folioCancelCheckoutButton").click(function (e) { folioCancelCheckoutButton_clicked(e); });
    $("#folioCancelClearButton").click(function (e) { folioCancelClearButton_clicked(e); });
    $("#folioCancelOutButton").click(function (e) { folioCancelOutButton_clicked(e); });
    $("#folioInvoiceButton").click(function (e) { folioInvoiceButton_clicked(e); });
    $("#folioItmsButton").click(function (e) { folioItemsButton_clicked(e) });
    $("#folioCancelAndRecoveryButton").click(function (e) { folioCancelAndRecoveryButton_clicked(e) });
    $("#folioResBillButton").click(function (e) { folioResBillButton_clicked(e) });
    $("#folioMoreButton").click(function (e) { folioMoreButton_clicked(e) });
    $("#adjustFolioWindow_saveEditFormButton").click(function (e) { adjustFolioWindow_saveEditFormButton_clicked(e); });
    $("#adjustFolioWindow_closeEditFormButton").click(function (e) { adjustFolioWindow_closeEditFormButton_clicked(e); });
    $("#folioRefundButton").click(function (e) { folioRefundButton_clicked(e); });
    $("#folioInAdvanceCheckoutButton").click(function (e) { folioInAdvanceCheckoutButton_click(e); });
    $("#folioSplitFolioButton").click(function (e) { orderSplitFolioWindow.Open(); });
    function folioMoreButton_clicked(e) {
        var folioMoreDivWindow = $("#folioMoreDiv").data("kendoWindow");
        if (folioMoreDivWindow == null || folioMoreDivWindow == undefined) {
            $("#folioMoreDiv").kendoWindow({
                width: "200px",
                height:"188px",
                title: "更多操作",
                visible: false,
                modal: true,
                actions: ["Close"],
                open: function () {
                    //长包房-预授权按钮位置
                    var div = $("#folioMoreDiv").parent(".k-widget.k-window"); div.css("height", "188px");
                    var btn = $("#folioAuthButtonCopy"); btn.css("display", "none");
                    if ($("#PermanentRoomOrderAdd").length > 0) {
                        btn.css("display", "inline-block");
                        div.css("height","230px");
                    }
                },
            }).data("kendoWindow").center().open();
        } else {
            folioMoreDivWindow.center().open();
        }
    }

    //长包房-预授权按钮 和 预结按钮 位置
    if ($("#PermanentRoomOrderAdd").length > 0) {
        $("#folioAuthButton").css("display", "none"); $("#folioInAdvanceCheckoutButton").css("display", "inline-block");
        $("#folioAuthButtonCopy").click(function (e) { folioCardAuthButton_clicked(e); });
    } else {
        $("#folioAuthButton").css("display", "inline-block"); $("#folioInAdvanceCheckoutButton").css("display", "none");
    }

};
function folioMoreDivClose() {
    try { $("#folioMoreDiv").data("kendoWindow").close(); } catch (e) { }
}