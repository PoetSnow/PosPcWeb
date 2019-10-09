var beginIndex = -1;
var endIndex = -1;
function SelectRow() {
    var grid = $("#gridBillDetailA").data("kendoGrid").dataSource; //获取选中行对象
    var dataRows = grid.data();
    var trList = $("#gridBillDetailA").find(".k-grid-content tr");
    //绑定点击事件
    $(trList).bind("click", function () {
        if (beginIndex == -1) {
            for (var i = 0; i < trList.length; i++) {
                $(trList).eq(i).removeClass("k-state-selected");
                $(trList).eq(i).attr("aria-selected", false);
            }
        }
        var onIndex = $(this).index();
        //判断当前点击的行是否选中
        var selected = $(this).attr("aria-selected");
        if (selected == undefined || selected == "false") {
            $(this).addClass("k-state-selected");
            $(this).attr("aria-selected", true);

        }
        else {
            $(this).removeClass("k-state-selected");
            $(this).attr("aria-selected", false);
        }
        //点击基数次给beginIndex赋值为当前选中的index。反之给endIndex赋值
        if (beginIndex <= -1) {
            beginIndex = $(this).index();
        }
        else {

            endIndex = $(this).index();
        }
        //.同时不为-1的时候 批量选中判断beginIndex与endIndex的大小 循环选中.最后把beginIndex与endIndex赋初始值
        if (beginIndex > -1 && endIndex > -1) {
            if (beginIndex > endIndex) {
                for (var i = endIndex ; i <= beginIndex ; i++) {
                    var s = $(trList).eq(i).attr("aria-selected");
                    $(trList).eq(i).addClass("k-state-selected");
                    $(trList).eq(i).attr("aria-selected", true);
                }
            }
            else {
                for (var i = beginIndex ; i <= endIndex; i++) {
                    $(trList).eq(i).addClass("k-state-selected");
                    $(trList).eq(i).attr("aria-selected", true);
                }
            }
            beginIndex = -1;
            endIndex = -1;
        }
    })

}

//退出
function exitBill() {
    layer.closeAll();
}
function exitMsg() {
    layer.close(layer.index);
}
//修改客位信息
function editBillDetailPlace(obj) {
    var grid = $("#gridBillDetailA").data("kendoGrid");
    var trList = $("#gridBillDetailA").find(".k-grid-content tr");
    var ids = "";//选中的ID集合
    for (var i = 0; i < trList.length; i++) {
        var selected = $(trList[i]).attr("aria-selected");
        if (selected == "true") {
            var id = $(trList[i]).find("td").eq(0).html();
            ids += id + ",";
        }
    }
    if (ids == "") {
        layer.alert("请选择消费项目", { title: "快点云Pos提示" });

    } else {
        var value = $(obj).val();
        if (value == "客位") {
            $.ajax({
                url: 'PosInSingle/BillDetailPlaceView',
                type: "post",
                data: { billDetailIds: ids },
                dataType: "html",
                success: function (data) {
                    layer.open({
                        type: 1,
                        title: "客位",
                        closeBtn: 0, //不显示关闭按钮
                        area: ['auto', 'auto'], //宽高
                        content: data
                    });

                },
                error: function (data) {
                    layer.alert(data.responseText, { title: "快点云Pos提示" });
                }
            });
        }
        else {
            $.ajax({
                url: "PosInSingle/BillDetailPlace",
                data: { billDetailIds: ids, place: value },
                type: "post",
                dataType: "json",
                success: function (result) {
                    if (result.Success == true) {
                        $("#gridBillDetailA").data("kendoGrid").dataSource.read();
                        $("#gridBillDetailA").data("kendoGrid").refresh();
                        //layer.alert("添加成功！", { title: "快点云Pos提示" })
                    }
                    else {
                        layer.alert(result.Data, { title: "快点云Pos提示" })
                    }
                },
                error: function (result) {
                    layer.alert(result.responseText, { title: "快点云Pos提示" })
                }
            });
        }
    }
}



