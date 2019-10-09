//商品录入js

//初始化窗口
function folioAddItemsDiv_Initialization() {
    $("#folioAddItemsWindow").kendoWindow({
                width: "700px",
                title: "商品录入",
                visible: false,
                modal: true,
                actions: [
                    "Close"
                ],
                close: function () { try { folioQueryButton_clicked(); } catch (e) { } }
            });
           
}

//商品录入点击事件
function folioItemsButton_clicked(e) {
    if (e) { e.preventDefault(); }
    if (!$("#folioAddItemsWindow").data("kendoWindow")) { folioAddItemsDiv_Initialization(); }
            var resIdValue = $("#Resid").val();
            if (!resIdValue) {
                jAlert("请先保存客情后再来取消结账", "确定");
                return;
            }
            var addItemWindow = $("#folioAddItemsWindow").data("kendoWindow");
             addItemWindow.refresh({
                url: FolioCommonValues.AddItems,
                data: { rnd: Math.random() },
                type: "get",
                iframe: false
            }).center().open();
              addItemWindow.bind("refresh", function () {
                  var obj = $("#folioAddItemsWindow").data("kendoWindow");
                  obj.center();
                  $("#bntAll").trigger("click");
                
            });
}

//商品分类点击事件
function AddItem_click(data) {
    var active = $(data).hasClass("bnt-active");
    if (active)
        return;
    var grid = $("#griditem").data("kendoGrid").dataSource.data();
    for (var i = 0; i < grid.length; i++) {
        debugger;
        var item = grid[i];
        if (item.ItemQty > 0) {
            jAlert("已修改数据还未保存，请先保存！","知道了");
            return;
        }
    }
    addActive(data);
    var itemType = $(data).attr("data-items");
    $("#hidItemsType").val(itemType);
    refreshGridItems()

}
//girdItem加载参数
function setItemQueryPara() {
    var data = $("#hidItemsType").val();
    return { itemid: data==""?"all":data }
}
//刷新item表格
function refreshGridItems() {
    var grid = $("#griditem").data("kendoGrid");
    grid.dataSource.page(1);
    grid.dataSource.read();
}
function saveItems_click() {
   var select = $("#gridFolioGuest").data("kendoGrid").select();
   var gridData = $("#gridFolioGuest").data("kendoGrid").dataItem(select);
   var grid = $("#griditem").data("kendoGrid").dataSource.data();
   var qtys = [];
   var list = [];
   for (var i = 0; i < grid.length; i++) {
       var data = grid[i];
       if (data.ItemQty != 0) {
           qtys.push(data.ItemQty);
           var obj = {
               ItemId: data.ItemId,
               ItemName:data.ItemName,
               ItemQty: data.ItemQty,
               ItemSumPrice: data.ItemSumPrice,
               InvoNo: data.InvoNo,
               Remark:data.Remark
           };
           list.push(obj);
       }
   }
   if (qtys.length == 0) {
       jAlert("请填写商品数量","知道了")
       return;
   }
   $.post(FolioCommonValues.SaveItems, { FolioRegId: gridData.RegId, itemList: JSON.stringify(list) }, function (da) {
       if (da.Success) {
           jAlert("录入成功", "知道了");
           refreshGridItems();
       }
       else {
           ajaxErrorHandle(da);
       }


   },'json')
}
function closeItems_click() {
    $("#folioAddItemsWindow").data("kendoWindow").close();
    //刷新客账明细
    folioQueryButton_clicked();
    //刷新客账左侧列表
    folioGuestGrid_refresh();
}
function addActive(data){
    $("#itemsListView button").removeClass("bnt-active");
    $(data).addClass("bnt-active");
}
function onChangeItems(e) {
    //改变数量时
    if (e.field == "ItemQty") {
        var item = e.items[0];
        item.ItemSumPrice = item.ItemQty * item.ItemPrice;
        $("#griditem").data("kendoGrid").refresh();
    }
}