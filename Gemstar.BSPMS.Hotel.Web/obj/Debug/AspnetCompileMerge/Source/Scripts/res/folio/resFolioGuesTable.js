//客账的客人列表相关js代码开始
function gridFolioGuest_Initialization() {
    //注册客人列表的全选更改事件
    $("input.folioGuestAllCheck").unbind("change").change(function (e) {
        e.preventDefault();
        folioGuestAllCheck_changed();
    });
}
function folio_changeColor() {
    var folioBalanceSumSpan = $("#folioBalanceSumSpan").text();
    if (!isNaN(folioBalanceSumSpan) && folioBalanceSumSpan < 0) {
        $("#folioBalanceSumSpan").css("color", "red");

    }
    var balance = $("#balance").text();
    if (!isNaN(balance) && balance < 0)
        $("#balance").css("color", "red");
}
function setQueryFolioGuestPara() {
    return {
        resId: $("#Resid").val()
    };
}
//获取选中的regid值，不是指复选框选中，而是指表格选中
function folioGetTableSelectedRegId() {
    var grid = $("#gridFolioGuest").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length == 0) {
        return "";
    } else {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
        return data["RegId"];
    }
}
//全选复选框的更改事件
function folioGuestAllCheck_changed() {
    var checked = $("input.folioGuestAllCheck")[0].checked;
    $("input.folioGuestRowCheck").each(function (index, obj) {
        obj.checked = checked;
    });
    //触发账务明细表格的查询事件，以便实时反应选中记录的账务数据
    //$("#folioQuery").click();
    folioQueryButton_clicked();
}
//客账客人信息数据绑定后的事件，用于注册其中行的复选框更改事件
function folioGuestTable_dataBound() {
    //注册客人列表的复选框更改事件，以便在更改时刷新账务明细
    $("input.folioGuestRowCheck").change(function (e) {
        e.preventDefault();
        //触发账务明细表格的查询事件，以便实时反应选中记录的账务数据
        //$("#folioQuery").click();
        folioQueryButton_clicked();
    });
    //默认选中客情中选中的记录
    var grid = $("#grid");
    if (grid != null && grid != undefined && grid.length > 0) {
        var guestGrid = grid.data("kendoGrid");
        var guestGridSelected = guestGrid.select();
        //新预定时table是没有数据的
        if (guestGridSelected.length > 0) {
            var guestSelectedRegId = guestGrid.dataItem(guestGridSelected).Regid;
            if (guestSelectedRegId) {
                var $tr = $("input.folioGuestRowCheck[value='" + guestSelectedRegId + "']").parents("tr");
                $("#gridFolioGuest").data("kendoGrid").select($tr);
            }
        }
    }
    folio_changeColor();
    //全选
    var folioGuestAllCheckObj = document.getElementById("folioGuestAllCheck");
    folioGuestAllCheckObj.checked = false;
    folioGuestAllCheckObj.click();
    //页面数据加载完毕后，判断执行自动弹框入账窗口
    autoOpenAddFolioWindow();
    IsExistsCardAuth();
}
//右侧客账列表，预授权按钮改变样式
function IsExistsCardAuth() {
    var sumCardAuth = 0;
    var list = $("#gridFolioGuest").data("kendoGrid").dataSource.data();
    $.each(list, function (index, item) {
        if (item.IsCardAuth != null && item.IsCardAuth != undefined) {
            sumCardAuth += item.IsCardAuth;
        }
    });
    var obj = $("#folioAuthButton");
    var objCopy = $("#folioAuthButtonCopy");
    if (sumCardAuth > 0) {
        obj.text("预授权...");
        objCopy.text("预授权...");
        obj.addClass("k-prominent");
        objCopy.addClass("k-prominent");
    } else {
        obj.text("预授权");
        objCopy.text("预授权");
        obj.removeClass("k-prominent");
        objCopy.removeClass("k-prominent");
    }
}