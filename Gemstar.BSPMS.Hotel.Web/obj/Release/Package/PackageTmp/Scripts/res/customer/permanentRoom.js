//长包房
//初始化事件
function permanentRoomWindow_Initialization() {
    //初始化窗口
    $("#permanentRoomWindow").kendoWindow({
        width: "600px",
        height: "462px",
        title: "长租房设置",
        content: "",
        iframe: true,
        modal: true,
        visible: false,
        resizable: false,
        scrollable: false,
        actions: ["Close"],
        close: function () {
            $("#permanentRoomWindow").empty();
        },
        refresh: function () {
            $("#permanentRoomWindow").data("kendoWindow").center().open();
        },
        activate: function () {
            $("#permanentRoomWindow").css("overflow-y", "hidden");
        }
    });
}
//长包房设置按钮点击事件
function permanentRoomIconButton_clicked(e) {
    if (e) { e.preventDefault(); }
    if (!$("#permanentRoomWindow").data("kendoWindow")) { permanentRoomWindow_Initialization(); }
    var regIdValue = $("#regId").val();
    if (!regIdValue) {
        jAlert("请先保存客情后再来设置长租房", "确定");
        return;
    }
    $("#permanentRoomWindow").data("kendoWindow").refresh({
        url: CustomerCommonValues.PermanentRoomSet + "?regid=" + regIdValue,
        type: "GET",
        iframe: true,
        cache: false,
    });
}