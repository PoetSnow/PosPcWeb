//换房
var ChangeRoomWindow = {
    Initialization: function () {
        //初始化
        $("#changeRoomWindow").kendoWindow({
            width: "470px",
            title: "换房",
            visible: false,
            modal: true,
            actions: ["Close"]
        });
        $("#changeRoom_save").unbind("click").click(function (e) { ChangeRoomWindow.Save(e); });
        $("#changeRoom_cancel").unbind("click").click(function (e) { ChangeRoomWindow.Cancel(e); });
        $("#changeRoom_editRatePlan").unbind("click").click(function (e) { ChangeRoomWindow.EditRatePlan(e); });
        $("#changeRoom_roomNoNew").data("kendoDropDownList").list.width(143);
        $("#changeRoom_selectRoom").unbind("click").click(function (e) { ChangeRoomWindow.SelectRoom.Open(e); });
    },
    Open: function () {
        //弹框
        if (!$("#changeRoomWindow").data("kendoWindow")) { ChangeRoomWindow.Initialization(); }
        var item = OrderList.GetSelected();
        if (!(item != null && item != undefined && item.Status == "I")) {
            return;
        }
        $("#changeRoom_roomTypeName").val(item.RoomTypeName);//原房类
        $("#changeRoom_roomNo").val(item.RoomNo);//原房号
        $("#changeRoom_roomPrice").val($("#roomPrice").val());//原房价
        $("#changeRoom_roomTypeNew").data("kendoDropDownList").value(item.RoomTypeId); //新房类
        ChangeRoomWindow.RoomTypeChange();//新房号
        $("#changeRoom_roomPriceNew").val(""); $("#changeRoom_roomPriceNewJson").val("");//新房价
        $("#changeRoomWindow").data("kendoWindow").center().open();
        if (item.RateCodeEntity.IsUseScore == true) {
            jAlert("当前订单属于[积分换房]，换房操作请注意！", "知道了");
        }
    },
    RoomTypeChange: function (postCompleteCallback) {
        //清空房号和房价
        $("#changeRoom_roomNoNew").data("kendoDropDownList").setDataSource(null); $("#changeRoom_roomNoNew").data("kendoDropDownList").select(-1);
        $("#changeRoom_roomPriceNew").val(""); $("#changeRoom_roomPriceNewJson").val("");
        //根据房间类型获取房号
        var roomTypeId = $("#changeRoom_roomTypeNew").data("kendoDropDownList").value();
        var regId = $("#regId").val();
        if (roomTypeId.length > 0 && regId.length > 0) {
            $.ajax({
                type: "POST",
                url: CustomerCommonValues.GetRoomForRoomType,
                data: { regId: regId, roomTypeId: roomTypeId },
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $("#changeRoom_roomNoNew").data("kendoDropDownList").setDataSource(result.Data);//赋值新可用房号列表
                        if (typeof (postCompleteCallback) == "function") {
                            postCompleteCallback();
                        }
                    } else {
                        jAlert(result.Data);
                    }
                },
                error: function (xhr, msg, ex) {
                    jAlert(msg);
                }
            });
        }
    },
    RoomNoChange: function () {
        //清空房价
        $("#changeRoom_roomPriceNew").val(""); $("#changeRoom_roomPriceNewJson").val("");
        //验证
        if ($("#changeRoom_roomNoNew").data("kendoDropDownList").value().length <= 0) { return; }
        var roomTypeId = $("#changeRoom_roomTypeNew").data("kendoDropDownList").value();
        if (roomTypeId.length <= 0) { return; }
        var item = OrderList.GetSelected();
        if (!(item != null && item != undefined && item.Status == "I")) {
            return;
        }
        //根据房类和房号等信息获取房价
        var beginDate = CustomerCommonValues.BusinessDate.replace(/-/g, "/");
        //if (new Date(beginDate) < new Date(item.ArrDate.substring(0, 10))) { beginDate = item.ArrDate.substring(0, 10); }

        var depDateTimeNew = new Date(item.DepDate.substring(0, 10) + " 00:00:00");
        depDateTimeNew.setDate(depDateTimeNew.getDate() + 1);
        if (depDateTimeNew == null) { return; }

        $.post(CustomerCommonValues.GetRateDetailPrices, { rateId: item.RateCode, roomTypeId: roomTypeId, beginDate: beginDate, endDate: depDateTimeNew.ToDateString() }, function (data) {
            var orderDetailPlans = [];
            if (data.Success && data.Data != null && data.Data.length > 0) {
                //添加当前日期以前的价格，用源信息
                for (var i = 0; i < item.OrderDetailPlans.length; i++) {
                    var isAdd = true;
                    for (var j = 0; j < data.Data.length; j++) {
                        if (item.OrderDetailPlans[i].Ratedate == data.Data[j].Ratedate) {
                            isAdd = false;
                            break;
                        }
                    }
                    if (isAdd) {
                        orderDetailPlans.push({ Ratedate: item.OrderDetailPlans[i].Ratedate, Price: item.OrderDetailPlans[i].Price, OriginPrice: item.OrderDetailPlans[i].OriginPrice });
                    }
                }
                //添加当前日期以后的价格，用获取到的新信息
                for (var i = 0; i < data.Data.length; i++) {
                    if (data.Data[i].Price != null) {
                        orderDetailPlans.push({ Ratedate: data.Data[i].Ratedate, Price: data.Data[i].Price, OriginPrice: data.Data[i].Price });
                    }
                }
            } else {
                //用源信息
                orderDetailPlans = item.OrderDetailPlans;
                jAlert("选中的价格代码在住店期间没有当前房型的明细价格，请手工调整价格", "知道了");
            }
            if (orderDetailPlans != null && orderDetailPlans != undefined && orderDetailPlans.length > 0) {
                //显示房价
                var prices = [];
                var length = orderDetailPlans.length;
                for (var i = 0; i < length; i++) {
                    if (orderDetailPlans[i].Ratedate == CustomerCommonValues.BusinessDate) {
                        prices.push(orderDetailPlans[i].Price);
                    }
                }
                $("#changeRoom_roomPriceNew").val(prices.join(","));
                $("#changeRoom_roomPriceNewJson").val(JSON.stringify(orderDetailPlans));
            }
        }, 'json');
    },
    EditRatePlan: function () {
        //修改房价
        OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#changeRoom_roomTypeNew", "#arriveDate", "#depDate", "#changeRoom_roomPriceNew", "#changeRoom_roomPriceNewJson", "#arrBsnsDate");
        OrderDetailRatePlanEditWindow.Open();
    },
    Save: function () {
        //保存换房
        var item = OrderList.GetSelected();
        if (!(item != null && item != undefined && item.Status == "I")) {
            return;
        }
        var roomNoId = $("#changeRoom_roomNoNew").data("kendoDropDownList").value();
        var roomTypeId = $("#changeRoom_roomTypeNew").data("kendoDropDownList").value();
        if (roomTypeId == null || roomTypeId == undefined || roomTypeId.length <= 0) {
            jAlert("请选择房类", "知道了");
            return;
        }
        if (roomNoId == null || roomNoId == undefined || roomNoId.length <= 0) {
            jAlert("请选择房号", "知道了");
            return;
        }
        var roomPriceNewJson = $("#changeRoom_roomPriceNewJson").val();
        if (roomPriceNewJson == null || roomPriceNewJson == undefined || roomPriceNewJson.length <= 0) {
            jAlert("请设置房价", "知道了");
            return;
        }
        var orderDetailPlans = JSON.parse(roomPriceNewJson);
        if (orderDetailPlans == null || orderDetailPlans == undefined || orderDetailPlans.length <= 0) {
            jAlert("请设置房价", "知道了");
            return;
        }
        if (document.getElementById("changeRoom_freeUpgrade").checked) {
            //免费升级使用源房价
            orderDetailPlans = [];
            var length = item.OrderDetailPlans.length;
            if (length > 0) {
                for (var i = 0; i < length; i++) {
                    orderDetailPlans.push({ Price: item.OrderDetailPlans[i].Price, Ratedate: item.OrderDetailPlans[i].Ratedate, OriginPrice: item.OrderDetailPlans[i].OriginPrice });
                }
            }
        }
        $.post(CustomerCommonValues.ChangeRoom, { regId: item.Regid, roomId: roomNoId, orderDetailPlans: orderDetailPlans, remark: $("#changeRoom_remark").val(), saveContinue: $("#SaveContinue").val(), authorizationSaveContinue: $("#authorizationSaveContinue").val() }, function (result) {
            if (result.Success) {
                jAlert("换房成功", "知道了", function () { try { LockWindow.Open(); } catch (e) { } });
                $("#changeRoomWindow").data("kendoWindow").close();
                if (result.ResLogId != null && result.ResLogId != undefined && result.ResLogId.length > 0) {//换房确认单
                    var parameterValues = ("@t00id={t00id}&@t00regid={t00regid}&@t00roomid={t00roomid}").replace("{t00id}", result.ResLogId).replace("{t00regid}", item.Regid).replace("{t00roomid}", roomNoId);
                    $.post(FolioCommonValues.AddQueryParaTemp, { ReportCode: "up_print_changeRoomConfirm", ParameterValues: parameterValues, ChineseName: "换房确认单" }, function (result) {
                        if(result.Success) { window.open(result.Data); }
                    }, 'json');
                }
            }
            else {
                if (result.ErrorCode == 4) { authorizationWindow.Open(1, result.Data, "ChangeRoomWindow.Save"); return; }
                ajaxErrorHandle(result);
            }
            OrderCustomer.RefreshData(result, "ChangeRoomWindow.Save");
        }, 'json');
    },
    Cancel: function () {
        //取消换房
        $("#changeRoomWindow").data("kendoWindow").close();
    },
    SelectRoom: {
        Initialization: function () {
            $("#changeRoom_selectRoomWindow").kendoWindow({
                width: "765px",
                height: "360px",
                title: "选房",
                visible: false,
                modal: true,
                actions: ["Close"]
            });
            $("#changeRoom_selectRoomWindowGrid").kendoGrid({
                columns: [{
                    field: "id",
                    title: "id",
                    hidden: true,
                    attributes: { "class": "roomtypeid" },
                },
                {
                    field: "name",
                    title: "房型",
                },
                {
                    field: "roomqty",
                    title: "可用数",
                }],
                height: 306,
                autoBind: false,
                scrollable: true,
                selectable: "row",
                messages: { "noRecords": "没有可用的记录。" },
                dataSource: {
                    type: (function () { if (kendo.data.transports['aspnetmvc-ajax']) { return 'aspnetmvc-ajax'; } else { throw new Error('The kendo.aspnetmvc.min.js script is not included.'); } })(),
                    transport: {
                        read: {
                            url: CustomerCommonValues.GetRoomType,
                            data: function () {
                                var arriveDateValue = $("#arriveDate").data("kendoDateTimePicker").value();
                                var depDateValue = $("#depDate").data("kendoDateTimePicker").value();
                                return { arrDate: (arriveDateValue == null ? null : arriveDateValue.ToDateTimeString()), depDate: (depDateValue == null ? null : depDateValue.ToDateTimeString()), regId: $("#regId").val(), type: "I" };
                            }
                        }
                    },
                },
                change: function (e) {
                    var selectedRows = this.select();
                    if (selectedRows != null && selectedRows != undefined && selectedRows.length > 0) {
                        var dataItem = this.dataItem(selectedRows[0]);
                        if (dataItem != null && dataItem != undefined) {
                            var floors = $("#changeRoom_selectRoomWindowListView_floor").data("kendoMultiSelect").value();
                            var features = $("#changeRoom_selectRoomWindowListView_feature").data("kendoMultiSelect").value();
                            var roomno = $("#changeRoom_selectRoomWindowListView_roomno").val();
                            ChangeRoomWindow.SelectRoom.GetRooms(dataItem.id, floors.join(","), features.join(","), roomno);
                        }
                    }
                },
                dataBound: function (e) {
                    var selectRow = null;
                    var roomTypeId = $("#changeRoom_roomTypeNew").data("kendoDropDownList").value();
                    if (roomTypeId != null && roomTypeId != undefined && roomTypeId.length > 0) {
                        selectRow = $("#changeRoom_selectRoomWindowGrid td.roomtypeid:contains('" + roomTypeId + "')").parents("tr");
                    }
                    if (selectRow == null || selectRow == undefined || selectRow.length <= 0) {
                        selectRow = $("#changeRoom_selectRoomWindowGrid td.roomtypeid:eq(0)").parents("tr");
                    }
                    if (selectRow != null && selectRow != undefined && selectRow.length > 0) {
                        $("#changeRoom_selectRoomWindowGrid").data("kendoGrid").select(selectRow);
                    }
                }
            });
            $("#changeRoom_selectRoomWindowListView").kendoListView({
                template: kendo.template($("#changeRoom_selectRoomWindow_Template").html()),
                dataBound: function () {
                    $(".house-state-bg").click(function (e) {
                        e.preventDefault();
                        $("#changeRoom_selectRoomWindowListView dl").each(function (index, item) {
                            $(item).removeClass("house-state-selected");
                        });
                        $(this).addClass("house-state-selected");
                    });
                }
            });
            $("#changeRoom_selectRoomWindowListView_floor").kendoMultiSelect({
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            url: CustomerCommonValues.GetFloorList
                        }
                    }
                },
                change: function (e) {
                    ChangeRoomWindow.SelectRoom.Search();
                }
            });
            $("#changeRoom_selectRoomWindowListView_feature").kendoMultiSelect({
                dataTextField: "Name",
                dataValueField: "Name",
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            url: CustomerCommonValues.GetRoomFeaturesSelectList
                        }
                    }
                },
                change: function (e) {
                    ChangeRoomWindow.SelectRoom.Search();
                }
            });
            $("#changeRoom_selectRoomWindowListView_roomno").change(function () { ChangeRoomWindow.SelectRoom.Search(); });
            $("#changeRoom_selectRoomWindow_save").click(function (e) { ChangeRoomWindow.SelectRoom.Save(e); });
            $("#changeRoom_selectRoomWindow_cancel").click(function (e) { ChangeRoomWindow.SelectRoom.Cancel(e); });
        },
        Open: function () {
            var btn = $("#changeRoom_selectRoom");
            if (btn.attr("isInit") != "true") {
                ChangeRoomWindow.SelectRoom.Initialization();
                btn.attr("isInit", "true");
            }
            else {
                $("#changeRoom_selectRoomWindowGrid").data("kendoGrid").clearSelection();
                $("#changeRoom_selectRoomWindowListView_floor").data("kendoMultiSelect").value("");
                $("#changeRoom_selectRoomWindowListView_feature").data("kendoMultiSelect").value("");
                $("#changeRoom_selectRoomWindowListView_roomno").val("");
            }
            $("#changeRoom_selectRoomWindowGrid").data("kendoGrid").dataSource.read();
            $("#changeRoom_selectRoomWindow").data("kendoWindow").center().open();
        },
        Save: function () {
            var roomid = $("#changeRoom_selectRoomWindowListView dl.house-state-selected").attr("data-Roomid");
            if (roomid != null || roomid != undefined && roomid.length > 0) {
                var grid = $("#changeRoom_selectRoomWindowGrid").data("kendoGrid");
                var selectedRows = grid.select();
                if (selectedRows != null && selectedRows != undefined && selectedRows.length > 0) {
                    var dataItem = grid.dataItem(selectedRows[0]);
                    if (dataItem != null && dataItem != undefined) {
                        ChangeRoomWindow.SelectRoom.UpdateFather(dataItem.id, roomid);
                        $("#changeRoom_selectRoomWindow").data("kendoWindow").close();
                        return;
                    }
                }
                jAlert("请选择房型", "知道了");
            } else {
                jAlert("请选择房号", "知道了");
            }
        },
        Cancel: function () {
            $("#changeRoom_selectRoomWindow").data("kendoWindow").close();
        },
        GetRooms: function (roomTypeId, floors, features, roomno) {
            var changeRoom_selectRoomWindowListView = $("#changeRoom_selectRoomWindowListView").data("kendoListView");
            changeRoom_selectRoomWindowListView.setDataSource(new kendo.data.DataSource({ data: [] }));
            changeRoom_selectRoomWindowListView.refresh();
            var regId = $("#regId").val();
            if (roomTypeId.length > 0 && regId.length > 0) {
                $.ajax({
                    type: "POST",
                    url: CustomerCommonValues.GetRoomForRoomType,
                    data: { regId: regId, roomTypeId: roomTypeId, floors: floors, features: features, roomno: roomno },
                    dataType: "json",
                    success: function (result) {
                        if (result.Success) {
                            changeRoom_selectRoomWindowListView.setDataSource(new kendo.data.DataSource({ data: result.Data }));
                            changeRoom_selectRoomWindowListView.refresh();
                            var roomNoId = $("#changeRoom_roomNoNew").data("kendoDropDownList").value();
                            if (roomNoId != null && roomNoId != undefined && roomNoId.length > 0) {
                                var selectRoom = $("#changeRoom_selectRoomWindowListView dl[data-Roomid='" + roomNoId + "']");
                                if (selectRoom != null && selectRoom != undefined && selectRoom.length > 0) {
                                    selectRoom[0].click();
                                }
                            }
                        } else {
                            jAlert(result.Data);
                        }
                    },
                    error: function (xhr, msg, ex) {
                        jAlert(msg);
                    }
                });
            }
        },
        Search: function () {
            var grid = $("#changeRoom_selectRoomWindowGrid").data("kendoGrid");
            var selectedRows = grid.select();
            if (selectedRows != null && selectedRows != undefined && selectedRows.length > 0) {
                var dataItem = grid.dataItem(selectedRows[0]);
                if (dataItem != null && dataItem != undefined) {
                    var floors = $("#changeRoom_selectRoomWindowListView_floor").data("kendoMultiSelect").value();
                    var features = $("#changeRoom_selectRoomWindowListView_feature").data("kendoMultiSelect").value();
                    var roomno = $("#changeRoom_selectRoomWindowListView_roomno").val();
                    ChangeRoomWindow.SelectRoom.GetRooms(dataItem.id, floors.join(","), features.join(","), roomno);
                }
            }
        },
        UpdateFather: function (roomtypeid, roomid) {
            $("#changeRoom_roomTypeNew").data("kendoDropDownList").value(roomtypeid); //新房类
            ChangeRoomWindow.RoomTypeChange(function () {
                $("#changeRoom_roomNoNew").data("kendoDropDownList").value(roomid);
                ChangeRoomWindow.RoomNoChange();
            });//新房号
        },
    },
    AutoSetRoom: function (roomTypeId, roomId) {
        if ($.trim(roomTypeId).length <= 0) { return; }
        if ($.trim(roomId).length <= 0) { return; }
        if ($("#btnChangeRoom").css("display") == "none") { return; }
        document.getElementById("btnChangeRoom").click();
        setTimeout(function () {
            $("#changeRoom_roomTypeNew").data("kendoDropDownList").value(roomTypeId); //新房类
            ChangeRoomWindow.RoomTypeChange(function () {
                $("#changeRoom_roomNoNew").data("kendoDropDownList").value(roomId);
                ChangeRoomWindow.RoomNoChange();
            });
        }, 1000);
    },
};