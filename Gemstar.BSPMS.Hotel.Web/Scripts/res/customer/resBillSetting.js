//开票
//初始化事件
function resBillSettingWindow_Initialization() {
    //初始化窗口
    $("#resBillSettingWindow").kendoWindow({
        width: "850px",
        height: "450px",
        title: "账单设置",
        content: "",
        iframe: true,
        modal: true,
        visible: false,
        resizable: false,
        scrollable: false,
        actions: ["Close"],
        close: function () {
            $("#resBillSettingWindow").empty();
        },
        refresh: function () {
            $("#resBillSettingWindow").data("kendoWindow").center().open();
        },
        activate: function () {
            $("#resBillSettingWindow").css("overflow-y", "hidden");
        }
    });
}
//发票管理按钮点击事件
function resBillSettingButton_clicked(e) {
    if (e) { e.preventDefault(); }
    if (!$("#resBillSettingWindow").data("kendoWindow")) { resBillSettingWindow_Initialization(); }
    var resIdValue = $("#Resid").val();
    if (!resIdValue) {
        jAlert("请先保存客情后再来开票", "确定");
        return;
    }
    $("#resBillSettingWindow").data("kendoWindow").refresh({
        url: CustomerCommonValues.ResBillSetting + "?resid=" + resIdValue,
        type: "GET",
        iframe: true,
        cache: false,
    });
}