function batchInit() {
    //修改内容
    //$("[data-controltype='editcontrol-main']").change(function (e) { editControl_main_changed(e); });
    ////修改内容
    //$("[data-controltype='editcontrol']").change(function (e) { editControl_changed(e); });


    //设置默认值
    //var defaultArrTime = CustomerCommonValues.defaultArrTime;//默认抵店时间
    //var defaultHoldTime = CustomerCommonValues.defaultHoldTime;//默认保留时间
    //if (CustomerCommonValues.CttSalesDefaultValue == 1) {
    //    $("#CttSales").data("kendoDropDownList").value(CustomerCommonValues.UserName);
    //}

    //if (businessDate != null && businessDate != undefined && businessDate.length > 0) {
    //    depDateTime = new Date(new Date(businessDate + " 00:00:00").valueOf() + 1 * 24 * 60 * 60 * 1000);
    //}
    //var arriveDateTimePicker = $("#arriveDate").data("kendoDateTimePicker");
    //var holdDateTimePicker = $("#holdDate").data("kendoDateTimePicker");
    //var depDateTimePicker = $("#depDate").data("kendoDateTimePicker");
    //arriveDateTimePicker.value(arrDateTime);
    //depDateTimePicker.value(depDateTime);
    //holdDateTimePicker.value(holdDateTime);
    //if (holdDateTimePicker.value() > depDateTimePicker.value()) {
    //    holdDateTimePicker.value(arrDateTime);
    //}

    //DateTimeRangeSelection();

}
  
  


function AjaxQueryTabTypeInfo_setPara() {
    return {
        refeId: $("#refeId").val(),
        ReserveDate: $("#orderDate").data("kendoDateTimePicker").value()
    };
}

function AjaxQueryTabInfo_setPara() {

    return {
        refeId: $("#refeId").val(),
        ReserveDate: $("#orderDate").data("kendoDateTimePicker").value(),
        tabTypeId: $("#AddTabTypeId").val()
    };

}

function getSelectedTabTypeId() {

    return $("#AddTabTypeId").val();
}


//餐台类型选中事件
function grid_rowChaned(e) {
    e.preventDefault();

    var grid = $("#grid").data("kendoGrid");
    var selectedRows = this.select();
    if (selectedRows.length > 0) {
        var row = selectedRows[0];
        var data = grid.dataItem(row);
        $("#AddTabTypeId").val(data["TabTypeId"]);
        refreshTabs();
    }
}

//刷新餐台
function refreshTabs() {
    var listView = $("#batchListView").data("kendoListView");
    if (listView != null) {
        listView.dataSource.read();
    }
}

//刷新餐台类型
function refreshTabTypes()
{
    var listView = $("#grid").data("kendoGrid");
    if (listView!=null) {
        listView.dataSource.read();
    }
}


//餐台类型数据绑定事件
function grid_dataBound(e) {

    var tr = $("#grid td[TabTypeId='" + CustomerCommonValues.tabTypeId + "']").parent("tr");
    if (tr != null && tr != undefined && tr.length > 0) {
        this.select(tr[0]);
    }
    else {
        this.select("tr:eq(0)");
    }
}

//餐台点击事件
function TabListView_dataBound() {
    $(".house-state-bg").click(function (e) {
        e.preventDefault();
        Tab_Click(this);
    });
}

function Tab_Click(tabDIV) {
    var selected = $(tabDIV).hasClass("house-state-selected");
    var tabId = $(tabDIV).data("tabid");
    var tabNo = $(tabDIV).data("tabno");
    var tabTypeId = getSelectedTabTypeId();


    if (CustomerCommonValues.selectedTabs[tabTypeId] == null || CustomerCommonValues.selectedTabs[tabTypeId] == undefined) {
        CustomerCommonValues.selectedTabs[tabTypeId] = { qty: 0, tabs: [] };
    }
    var changeQty = CustomerCommonValues.selectedTabs[tabTypeId].qty == CustomerCommonValues.selectedTabs[tabTypeId].tabs.length;
    if (selected) {
        //移除选中的餐台
        var otherTabs = [];
        var count = CustomerCommonValues.selectedTabs[tabTypeId].tabs.length;
        for (var i = 0; i < count; i++) {

            var tabInfo = CustomerCommonValues.selectedTabs[tabTypeId].tabs[i];
            if (tabInfo["tabId"] !== tabId) {
                otherTabs.push({ tabId: tabInfo["tabId"], tabNo: tabInfo["tabNo"] });
            }
        }
        CustomerCommonValues.selectedTabs[tabTypeId].tabs = otherTabs;
        if (changeQty) {
            CustomerCommonValues.selectedTabs[tabTypeId].qty--;
        }
        $(tabDIV).removeClass("house-state-selected");
        $(tabDIV).addClass("house-state-default");
    } else {
        //增加选中的房号
        CustomerCommonValues.selectedTabs[tabTypeId].tabs.push({ tabId: tabId, tabNo: tabNo });

        $(tabDIV).removeClass("house-state-default");
        $(tabDIV).addClass("house-state-selected");
        if (changeQty) {
            CustomerCommonValues.selectedTabs[tabTypeId].qty++;
        }

    }
    updateGridSelectedQtyAndRoomNos();
}

//更新餐台列表中的选中数和已选餐台号显示
function updateGridSelectedQtyAndRoomNos() {
    //更新房型列表中的已选房数
    var grid = $("#grid").data("kendoGrid");
    var trs = grid.items();
    var alltabNos = [];
    var alltabIds = [];

    for (var j = 0; j < trs.length; j++) {
        var tr = trs[j];
        var dataItem = grid.dataItem(tr);
        if (dataItem) {
            var currentRoomTypeId = dataItem.TabTypeId;
            var count = 0;
            if (CustomerCommonValues.selectedTabs[currentRoomTypeId]) {
                count = CustomerCommonValues.selectedTabs[currentRoomTypeId].tabs.length;
                for (var k = 0; k < count; k++) {
                    var TabInfo = CustomerCommonValues.selectedTabs[currentRoomTypeId].tabs[k];
                    alltabNos.push(TabInfo["tabNo"]);
                    alltabIds.push(TabInfo["tabId"]);
                }
                count = CustomerCommonValues.selectedTabs[currentRoomTypeId].qty;
            }
            dataItem["selectedQty"] = count;
            //更新控件中的显示
            $(tr).find("input").val("" + count);
        }
    }
    //重新显示所有选中房号到已选房号中
    $("#selectedTabNos").val(alltabNos.join(','));
    $("#selectedTabIds").val(alltabIds.join(','));
    
}

//保存事件
function save_clicked()
{
    var name = $("#Name").val();
    var phone = $("#MobilePhone").val();
    var orderData = $("#orderDate").data("kendoDateTimePicker").value().ToDateTimeString();

    var tabNo = $("#selectedTabIds").val();
    if (name == "") {
        layer.alert("客人姓名不能为空", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    if (phone == "") {
        layer.alert("手机号不能为空", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    if (orderData == "") {
        layer.alert("预抵日期不能为空", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    if (tabNo == "") {
        layer.alert("请选择餐台", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    var model = setModel();
    $.ajax({
        url: '/PosManage/PosReserve/AddPosReserve',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                // layer.alert("添加成功", { title: "快点云Pos提示" });             
                layer.confirm("添加成功", {
                    btn: ['确认'] //按钮
                    , title: '快点云Pos提示'
                    , shade: 'rgba(0,0,0,0)'
                    , skin:'err'
                }, function () {
                    layer.closeAll();
                    $("#btnQuery").trigger("click");
                });
            } else {
                layer.alert("添加失败：" + data.Data, { title: "快点云Pos提示",skin:"err" });
            }
        },
        error: function (data) {
          
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err"  });
        }
    });

}
function setModel()
{
    var name = $("#Name").val();
    var phone = $("#MobilePhone").val();
    var orderData = $("#orderDate").data("kendoDateTimePicker").value().ToDateTimeString();
  
    var tabNo = $("#selectedTabIds").val();
   
    var endDate = $("#EndeDate").data("kendoDatePicker").value().ToDateTimeString();

    var saveInfo = {
        ExternalOrder: $("#ExternalOrder").val(),
        Name: name,
        TeamName: $("#TeamName").val(),
        MobilePhone: phone,
        ProfileId: $("#profileComboBox").data("kendoComboBox").value(),      //会员
        Cttid: $("#Cttid").data("kendoComboBox").value(),
        FYAmount: $("#FYAmount").val(),
        Sale: $("#Sale").val(),
        OrderDate: orderData,      //预抵日期
        Shuffle: $("#Shuffle").data("kendoDropDownList").value(),              //市别
        GuestType: $("#GuestType").data("kendoDropDownList").value(),           //类别
        ReservationMode: $("#ReservationMode").val(),
        EndeDate: endDate,
        EarnestMoney: $("#EarnestMoney").val(),
        Company: $("#Company").val(),
        ReservedTime: $("#ReservedTime").val(),
        Remark: $("#Remark").val(),
        TabId: tabNo,
        RefeId: $("#refeId").val(),
        Business: $("#business").val(),
        IGuest: $("#IGuest").val(),
        HYFYAmount: $("#HYFYAmount").val(),
        CYFYAmount: $("#CYFYAmount").val()
    }
    return saveInfo;
}

function update_billOrder()
{
    var name = $("#Name").val();
    var phone = $("#MobilePhone").val();
   
    var orderData = $("#orderDate").data("kendoDateTimePicker").value().ToDateTimeString();
    if (name == "") {
        layer.alert("客人姓名不能为空", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    if (phone == "") {
        layer.alert("手机号不能为空", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    if (orderData == "") {
        layer.alert("预抵日期不能为空", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    var model = setModel();
    model.BillId = $("#billId").val();
    $.ajax({
        url: '/PosManage/PosReserve/UpdatePosReserve',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                // layer.alert("添加成功", { title: "快点云Pos提示" });
                layer.confirm("修改成功", {
                    btn: ['确认'] //按钮
                    , title: '快点云Pos提示'
                    , shade: 'rgba(0,0,0,0)'
                    , skin:'layrt_confirm'
                }, function () {
                    layer.closeAll();
                    $("#btnQuery").trigger("click");
                });
            } else {
                layer.alert("修改失败：" + data.Data, { title: "快点云Pos提示", skin: "err"  });
            }
        },
        error: function (data) {

            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err"  });
        }
    });
}


//预抵日期变化事件
function orderDate_changed() {
    refreshTabTypes();
    refreshTabs();

    var orderData = $("#orderDate").data("kendoDateTimePicker").value().ToDateTimeString();
    var refeid = $("#refeId").val();
    //更换市别
    $.ajax({
        url: '/PosManage/PosReserve/SetShuffleId',
        type: "post",
        data: { refeId: refeid, time: orderData},
        dataType: "html",
        success: function (data) {
            $("#Shuffle").data("kendoDropDownList").value(data);
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
} 
//市别切换
function Shuffle_changed()
{
    var orderData = $("#orderDate").data("kendoDateTimePicker").value();//.ToDateTimeString();
    orderData = orderData.ToDateTimeString();
  
    var ShuffleId = $("#Shuffle").data("kendoDropDownList").value();
    $.ajax({
        url: '/PosManage/PosReserve/SetDateTimeByShuffleId',
        type: "post",
        data: { shuffleId: ShuffleId, time: orderData },
        dataType: "html",
        success: function (data) {
            var date = new Date(data);
            $("#orderDate").data("kendoDateTimePicker").value(date)
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err"  });
        }
    });
}

function editControl_changed()
{
}


function cttid_selected(e) { }

function editControl_main_changed() { }

//会员
function profileComboBox_databound() { }


function profileComboBox_selected() { }


function cancel_clicked()
{
    layer.closeAll();
}