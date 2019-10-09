//子单
//子单列表
var OrderList = {
    //子单列表ID
    Id: "#grid",
    //获取子单列表
    Get: function () {
        return $(this.Id).data("kendoGrid").dataItems();
    },
    //设置子单列表
    Set: function (data, selectid) {
        var grid = $(this.Id).data("kendoGrid");
        grid.setDataSource(new kendo.data.DataSource({ data: data }));
        grid.clearSelection();
        //this.Order.Set(null);
        this.Select(selectid);
    },
    //在子单列表中选择一项
    Select: function (regid) {
        var obj = $(this.Id);
        var grid = obj.data("kendoGrid");
        grid.clearSelection();

        var tr = null;
        if (regid != null && regid != undefined && regid.length > 0) {
            tr = obj.find("tbody tr td[regid='" + regid + "']").parent();
        } else {
            tr = obj.find("tbody tr:eq(0)");
        }

        if (tr != null && tr != undefined && tr.length > 0) {
            grid.select(tr[0]);
        }

        var row = grid.select();
        this.Order.Set((row != null && row != undefined && row.length > 0) ? grid.dataItem(row) : null);
    },
    //在子单列表中选择一项
    SelectOnly: function (regid) {
        var obj = $(this.Id);
        var grid = obj.data("kendoGrid");
        grid.clearSelection();

        var tr = null;
        if (regid != null && regid != undefined && regid.length > 0) {
            tr = obj.find("tbody tr td[regid='" + regid + "']").parent();
        }

        if (tr != null && tr != undefined && tr.length > 0) {
            grid.select(tr[0]);
        }
    },
    //获取子单列表中的选择项
    GetSelected: function () {
        var grid = $(this.Id).data("kendoGrid");
        var row = grid.select();
        if (row != null && row != undefined && row.length > 0) {
            return grid.dataItem(row[0]);
        }
        return null;
    },
    //清除选中项
    ClearSelect: function () {
        $(this.Id).data("kendoGrid").clearSelection();
        this.Order.Set(null);
    },
    //在子单列表中获取单个项
    GetItem: function (regid) {
        var obj = $(this.Id);
        var grid = obj.data("kendoGrid");
        var tr = null;
        if (regid != null && regid != undefined && regid.length > 0) {
            tr = obj.find("tbody tr td[regid='" + regid + "']").parent();
        } else {
            return null;
        }
        return (tr != null && tr != undefined) ? grid.dataItem(tr): null;
    },
    //子单
    Order: {
        //获取子单
        Get: function () {
            OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
            var arriveDate = $("#arriveDate").data("kendoDateTimePicker").value();
            var depDate = $("#depDate").data("kendoDateTimePicker").value();
            var holdDate = $("#holdDate").data("kendoDateTimePicker").value();
            var data = {
                Regid: $("#regId").val(),
                RateCode: $("#rateCode").data("kendoDropDownList").value(),
                RoomTypeId: $("#roomType").data("kendoDropDownList").value(),
                Sourceid: $("#customerSource").data("kendoDropDownList").value(),
                Marketid: $("#marketType").data("kendoDropDownList").value(),
                ArrDate: arriveDate != null ? arriveDate.ToDateTimeString() : null,
                DepDate: depDate != null ? depDate.ToDateTimeString() : null,
                HoldDate: holdDate != null ? holdDate.ToDateTimeString() : null,
                Bbf: $("#breakfastQty").data("kendoNumericTextBox").value(),
                RoomQty: $("#roomQty").data("kendoNumericTextBox").value(),
                Profileid: $("#profileComboBox").data("kendoComboBox").value(),
                Remark: $("#remark").val(),
                Spec: $("#special").val(),
                OrderDetailPlans: OrderDetailRatePlanEditWindow.Get(),
                Status: "R",
            };
            if (data.Regid.length > 0) {
                var originData = OrderList.GetItem(data.Regid);
                if (data.Regid == originData.Regid) {
                    data.OriginResDetailInfo = originData.OriginResDetailInfo;
                    data.Status = originData.Status;
                    data.Roomid = originData.Roomid;
                    data.RoomNo = originData.RoomNo;
                    data.Guestid = originData.Guestid;
                } else {
                    console.log("异常信息，左侧子单列表中选择的行 和 右侧详细信息 不一致。");
                    return null;
                }
            } else {
                var type = $("#resOrderCustomerType").val();
                if (type == "I") {
                    data.Status = "I";
                    var roomNoDropDownList = $("#roomNo").data("kendoDropDownList");
                    data.Roomid = roomNoDropDownList.value();
                    data.RoomNo = roomNoDropDownList.text();
                }
            }
            return data;
        },
        //设置子单
        Set: function (data) {
            $("#roomTypeIsNotRoom").val("false");
            $("#permanentRoomIconButton").css("display", "none");
            var rateCodeChangedIsDayRoom = $("#rateCodeChangedIsDayRoom"); rateCodeChangedIsDayRoom.val("false"); rateCodeChangedIsDayRoom.attr("data-dayroomtime", "");
            var rateCodeChangedIsHou = $("#rateCodeChangedIsHou"); rateCodeChangedIsHou.val("false"); rateCodeChangedIsHou.attr("data-baseminute", "");
            var rateCodeChangedHalfTime = $("#rateCodeChangedHalfTime"); rateCodeChangedHalfTime.val("false"); rateCodeChangedHalfTime.attr("data-halftime", "");
            $("#rateCodeNoPrintProfile").attr("data-id", ""); $("#rateCodeNoPrintProfile").val(""); $("#rateCodeNoPrintProfile").attr("data-isContinue", "");
            $("#rateCodeNoPrintCompany").attr("data-id", ""); $("#rateCodeNoPrintCompany").val(""); $("#rateCodeNoPrintCompany").attr("data-isContinue", "");
            $("#rateCodeChangedRoomTypeids").attr("data-id", ""); $("#rateCodeChangedRoomTypeids").val("");
            $("#rateCodeIsUseScore").val("");
            var rateCodeIsPriceAdjustment = $("#rateCodeIsPriceAdjustment"); rateCodeIsPriceAdjustment.attr("data-id", ""); rateCodeIsPriceAdjustment.val("");
            DateTimeRangeSelection("true");
            OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
            if (data != null && data != undefined) {
                var isRateCodeEntity = (data.RateCodeEntity != null && data.RateCodeEntity != undefined);
                if (isRateCodeEntity && data.RateCodeEntity.IsDayRoom == true) {
                    rateCodeChangedIsDayRoom.val("true"); rateCodeChangedIsDayRoom.attr("data-dayroomtime", data.RateCodeEntity.DayRoomTime);
                }
                if (isRateCodeEntity && data.RateCodeEntity.IsHou == true) {
                    rateCodeChangedIsHou.val("true"); rateCodeChangedIsHou.attr("data-baseminute", data.RateCodeEntity.BaseMinute);
                }
                if (isRateCodeEntity && $.trim(data.RateCodeEntity.HalfTime) != "") {
                    rateCodeChangedHalfTime.val("true"); rateCodeChangedHalfTime.attr("data-halftime", data.RateCodeEntity.HalfTime);
                }
                data.RateCodeIsMonth = (isRateCodeEntity && data.RateCodeEntity.isMonth == true) ? true : false;
                data.RateCodeNoPrintProfile = (isRateCodeEntity && $.trim(data.RateCodeEntity.NoPrintProfile) != "") ? data.RateCodeEntity.NoPrintProfile : "";
                data.RateCodeNoPrintCompany = (isRateCodeEntity && $.trim(data.RateCodeEntity.NoPrintCompany) != "") ? data.RateCodeEntity.NoPrintCompany : "";
                data.RateCodeRoomTypeids = (isRateCodeEntity && $.trim(data.RateCodeEntity.RoomTypeids) != "") ? data.RateCodeEntity.RoomTypeids : "";
                data.RateCodeIsUseScore = (isRateCodeEntity && data.RateCodeEntity.IsUseScore == true) ? true : false;

                $("#resOrderCustomerType").val(data.Status);
                if (data.RateCodeIsMonth == true && (data.Status == "I" || data.Status == "O" || data.Status == "C")) {
                    $("#permanentRoomIconButton").css("display", "inline-block");
                }
                if (data.ArrDate != null && data.ArrDate != undefined && data.ArrDate.length > 0) {
                    $("#arriveDate").data("kendoDateTimePicker").value(data.ArrDate);
                } else {
                    $("#arriveDate").data("kendoDateTimePicker").value("");
                }
                $("#arrBsnsDate").val(data.ArrBsnsDate);
                if (data.DepDate != null && data.DepDate != undefined && data.DepDate.length > 0) {
                    $("#depDate").data("kendoDateTimePicker").value(data.DepDate);
                } else {
                    $("#depDate").data("kendoDateTimePicker").value("");
                }
                if (data.HoldDate != null && data.HoldDate != undefined && data.HoldDate.length > 0) {
                    $("#holdDate").data("kendoDateTimePicker").value(data.HoldDate);
                } else {
                    $("#holdDate").data("kendoDateTimePicker").value("");
                }
                $("#roomNoDiv").css("display", "none");
                $("#roomNo").data("kendoDropDownList").dataSource.data([]);
                $("#roomNoInput").css("display", "inline-block");
                $("#roomNoInput").val(data.RoomNo);
                $("#regId").val(data.Regid);
                var rateCodeKendo = $("#rateCode").data("kendoDropDownList");
                $("#rateCodeFilter").val("true");
                rateCodeKendo.dataSource.filter({
                    logic: "or",
                    filters: [
                        { field: "Disabled", operator: "eq", value: true },
                        { field: "Value", operator: "eq", value: data.RateCode }
                    ]
                });
                rateCodeKendo.value(data.RateCode);
                $("#rateCodeFilter").val("false");
                $("#rateCodeNoPrintProfile").attr("data-id", data.RateCode); $("#rateCodeNoPrintProfile").val(data.RateCodeNoPrintProfile); $("#rateCodeNoPrintProfile").attr("data-isContinue", "");
                $("#rateCodeNoPrintCompany").attr("data-id", data.RateCode); $("#rateCodeNoPrintCompany").val(data.RateCodeNoPrintCompany); $("#rateCodeNoPrintCompany").attr("data-isContinue", "");
                $("#rateCodeChangedRoomTypeids").attr("data-id", data.RateCode); $("#rateCodeChangedRoomTypeids").val(data.RateCodeRoomTypeids);
                $("#rateCodeIsUseScore").val(data.RateCodeIsUseScore);
                $("#roomType").data("kendoDropDownList").value(data.RoomTypeId);
                if ($.trim($("#roomType").data("kendoDropDownList").value()) == "") {
                    showDisabledRoomType(data.RoomTypeId);
                }
                $("#customerSource").data("kendoDropDownList").value(data.Sourceid);
                $("#marketType").data("kendoDropDownList").value(data.Marketid);
                $("#breakfastQty").data("kendoNumericTextBox").value(data.Bbf);
                $("#roomQty").data("kendoNumericTextBox").value(data.RoomQty);
                $("#remark").val(data.Remark);
                $("#special").val(data.Spec);
                var profileComboBox = $("#profileComboBox").data("kendoComboBox");
                profileComboBox.dataSource.data([]); 
                if (data.Profileid != null) {
                    profileComboBox.dataSource.add({ GuestName: data.ProfileName, Id: data.Profileid, MbrCardNo: data.ProfileNo, Mobile: data.ProfileMobile });
                    profileComboBox.text(data.ProfileNo);
                    profileComboBox.value(data.Profileid);
                } else {
                    profileComboBox.text("");
                    profileComboBox.value("");
                }
                OrderDetailRatePlanEditWindow.Set(data.OrderDetailPlans);
                CustomerList.Set(data.OrderDetailRegInfos, data.SelectCustomerId);
                OrderCustomer.Control.Form(data.Status, (data.Regid != null && data.Regid != undefined && data.Regid.length > 0 ? true : false), data.ResStatus, data.RecStatus);
                if (isRateCodeEntity && data.RateCodeEntity.IsPriceAdjustment == true) {
                    rateCodeIsPriceAdjustment.attr("data-id", data.Regid); rateCodeIsPriceAdjustment.val("true");
                    if(OrderDetailRatePlanEditWindow.Button.Status()) {
                        OrderDetailRatePlanEditWindow.Button.Enabled();
                    }
                } else {
                    OrderDetailRatePlanEditWindow.Button.Disabled(OrderDetailRatePlanEditWindow.Button.Status());
                }

                if (data.RateCodeIsUseScore && data.Status == "I") {
                    rateCodeKendo.enable(false);
                    OrderDetailRatePlanEditWindow.Button.Disabled(true);
                }
                $("#roomTypeIsNotRoom").val(data.RoomTypeIsNotRoom);
            } else {
                //设置默认值
                var type = $("#resOrderCustomerType").val();
                var defaultArrTime = CustomerCommonValues.defaultArrTime;//默认抵店时间
                var defaultHoldTime = CustomerCommonValues.defaultHoldTime;//默认保留时间
                var nowDateTime = kendo.date.today();
                var arrDate = CustomerCommonValues.ArrDate;
                if (arrDate != null && arrDate != undefined && arrDate.length > 0) {
                    nowDateTime = new Date(arrDate + " 00:00:00");
                }
                var arrDateTime = nowDateTime;
                var holdDateTime = nowDateTime;
                var depDateTime = new Date(nowDateTime.valueOf() + 1 * 24 * 60 * 60 * 1000);
                if (type == "I") {
                    var currentDateTime = (new Date()).ToDateTimeWithoutSecondString();
                    arrDateTime = currentDateTime;
                    holdDateTime = currentDateTime;
                    var businessDate = CustomerCommonValues.BusinessDate;
                    if (businessDate != null && businessDate != undefined && businessDate.length > 0) {
                        depDateTime = new Date(new Date(businessDate + " 00:00:00").valueOf() + 1 * 24 * 60 * 60 * 1000);
                        $("#arrBsnsDate").val(businessDate);
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
                    if (new Date(arrDateTime) < new Date().setMinutes(new Date().getMinutes() + 30)) {
                        arrDateTime = new Date(new Date().valueOf() + 1 * 60 * 60 * 1000);
                    }
                    if (new Date(holdDateTime) < arrDateTime) {
                        holdDateTime = new Date(arrDateTime.valueOf() + 1 * 60 * 60 * 1000);
                    }
                    $("#arrBsnsDate").val("");
                }
                $("#arriveDate").data("kendoDateTimePicker").value(arrDateTime);
                $("#depDate").data("kendoDateTimePicker").value(depDateTime);
                $("#holdDate").data("kendoDateTimePicker").value(holdDateTime);
                var roomTypeId = CustomerCommonValues.RoomTypeId;
                var roomId = CustomerCommonValues.RoomId;
                if (type == "I") {
                    $("#roomNoInput").css("display", "none"); $("#roomNoInput").val("");
                    $("#roomNoDiv").css("display", "block"); 
                    $("#tabstripAuth-1").css("height", "auto");
                } else {
                    $("#roomNoDiv").css("display", "none"); 
                    $("#roomNoInput").css("display", "inline-block"); $("#roomNoInput").val("");
                }
                $("#roomNo").data("kendoDropDownList").dataSource.data([]);
                //初始化
                $("#regId").val("");
                //$("#customerSource").data("kendoDropDownList").select(0);
                //$("#marketType").data("kendoDropDownList").select(0);
                var rateCodeKendo = $("#rateCode").data("kendoDropDownList");
                $("#rateCodeFilter").val("true");
                rateCodeKendo.dataSource.filter({ field: "Disabled", operator: "eq", value: true });
                var isExistsRateCode = false;
                var rateCodeList = rateCodeKendo.dataSource.view();
                $.each(rateCodeList, function (index, item) {
                    if (item.Value == rateCodeKendo.value()) {
                        isExistsRateCode = true;
                        return false;
                    }
                });
                if (!isExistsRateCode) {
                    rateCodeKendo.select(0);
                }
                $("#rateCodeFilter").val("false");
                editControl_onChange("rateCode", "false");//$("#rateCode").data("kendoDropDownList").select(0);
                checkDisabledRoomType();
                if (roomTypeId != null && roomTypeId != undefined && roomTypeId.length > 0) {
                    $("#roomType").data("kendoDropDownList").value(roomTypeId);
                } else {
                    editControl_onChange("roomType", "false");//$("#roomType").data("kendoDropDownList").select(0);
                }
                $("#breakfastQty").data("kendoNumericTextBox").value(0);
                $("#roomQty").data("kendoNumericTextBox").value(1);
                $("#remark").val("");
                $("#special").val("");
                var profileComboBox = $("#profileComboBox").data("kendoComboBox");
                profileComboBox.dataSource.data([]); profileComboBox.text(""); profileComboBox.value("");
                OrderDetailRatePlanEditWindow.Set(null);
                CustomerList.Set(null, null);
                OrderCustomer.Control.Form(type == "I" ? "I" : "R", false, null, null);
                if (type == "R") { $("#guestName").val($("#ResCustName").val()); }
            }
            $("#btnSave").removeAttr("data-IsRelationUpdateAllRoonTypeRatePlan");
            $("#IsRelationUpdateAllRemark")[0].checked = false;
            days_depDate_arriveDate_change("arriveDate");
        }
    },
    //获取订单内的会员卡号，如果订单有多个会员卡号则返回NULL
    GetOrderMbrCard: function (selectedRegIds) {
        if (selectedRegIds == null || selectedRegIds == undefined || selectedRegIds.length <= 0) { return null;}
        var profileNos = [];
        var list = OrderList.Get();
        if (list != null && list != undefined && list.length > 0) {
            $.each(list, function (index, item) {
                if (item != null && item != undefined && item.Profileid != null && item.Profileid != undefined) {
                    $.each(selectedRegIds, function (regidIndex, regidItem) {
                        if (regidItem == item.Regid) {
                            profileNos.push(item.ProfileNo);
                        }
                    });
                }
            });
        }
        if (profileNos != null && profileNos != undefined && profileNos.length > 0) {
            var profileNo = profileNos[0];
            $.each(profileNos, function (index, item) {
                if (profileNo != item) {
                    profileNo = null;
                    return false;
                }
            });
            return (profileNo != null && profileNo != undefined && profileNo.length > 0) ? profileNo : null;
        }
        return null;
    },
};