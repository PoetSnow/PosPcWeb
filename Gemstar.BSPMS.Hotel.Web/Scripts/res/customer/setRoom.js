//分房入住
var SetRoomWindow = {
    //初始化
    Initialization: function () {
        $("#setRoomWindow").kendoWindow({
            width: "750px",
            title: "分房/入住",
            visible: false,
            modal: true,
            actions: ["Close"],
            close: function () { document.getElementById("checkAllSetRoom").checked = false; }
        });
        $("#autoSetRoomButton").unbind("click").click(function (e) { SetRoomWindow.AutoSetRoom(e); });
        $("#saveSetRoomButton").unbind("click").click(function (e) { SetRoomWindow.SaveSetRoom(e); });
        $("#checkInRoomButton").unbind("click").click(function (e) { SetRoomWindow.CheckInSetRoom(e); });
        $("#clearSetRoomButton").unbind("click").click(function (e) { SetRoomWindow.ClearSetRoom(e); });
    },
    //打开
    Open: function () {
        $("#setRoom_selectRoomIds").val("");
        if (!$("#setRoomWindow").data("kendoWindow")) { SetRoomWindow.Initialization(); }
        var trHtml = [];
        var items = OrderList.Get();
        if (items != null && items != undefined && items.length > 0) {
            var length = items.length;
            for (var i = 0; i < length; i++) {
                if (items[i].Status == "R") {
                    var prices = [];
                    if (items[i].OrderDetailPlans != null && items[i].OrderDetailPlans != undefined && items[i].OrderDetailPlans.length > 0) {
                        for (var j = 0; j < items[i].OrderDetailPlans.length; j++) {
                            prices.push(items[i].OrderDetailPlans[j]["Price"]);
                        }
                    }
                    for (var k = 0; k < items[i].RoomQty; k++) {
                        var checkName = "checkSetRoom";
                        var checkId = checkName + "_" + i + "_" + k;
                        var inputName = "inputRoomNo_" + items[i].Regid;
                        var inputId = inputName + "_" + k;

                        var tdHtml = [];
                        tdHtml.push("<tr>");
                        tdHtml.push("<td><input type=\"checkbox\" class=\"k-checkbox\" name=\"" + checkName + "\" id=\"" + checkId + "\"/><label class=\"k-checkbox-label\" for=\"" + checkId + "\"></label></td>");
                        tdHtml.push("<td><input name=\"" + inputName + "\" id=\"" + inputId + "\" data-roomid=\"" + items[i].Roomid + "\" data-roomno=\"" + items[i].RoomNo + "\" /></td>");
                        tdHtml.push("<td>" + items[i].RoomTypeName + "</td>");
                        tdHtml.push("<td>" + prices.join(",") + "</td>");
                        tdHtml.push("<td>" + items[i].Guestname + "</td>");
                        tdHtml.push("<td>" + new Date(items[i].ArrDate.replace(/-/g, "/")).ToDateTimeWithoutSecondString() + "</td>");
                        tdHtml.push("</tr>");
                        trHtml.push(tdHtml.join(""));
                    }
                }
            }
        }
        $("#setRoomTbody").empty().html(trHtml.join(""));
        if (trHtml.length > 0) {
            var roomlist = SetRoomWindow.GetRoomAutoChoose(false);
            if (roomlist != null && roomlist.length > 0) {
                $.each(roomlist, function (k, obj) {
                    $("#setRoomTbody").find("input[name='inputRoomNo_" + obj.Key + "']").each(function (index, item) {
                        var roomObj = $("#" + item.id);
                        roomObj.kendoDropDownList({
                            template: "<span style=\"display:inline-block;width:50px;\">#= preFix ##= roomno #</span>#= remark #",
                            dataTextField: "roomno",
                            dataValueField: "Roomid",
                            filter: "contains",
                            dataSource: obj.Value,
                            optionLabel: " ",
                            change: function (e) {
                                SetRoomWindow.RoomIdChanged();
                            },
                            open: function (e) {
                                if (e != null && e != undefined && e.sender != null && e.sender != undefined) {
                                    var selectRoomid = e.sender.value();
                                    e.sender.dataSource.filter({
                                        logic: "and",
                                        filters: SetRoomWindow.RoomIdOpened(selectRoomid)
                                    });
                                }
                            },
                        });
                        var roomDropDownList = roomObj.data("kendoDropDownList");
                        roomDropDownList.list.width(168);
                        var roomid = roomObj.attr("data-roomid");
                        if (roomid != null && roomid != "null" && roomid != undefined && roomid != "undefined" && roomid != "" && roomid.length > 0) {
                            roomDropDownList.value(roomid);
                        } else {
                            roomDropDownList.select(-1);
                        }
                    });
                });
                SetRoomWindow.RoomIdChanged();
                $("#setRoomWindow").data("kendoWindow").center().open();
            } else {
                jAlert("没有需要分房的订单", "知道了");
            }
        } else {
            jAlert("只有在预订状态下才可以使用分房/入住", "知道了");
        }
    },
    //获取可用房号
    GetRoomAutoChoose: function (isAuto) {
        var result = null;
        $.ajax({
            async: false,
            type: "POST",
            url: CustomerCommonValues.GetRoomAutoChoose,
            data: { resId: $("#Resid").val(), isAuto: isAuto },
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    result = data.Data;
                } else {
                    jAlert(data.Data, "知道了");
                }
            },
            error: function (xhr, msg, ex) {
                jAlert(msg, "知道了");
            }
        });
        return result;
    },
    //全选
    SelectAll: function () {
        //全选
        var checked = $("input#checkAllSetRoom")[0].checked;
        $("#setRoomTbody").find("input[name='checkSetRoom']").each(function (index, item) {
            if (!item.disabled) {
                item.checked = checked;
            }
        });
    },
    //自动分房
    AutoSetRoom: function () {
        var roomlistAuto = SetRoomWindow.GetRoomAutoChoose(true);
        if (roomlistAuto == null || roomlistAuto.length <= 0) {
            $("#setRoomWindow").data("kendoWindow").close(); return;
        }
        var roomlist = SetRoomWindow.GetRoomAutoChoose(false);
        if (roomlist != null && roomlist.length > 0) {
            $.each(roomlist, function (k, obj) {
                $("#setRoomTbody").find("input[name='inputRoomNo_" + obj.Key + "']").each(function (index, item) {
                    var dropdownlist = $(item).data("kendoDropDownList");
                    dropdownlist.setDataSource(obj.Value);
                    $.each(roomlistAuto, function (t, room) {
                        if (room.Key == obj.Key) {
                            if (room.Value[index] != null && room.Value[index] != undefined) {
                                dropdownlist.value(room.Value[index].Roomid);
                            }
                        }
                    });

                });
            });
            jAlert("自动分房完毕！", "知道了");
            SetRoomWindow.RoomIdChanged();
        } else {
            $("#setRoomWindow").data("kendoWindow").close(); return;
        }
    },
    //保存分房
    SaveSetRoom: function () {
        $("#saveSetRoomButton").attr("disabled", "disabled");
        var items = OrderList.Get();
        if (!(items != null && items != undefined && items.length > 0)) { $("#saveSetRoomButton").removeAttr("disabled"); return; }
        var regidList = [];
        var itemsLength = items.length;
        for (var i = 0; i < itemsLength; i++) {
            if (items[i].Status == "R") {
                var roomIdAndName = [];
                var inputName = "inputRoomNo_" + items[i].Regid;
                $("#setRoomTbody").find("input[name='" + inputName + "']").each(function (index, item) {
                    var dropdownlist = $(item).data("kendoDropDownList");
                    var text = dropdownlist.text();
                    var value = dropdownlist.value();
                    if (text != null && text != undefined && text.length > 0 && value != null && value != undefined && value.length > 0) {
                        roomIdAndName.push({ Key: value, Value: text });
                    }
                });
                if (roomIdAndName.length > 0) {
                    regidList.push({ Key: items[i].Regid.toString(), Value: roomIdAndName });
                }
            }
        }
        $.post(CustomerCommonValues.SaveRooms, { resId: $("#Resid").val(), data: regidList, saveContinue: $("#SaveContinue").val() }, function (result) {
            if (result.Success) {
                jAlert("保存成功！", "知道了", function () { $("#setRoomWindow").data("kendoWindow").center().close(); OrderCustomer.RefreshData(result, "SetRoomWindow.SaveSetRoom"); });
            }
            else {
                ajaxErrorHandle(result);
                OrderCustomer.RefreshData(result, "SetRoomWindow.SaveSetRoom");
            }
            $("#saveSetRoomButton").removeAttr("disabled");
        }, 'json');
    },
    //保存分房并入住
    CheckInSetRoom: function () {
        $("#checkInRoomButton").attr("disabled", "disabled");
        var items = OrderList.Get();
        if (!(items != null && items != undefined && items.length > 0)) { $("#checkInRoomButton").removeAttr("disabled"); return; }
        var regidList = [];
        var itemsLength = items.length;
        for (var i = 0; i < itemsLength; i++) {
            if (items[i].Status == "R") {
                var roomIdAndName = [];
                var inputName = "inputRoomNo_" + items[i].Regid;
                $("#setRoomTbody").find("input[name='" + inputName + "']").each(function (index, item) {
                    if ($(item).parents("#setRoomTbody tr").find('input[type="checkbox"]:checked').length > 0) {
                        var dropdownlist = $(item).data("kendoDropDownList");
                        var text = dropdownlist.text();
                        var value = dropdownlist.value();
                        if (text != null && text != undefined && text.length > 0 && value != null && value != undefined && value.length > 0) {
                            roomIdAndName.push({ Key: value, Value: text });
                        }
                    }
                });
                if (roomIdAndName.length > 0) {
                    regidList.push({ Key: items[i].Regid.toString(), Value: roomIdAndName });
                }
            }
        }
        if (regidList.length <= 0) {
            $("#checkInRoomButton").removeAttr("disabled");
            jAlert("请勾选房号！", "知道了"); return;
        }
        $.post(CustomerCommonValues.SaveRoomsAndCheckIn, { resId: $("#Resid").val(), data: regidList, saveContinue: $("#SaveContinue").val(), useScoreSaveContinue: $("#UseScoreSaveContinue").val() }, function (result) {
            if (result.Success) {
                jAlert("保存并入住成功！", "知道了", function () { $("#setRoomWindow").data("kendoWindow").center().close(); OrderCustomer.RefreshData(result, "SetRoomWindow.CheckInSetRoom"); });
            }
            else {
                if (result.ErrorCode != 5) {
                    ajaxErrorHandle(result);
                }
                OrderCustomer.RefreshData(result, "SetRoomWindow.CheckInSetRoom");
            }
            $("#checkInRoomButton").removeAttr("disabled");
        }, 'json');
    },
    //清除分房
    ClearSetRoom: function () {
        var items = OrderList.Get();
        if (!(items != null && items != undefined && items.length > 0)) { return; }
        var regidList = [];
        var itemsLength = items.length;
        var isChecked = false;
        for (var i = 0; i < itemsLength; i++) {
            if (items[i].Status == "R") {
                var inputName = "inputRoomNo_" + items[i].Regid;
                $("#setRoomTbody").find("input[name='" + inputName + "']").each(function (index, item) {
                    var itemObj = $(item);
                    if (itemObj.parents("#setRoomTbody tr").find('input[type="checkbox"]:checked').length > 0) {
                        isChecked = true;
                        var roomid = itemObj.attr("data-roomid");
                        if (roomid != null && roomid != "null" && roomid != undefined && roomid != "undefined" && roomid != "" && roomid.length > 0) {
                            regidList.push({ Key: items[i].Regid.toString(), Value: [{ Key: roomid, Value: itemObj.attr("data-roomno") }] });
                        }
                    }
                });
            }
        }
        if (regidList.length <= 0) {
            if (isChecked) {
                jAlert("清除留房成功", "知道了");
                $("#setRoomWindow").data("kendoWindow").center().close();
                return;
            }
            jAlert("请勾选房号！", "知道了"); return;
        }
        $.post(CustomerCommonValues.ClearRooms, { resId: $("#Resid").val(), data: regidList }, function (result) {
            if (result.Success) {
                jAlert("清除留房成功", "知道了");
                $("#setRoomWindow").data("kendoWindow").center().close();
            }
            else {
                ajaxErrorHandle(result);
            }
            OrderCustomer.RefreshData(result, "SetRoomWindow.ClearSetRoom");
        }, 'json');
    },
    //房间下拉框Change事件
    RoomIdChanged: function () {
        var roomIdAndName = [];
        var items = OrderList.Get();
        if (items != null && items != undefined && items.length > 0) {
            var itemsLength = items.length;
            for (var i = 0; i < itemsLength; i++) {
                if (items[i].Status == "R") {
                    var inputName = "inputRoomNo_" + items[i].Regid;
                    $("#setRoomTbody").find("input[name='" + inputName + "']").each(function (index, item) {
                        var dropdownlist = $(item).data("kendoDropDownList");
                        var text = dropdownlist.text();
                        var value = dropdownlist.value();
                        if (text != null && text != undefined && text.length > 0 && value != null && value != undefined && value.length > 0) {
                            roomIdAndName.push(value);
                        }
                    });
                }
            }
        }
        $("#setRoom_selectRoomIds").val(roomIdAndName.join("|"));
    },
    //房间下拉框Open事件
    RoomIdOpened: function (selectRoomid) {
        var result = [];
        var roomids = $("#setRoom_selectRoomIds").val();
        if ($.trim(roomids).length > 0) {
            var list = roomids.split("|");
            if (list != null && list != undefined && list.length > 0) {
                $.each(list, function (index, roomid) {
                    if (roomid != selectRoomid) {
                        result.push({ field: "Roomid", operator: "neq", value: roomid });
                    }
                });
            }
        }
        return result;
    },
};