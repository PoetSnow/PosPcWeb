﻿//子单
//子单列表
var permanentRoom_OrderList = {
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
            permanentRoom_OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
            var arriveDate = $("#arriveDate").data("kendoDatePicker").value();
            var depDate = $("#depDate").data("kendoDatePicker").value();
            var holdDate = $("#holdDate").data("kendoDatePicker").value();
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
                OrderDetailPlans: permanentRoom_OrderDetailRatePlanEditWindow.Get(),
                Status: "R",
                RoomPriceRate: $("#roomPriceRate").data("kendoNumericTextBox").value(),
                CalculateCostCycle: $("#calculateCostCycle").data("kendoDropDownList").value(),
                GenerateCostsCycle: $("#generateCostsCycle").data("kendoNumericTextBox").value(),
                GenerateCostsCycle_Deposit: $("#generateCostsCycle_deposit").data("kendoNumericTextBox").value(),
                GenerateCostsDateAdd: $("#generateCostsDateAdd").data("kendoNumericTextBox").value() * parseInt($("#generateCostsDateAddType").data("kendoDropDownList").value()),
            };
            if (data.Regid.length > 0) {
                var originData = permanentRoom_OrderList.GetItem(data.Regid);
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

            var regId = $("#regId").val();
            var type = $("#resOrderCustomerType").val();
            if (type == "R" || (type == "I" && $.trim(regId).length <= 0)) {
                data.Roomid = $("#permanentRoom_roomId").val();
                data.RoomNo = $("#permanentRoom_roomNo").val();
            }

            return data;
        },
        //设置子单
        Set: function (data) {
            $("#permanentRoomIconButton").css("display", "none");
            var rateCodeChangedIsDayRoom = $("#rateCodeChangedIsDayRoom"); rateCodeChangedIsDayRoom.val("false"); rateCodeChangedIsDayRoom.attr("data-dayroomtime", "");
            var rateCodeChangedIsHou = $("#rateCodeChangedIsHou"); rateCodeChangedIsHou.val("false"); rateCodeChangedIsHou.attr("data-baseminute", "");
            var rateCodeChangedHalfTime = $("#rateCodeChangedHalfTime"); rateCodeChangedHalfTime.val("false"); rateCodeChangedHalfTime.attr("data-halftime", "");
            $("#rateCodeNoPrintProfile").attr("data-id", ""); $("#rateCodeNoPrintProfile").val(""); $("#rateCodeNoPrintProfile").attr("data-isContinue", "");
            $("#rateCodeNoPrintCompany").attr("data-id", ""); $("#rateCodeNoPrintCompany").val(""); $("#rateCodeNoPrintCompany").attr("data-isContinue", "");
            $("#rateCodeChangedRoomTypeids").attr("data-id", ""); $("#rateCodeChangedRoomTypeids").val("");
            var rateCodeIsPriceAdjustment = $("#rateCodeIsPriceAdjustment"); rateCodeIsPriceAdjustment.attr("data-id", ""); rateCodeIsPriceAdjustment.val("");
            permanentRoom_DateTimeRangeSelection("true");
            permanentRoom_OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
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
                data.RateCodeRoomTypeids = (isRateCodeEntity && $.trim(data.RateCodeEntity.RoomTypeids) != "") ? data.RateCodeEntity.RoomTypeids : "";;

                $("#resOrderCustomerType").val(data.Status);
                if (data.RateCodeIsMonth == true && (data.Status == "I" || data.Status == "O" || data.Status == "C")) {
                    $("#permanentRoomIconButton").css("display", "inline-block");
                }
                if (data.ArrDate != null && data.ArrDate != undefined && data.ArrDate.length > 0) {
                    $("#arriveDate").data("kendoDatePicker").value(data.ArrDate);
                } else {
                    $("#arriveDate").data("kendoDatePicker").value("");
                }
                $("#arrBsnsDate").val(data.ArrBsnsDate);
                if (data.DepDate != null && data.DepDate != undefined && data.DepDate.length > 0) {
                    $("#depDate").data("kendoDatePicker").value(data.DepDate);
                } else {
                    $("#depDate").data("kendoDatePicker").value("");
                }
                if (data.HoldDate != null && data.HoldDate != undefined && data.HoldDate.length > 0) {
                    $("#holdDate").data("kendoDatePicker").value(data.HoldDate);
                } else {
                    $("#holdDate").data("kendoDatePicker").value("");
                }
                $("#roomNoDiv").css("display", "none");
                $("#roomNo").data("kendoDropDownList").dataSource.data([]);
                $("#roomNoInput").css("display", "inline-block");
                $("#roomNoInput").val(data.RoomNo);
                if (data.Status == "R") {
                    permanentRoom_roomNo.Enabled();
                } else {
                    permanentRoom_roomNo.Disabled();
                }
                permanentRoom_roomNo.Set(data.Roomid, data.RoomNo);
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
                $("#roomType").data("kendoDropDownList").value(data.RoomTypeId);
                if ($.trim($("#roomType").data("kendoDropDownList").value()) == "") {
                    permanentRoom_showDisabledRoomType(data.RoomTypeId);
                }
                $("#customerSource").data("kendoDropDownList").value(data.Sourceid);
                $("#marketType").data("kendoDropDownList").value(data.Marketid);
                $("#breakfastQty").data("kendoNumericTextBox").value(data.Bbf);
                $("#roomQty").data("kendoNumericTextBox").value(data.RoomQty);
                $("#remark").val(data.Remark);
                $("#special").val(data.Spec);
                
                $("#roomPriceRate").data("kendoNumericTextBox").value(data.RoomPriceRate);
                $("#calculateCostCycle").data("kendoDropDownList").value(data.CalculateCostCycle);
                $("#generateCostsCycle").data("kendoNumericTextBox").value(data.GenerateCostsCycle);
                $("#generateCostsCycle_deposit").data("kendoNumericTextBox").value(data.GenerateCostsCycle_Deposit);
                $("#generateCostsDateAdd").data("kendoNumericTextBox").value(Math.abs(data.GenerateCostsDateAdd));
                var generateCostsDateAddType = "0";if (data.GenerateCostsDateAdd > 0) {generateCostsDateAddType = "1";}else if (data.GenerateCostsDateAdd < 0) {generateCostsDateAddType = "-1";}
                $("#generateCostsDateAddType").data("kendoDropDownList").value(generateCostsDateAddType);

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
                permanentRoom_OrderDetailRatePlanEditWindow.Set(data.OrderDetailPlans);
                permanentRoom_CustomerList.Set(data.OrderDetailRegInfos, data.SelectCustomerId);
                permanentRoom_OrderCustomer.Control.Form(data.Status, (data.Regid != null && data.Regid != undefined && data.Regid.length > 0 ? true : false), data.ResStatus, data.RecStatus);
                if (isRateCodeEntity && data.RateCodeEntity.IsPriceAdjustment == true) {
                    rateCodeIsPriceAdjustment.attr("data-id", data.Regid); rateCodeIsPriceAdjustment.val("true");
                    if (permanentRoom_OrderDetailRatePlanEditWindow.Button.Status()) {
                        permanentRoom_OrderDetailRatePlanEditWindow.Button.Enabled();
                    }
                } else {
                    permanentRoom_OrderDetailRatePlanEditWindow.Button.Disabled(permanentRoom_OrderDetailRatePlanEditWindow.Button.Status());
                }
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
                $("#arriveDate").data("kendoDatePicker").value(arrDateTime);
                $("#depDate").data("kendoDatePicker").value(depDateTime);
                $("#holdDate").data("kendoDatePicker").value(holdDateTime);
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
                permanentRoom_roomNo.Enabled();
                permanentRoom_roomNo.Set("", "");
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
                permanentRoom_editControl_onChange("rateCode", "false");//$("#rateCode").data("kendoDropDownList").select(0);
                permanentRoom_checkDisabledRoomType();
                if (roomTypeId != null && roomTypeId != undefined && roomTypeId.length > 0) {
                    $("#roomType").data("kendoDropDownList").value(roomTypeId);
                } else {
                    permanentRoom_editControl_onChange("roomType", "false");//$("#roomType").data("kendoDropDownList").select(0);
                }
                $("#breakfastQty").data("kendoNumericTextBox").value(0);
                $("#roomQty").data("kendoNumericTextBox").value(1);
                $("#remark").val("");
                $("#special").val("");

                $("#roomPriceRate").data("kendoNumericTextBox").value(0);
                $("#calculateCostCycle").data("kendoDropDownList").value("month");
                $("#generateCostsCycle").data("kendoNumericTextBox").value(1);
                $("#generateCostsCycle_deposit").data("kendoNumericTextBox").value(1);
                $("#generateCostsDateAdd").data("kendoNumericTextBox").value(0);
                $("#generateCostsDateAddType").data("kendoDropDownList").value("0");

                var profileComboBox = $("#profileComboBox").data("kendoComboBox");
                profileComboBox.dataSource.data([]); profileComboBox.text(""); profileComboBox.value("");
                permanentRoom_OrderDetailRatePlanEditWindow.Set(null);
                permanentRoom_CustomerList.Set(null, null);
                permanentRoom_OrderCustomer.Control.Form(type == "I" ? "I" : "R", false, null, null);
                if (type == "R") { $("#guestName").val($("#ResCustName").val()); }
                permanentRoom_DeptDateTime();
            }
            $("#btnSave").removeAttr("data-IsRelationUpdateAllRoonTypeRatePlan");
            $("#IsRelationUpdateAllRemark")[0].checked = false;

            PermanentRoomOrderCustomerJS.Others.CalculateCostCyclee_Change($("#calculateCostCycle").data("kendoDropDownList").value());
            PermanentRoomOrderCustomerJS.Others.GenerateCostsDateAddType_Change($("#generateCostsDateAddType").data("kendoDropDownList").value());

            if (!(data != null && data != undefined)) {
                var roomId = CustomerCommonValues.RoomId;
                var roomNo = CustomerCommonValues.RoomNo;
                CustomerCommonValues.RoomId = "";
                CustomerCommonValues.RoomNo = "";
                if ($.trim(roomId).length > 0 && $.trim(roomNo).length > 0) {
                    permanentRoom_roomNo.Set(roomId, roomNo);
                    permanentRoom_roomNo.GetRoomPrice();
                }
            }
            permanentRoom_days_depDate_arriveDate_change("arriveDate");
        }
    },
    //获取订单内的会员卡号，如果订单有多个会员卡号则返回NULL
    GetOrderMbrCard: function (selectedRegIds) {
        if (selectedRegIds == null || selectedRegIds == undefined || selectedRegIds.length <= 0) { return null;}
        var profileNos = [];
        var list = permanentRoom_OrderList.Get();
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