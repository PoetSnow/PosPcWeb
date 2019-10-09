//批量入住预订页面上的控件事件
//页面初始化
function batchInit() {
    //修改内容
    $("[data-controltype='editcontrol-main']").change(function (e) { editControl_main_changed(e); });
    //修改内容
    $("[data-controltype='editcontrol']").change(function (e) { editControl_changed(e); });
    //会员下拉框宽度
    $("#profileComboBox").data("kendoComboBox").list.width(230);
    //合约单位下拉框宽度
    $("#Cttid").data("kendoDropDownList").list.width(230);
    $($("#Cttid").data("kendoDropDownList").filterInput).attr("placeholder", "最少输入两个字符");
    //设置默认值
    var defaultArrTime = CustomerCommonValues.defaultArrTime;//默认抵店时间
    var defaultHoldTime = CustomerCommonValues.defaultHoldTime;//默认保留时间
    var nowDateTime = kendo.date.today();
    var arrDateTime = nowDateTime;
    var holdDateTime = nowDateTime;
    var depDateTime = new Date(nowDateTime.valueOf() + 1 * 24 * 60 * 60 * 1000);
    if (CustomerCommonValues.isCheckIn == "1") {
        var currentDateTime = (new Date()).ToDateTimeWithoutSecondString();
        arrDateTime = currentDateTime;
        holdDateTime = currentDateTime;
        var businessDate = CustomerCommonValues.businessDate;
        if (businessDate != null && businessDate != undefined && businessDate.length > 0) {
            depDateTime = new Date(new Date(businessDate + " 00:00:00").valueOf() + 1 * 24 * 60 * 60 * 1000);
        }
    } else {
        if (defaultArrTime != null && defaultArrTime != undefined && defaultArrTime.length > 0) {
            arrDateTime = nowDateTime.ToDateString() + " " + (defaultArrTime + ":00");
        } else {
            arrDateTime = nowDateTime.ToDateString() + " " + ("00:00:00");
        }
        if (defaultHoldTime != null && defaultHoldTime != undefined && defaultHoldTime.length > 0) {
            holdDateTime = nowDateTime.ToDateString() + " " + (defaultHoldTime + ":00");
        } else {
            holdDateTime = nowDateTime.ToDateString() + " " + ("00:00:00");
        }
        if (new Date(arrDateTime) < new Date().setMinutes(new Date().getMinutes() +30)) {
            arrDateTime = new Date(new Date().valueOf() +1 * 60 * 60 * 1000);
        }
        if (new Date(holdDateTime) < arrDateTime) {
            holdDateTime = new Date(arrDateTime.valueOf() +1 * 60 * 60 * 1000);
        }
    }
    $("#arriveDate").data("kendoDateTimePicker").value(arrDateTime);
    $("#depDate").data("kendoDateTimePicker").value(depDateTime);
    $("#holdDate").data("kendoDateTimePicker").value(holdDateTime);
    DateTimeRangeSelection();
    //如果是入住的话，则禁止修改抵店时间和保留时间
    if (CustomerCommonValues.isCheckIn == "1") {
        $("#arriveDate").data("kendoDateTimePicker").enable(false);
        $("#holdDate").data("kendoDateTimePicker").enable(false);
    }
    $("#customerSource").data("kendoDropDownList").dataSource.read();
    $("#rateCode").data("kendoDropDownList").dataSource.read();
    days_depDate_arriveDate_events();
    profileComboBox_Tooltip();
    cttidDropDownList_Tooltip();
    $("[for='Cttid']").dblclick(function () {companyDetailInfo();});
    $("[for='profileComboBox']").dblclick(function () { profileTransDetailInfo(); });
    $("#ResCustName").focus(function () { autoSelectTextBoxContentByName("ResCustName"); });
    $("#Name").focus(function () { autoSelectTextBoxContentByName("Name"); });
    $("[name='IsGroup']").click(function (e) { isGroup_clicked(e); });
}
//内容修改
function editControl_main_changed(e) {
    if (e) { e.preventDefault(); }
    hasChangedMain = true;
    var id = $(e.target).prop("id");
    //如果是修改的联系人，则自动更改主单名称
    if (id == "ResCustName") {
        //判断主单名称中的值是否与原始值相同，相同则修改
        var $name = $("#Name");
        var nameValue = $name.val();
        if(nameValue == CustomerCommonValues.originCustomName)
        {
            $name.val($("#ResCustName").val());
        }
    }
}
function editControl_changed(e) {
    if (e) { e.preventDefault(); }
    var id = $(this.element).prop("id");
    editControl_onChange(id);
}
function editControl_onChange(id, isChanged) {
    hasChanged = true;
    if (isChanged == "false") { hasChanged = false; }
    if (id == "arriveDate" || id == "depDate") {
        if (id == "arriveDate") {
            //如果触发事件是抵店时间，则保留时间为同一天，离店时间为抵店时间加1天
            arriveDate_changed();
        }
        //如果触发事件的是抵店日期，离店日期，则重新加载房间类型信息
        DateTimeRangeSelection();
    }
    if (id == "rateCode") {
        //如果触发事件的是价格码，则从服务器获取客人来源，市场分类，离店时间，是否有早餐
        rateCode_changed();
    }  
    if (id == "roomType" || id == "arriveDate" || id == "depDate") {
        //如果触发事件的是价格码，房型，抵店日期，离店日期，则重新检查是否四个值都已经有值了，有则重新加载价格信息
        refreshRoomTypes();
    }
}

//房型列表查询参数设置
function AjaxQueryRoomTypes_setPara() {
    var nowDate = new Date(CustomerCommonValues.businessDate.replace(/-/g, "/") + " 00:00:00");
    var arrDate = (CustomerCommonValues.isCheckIn == "1") ? nowDate: $("#arriveDate").data("kendoDateTimePicker").value();
    if (arrDate) {
        arrDate = arrDate.ToDateTimeString();
    }
    var depDate = $("#depDate").data("kendoDateTimePicker").value();
    depDate = new Date(depDate.valueOf());
    depDate.setDate(depDate.getDate() +1);
    if (depDate) {
        depDate = depDate.ToDateTimeString();
    }
    return {
        ArrDate: arrDate,
        DepDate: depDate,
        RateId: $("#rateCode").data("kendoDropDownList").value(),
        IsCheckIn: CustomerCommonValues.isCheckIn,
    };
}
function getSelectedRoomTypeId() {
    //get selected room type id
    var roomTypeId = "";
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    if (selectedRows.length > 0) {
        var dataItem = grid.dataItem(selectedRows[0]);
        roomTypeId = dataItem.id;
    }
    return roomTypeId;
}

//房间列表查询参数设置
function AjaxQueryRooms_setPara() {
    //get selected room type id
    var roomTypeId = getSelectedRoomTypeId();

    var arrDate = $("#arriveDate").data("kendoDateTimePicker").value();
    if (arrDate) {
        arrDate = arrDate.ToDateTimeString();
    }
    var depDate = $("#depDate").data("kendoDateTimePicker").value();
    if (depDate) {
        depDate = depDate.ToDateTimeString();
    }
    return {
        isCheckIn: CustomerCommonValues.isCheckIn,
        roomTypeId: roomTypeId,
        arrDate: arrDate,
        depDate: depDate
    };
}
//房型列表选中行事件
function grid_rowChaned(e) {
    e.preventDefault();
    var selectedRows = this.select();
    if (selectedRows.length > 0) {
        refreshRooms();
    }
}

//房型列表数据绑定事件
function grid_dataBound(e) {
    var tr = $("#grid td[roomtypeid='" + CustomerCommonValues.roomTypeId + "']").parent("tr");
    if (tr != null && tr != undefined && tr.length > 0) {
        this.select(tr[0]);
    }
    else {
        this.select("tr:eq(0)");
    }
    //set disable if is checkin
    if (CustomerCommonValues.isCheckIn == 1) {
        this.items().find("input").prop("disabled", "disabled").addClass("bg-disabled");
    }
    this.items().find("input").change(function (e) {
        e.preventDefault();
        var $tr = $(this).parents("tr");
        var grid = $("#grid").data("kendoGrid");
        var dataItem = grid.dataItem($tr);
        grid_selectedQty_changed($(this), dataItem.id);
    });
    updateGridSelectedQtyAndRoomNos();
}
//房型列表中已选数量更改事件
function grid_selectedQty_changed(obj, roomTypeId) {
    var qty = parseInt($(obj).val());
    if (CustomerCommonValues.selectedRooms[roomTypeId]) {
        CustomerCommonValues.selectedRooms[roomTypeId].qty = qty;
    } else {
        CustomerCommonValues.selectedRooms[roomTypeId] = { qty: qty, rooms: [] };
    }
}
function refreshRoomTypes() {
    var listView = $("#grid").data("kendoGrid");
    listView.dataSource.read();
}
function refreshRooms() {
    var listView = $("#batchListView").data("kendoListView");
    listView.dataSource.read();
}

//价格码改变事件
function rateCode_changed() {
    //设置客人来源，市场分类，离店时间，是否有早餐
    var rateCodeValue = $("#rateCode").data("kendoDropDownList").value();
    if (rateCodeValue.length <= 0) {
        return;
    }
    $.post(CustomerCommonValues.GetRate, { rateId: rateCodeValue }, function (result) {
        if (result.Success) {
            if (result.Data != null && result.Data != undefined) {
                $("#rateCodeIsUseScore").val(result.Data.IsUseScore);
                $("#rateCodeNoPrintProfile").attr("data-id", rateCodeValue); $("#rateCodeNoPrintProfile").val(result.Data.NoPrintProfile); $("#rateCodeNoPrintProfile").attr("data-isContinue", "");
                $("#rateCodeNoPrintCompany").attr("data-id", rateCodeValue); $("#rateCodeNoPrintCompany").val(result.Data.NoPrintCompany); $("#rateCodeNoPrintCompany").attr("data-isContinue", "");
                $("#rateCodeChangedRoomTypeids").attr("data-id", rateCodeValue); $("#rateCodeChangedRoomTypeids").val(result.Data.RoomTypeids);
                if ($("#txtAddROrderDetail").val() != "true") {
                    //客人来源
                    if (result.Data.Sourceid != null && result.Data.Sourceid != undefined && result.Data.Sourceid.length > 0) { $("#customerSource").data("kendoDropDownList").value(result.Data.Sourceid); }
                    else {
                        var customerSourceDropdown = $("#customerSource").data("kendoDropDownList");
                        var type = CustomerCommonValues.isCheckIn;
                        if (type == "1") {
                            customerSourceDropdown.value(CustomerCommonValues.hotelId + "05walk");
                        } else {
                            customerSourceDropdown.value(CustomerCommonValues.hotelId + "05resv");
                        }
                        var valueStr = customerSourceDropdown.value();
                        if (!(valueStr != null && valueStr != undefined && valueStr.length > 0)) {
                            customerSourceDropdown.select(0);
                        }
                    }
                    //市场分类
                    if (result.Data.Marketid != null && result.Data.Marketid != undefined && result.Data.Marketid.length > 0) { $("#marketType").data("kendoDropDownList").value(result.Data.Marketid); }
                }
                //离店时间
                var rateCodeChangedIsDayRoom = $("#rateCodeChangedIsDayRoom"); rateCodeChangedIsDayRoom.val("false"); rateCodeChangedIsDayRoom.attr("data-dayroomtime", "");
                var rateCodeChangedIsHou = $("#rateCodeChangedIsHou"); rateCodeChangedIsHou.val("false"); rateCodeChangedIsHou.attr("data-baseminute", "");
                var rateCodeChangedHalfTime = $("#rateCodeChangedHalfTime");
                rateCodeChangedHalfTime.val("false");
                rateCodeChangedHalfTime.attr("data-halftime", "");
                var depDateTimePicker = $("#depDate").data("kendoDateTimePicker");
                var arriveDateTime = $("#arriveDate").data("kendoDateTimePicker").value();
                if (result.Data.IsHou == true) {
                    rateCodeChangedIsHou.val("true"); rateCodeChangedIsHou.attr("data-baseminute", result.Data.BaseMinute);
                    if (arriveDateTime != null && arriveDateTime != undefined) {
                        if (result.Data.BaseMinute != null && result.Data.BaseMinute != undefined) {
                            var arriveDateTimeNew = new Date(arriveDateTime.valueOf());
                            arriveDateTimeNew.setMinutes(arriveDateTimeNew.getMinutes() + result.Data.BaseMinute);
                            depDateTimePicker.value(arriveDateTimeNew);
                        }
                    }
                }
                else if (result.Data.IsDayRoom == true) {
                    rateCodeChangedIsDayRoom.val("true"); rateCodeChangedIsDayRoom.attr("data-dayroomtime", result.Data.DayRoomTime);
                    if ($.trim(result.Data.DayRoomTime) != "") {
                        depDateTimePicker.value(arriveDateTime.ToDateString() + " " + result.Data.DayRoomTime + ":00");
                    }
                } else {
                    rateCodeChangedIsHou.val("false"); rateCodeChangedIsHou.attr("data-baseminute", "");
                    if (result.Data.HalfTime != null && result.Data.HalfTime != undefined && result.Data.HalfTime.length > 0) {
                        rateCodeChangedHalfTime.val("true"); rateCodeChangedHalfTime.attr("data-halftime", result.Data.HalfTime);
                        var depDateTime = depDateTimePicker.value();
                        if (arriveDateTime != null && arriveDateTime != undefined) {
                            //只在没有预离时间或者预离时间与预抵时间的日期相同时才加一天，其他情况下不改变
                            if (depDateTime == null || depDateTime == undefined || depDateTime.ToDateString() == arriveDateTime.ToDateString()) {
                                depDateTime = new Date(arriveDateTime.valueOf() + 1 * 24 * 60 * 60 * 1000);
                                if (CustomerCommonValues.isCheckIn == "1") {
                                    var businessDate = CustomerCommonValues.businessDate;
                                    if (businessDate != null && businessDate != undefined && businessDate.length > 0) {
                                        depDateTime = new Date(new Date(businessDate + " 00:00:00").valueOf() + 1 * 24 * 60 * 60 * 1000);
                                    }
                                }
                            }
                        }
                        if (depDateTime != null && depDateTime != undefined) {
                            depDateTimePicker.value(depDateTime.ToDateString() + " " + result.Data.HalfTime + ":00");
                        }
                    }
                }
                DateTimeRangeSelection();
                refreshRoomTypes();
                if (result.Data.IsPriceAdjustment == true) {
                    $("#editPriceDiv").css("display", "block"); $("#editPriceDivMsg").css("display", "none");
                } else {
                    $("#editPriceDiv").css("display", "none"); $("#editPriceDivMsg").css("display", "block");
                }
            }
        } else {
            //jAlert(result.Data);
            ajaxErrorHandle(result);
        }
        days_depDate_arriveDate_change();
    }, 'json');
}

//抵店时间改变事件
function arriveDate_changed() {
    //先去掉设置的时间范围，否则可能赋值不在范围内，导致赋值不成功
    DateTimeRangeSelection("true");
    //设置离店时间和保留时间同步更改
    var arriveDateTimePicker = $("#arriveDate").data("kendoDateTimePicker");
    var holdDateTimePicker = $("#holdDate").data("kendoDateTimePicker");
    var depDateTimePicker = $("#depDate").data("kendoDateTimePicker");
    if (arriveDateTimePicker.value() != null) {        
        //设置默认值
        var defaultArrTime = CustomerCommonValues.defaultArrTime;//默认抵店时间
        var defaultHoldTime = CustomerCommonValues.defaultHoldTime;//默认保留时间
        var nowDateTime = arriveDateTimePicker.value();
            
        var rateCodeChangedIsHou = $("#rateCodeChangedIsHou");
        var rateCodeChangedIsDayRoom = $("#rateCodeChangedIsDayRoom");
        if (rateCodeChangedIsHou.val() == "true") {
            var baseminute = rateCodeChangedIsHou.attr("data-baseminute");
            if (baseminute != null && baseminute != undefined && baseminute.length > 0) {
                var arriveDateTimeNew = new Date(nowDateTime.valueOf());
                arriveDateTimeNew.setMinutes(arriveDateTimeNew.getMinutes() + parseInt(baseminute));
                depDateTimePicker.value(arriveDateTimeNew);
            }
        }
        else if (rateCodeChangedIsDayRoom.val() == "true") {
            var dayroomtime = rateCodeChangedIsDayRoom.attr("data-dayroomtime");
            if ($.trim(dayroomtime) != "") {
                depDateTimePicker.value(nowDateTime.ToDateString() + " " + dayroomtime + ":00");
            }
        } else {
            var isHalftime = false;
            var depDateTime = new Date(nowDateTime.valueOf() + 1 * 24 * 60 * 60 * 1000);
            var rateCodeChangedHalfTime = $("#rateCodeChangedHalfTime");
            if (rateCodeChangedHalfTime.val() == "true") {
                var halftime = rateCodeChangedHalfTime.attr("data-halftime");
                if (halftime != null && halftime != undefined && halftime.length > 0) {
                    depDateTimePicker.value(depDateTime.ToDateString() + " " + (halftime + ":00"));
                    isHalftime = true;
                }
            }
            if (!isHalftime) {
                if (defaultArrTime != null && defaultArrTime != undefined && defaultArrTime.length > 0) {
                    depDateTimePicker.value(depDateTime.ToDateString() + " " + (defaultArrTime + ":00"));
                } else {
                    depDateTimePicker.value(depDateTime.ToDateString() + " " + ("00:00:00"));
                }
            }
        }
        if (defaultHoldTime != null && defaultHoldTime != undefined && defaultHoldTime.length > 0) {
            holdDateTimePicker.value(nowDateTime.ToDateString() + " " + (defaultHoldTime + ":00"));
        } else {
            holdDateTimePicker.value(nowDateTime);
        }
        if (holdDateTimePicker.value() > depDateTimePicker.value()) {
            holdDateTimePicker.value(nowDateTime);
        }
    }
    DateTimeRangeSelection();
}
//日期时间控件选择范围
function DateTimeRangeSelection(notRange) {
    var arriveDateTimePicker = $("#arriveDate").data("kendoDateTimePicker");
    var holdDateTimePicker = $("#holdDate").data("kendoDateTimePicker");
    var depDateTimePicker = $("#depDate").data("kendoDateTimePicker");
    if (notRange == "true") {
        var mindate = "1900-01-01";
        var maxdate = "9999-12-31";
        arriveDateTimePicker.min(mindate);
        holdDateTimePicker.min(mindate); holdDateTimePicker.max(maxdate);
        depDateTimePicker.min(mindate); depDateTimePicker.max(maxdate);
        return;
    }
    arriveDateTimePicker.min(new Date());
    var arriveDateTimePickerValue = arriveDateTimePicker.value();
    if (arriveDateTimePickerValue != null) {
        holdDateTimePicker.min(arriveDateTimePickerValue);
        depDateTimePicker.min(arriveDateTimePickerValue);
    }
}
//listview数据绑定完成后，注册点击后显示房间信息事件
function roomsListView_dataBound(da) {
    $(".house-state-bg").click(function (e) {
        e.preventDefault();
        room_clicked(this);
    });
    //选中参数带入值
    var roomDiv = $("#batchListView dl[data-roomid='" + CustomerCommonValues.roomId + "']");
    if (roomDiv != null && roomDiv != undefined && roomDiv.length > 0) {
        CustomerCommonValues.roomId = "";
        var selected = $(roomDiv).hasClass("house-state-selected");
        if (!selected) {
            roomDiv[0].click();
        }
    }
}
//房间点击事件
function room_clicked(roomDiv) {
    var selected = $(roomDiv).hasClass("house-state-selected");
    var roomId = $(roomDiv).data("roomid");
    var roomNo = $(roomDiv).data("roomno");
    var roomTypeId = getSelectedRoomTypeId();
    if (CustomerCommonValues.selectedRooms[roomTypeId] == null || CustomerCommonValues.selectedRooms[roomTypeId] == undefined) {
        CustomerCommonValues.selectedRooms[roomTypeId] = { qty: 0, rooms: [] };
    }
    var changeQty = CustomerCommonValues.selectedRooms[roomTypeId].qty == CustomerCommonValues.selectedRooms[roomTypeId].rooms.length;
    if (selected) {
        //移除选中的房号
        var otherRooms = [];
        var count = CustomerCommonValues.selectedRooms[roomTypeId].rooms.length;
        for (var i = 0; i < count; i++) {
            var roomInfo = CustomerCommonValues.selectedRooms[roomTypeId].rooms[i];
            if (roomInfo["roomId"] !== roomId) {
                otherRooms.push({ roomId: roomInfo["roomId"], roomNo: roomInfo["roomNo"] });
            }
        }
        CustomerCommonValues.selectedRooms[roomTypeId].rooms = otherRooms;
        if (changeQty) {
            CustomerCommonValues.selectedRooms[roomTypeId].qty--;
        }
        $(roomDiv).removeClass("house-state-selected");
        $(roomDiv).addClass("house-state-default");
    } else {
        //增加选中的房号
        CustomerCommonValues.selectedRooms[roomTypeId].rooms.push({ roomId: roomId, roomNo: roomNo });
        $(roomDiv).removeClass("house-state-default");
        $(roomDiv).addClass("house-state-selected");
        if (changeQty) {
            CustomerCommonValues.selectedRooms[roomTypeId].qty++;
        }
    }
    updateGridSelectedQtyAndRoomNos();
}
//更新房型列表中的选中数和已选房号显示
function updateGridSelectedQtyAndRoomNos() {
    //更新房型列表中的已选房数
    var grid = $("#grid").data("kendoGrid");
    var trs = grid.items();
    var allRoomNos = [];
    for (var j = 0; j < trs.length; j++) {
        var tr = trs[j];
        var dataItem = grid.dataItem(tr);
        if (dataItem) {
            var currentRoomTypeId = dataItem.id;
            var count = 0;
            if (CustomerCommonValues.selectedRooms[currentRoomTypeId]) {
                count = CustomerCommonValues.selectedRooms[currentRoomTypeId].rooms.length;
                for (var k = 0; k < count; k++) {
                    var roomInfo = CustomerCommonValues.selectedRooms[currentRoomTypeId].rooms[k];
                    allRoomNos.push(roomInfo["roomNo"]);
                }
                count = CustomerCommonValues.selectedRooms[currentRoomTypeId].qty;
            }
            dataItem["selectedQty"] = count;
            //更新控件中的显示
            $(tr).find("input").val("" + count);
        }
    }
    //重新显示所有选中房号到已选房号中
    $("#selectedRoomNos").val(allRoomNos.join(','));
}
//保存按钮点击事件
function save_clicked(e) {
    if (e) { e.preventDefault(); }
    var RoomTypeInfos = [];
    //将当前选中房型中的数量房号等信息写入对象中
    var hasRoom = false;
    var grid = $("#grid").data("kendoGrid");
    var trs = grid.items();
    for (var i = 0; i < trs.length; i++) {
        var tr = trs[i];
        var dataItem = grid.dataItem(tr);
        var roomTypeId = dataItem.id;
        var roomTypeInfo = CustomerCommonValues.selectedRooms[roomTypeId];
        if (roomTypeInfo) {
            if (roomTypeInfo.qty > 0) {
                hasRoom = true;
                var roomTypeSaveInfo = {
                    qty: roomTypeInfo.qty,
                    roomTypeId: roomTypeId,
                    priceStr: dataItem.rate,
                    Bbf: dataItem.bbf,
                    RoomInfos: []
                };
                for (var j = 0; j < roomTypeInfo.rooms.length; j++) {
                    var roomInfo = roomTypeInfo.rooms[j];
                    roomTypeSaveInfo.RoomInfos.push({ roomId: roomInfo["roomId"], roomNo: roomInfo["roomNo"] });
                }
                RoomTypeInfos.push(roomTypeSaveInfo);
                //如果是入住的话，则必须房间数与选择的房号数完全相同
                if (CustomerCommonValues.isCheckIn == "1") {
                    if (roomTypeSaveInfo.qty != roomTypeSaveInfo.RoomInfos.length) {
                        jAlert("请选择房间");
                        return;
                    }
                }
            }
        }
    }
    if (!hasRoom) {
        jAlert("请选择房间");
        return;
    }
    var rateCode = $("#rateCode").data("kendoDropDownList").value();
    if (rateCode == null || rateCode == undefined || rateCode.length <= 0) {
        jAlert("请选择价格代码");
        return;
    }
    //验证价格代码适用房型
    var tempRateCode = $("#rateCodeChangedRoomTypeids");
    var tempRateId = tempRateCode.attr("data-id"); var tempRateTypeids = tempRateCode.val();
    if (tempRateId != null && tempRateId != undefined && tempRateId.length > 0 && tempRateId == rateCode) {
        if (tempRateTypeids != null && tempRateTypeids != undefined && tempRateTypeids.length > 0) {
            $.each(RoomTypeInfos, function (index, item) {
                if (("," + tempRateTypeids + ",").indexOf("," + item.roomTypeId + ",") == -1) {
                    jAlert("<" + $("#rateCode").data("kendoDropDownList").text() + ">价格代码不适用当前房型"); return;
                }
            });
        }
    }
    var arrDate = $("#arriveDate").data("kendoDateTimePicker").value();
    if (arrDate) {
        arrDate = arrDate.ToDateTimeString();
    } else {
        jAlert("请选择抵店时间");
        return;
    }
    var depDate = $("#depDate").data("kendoDateTimePicker").value();
    if (depDate) {
        depDate = depDate.ToDateTimeString();
    } else {
        jAlert("请选择离店时间");
        return;
    }
    var holdDate = $("#holdDate").data("kendoDateTimePicker").value();
    if (holdDate) {
        holdDate = holdDate.ToDateTimeString();
    } else {
        jAlert("请选择保留时间");
        return;
    }
    var saveInfo = {
        IsCheckIn: CustomerCommonValues.isCheckIn,
        saveContinue: $("#saveContinue").val(),
        AuthorizationSaveContinue: $("#authorizationSaveContinue").val(),
        UseScoreSaveContinue: $("#UseScoreSaveContinue").val(),
        IsGroup: $("input[name='IsGroup']:checked").val(),
        ResCustName: $("#ResCustName").val(),
        ResCustMobile: $("#ResCustMobile").val(),
        ProfileId: $("#profileComboBox").data("kendoComboBox").value(),
        ResNoExt: $("#ResNoExt").val(),
        Resno: $("#Resno").val(),
        Name: $("#Name").val(),
        Cttid: $("#Cttid").data("kendoDropDownList").value(),
        arriveDate: arrDate,
        holdDate: holdDate,
        marketType: $("#marketType").data("kendoDropDownList").value(),
        special: $("#special").val(),
        depDate: depDate,
        rateCode: $("#rateCode").data("kendoDropDownList").value(),
        customerSource: $("#customerSource").data("kendoDropDownList").value(),
        remark: $("#remark").val(),
        RoomTypeInfos: RoomTypeInfos
    };
    if (!saveInfo.ResCustName) {
        jAlert("请输入联系人");
        return;
    }
    if (!saveInfo.customerSource) {
        jAlert("请选择客人来源");
        return;
    }
    if (!saveInfo.marketType) {
        jAlert("请选择市场分类");
        return;
    }
    //根据价格代码设置选择是否必填会员和合约单位
    var rateCodeId = saveInfo.rateCode;
    if (rateCodeId == $("#rateCodeNoPrintProfile").attr("data-id") && $.trim(saveInfo.ProfileId) == '') {
        var rateCodeNoPrintProfile = $("#rateCodeNoPrintProfile").val();
        switch (rateCodeNoPrintProfile) {
            case "1": {
                if ($("#rateCodeNoPrintProfile").attr("data-isContinue") != "true") {
                    jConfirm("此价格代码需要录入会员，是否继续保存？", "  是  ", "  否  ", function (confirmed) {
                        if (confirmed) {
                            $("#rateCodeNoPrintProfile").attr("data-isContinue", "true");
                            $("#save")[0].click();
                            return;
                        } else {
                            $("#rateCodeNoPrintProfile").attr("data-isContinue", "");
                        }
                    }); return;
                }
                break;
            }
            case "2": {
                jAlert("此价格代码需要录入会员，请录入会员再保存！"); return;
                break;
            }
        }
    }
    if (rateCodeId == $("#rateCodeNoPrintCompany").attr("data-id") && $.trim(saveInfo.Cttid) == '') {
        var rateCodeNoPrintCompany = $("#rateCodeNoPrintCompany").val();
        switch (rateCodeNoPrintCompany) {
            case "1": {
                if ($("#rateCodeNoPrintCompany").attr("data-isContinue") != "true") {
                    jConfirm("此价格代码需要录入合约单位，是否继续保存？", "  是  ", "  否  ", function (confirmed) {
                        if (confirmed) {
                            $("#rateCodeNoPrintCompany").attr("data-isContinue", "true");
                            $("#save")[0].click();
                            return;
                        } else {
                            $("#rateCodeNoPrintCompany").attr("data-isContinue", "");
                        }
                    }); return;
                }
                break;
            }
            case "2": {
                jAlert("此价格代码需要录入合约单位，请录入合约单位再保存！"); return;
                break;
            }
        }
    }
    $("#save").attr("disabled", "disabled");
    $.post(CustomerCommonValues.saveUrl, saveInfo, function (data) {
        if (data.Success) {
            //刷新房态
            $(parent.document).find("iframe")[0].contentWindow.refreshRoomStatus();
            //打开维护窗口
            top.openResKendoWindow("客单", data.Data, null, "20020", "新预订/入住", null, 1);
            top.closeIframeKendoWindow();
        } else {
            //判断是出错信息还是对象，如果是对象，则可能是房间有冲突，需要提示后询问是否继续
            var result = data;
            if ($.type(result.Data) == "array" && result.Data[0]["CanSave"] != undefined) {
                var msg = '<p>房间冲突信息如下:</p><div class="k-widget k-grid"><table class="k-selectable"><thead class="k-grid-header"><tr><th class="k-header">房型</th><th class="k-header">房号</th><th class="k-header">冲突日期</th><th class="k-header">冲突原因</th></tr></thead>';
                var canContinue = true;
                for (var i = 0; i < result.Data.length; i++) {
                    var info = result.Data[i];
                    if (info["CanSave"] == 0) {
                        canContinue = false;
                    }
                    msg += '<tr><td>' + (info["RoomTypeName"] == null ? "" : info["RoomTypeName"]) + '</td><td>' + (info["RoomNo"] == null ? "" : info["RoomNo"]) + '</td><td>' + (info["Usedate"] == null ? "" : info["Usedate"]) + '</td><td>' + (info["Remark"] == null ? "" : info["Remark"]) + '</td></tr>';
                }
                msg += '</table></div>';
                if (!canContinue) {
                    jAlert(msg, "知道了");
                } else {
                    jConfirm(msg + '<p>是否继续?</p>', '继续', '返回修改', function (confirmed) {
                        if (confirmed) {
                            $("#saveContinue").val("1");
                            try {
                                save_clicked();
                            } catch (e) { }
                        }
                    });
                }
            }
            else {
                if (data.ErrorCode == 4) { authorizationWindow.Open(1, data.Data, "save_clicked"); return; }
                if (data.ErrorCode == 5) { useScoreToCheckinWindow.Open(result.Data, "save_clicked"); return; }
                //如果是出错信息，则直接提示一下即可
                //jAlert(result.Data, "知道了");
                ajaxErrorHandle(result);
            }
        }
        $("#save").removeAttr("disabled");
    }, 'json');
}
//更改选中房型价格按钮点击事件
function changeRatePrice_clicked(e) {
    if (e) { e.preventDefault(); }
    //check ready to change price
    var roomTypeId = getSelectedRoomTypeId();
    if (!roomTypeId) {
        jAlert("请选择要调整价格的房型记录");
        return;
    }
    var ratePlanCode = $("#rateCode").data("kendoDropDownList").value();
    if (!ratePlanCode) {
        jAlert("请选择价格代码");
        return;
    }
    var arrDate = $("#arriveDate").data("kendoDateTimePicker").value();
    if (arrDate) {
        arrDate = arrDate.ToDateTimeString();
    } else {
        jAlert("请选择抵店时间");
        return;
    }
    var depDate = $("#depDate").data("kendoDateTimePicker").value();
    if (depDate) {
        depDate = depDate.ToDateTimeString();
    } else {
        jAlert("请选择离店时间");
        return;
    }
    batchChangeRatePlanWindow.Open();
}
//价格代码绑定事件
function rateCode_dataBound() {
    editControl_onChange("rateCode", "false");
}
//会员选择事件
function profileComboBox_selected(e) {
    if (e == null || e == undefined || e.item == null || e.item == undefined) { return; }
    var dataItem = this.dataItem(e.item.index());
    if (dataItem != null && dataItem != undefined) {
        var id = dataItem.Id;
        if (id != null && id != undefined && id.length > 0) {
            $.post(CustomerCommonValues.GetMbrCardInfo, { id: id }, function (result) {
                if (result.Success && result.Data != null) {
                    var resCustName = $("#ResCustName");
                    var resCustNameValue = $.trim(resCustName.val());
                    var name = $("#Name");
                    var nameValue = $.trim(name.val());
                    var rName = "新预订客人";
                    var iName = "新入住客人";
                    if ((resCustNameValue == rName && nameValue == rName) || (resCustNameValue == iName && nameValue == iName)) {
                        resCustName.val(result.Data.GuestName);
                        name.val(result.Data.GuestName);
                        $("#ResCustMobile").val(result.Data.Mobile);
                    }
                    if (result.Data.RateCodeid != null && result.Data.RateCodeid != undefined && result.Data.RateCodeid.length > 0) {

                        //集团分店模式兼容
                        if (result.Data.RateCodeid.substring(0, 6) != CustomerCommonValues.hotelId && $.trim(CustomerCommonValues.hotelId).length > 0) {
                            var isExistsRateCodeId = false;
                            var tempList = $("#rateCode").data("kendoDropDownList").dataItems();
                            if (tempList != null && tempList.length > 0) {
                                $.each(tempList, function (index, item) {
                                    if (item.Value.substring(6) == result.Data.RateCodeid.substring(6)) {
                                        isExistsRateCodeId = true;
                                        return false;
                                    }
                                });
                            }
                            if (isExistsRateCodeId == true) {
                                result.Data.RateCodeid = CustomerCommonValues.hotelId + result.Data.RateCodeid.substring(6);
                            }
                            else {
                                return;
                            }
                        }

                        var rateCodeObj = $("#rateCode").data("kendoDropDownList");
                        if (rateCodeObj.value() != result.Data.RateCodeid) {
                            rateCodeObj.value(result.Data.RateCodeid);
                            editControl_onChange("rateCode", "false");
                        }
                    }
                }
            }, 'json');
        }
    }
}
//合约单位选择事件
function cttid_selected(e) {
    var dataItem = this.dataItem(e.item);
    if (dataItem != null && dataItem != undefined) {
        var id = dataItem.Value;
        if (id != null && id != undefined && id.length > 0) {
            $.post(CustomerCommonValues.GetCompanyInfo, { id: id }, function (result) {
                if (result.Success && result.Data != null && result.Data != undefined) {
                    var rateCode = result.Data.RateCode;
                    var contact = result.Data.Contact;
                    var contactMobile = result.Data.ContactMobile;
                    if ($.trim(contact).length > 0) {
                        var resCustNameObj = $("#ResCustName");
                        if (resCustNameObj.val() == "新预订客人" || resCustNameObj.val() == "新入住客人") {
                            resCustNameObj.val(contact);
                            if ($.trim(contactMobile).length > 0) {
                                var resCustMobileObj = $("#ResCustMobile");
                                resCustMobileObj.val(contactMobile);
                            }
                        }
                    }
                    if ($.trim(rateCode).length > 0) {
                        var rateCodeObj = $("#rateCode").data("kendoDropDownList");
                        if (rateCodeObj.value() != rateCode) {
                            rateCodeObj.value(rateCode);
                            editControl_onChange("rateCode", "false");
                        }
                    }
                }
            }, 'json');
        }
    }
}
//天数改变事件
function days_depDate_arriveDate_events() {
    $("#arriveDate").data("kendoDateTimePicker").bind("change", function () { days_depDate_arriveDate_change("arriveDate"); });
    $("#depDate").data("kendoDateTimePicker").bind("change", function () { days_depDate_arriveDate_change("depDate"); });
    $("#days").data("kendoNumericTextBox").bind("change", function () { days_depDate_arriveDate_change("days"); });
    $("#rateCode").data("kendoDropDownList").bind("change", function () { days_depDate_arriveDate_change("rateCode"); });
}
function days_depDate_arriveDate_change(changeid) {
    var daysObj = $("#days").data("kendoNumericTextBox");
    var arrDate = kendo.date.getDate($("#arriveDate").data("kendoDateTimePicker").value());
    var depDateObj = $("#depDate").data("kendoDateTimePicker");
    var depDate = depDateObj.value();
    var days = daysObj.value();
    if (changeid == "days") {
        //天数修改
        if (arrDate == null || arrDate == undefined || days == null || days == undefined) { daysObj.value(null); return; }
        if (depDate == null || depDate == undefined) { depDate = arrDate; }
        arrDate.setDate((arrDate.getDate() + days));
        $("#depDate").data("kendoDateTimePicker").value((arrDate.ToDateString() + " " + depDate.ToTimeString()));
        editControl_onChange("depDate");
    } else {
        //离店时间修改
        if (arrDate == null || arrDate == undefined || depDate == null || depDate == undefined) { daysObj.value(null); return; }
        var num = parseInt((kendo.date.getDate(depDate) - arrDate) / (1000 * 60 * 60 * 24));
        daysObj.value(num < 1 ? null : num);
    }
}
//会员余额提示
function profileComboBox_Tooltip() {
    $("[name='profileComboBox_input']").attr("title", "鼠标移上“会员”两字可查会员账务余额。");
    $("[for='profileComboBox']").kendoTooltip({
        content: { url: CustomerCommonValues.GetMbrCardBlance },
        width: 120,
        height: 127,
        position: "bottom",
        showAfter: 500,
        requestStart: function (e) {
            e.options.url = kendo.format(CustomerCommonValues.GetMbrCardBlance + "/{0}", $("#profileComboBox").data("kendoComboBox").value());
        },
        show: function () {
            var obj = $("[for='profileComboBox']");
            var oldDataId = obj.attr("data-id");
            var newDataId = $("#profileComboBox").data("kendoComboBox").value();
            if (oldDataId != newDataId) {
                obj.attr("data-id", newDataId);
                if (oldDataId != "0") {
                    var objKendoTooltip = obj.data("kendoTooltip");
                    objKendoTooltip.content.html("");
                    objKendoTooltip.refresh();
                }
            }
        }
    });
}
//合约单位提示
function cttidDropDownList_Tooltip() {
    $("[for='Cttid']").kendoTooltip({
        content: { url: CustomerCommonValues.GetCommpanyBlance },
        width: 130,
        height: 95,
        position: "bottom",
        showAfter: 500,
        requestStart: function (e) {
            e.options.url = kendo.format(CustomerCommonValues.GetCommpanyBlance + "/{0}", $("#Cttid").data("kendoDropDownList").value());
        },
        show: function () {
            var obj = $("[for='Cttid']");
            var oldDataId = obj.attr("data-id").toLowerCase();
            var newDataId = $("#Cttid").data("kendoDropDownList").value().toLowerCase();
            if (oldDataId != newDataId) {
                obj.attr("data-id", newDataId);
                if (oldDataId != "0") {
                    var objKendoTooltip = obj.data("kendoTooltip");
                    objKendoTooltip.content.html("");
                    objKendoTooltip.refresh();
                }
            }
        }
    });
}
//合约单位信息展示
function companyDetailInfo() {
    var url = CustomerCommonValues.CompanyManageDetail;
    if (url == null || url == undefined || url.length <= 0) {
        return;
    }
    var cttid = $("#Cttid").data("kendoDropDownList").value();
    if (cttid == null || cttid == undefined || cttid.length <= 0) {
        jAlert("请选择合约单位！"); return;
    }
    url = url + "?id=" +cttid;
    top.openKendoWindow("合约单位详情", url, null);
}
//会员消费信息展示
function profileTransDetailInfo() {
    var url = CustomerCommonValues.ProfileTransDetail;
    if (url == null || url == undefined || url.length <= 0) {
        return;
    }
    var profileid = $("#profileComboBox").data("kendoComboBox").value();
    if (profileid == null || profileid == undefined || profileid.length <= 0) {
        jAlert("请选择会员！"); return;
    }
    top.openSecondIframeKendoWindow("会员消费记录", url, { profileId: profileid }, "30001_17179869184", "消费记录");
}
//客人姓名获得焦点时，自动全选文本框内容
function autoSelectTextBoxContentByName(id) {
    if (id != "ResCustName" && id != "Name" && id != "guestName") { return; }
    var textBox = document.getElementById(id);
    if (textBox == null || textBox == undefined) { return; }
    if (textBox.value != "新预订客人" && textBox.value != "新入住客人") { return; }
    var start = 0;
    var end = 100;
    if (textBox.setSelectionRange) {
        textBox.setSelectionRange(start, end);
    } else if (textBox.createTextRange) {
        var rang = textBox.createTextRange();
        rang.collapse(true);
        rang.moveStart('character', start);
        rang.moveEnd('character', end - start);
        rang.select();
    }
}
//团体/散客单选按钮
function isGroup_clicked(e) {
    var isGroup = $("[name='IsGroup']:checked").val();
    if (isGroup == 1 && $.trim(CustomerCommonValues.GroupDefaultRateCode).length > 0) {
        var isExistsRateCodeId = false;
        var rateCodeObj = $("#rateCode").data("kendoDropDownList");
        var tempList = rateCodeObj.dataItems();
        if (tempList != null && tempList.length > 0) {
            $.each(tempList, function (index, item) {
                if (item.Value == CustomerCommonValues.GroupDefaultRateCode) {
                    isExistsRateCodeId = true;
                    return false;
                }
            });
        }
        if (isExistsRateCodeId == true) {
            if (rateCodeObj.value() != CustomerCommonValues.GroupDefaultRateCode) {
                rateCodeObj.value(CustomerCommonValues.GroupDefaultRateCode);
                editControl_onChange("rateCode");
            }
        }
    }
    else if (isGroup == 0) {
        var rateCodeKendo = $("#rateCode").data("kendoDropDownList");
        var firstItem = rateCodeKendo.dataItem(0);
        if (firstItem == null || firstItem == undefined) { return; }
        var selectedValue = $("#rateCode").data("kendoDropDownList").value();
        if (firstItem.Value == selectedValue) { return; }//判断选中项 是不是第一个
        rateCodeKendo.value(firstItem.Value);
        editControl_onChange("rateCode");
    }
}