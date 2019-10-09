//客情
//用户是否更改了当前表单的内容。以便在做其他操作时提示是否要保存修改。
var hasChangedMain = false;
var hasChanged = false;
var showMbr = false;
function orderCustomerInitialization() {
    var cttDropdown = $("#Cttid").data("kendoDropDownList");
    if(cttDropdown){
        cttDropdown.list.width(230);
        cttDropdown.dataSource.read();
        $(cttDropdown.filterInput).attr("placeholder", "最少输入两个字符");
    }
    var rateCodeDropdown = $("#rateCode").data("kendoDropDownList");
    if(rateCodeDropdown){
        rateCodeDropdown.dataSource.read();
    }
    var marketTypeDropdown = $("#marketType").data("kendoDropDownList");
    if(marketTypeDropdown){
        marketTypeDropdown.dataSource.read();
    }
    var customerSourceDropdown = $("#customerSource").data("kendoDropDownList");
    if(customerSourceDropdown){
        customerSourceDropdown.dataSource.read();
    }
    var cerTypeDropdown = $("#cerType").data("kendoDropDownList");
    if(cerTypeDropdown){
        cerTypeDropdown.dataSource.read();
    }

    //选择子单
    if (CustomerCommonValues.SelectRegId.length > 0) {
        OrderCustomer.GetRemote(CustomerCommonValues.SelectRegId);
    } else {
        OrderList.Select(CustomerCommonValues.SelectRegId);
    }
    //初始化按钮名称
    OrderCustomer.TypeLoad();
    //会员下拉框宽度
    $("#profileComboBox").data("kendoComboBox").list.width(230);
    //客人姓名下拉框宽度
    $("#guestName").data("kendoAutoComplete").list.width(300);
    //发票
    $("#btnInvoiceInfo").click(function (e) { btnInvoiceInfo_clicked(e); });
    //房价修改
    $("#btnRatePlan").click(function (e) { btnRatePlan_clicked(e); });
    //扫描
    $("#scanIdButton").unbind("click");
    $("#scanIdButton").click(function (e) { $("#certificatesScan").val("customer"); scanIdButton_clicked(e); });
    //扫描会员卡
    $("#mbrScanIdButton").unbind("click");
    $("#mbrScanIdButton").click(function (e) { $("#MbrCardScan").val("CustomMbr"); MbrCardButton_clicked(e); });
    //修改内容
    $("[data-controltype='editcontrol-main']").change(function (e) { editControl_main_changed(e); });
    //修改内容
    $("[data-controltype='editcontrol']").change(function (e) { editControl_changed(e); });
    //增加预订
    $("#btnAddROrderDetail").click(function (e) { btnAddROrderDetail_clicked(e); });
    //增加入住
    $("#btnAddIOrderDetail").click(function (e) { btnAddIOrderDetail_clicked(e); });
    //取消子单
    $("#btnCancelOrderDetail").click(function (e) { btnCancelOrderDetail_clicked(e); });
    //恢复子单
    $("#btnRecoveryOrderDetail").click(function (e) { btnRecoveryOrderDetail_clicked(e); });
    //保存
    $("#btnSave").click(function (e) { btnSave_clicked(e); });
    //分房/入住
    $("#btnSetRoom").click(function (e) { btnSetRoom_clicked(e); });
    //预订确认单
    $("#btnResReoprt").click(function (e) { resReport_clicked("btnResReoprt"); });
    //门卡
    $("#btnLock").click(function (e) { btnLock_clicked(e); });
    //换房
    $("#btnChangeRoom").click(function (e) { btnChangeRoom_clicked(e); });
    //延期
    $("#btnDelay").click(function (e) { btnDelay_clicked(e); });
    //关联房
    $("#btnRelation").click(function (e) { btnRelation_clicked(e); });
    //RC单
    $("#btnResRCReoprt").click(function (e) { resReport_clicked("btnResRCReoprt");});
    //增加客人
    $("#btnAddOrderDetailRegInfo").click(function (e) { btnAddOrderDetailRegInfo_clicked(e); });
    //移除客人
    $("#btnCancelOrderDetailRegInfo").click(function (e) { btnCancelOrderDetailRegInfo_clicked(e); });
    //客情维护
    $("#btnCustomersEditInfo").click(function (e) { openAddCustomerInfosWindow(false); });
    //房号读取
    roomNo_read();
    //初始化
    OrderCustomer.Initialization();
    //证件号改变事件
    $("#cerId").change(function () { cerId_changed("true"); });
    $("#ResCustName").change(function () { customerName_changed("ResCustName"); });
    $("#Name").change(function () { customerName_changed("Name"); });
    $("#ResCustMobile").change(function () { resCustMobile_changed("ResCustMobile"); });
    $("#guestMobile").change(function () { resCustMobile_changed("guestMobile"); });
    profileComboBox_Tooltip();
    cerIdPhoto_Tooltip();
    $("[for='guestName']").dblclick(function () { guestDetailInfo(); });
    $("#guestName").attr("title", "双击查看客历详情");
    $("#remarkTd").dblclick(function (e) { remark_dblclick(e); });
    $("#customerUpdateRemarkWindow_SubmitButton").click(function (e) { customerUpdateRemarkWindow_SubmitButton(); });
    $("#customerUpdateRemarkWindow_CancelButton").click(function (e) { customerUpdateRemarkWindow_CancelButton(); });
    //客情设置
    $("#btnResBillSetting").click(function (e) {resBillSettingButton_clicked(false);});
    cttidDropDownList_Tooltip();
    $("#permanentRoomIconButton").click(function (e) { permanentRoomIconButton_clicked(e); });
    $("[for='Cttid']").dblclick(function () { companyDetailInfo(); });
    $("[for='profileComboBox']").dblclick(function () { profileTransDetailInfo(); });
    $("#dateDays").data("kendoNumericTextBox").bind("change", function () {days_depDate_arriveDate_change("dateDays");});
    $("[name='IsGroup']").click(function (e) {isGroup_clicked(e); });
    $("#ResCustName").focus(function () { autoSelectTextBoxContentByName("ResCustName"); });
    $("#Name").focus(function () { autoSelectTextBoxContentByName("Name"); });
    $("#guestName").focus(function () { autoSelectTextBoxContentByName("guestName"); });
    //批量操作
    $("#btnBatchChangeOrderDetailStatus").click(function (e) { batchChangeOrderStatusButton_clicked(e); });
};
//客情
var OrderCustomer = {
    //初始化
    Initialization: function () {
        //初始化发票弹框
        //OrderMainInvoiceInfoWindow.Initialization();
        //初始化房价修改弹框
        //OrderDetailRatePlanEditWindow.Initialization();
        //初始化门卡弹框
        //LockWindow.Initialization();
        //初始化延期弹框
        //DelayWindow.Initialization();
        //初始化换房弹框
        //ChangeRoomWindow.Initialization();
        //初始化关联房弹框
        //RelationWindow.Initialization();
        //初始化分房入住弹框
        //SetRoomWindow.Initialization();
    },
    //保存
    Save: function () {
        if ($("#btnSave").css("display") != "inline-block") {
            jAlert("当前状态内不能保存"); return;
        }
        //获取主单
        var data = OrderMain.Get();
        if (data == null) {
            jAlert("请填写表单"); return;
        }
        if (data.ResCustName.length <= 0) {
            jAlert("请输入" + $("label[for='ResCustName']").text()); return;
        }
        //获取子单
        data.ResDetailInfos = [OrderList.Order.Get()];
        if (data.ResDetailInfos == null || data.ResDetailInfos.length <= 0 || data.ResDetailInfos[0] == null || data.ResDetailInfos[0] == undefined) {
            jAlert("请填写子单"); return;
        }
        if (data.ResDetailInfos[0].Status == "I") {
            if (data.ResDetailInfos[0].Roomid == null || data.ResDetailInfos[0].Roomid == undefined || data.ResDetailInfos[0].Roomid.length <= 0
                ||
                data.ResDetailInfos[0].RoomNo == null || data.ResDetailInfos[0].RoomNo == undefined || data.ResDetailInfos[0].RoomNo.length <= 0) {
                $("#roomNo").data("kendoDropDownList").dataSource.read();
                jAlert("请选择房号"); return;
            }
        }
        //设置子单列表的选中项
        data.SelectRegId = data.ResDetailInfos[0].Regid;
        if (data.ResDetailInfos[0].RoomTypeId.length <= 0) {
            jAlert("请选择房间类型"); return;
        }
        if (data.ResDetailInfos[0].RateCode.length <= 0) {
            jAlert("请选择价格代码"); return;
        }
        //验证价格代码适用房型
        var tempRateCode = $("#rateCodeChangedRoomTypeids");
        var tempRateId = tempRateCode.attr("data-id");var tempRateTypeids = tempRateCode.val();
        if (tempRateId != null && tempRateId != undefined && tempRateId.length > 0 && tempRateId == data.ResDetailInfos[0].RateCode) {
            if (tempRateTypeids != null && tempRateTypeids != undefined && tempRateTypeids.length > 0) {
                if (("," + tempRateTypeids + ",").indexOf("," + data.ResDetailInfos[0].RoomTypeId + ",") == -1) {
                    jAlert("<" + $("#rateCode").data("kendoDropDownList").text() + ">价格代码不适用当前房型"); return;
                }
            }
        }
        if (data.ResDetailInfos[0].Sourceid.length <= 0) {
            jAlert("请选择客人来源"); return;
        }
        if (data.ResDetailInfos[0].Marketid.length <= 0) {
            jAlert("请选择市场分类"); return;
        }
        if (data.ResDetailInfos[0].Bbf == null) {
            jAlert("请输入早餐份数"); return;
        }
        if (data.ResDetailInfos[0].RoomQty == null) {
            jAlert("请输入房数"); return;
        }
        if (data.ResDetailInfos[0].ArrDate == null || data.ResDetailInfos[0].ArrDate == undefined || data.ResDetailInfos[0].ArrDate.length <= 0) {
            jAlert("请选择抵店时间"); return;
        }
        if (data.ResDetailInfos[0].DepDate == null || data.ResDetailInfos[0].DepDate == undefined || data.ResDetailInfos[0].DepDate.length <= 0) {
            jAlert("请选择离店时间"); return;
        }
        if (data.ResDetailInfos[0].HoldDate == null || data.ResDetailInfos[0].HoldDate == undefined || data.ResDetailInfos[0].HoldDate.length <= 0) {
            jAlert("请选择保留时间"); return;
        }
        if (data.ResDetailInfos[0].OrderDetailPlans == null || data.ResDetailInfos[0].OrderDetailPlans.length <= 0) {
            jAlert("请设置房价"); return;
        }
        if ($.trim($("#guestName").val()).length > 0) {
            //先把表单上的客人信息更新到客人列表中
            var selectCustomerId = CustomerList.Add();
            if (selectCustomerId == null) {return;}
            //设置客人列表中的选中项
            data.ResDetailInfos[0].SelectCustomerId = selectCustomerId;
        }
        //获取客人列表
        data.ResDetailInfos[0].OrderDetailRegInfos = CustomerList.Get();
        if (data.ResDetailInfos[0].OrderDetailRegInfos == null || data.ResDetailInfos[0].OrderDetailRegInfos.length <= 0) {
            jAlert("请输入客人姓名"); return;
        }
        var OrderDetailRegInfosLength = data.ResDetailInfos[0].OrderDetailRegInfos.length;
        for (var i = 0; i < OrderDetailRegInfosLength; i++) {
            var guestid = data.ResDetailInfos[0].OrderDetailRegInfos[i].Guestid;
            if (guestid != null && guestid != undefined && guestid.length > 0) {
                //客人列表中有熟客，则给子单熟客ID赋值
                data.ResDetailInfos[0].Guestid = guestid;
                break;
            }
        }
        //根据价格代码设置选择是否必填会员和合约单位
        var rateCodeId = data.ResDetailInfos[0].RateCode;
        if (rateCodeId == $("#rateCodeNoPrintProfile").attr("data-id") && $.trim(data.ResDetailInfos[0].Profileid) == '') {
            var rateCodeNoPrintProfile = $("#rateCodeNoPrintProfile").val();
            switch (rateCodeNoPrintProfile) {
                case "1": {
                    if ($("#rateCodeNoPrintProfile").attr("data-isContinue") != "true") {
                        jConfirm("此价格代码需要录入会员，是否继续保存？", "  是  ", "  否  ", function (confirmed) {
                            if (confirmed) {
                                $("#rateCodeNoPrintProfile").attr("data-isContinue", "true");
                                $("#btnSave")[0].click();
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
        if (rateCodeId == $("#rateCodeNoPrintCompany").attr("data-id") && $.trim(data.Cttid) == '') {
            var rateCodeNoPrintCompany = $("#rateCodeNoPrintCompany").val();
            switch (rateCodeNoPrintCompany) {
                case "1": {
                    if ($("#rateCodeNoPrintCompany").attr("data-isContinue") != "true") {
                        jConfirm("此价格代码需要录入合约单位，是否继续保存？", "  是  ", "  否  ", function (confirmed) {
                            if (confirmed) {
                                $("#rateCodeNoPrintCompany").attr("data-isContinue", "true");
                                $("#btnSave")[0].click();
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
        //保存
        $("#btnSave").attr("disabled", "disabled");
        if ($("#btnSave").attr("data-IsRelationUpdateAllRoonTypeRatePlan") == "true") { data.IsRelationUpdateAllRoonTypeRatePlan = true; } $("#btnSave").removeAttr("data-IsRelationUpdateAllRoonTypeRatePlan");
        data.IsRelationUpdateAllRemark = $("#IsRelationUpdateAllRemark")[0].checked; $("#IsRelationUpdateAllRemark")[0].checked = false;
        $.post(CustomerCommonValues.Save, { resInfo: data }, function (result) {
            OrderCustomer.RefreshData(result, "OrderCustomer.Save");
            $("#btnSave").removeAttr("disabled");
        }, 'json');
    },
    //从服务器获取数据，和刷新页面等价
    GetRemote: function (regid) {
        if (regid == null || regid == undefined || regid.length <= 0) {
            var selectRegid = $("#regId").val();
            if (selectRegid == null || selectRegid == undefined || selectRegid.length <= 0) {
                var list = OrderList.Get();
                if (list == null || list == undefined || list.length <= 0) {
                    return;
                } else {
                    regid = list[0].Regid;
                }
            } else {
                regid = selectRegid;
            }
        }
        $.post(CustomerCommonValues.GetResMainInfoByRegId, { regid: regid }, function (result) {
            OrderCustomer.RefreshData(result, null);
        }, 'json');
    },
    //把获取到的结果数据更新到页面,和刷新页面等价
    RefreshData: function (result, funcName) {
        if (result.Success) {
            hasChanged = false;
            hasChangedMain = false;
            OrderMain.Set(result.Data);
            OrderList.Set(result.Data.ResDetailInfos, result.Data.SelectRegId);
            setAutoOpenRC(result.Data.SelectRegIdIsNewCheckIn || CustomerCommonValues.IsBatchCheckIn == "True");
            setAutoOpenWindow(result.Data.SelectRegIdIsNewCheckIn);
            if (result.Data != null && result.Data != undefined && result.Data.OtherMessage != null && result.Data.OtherMessage != undefined && result.Data.OtherMessage.length > 0) {
                jAlert(result.Data.OtherMessage, "知道了");
            }
            RefreshData_End();
        }
        else {
            //判断是出错信息还是对象，如果是对象，则可能是房间有冲突，需要提示后询问是否继续
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
                            $("#SaveContinue").val("1");
                            try {
                                var func = eval(funcName);
                                if (typeof (func) == "function") {
                                    func();
                                }
                            } catch (e) { }
                        }
                    });
                }
            }
            else {
                if (result.ErrorCode == 4) { authorizationWindow.Open(1, result.Data, "OrderCustomer.Save"); return; }
                if (result.ErrorCode == 5) { useScoreToCheckinWindow.Open(result.Data, ($.trim(funcName).length <= 0 ? "OrderCustomer.Save": funcName)); return; }
                //如果是出错信息，则直接提示一下即可
                //jAlert(result.Data, "知道了");
                ajaxErrorHandle(result);
            }
        }
    },
    //控制页面元素
    Control: {
        //按钮控制 status：当前子单状态，isUpdate：是否要更新子单，resStatus：预订状态，recStatus：入住状态
        Button: function (status, isUpdate, resStatus, recStatus) {
            $("#btnSave").css("display", "none");
            $("#btnSetRoom").css("display", "none");
            $("#btnResReoprt").css("display", "none");
            $("#btnLock").css("display", "none");
            $("#btnChangeRoom").css("display", "none");
            $("#btnDelay").css("display", "none");
            $("#btnRelation").css("display", "none");
            $("#btnResRCReoprt").css("display", "none");
            $("#btnCancelOrderDetail").css("display", "none");
            $("#btnRecoveryOrderDetail").css("display", "none");
            $("#btnAddOrderDetailRegInfo").css("visibility", "visible");
            $("#btnCancelOrderDetailRegInfo").css("visibility", "visible");
            //$("#orderMainInvoiceInfoWindow_submit").css("display", "inline-block");
            OrderDetailRatePlanEditWindow.Button.Enabled();
            $("#btnCustomersEditInfo").css("visibility", "hidden");
            switch (status) {
                case "R"://预订状态
                    {
                        $("#btnSave").css("display", "inline-block");
                        if (isUpdate) {
                            $("#btnSetRoom").css("display", "inline-block");
                            $("#btnResReoprt").css("display", "inline-block");
                            $("#btnLock").css("display", "inline-block");
                        }
                    }
                    break;
                case "I"://在住状态
                    {
                        $("#btnSave").css("display", "inline-block");
                        if (isUpdate) {
                            $("#btnLock").css("display", "inline-block");
                            $("#btnChangeRoom").css("display", "inline-block");
                            $("#btnDelay").css("display", "inline-block");
                            $("#btnRelation").css("display", "inline-block");
                            $("#btnResRCReoprt").css("display", "inline-block");
                            $("#btnCustomersEditInfo").css("visibility", "visible");
                        }
                    }
                    break;
                case "N"://noshow
                case "X"://取消状态
                case "O"://离店迟付状态
                case "C"://离店且结账状态
                case "Z"://Z(或其它) :未入住 Z(或其它):非预订单
                    {
                        //所有功能按钮禁用
                        $("#btnAddOrderDetailRegInfo").css("visibility", "hidden");
                        $("#btnCancelOrderDetailRegInfo").css("visibility", "hidden");
                        //$("#orderMainInvoiceInfoWindow_submit").css("display", "none");
                        OrderDetailRatePlanEditWindow.Button.Disabled();
                        if (status == "C" || status == "O") { $("#btnLock").css("display", "inline-block"); $("#btnResRCReoprt").css("display", "inline-block"); }
                        if (status == "N") { $("#btnLock").css("display", "inline-block"); }
                    }
                    break;
            }
            if (status == "R") {//预订状态
                if (resStatus == "R" && (recStatus == null || recStatus == undefined || recStatus.length <= 0)) {
                    //直接预订，没有其他操作。最后结果是：预订状态
                    $("#btnCancelOrderDetail").text("取消预订");
                    $("#btnCancelOrderDetail").css("display", "inline-block");
                } else if (resStatus == "R" && recStatus == "Z") {
                    //先预订，再入住，再取消入住。最后结果是：预订状态
                    $("#btnCancelOrderDetail").text("取消预订");
                    $("#btnCancelOrderDetail").css("display", "inline-block");
                    $("#btnRecoveryOrderDetail").text("恢复入住");
                    $("#btnRecoveryOrderDetail").css("display", "inline-block");
                    $("#btnRecoveryOrderDetail").attr("data-status", "Z");
                }
                //设置状态，区分预订、入住
                $("#btnCancelOrderDetail").attr("data-status", status);
            } else if (status == "I") {//在住状态
                if ((resStatus == null || resStatus == undefined || resStatus.length <= 0) && recStatus == "I") {
                    //直接入住，没有其他操作。最后结果是：在住状态
                    $("#btnCancelOrderDetail").text("取消入住");
                    $("#btnCancelOrderDetail").css("display", "inline-block");
                } else if (resStatus == "R" && recStatus == "I") {
                    //先预订，在入住。最后结果是：在住状态
                    $("#btnCancelOrderDetail").text("取消入住");
                    $("#btnCancelOrderDetail").css("display", "inline-block");
                }
                $("#btnCancelOrderDetail").attr("data-status", status);
            } else if (status == "X") {//取消状态
                if (resStatus == "X" && (recStatus == null || recStatus == undefined || recStatus.length <= 0)) {
                    //直接预订，再取消预订。最后结果是：取消状态
                    $("#btnRecoveryOrderDetail").text("恢复预订");
                    $("#btnRecoveryOrderDetail").css("display", "inline-block");
                    $("#btnRecoveryOrderDetail").attr("data-status", "Y");
                } else if (resStatus == "X" && recStatus == "Z") {
                    //先预订，再入住，再取消入住，再取消预订。最后结果是：取消状态
                    $("#btnRecoveryOrderDetail").text("恢复预订");
                    $("#btnRecoveryOrderDetail").css("display", "inline-block");
                    $("#btnRecoveryOrderDetail").attr("data-status", "Y");
                } else if ((resStatus == null || resStatus == undefined || resStatus.length <= 0) && recStatus == "X") {
                    //直接入住，再取消入住。最后结果是：取消状态
                    $("#btnRecoveryOrderDetail").text("恢复入住");
                    $("#btnRecoveryOrderDetail").css("display", "inline-block");
                    $("#btnRecoveryOrderDetail").attr("data-status", "Z");
                }
            } else if (status == "N") {//NoShow
                $("#btnRecoveryOrderDetail").text("恢复预订");
                $("#btnRecoveryOrderDetail").css("display", "inline-block");
                $("#btnRecoveryOrderDetail").attr("data-status", "N");
            }

        },
        //表单内容控制 status：当前子单状态，isUpdate：是否要更新子单，resStatus：预订状态，recStatus：入住状态
        Form: function (status, isUpdate, resStatus, recStatus) {
            $("#arriveDate").data("kendoDateTimePicker").enable();
            $("#depDate").data("kendoDateTimePicker").enable();
            $("#holdDate").data("kendoDateTimePicker").enable();
            $("#roomType").data("kendoDropDownList").enable();
            $("#roomNo").data("kendoDropDownList").enable();
            $("#roomQty").data("kendoNumericTextBox").enable();
            if (status == "R" || status == "I" || status == "N") { OrderCustomer.Control.AllControl.Enabled();} else {OrderCustomer.Control.AllControl.Disabled(); }
            if ($("#roomNoInput").val().length > 0 && $("#roomNoInput").css("display") == "inline-block") {
                $("#roomQty").data("kendoNumericTextBox").enable(false);
            }
            if (status == "I") {
                if (isUpdate) {
                    $("#arriveDate").data("kendoDateTimePicker").enable(false);
                    $("#depDate").data("kendoDateTimePicker").enable(false);
                    $("#holdDate").data("kendoDateTimePicker").enable(false);
                    $("#roomType").data("kendoDropDownList").enable(false);
                    $("#roomNo").data("kendoDropDownList").enable(false);
                } else {
                    $("#arriveDate").data("kendoDateTimePicker").enable(false);
                    $("#holdDate").data("kendoDateTimePicker").enable(false);
                }
                $("#roomQty").data("kendoNumericTextBox").enable(false);
            }
            this.Button(status, isUpdate, resStatus, recStatus);//表单内包含按钮
            DateTimeRangeSelection();
        },
        //表单内除按钮外所有控件
        AllControl: {
            Enabled: function () {
                $("[data-controltype='editcontrol-main']").removeAttr("disabled", "disabled");
                $("[data-controltype='editcontrol']").removeAttr("disabled", "disabled");
                $("#Cttid").data("kendoDropDownList").enable();
                $("#arriveDate").data("kendoDateTimePicker").enable();
                $("#depDate").data("kendoDateTimePicker").enable();
                $("#holdDate").data("kendoDateTimePicker").enable();
                $("#roomType").data("kendoDropDownList").enable();
                $("#roomNo").data("kendoDropDownList").enable();
                $("#roomQty").data("kendoNumericTextBox").enable();
                $("#profileComboBox").data("kendoComboBox").enable();
                $("#marketType").data("kendoDropDownList").enable();
                $("#customerSource").data("kendoDropDownList").enable();
                $("#rateCode").data("kendoDropDownList").enable();
                $("#breakfastQty").data("kendoNumericTextBox").enable();
                $("#guestName").data("kendoAutoComplete").enable();
                $("#cerType").data("kendoDropDownList").enable();
                $("#nation").data("kendoDropDownList").enable();
                $("#birthday").data("kendoDatePicker").enable();
                $("#scanIdButton").unbind("click");
                $("#scanIdButton").click(function (e) { $("#certificatesScan").val("customer"); scanIdButton_clicked(e); });
                $("#resCustomerTable .k-input,#resCustomerTable .k-textbox:not(#remark)").css("background-color", "#fff");
            },
            Disabled: function () {
                $("[data-controltype='editcontrol-main']").attr("disabled", "disabled");
                $("[data-controltype='editcontrol']").attr("disabled", "disabled");
                $("#Cttid").data("kendoDropDownList").enable(false);
                $("#arriveDate").data("kendoDateTimePicker").enable(false);
                $("#depDate").data("kendoDateTimePicker").enable(false);
                $("#holdDate").data("kendoDateTimePicker").enable(false);
                $("#roomType").data("kendoDropDownList").enable(false);
                $("#roomNo").data("kendoDropDownList").enable(false);
                $("#roomQty").data("kendoNumericTextBox").enable(false);
                $("#profileComboBox").data("kendoComboBox").enable(false);
                $("#marketType").data("kendoDropDownList").enable(false);
                $("#customerSource").data("kendoDropDownList").enable(false);
                $("#rateCode").data("kendoDropDownList").enable(false);
                $("#breakfastQty").data("kendoNumericTextBox").enable(false);
                $("#guestName").data("kendoAutoComplete").enable(false);
                $("#cerType").data("kendoDropDownList").enable(false);
                $("#nation").data("kendoDropDownList").enable(false);
                $("#birthday").data("kendoDatePicker").enable(false);
                $("#scanIdButton").unbind("click");
                $("#resCustomerTable .k-input,#resCustomerTable .k-textbox:not(#remark)").css("background-color", "#f5f5f5");
            },
        },
    },
    //取消子单
    CancelOrderDetail: function () {
        var regId = $("#regId").val();
        if (regId == null || regId == undefined || regId.length <= 0) {
            jAlert("请先选择子单"); return;
        }
        var saveContinue = $("#SaveContinue").val();
        var orderDetail = $("#btnCancelOrderDetail").attr("data-status");//当前按钮是取消预订还是取消入住 R:预订 I：入住
        var url = orderDetail == "R" ? CustomerCommonValues.CancelOrderDetailY : CustomerCommonValues.CancelOrderDetailZ;
        $.post(url, { regId: regId, saveContinue: saveContinue }, function (result) {
            OrderCustomer.RefreshData(result, "OrderCustomer.CancelOrderDetail");
        }, 'json');
    },
    //恢复子单
    RecoveryOrderDetail: function () {
        var regId = $("#regId").val();
        if (regId == null || regId == undefined || regId.length <= 0) {
            jAlert("请先选择子单"); return;
        }
        var saveContinue = $("#SaveContinue").val();
        var orderDetail = $("#btnRecoveryOrderDetail").attr("data-status");
        var url = orderDetail == "Z" ? CustomerCommonValues.RecoveryOrderDetailZ : CustomerCommonValues.RecoveryOrderDetailY;
        $.post(url, { regId: regId, saveContinue: saveContinue }, function (result) {
            OrderCustomer.RefreshData(result, "OrderCustomer.RecoveryOrderDetail");
        }, 'json');
    },
    //初始化按钮名称
    TypeLoad: function () {
        var type = $("#resOrderCustomerType").val();
        var resCustName = $("#ResCustName");
        var guestName = $("#guestName");
        var name = $("#Name");
        var rName = "新预订客人";
        var iName = "新入住客人";
        if (type == "R") {
            if (resCustName.val().length <= 0 || $("#Resid").val().length <= 0) {
                resCustName.val(rName);
                guestName.val(rName);
                name.val(rName);
            }
        } else if (type == "I") {
            if (resCustName.val().length <= 0 || $("#Resid").val().length <= 0) {
                resCustName.val(iName);
                guestName.val(iName);
                name.val(iName);
            }
        }
    }
};

/*---------- events start ----------*/
//发票
function btnInvoiceInfo_clicked(e) {
    if (e) { e.preventDefault(); }
    //OrderMainInvoiceInfoWindow.Open();
}
//订单列表
function grid_databound() {
    $(OrderList.Id).find("tr").on("click", function (e) {
        if (e) { e.preventDefault(); }
        var thisObj = $(this);
        var regid = $("#regId").val();
        if (hasChanged) {
            jConfirm("当前数据已经修改，是否保存?", "  是  ", "  否  ", function (confirmed) {
                if (confirmed) {
                    OrderList.SelectOnly(regid);
                    OrderCustomer.Save();
                    return;
                } else {
                    hasChanged = false;
                    OrderList.Select(thisObj.find("td:eq(0)").attr("regid"));
                }
            }, null, "取消", function () {
                OrderList.SelectOnly(regid);
            });
        } else {
            hasChanged = false;
            OrderList.Select(thisObj.find("td:eq(0)").attr("regid"));
        }
    });
}
//客人列表
function orderDetailCustomersListView_databound() {
    $(CustomerList.Id).find("li").on("click", function (e) {
        if (e) { e.preventDefault(); }
        CustomerList.Select(this.id);
    });
}
//内容修改
function editControl_main_changed(e) {
    if (e) { e.preventDefault(); }
    hasChangedMain = true;
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
        //如果触发事件的是抵店日期，离店日期，则重新加载房间类型信息
        $("#roomType").data("kendoDropDownList").dataSource.read();
        DateTimeRangeSelection();
    }
    if (id == "rateCode") {
        //如果触发事件的是价格码，则从服务器获取客人来源，市场分类，离店时间，是否有早餐
        rateCode_changed();
    } else if (id == "roomType") {
        //如果触发事件是房间类型，则从服务器获取早餐份数
        roomType_changed();
    } else if (id == "arriveDate") {
        //如果触发事件是抵店时间，则保留时间为同一天，离店时间为抵店时间加1天
        arriveDate_changed();
    }
    if (id == "roomType" || id == "arriveDate" || id == "depDate") {
        //如果触发事件的是价格码，房型，抵店日期，离店日期，则重新检查是否四个值都已经有值了，有则重新加载价格信息
        OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
        OrderDetailRatePlanEditWindow.GetRemote();
    }
    if (id == "roomType" || id == "arriveDate" || id == "depDate") {
        //如果触发事件的是房型，抵店日期，离店日期，则清空房号数据
        roomNo_read();
    }
    if (id == "guestName") {
        ////如果触发事件的是客人姓名，则清空客人ID数据
        var guestidObj = $("#guestid");
        if (guestidObj.attr("selected") != "selected") {
            //guestidObj.val("");
        } else {
            guestidObj.removeAttr("selected");
        }
        customerName_changed(id);
    }
    if (id == "arriveDate" || id == "depDate") {
        days_depDate_arriveDate_change(id);
    }
    if (id == "profileComboBox") {
        showMbr = true;
        $("#profileComboBox").data("kendoComboBox").dataSource.read();
    }
}
//房号读取
function roomNo_read() {
    //如果触发事件的是房型，抵店日期，离店日期，则清空房号数据
    $("#roomNo").data("kendoDropDownList").dataSource.data([]);
    if ($("#roomNoDiv").css("display") == "block") {
        if ($("#arriveDate").data("kendoDateTimePicker").value() != null && $("#depDate").data("kendoDateTimePicker").value() != null && $("#roomType").data("kendoDropDownList").value().length > 0) {
            $("#roomNo").data("kendoDropDownList").dataSource.read();
        }
    }
}
//房价修改
function btnRatePlan_clicked(e) {
    if (e) { e.preventDefault(); }
    OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
    OrderDetailRatePlanEditWindow.Open();
}
//增加子单
function btnAddROrderDetail_clicked(e) {
    $("#resOrderCustomerType").val("R");
    OrderCustomer.TypeLoad();
    btnAddOrderDetail_clicked(e);
}
function btnAddIOrderDetail_clicked(e) {
    $("#resOrderCustomerType").val("I");
    OrderCustomer.TypeLoad();
    btnAddOrderDetail_clicked(e);
}
function btnAddOrderDetail_clicked(e) {
    if (e) { e.preventDefault(); }
    if (hasChanged) {
        jConfirm("当前数据已经修改，是否保存?", "  是  ", "  否  ", function (confirmed) {
            if (confirmed) {
                OrderList.SelectOnly($("#regId").val());
                OrderCustomer.Save();
                return;
            } else {
                hasChanged = false;
                $("#txtAddROrderDetail").val("true");
                OrderList.ClearSelect();
            }
        }, null, "取消");
    } else {
        hasChanged = false;
        $("#txtAddROrderDetail").val("true");
        OrderList.ClearSelect();
    }
}
//取消子单
function btnCancelOrderDetail_clicked(e) {
    if (e) { e.preventDefault(); }
    var name = $("#btnCancelOrderDetail").text();
    jConfirm("确定要" + name + "吗?", "确定", "返回", function (confirmed) {
        if (confirmed) {
            OrderCustomer.CancelOrderDetail();
        }
    });
}
//恢复子单
function btnRecoveryOrderDetail_clicked(e) {
    if (e) { e.preventDefault(); }
    var dataStatus = $("#btnRecoveryOrderDetail").attr("data-status");
    var name = $("#btnRecoveryOrderDetail").text();
    if (dataStatus == "N" && name == "恢复预订") {
        jAlert("请先修改抵店时间，离店时间，保留时间，价格等信息，再单击保存按钮来恢复预订。", "知道了", function () {
            $("#btnAddOrderDetailRegInfo").css("visibility", "visible");
            $("#btnCancelOrderDetailRegInfo").css("visibility", "visible");
            //$("#orderMainInvoiceInfoWindow_submit").css("display", "inline-block");
            var rateCodeIsPriceAdjustment = $("#rateCodeIsPriceAdjustment");
            var regId = $("#regId").val();
            if ($.trim(regId) != "" && rateCodeIsPriceAdjustment.attr("data-id") == regId && rateCodeIsPriceAdjustment.val() == "true") {
                OrderDetailRatePlanEditWindow.Button.Enabled();
            } else {
                OrderDetailRatePlanEditWindow.Button.Disabled(true);
            }
            $("#btnSave").css("display", "inline-block");
        });
        return;
    }
    jConfirm("确定要" + name + "吗?", "确定", "返回", function (confirmed) {
        if (confirmed) {
            OrderCustomer.RecoveryOrderDetail();
        }
    });
}
//增加客人
function btnAddOrderDetailRegInfo_clicked(e) {
    if (e) { e.preventDefault(); }
    CustomerList.Add();
}
//移除客人
function btnCancelOrderDetailRegInfo_clicked(e) {
    if (e) { e.preventDefault(); }
    var removeId = CustomerList.Remove();
    if (removeId != null && removeId != undefined && removeId.length > 0) {
        hasChanged = true;
    }
}
//客人姓名选择
function guestName_selected(e) {
    if (e != null && e != undefined && e.item != null && e.item != undefined) {
        var dataItem = this.dataItem(e.item.index());
        setCustomerInfo(dataItem);
    }
}
function setCustomerInfo(dataItem) {
    if (dataItem != null && dataItem != undefined) {
        $("#guestName").val(dataItem.GuestName);
        $("#guestMobile").val(dataItem.Mobile);
        $("#cerId").val(dataItem.Cerid);
        $("#city").val(dataItem.City);
        $("#address").val(dataItem.Address);
        $("#qq").val(dataItem.Qq);
        $("#email").val(dataItem.Email);
        $("#carNo").val(dataItem.CarNo);
        $("#love").val(dataItem.Interest);
        var guestidObj = $("#guestid");
        guestidObj.val(dataItem.Id);
        guestidObj.attr("selected", "selected");
        if (dataItem.CerType != null && dataItem.CerType != undefined && dataItem.CerType.length > 0) {
            $("#cerType").data("kendoDropDownList").value(dataItem.CerType);
        }
        if (dataItem.Gender != null && dataItem.Gender != undefined && dataItem.Gender.length > 0) {
            if (dataItem.Gender == "F") {
                $("[name='gender'][value='F']")[0].checked = true;
            }
            else {
                $("[name='gender'][value='M']")[0].checked = true;
            }
        }
        if (dataItem.Birthday != null && dataItem.Birthday != undefined) {
            $("#birthday").data("kendoDatePicker").value(dataItem.Birthday);
        }
        if (dataItem.Nation != null && dataItem.Nation != undefined && dataItem.Nation.length > 0) {
            $("#nation").data("kendoDropDownList").value(dataItem.Nation);
        }
        customerName_changed("guestName");
        resCustMobile_changed("guestMobile");
        if (dataItem.BlacklistReason != null && dataItem.BlacklistReason != undefined && $.trim(dataItem.BlacklistReason).length > 0) {
            jAlert("此客人在黑名单上，原因：\n" + dataItem.BlacklistReason);
        }
        if ($.trim(dataItem.CompanyName) != "") {
            var cttidDropdownlist = $("#Cttid").data("kendoDropDownList");
            if (cttidDropdownlist != null && cttidDropdownlist != undefined && $.trim(cttidDropdownlist.value()) == "") {
                cttidDropdownlist.value(dataItem.CompanyName);
                cttidDropdownlist.trigger("select");
                cttidDropdownlist.trigger("change");
            }
        }
    }
}

//保存
function btnSave_clicked(e) {
    if (e) { e.preventDefault(); }
    OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
    OrderCustomer.Save();
}
//分房/入住
function btnSetRoom_clicked(e) {
    if (e) { e.preventDefault(); }
    SetRoomWindow.Open();
}
//预订确认单 RC单
function resReport_clicked(id) {
    openReportWindow(id);
}
//门卡
function btnLock_clicked(e) {
    if (e) { e.preventDefault(); }
    var lockKendoWindow = $("#lockWindow").data("kendoWindow");
    if (!lockKendoWindow) {
        LockWindow.Initialization();
    }
    LockWindow.Open();
}
//换房
function btnChangeRoom_clicked(e) {
    if (e) { e.preventDefault(); }
    ChangeRoomWindow.Open();
}
//延期
function btnDelay_clicked(e) {
    if (e) { e.preventDefault(); }
    DelayWindow.Open();
}
//关联房
function btnRelation_clicked(e) {
    if (e) { e.preventDefault(); }
    RelationWindow.Open();
}
/*---------- events end ----------*/

//价格码改变事件
function rateCode_changed() {
    //设置客人来源，市场分类，离店时间，是否有早餐
    $("#permanentRoomIconButton").css("display", "none");
    var rateCodeValue = $("#rateCode").data("kendoDropDownList").value();
    if (rateCodeValue.length <= 0) {
        return;
    }
    $.post(CustomerCommonValues.GetRate, { rateId: rateCodeValue }, function (result) {
        if (result.Success) {
            if (result.Data != null && result.Data != undefined) {
                $("#rateCodeIsUseScore").val(result.Data.IsUseScore);
                var rateCodeChangedIsDayRoom = $("#rateCodeChangedIsDayRoom"); rateCodeChangedIsDayRoom.val("false"); rateCodeChangedIsDayRoom.attr("data-dayroomtime", "");
                var rateCodeChangedIsHou = $("#rateCodeChangedIsHou"); rateCodeChangedIsHou.val("false"); rateCodeChangedIsHou.attr("data-baseminute", "");
                var rateCodeChangedHalfTime = $("#rateCodeChangedHalfTime"); rateCodeChangedHalfTime.val("false"); rateCodeChangedHalfTime.attr("data-halftime", "");
                if (result.Data.isMonth == true && ($.trim($("#resOrderCustomerType").val()) == "I" || $.trim($("#resOrderCustomerType").val()) == "O" || $.trim($("#resOrderCustomerType").val()) == "C")) {
                    $("#permanentRoomIconButton").css("display", "inline-block");
                }
                $("#rateCodeNoPrintProfile").attr("data-id", rateCodeValue); $("#rateCodeNoPrintProfile").val(result.Data.NoPrintProfile); $("#rateCodeNoPrintProfile").attr("data-isContinue", "");
                $("#rateCodeNoPrintCompany").attr("data-id", rateCodeValue); $("#rateCodeNoPrintCompany").val(result.Data.NoPrintCompany); $("#rateCodeNoPrintCompany").attr("data-isContinue", "");
                $("#rateCodeChangedRoomTypeids").attr("data-id", rateCodeValue);$("#rateCodeChangedRoomTypeids").val(result.Data.RoomTypeids);
                var rateCodeIsPriceAdjustment = $("#rateCodeIsPriceAdjustment"); rateCodeIsPriceAdjustment.attr("data-id", ""); rateCodeIsPriceAdjustment.val("");
                if (result.Data.IsPriceAdjustment == true) {
                    rateCodeIsPriceAdjustment.attr("data-id", rateCodeValue); rateCodeIsPriceAdjustment.val("true");
                    OrderDetailRatePlanEditWindow.Button.Enabled();
                } else {
                    OrderDetailRatePlanEditWindow.Button.Disabled(true);
                }
                if ($("#txtAddROrderDetail").val() != "true") {
                    //客人来源
                    if (result.Data.Sourceid != null && result.Data.Sourceid != undefined && result.Data.Sourceid.length > 0) { $("#customerSource").data("kendoDropDownList").value(result.Data.Sourceid); }
                    else {
                        var customerSourceDropdown = $("#customerSource").data("kendoDropDownList");
                        var type = $("#resOrderCustomerType").val();
                        if (type == "I") {
                            customerSourceDropdown.value(CustomerCommonValues.CurrentHotelId + "05walk");
                        } else if (type == "R") {
                            customerSourceDropdown.value(CustomerCommonValues.CurrentHotelId + "05resv");
                        }
                        var valueStr = customerSourceDropdown.value();
                        if (!(valueStr != null && valueStr != undefined && valueStr.length > 0)) {
                            customerSourceDropdown.select(0);
                        }
                    }
                    //市场分类
                    if (result.Data.Marketid != null && result.Data.Marketid != undefined && result.Data.Marketid.length > 0) { $("#marketType").data("kendoDropDownList").value(result.Data.Marketid); }
                }
                //是否有早餐
                if (result.Data.Bbf != null && result.Data.Bbf != undefined && result.Data.Bbf == 0) {
                    $("#rateCodeChangedBbf").val("0"); $("#breakfastQty").data("kendoNumericTextBox").value(0);//无早餐
                }
                else {
                    $("#rateCodeChangedBbf").val("1"); $("#breakfastQty").data("kendoNumericTextBox").value(""); roomType_changed();//有早餐,获取早餐份数
                }
                var originData = OrderList.GetItem($("#regId").val());
                if (!(originData != null && originData != undefined && originData.Status == "I")) {
                    //离店时间
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
                                if (depDateTime != null && depDateTime != undefined && arriveDateTime.ToDateString() == depDateTime.ToDateString()) {
                                    depDateTime = new Date(arriveDateTime.valueOf() + 1 * 24 * 60 * 60 * 1000);
                                    if ($.trim($("#regId").val()).length <= 0 && $.trim($("#resOrderCustomerType").val()) == "I") {
                                        var businessDate = CustomerCommonValues.BusinessDate;
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
                    days_depDate_arriveDate_change("rateCode");
                }
                //更改时间之后查询房价
                OrderDetailRatePlanEditWindow.SetConstructor("#rateCode", "#roomType", "#arriveDate", "#depDate", "#roomPrice", "#roomPriceJson", "#arrBsnsDate");
                OrderDetailRatePlanEditWindow.GetRemote();
            }
        } else {
            //jAlert(result.Data);
            ajaxErrorHandle(result);
        }
        $("#txtAddROrderDetail").val("false");
    }, 'json');
}
//房间类型改变事件
function roomType_changed() {
    //设置早餐份数
    if ($.trim($("#rateCodeChangedBbf").val()) != "1") {
        return;
    }
    var roomTypeId = $("#roomType").data("kendoDropDownList").value();
    if (roomTypeId.length <= 0) {
        return;
    }
    $.post(CustomerCommonValues.GetBreakFastQty, { roomTypeId: roomTypeId }, function (result) {
        if (result.Success) {
            $("#breakfastQty").data("kendoNumericTextBox").value(result.Data);
        } else {
            //jAlert(result.Data);
            ajaxErrorHandle(result);
        }
    }, 'json');
}
//抵店时间改变事件
function arriveDate_changed() {
    //设置离店时间和保留时间同步更改
    var arriveDateTimePicker = $("#arriveDate").data("kendoDateTimePicker");
    var holdDateTimePicker = $("#holdDate").data("kendoDateTimePicker");
    var depDateTimePicker = $("#depDate").data("kendoDateTimePicker");
    if (arriveDateTimePicker.value() != null) {
        if (true) {
            //设置默认值
            var defaultArrTime = CustomerCommonValues.defaultArrTime;//默认抵店时间
            var defaultHoldTime = CustomerCommonValues.defaultHoldTime;//默认保留时间
            var nowDateTime = arriveDateTimePicker.value();
            if (true) {
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
                        var time = (depDateTimePicker.value() != null && depDateTimePicker.value() != undefined) ?  depDateTimePicker.value().ToTimeString() : "";
                        if ($.trim(time) != "" && time != "00:00:00") {
                            depDateTimePicker.value(depDateTime.ToDateString() + " " + (time));
                        }
                        else {
                            if (defaultArrTime != null && defaultArrTime != undefined && defaultArrTime.length > 0) {
                                depDateTimePicker.value(depDateTime.ToDateString() + " " + (defaultArrTime + ":00"));
                            } else {
                                depDateTimePicker.value(depDateTime.ToDateString() + " " + ("00:00:00"));
                            }
                        }
                    }
                }
            }
            if (true) {
                if (defaultHoldTime != null && defaultHoldTime != undefined && defaultHoldTime.length > 0) {
                    holdDateTimePicker.value(nowDateTime.ToDateString() + " " + (defaultHoldTime + ":00"));
                } else {
                    holdDateTimePicker.value(nowDateTime);
                }
                if (holdDateTimePicker.value() > depDateTimePicker.value()) {
                    holdDateTimePicker.value(nowDateTime);
                }
            }
        }
    }
    DateTimeRangeSelection();
}
//打开报表窗口
function openReportWindow(id) {
    //初始化
    var peportCode = "";
    var parameterValues = "";
    var chineseName = "";
    if (id == "btnResReoprt") {
        //预定确认单
        var resid = $("#Resid").val();
        if (resid == null || resid == undefined || resid.length <= 0) { return; }
        peportCode = "resBill";
        parameterValues = ("@resid={0}^{1}").replace("{0}", resid).replace("{1}", resid);
        chineseName = "预订确认单";
    } else if (id == "btnResRCReoprt") {
        //RC单
        var regid = $("#regId").val();
        if (regid == null || regid == undefined || regid.length <= 0) { return; }
        peportCode = "resRCBill";
        parameterValues = ("@regid={0}^{1}").replace("{0}", regid).replace("{1}", regid);
        chineseName = "RC单";
    } else {
        //其他
        return;
    }
    //1： rc单
    debugger;
    var isSignature = $("#IsSignature").val();
    var printIndex = isSignature == 1 ? 10 : 1;
    var regids = $("#regId").val();
    var roomNo = $("#roomNoInput").val();
    $.post(CustomerCommonValues.AddQueryParaTemp, { ReportCode: peportCode, ParameterValues: parameterValues, ChineseName: chineseName, print: printIndex }, function (result) {
        if (result.Success) {
            window.open(result.Data+"&sType=1&regid="+regids+"&roomNo="+roomNo);
        } else {
            //jAlert(data.Data, "知道了");
            ajaxErrorHandle(result);
        }
    }, 'json');
}
//检查客情是否有修改
function orderCustomerIsChanged() {
    return (hasChanged || hasChangedMain);
}
//设置客情是否有修改
function orderCustomerIsChangedSet(isChanged) {
    isChanged = isChanged ? true : false;
    hasChanged = isChanged;
    hasChangedMain = isChanged;
}
//联系人改变事件
function customerName_changed(id) {
    //联系人
    var resCustName = $("#ResCustName");
    var resCustNameValue = $.trim(resCustName.val());
    //主单名称
    var name = $("#Name");
    var nameValue = $.trim(name.val());
    //客人名称
    var guestName = $("#guestName");
    var guestNameValue = $.trim(guestName.val());
    //初始值
    var rName = "新预订客人";
    var iName = "新入住客人";
    switch (id) {
        case "ResCustName":
            {
                if (resCustNameValue.length > 0) {
                    if (nameValue == rName || nameValue == iName) {
                        name.val(resCustNameValue);
                    }
                    if (guestNameValue == rName || guestNameValue == iName) {
                        guestName.val(resCustNameValue);
                        guestName.data("kendoAutoComplete").search($("#guestName").val());
                    }
                }
            }
            break;
        case "Name":
            {
                if (nameValue.length > 0) {
                    if (resCustNameValue == rName || resCustNameValue == iName) {
                        resCustName.val(nameValue);
                    }
                    if (guestNameValue == rName || guestNameValue == iName) {
                        guestName.val(nameValue);
                        guestName.data("kendoAutoComplete").search($("#guestName").val());
                    }
                }
            }
            break;
        case "guestName":
            {
                if (guestNameValue.length > 0) {
                    if (nameValue == rName || nameValue == iName) {
                        name.val(guestNameValue);
                    }
                    if (resCustNameValue == rName || resCustNameValue == iName) {
                        resCustName.val(guestNameValue);
                    }
                }
            }
            break;
    }
}
//手机号改变事件
function resCustMobile_changed(id) {    
    //主单手机号
    var resCustMobile = $("#ResCustMobile");
    var resCustMobileValue = $.trim(resCustMobile.val());
    //客人手机号
    var guestMobile = $("#guestMobile");
    var guestMobileValue = $.trim(guestMobile.val());

    if (resCustMobileValue.length > 0 && guestMobileValue.length > 0) {
        return;
    }
    if (resCustMobileValue.length <= 0 && id == "guestMobile") {
        resCustMobile.val(guestMobileValue);
    }
    else if (guestMobileValue.length <= 0 && id == "ResCustMobile") {
        guestMobile.val(resCustMobileValue);
    }
}
//日期时间控件选择范围
function DateTimeRangeSelection(notRange) {
    var arriveDateTimePicker = $("#arriveDate").data("kendoDateTimePicker");
    var holdDateTimePicker = $("#holdDate").data("kendoDateTimePicker");
    var depDateTimePicker = $("#depDate").data("kendoDateTimePicker");
    if (notRange == "true") {
        var mindate = "1900-01-01";
        var maxdate = "9999-12-31";
        arriveDateTimePicker.min(mindate); arriveDateTimePicker.max(maxdate);
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
//证件号改变事件
function cerId_changed(isBirthDay) {
    $("#guestid").val("");
    var cerType = $("#cerType").data("kendoDropDownList").value();
    var cerId = $("#cerId").val();
    if ($.trim(cerType).length > 0 && $.trim(cerId).length > 0) {
        $.post(CustomerCommonValues.GetGuestByCerId, { cerType: cerType, cerId: cerId }, function (result) {
            if (result != null && result != undefined && result.Success && result.Data != null && result.Data != undefined) {
                setCustomerInfo(result.Data);
            } else {
                customerName_changed("guestName");
            }
            if (cerType == "01") {
                if ($.trim($("#city").val()).length <= 0) {
                    getCity(cerId, $("#city"));
                }
                if (isBirthDay == "true" && $.trim(cerId).length > 0) {
                    $("#birthday").data("kendoDatePicker").value(getBirthDay(cerId));
                    $("[name='gender'][value='" + getGender(cerId) + "']")[0].checked = true;
                }
            }
            getProfileInfoByCerId();
        }, 'json');
    }
}
//会员选择事件
function profileComboBox_selected(e) {
    if(e == null || e == undefined || e.item == null || e.item == undefined){ return true; }
    var dataItem = this.dataItem(e.item.index());
    if (dataItem != null && dataItem != undefined) {
        var id = dataItem.Id;
        if (id != null && id != undefined && id.length > 0) {
            $.post(CustomerCommonValues.GetMbrCardInfo, { id: id }, function (result) {
                if (result.Success && result.Data != null) {
                    var guestName = $("#guestName");
                    var guestNameValue = $.trim(guestName.val());
                    var rName = "新预订客人";
                    var iName = "新入住客人";
                    if (guestNameValue == rName || guestNameValue == iName) {
                        $("#guestName").val(result.Data.GuestName);
                        $("#guestMobile").val(result.Data.Mobile);
                        $("#cerType").data("kendoDropDownList").text(result.Data.CerType);
                        $("#cerId").val(result.Data.Cerid);
                        if (result.Data.Gender == "M") {
                            $("[name='gender'][value='M']")[0].checked = true;
                        } else {
                            $("[name='gender'][value='F']")[0].checked = true;
                        }
                        $("#birthday").data("kendoDatePicker").value(result.Data.Birthday);
                        $("#city").val(result.Data.City);
                        $("#address").val(result.Data.Address);
                        $("#email").val(result.Data.Email);
                        $("#carNo").val(result.Data.CarNo);
                        $("#love").val(result.Data.Interest);
                        $("#remark").val(result.Data.Remark);
                        customerName_changed("guestName");
                        resCustMobile_changed("guestMobile");
                    }
                    if (result.Data.RateCodeid != null && result.Data.RateCodeid != undefined && result.Data.RateCodeid.length > 0) {
                        if ($("#rateCode").attr("disabled") == "disabled") { return; }
                        //集团分店模式兼容
                        if (result.Data.RateCodeid.substring(0, 6) != FolioCommonValues.HotelId && $.trim(FolioCommonValues.HotelId).length > 0) {
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
                                result.Data.RateCodeid = FolioCommonValues.HotelId + result.Data.RateCodeid.substring(6);
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
                    if ($("#rateCode").attr("disabled") == "disabled") { return; }
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
//房号下拉,单击重新从服务器获取数据
function roomNo_open() {
    setTimeout(function () { $("#roomNo").data("kendoDropDownList").dataSource.read(); },100);
}
//房间类型下拉,单击重新从服务器获取数据
function roomType_open() {
    setTimeout(function () { $("#roomTypeDefaultValue").attr("isOnlyRead", "true"); $("#roomType").data("kendoDropDownList").dataSource.read(); }, 100);
}
//自动弹框
function setAutoOpenWindow(isOpen) {
    var isOpenAddCustomerInfosWindow = autoOpenAddCustomerInfosWindow();
    if (isOpenAddCustomerInfosWindow == true) {
        return;
    }
    var value = "false";
    if (isOpen == true) {
        value = "true";
    }
    $("#folioAddButton").attr("data-auto-open", value);
    var roomTypeIsNotRoom = $("#roomTypeIsNotRoom").val();
    if (roomTypeIsNotRoom == "true" || roomTypeIsNotRoom == "True") {
        value = "false";
    }
    $("#btnLock").attr("data-auto-open", value);
    if (isOpen == true) {
        $("#tabstripAuth").data("kendoTabStrip").activateTab($("[aria-controls='tabstripAuth-2']"));
    }
}
//自动弹出主单内所有客人信息列表窗口，进行证件扫描和信息填写
function autoOpenAddCustomerInfosWindow() {
    if (CustomerCommonValues.IsBatchCheckIn == "True" || CustomerCommonValues.IsBatchCheckIn == "true") {
        CustomerCommonValues.IsBatchCheckIn = "false";
        return openAddCustomerInfosWindow(true);
    }
    return false;
}
function openAddCustomerInfosWindow(isAuto) {
    var resIdValue = $("#Resid").val();
    if (resIdValue != null && resIdValue != undefined && resIdValue.length > 0) {
        var customerInfosWindowDivKendo = $("#customerInfosWindowDiv").data("kendoWindow");
        if (customerInfosWindowDivKendo == null || customerInfosWindowDivKendo == undefined) {
            $("#customerInfosWindowDiv").kendoWindow({
                width: "1200px",
                title: "客情维护",
                visible: false,
                modal: true,
                actions: ["Close"],
                close: function () {
                    $("#customerInfosWindowDiv").empty();
                    if (isAuto == true) {
                        setAutoOpenWindow(true);
                    }
                },
                refresh: function () {
                    $("#customerInfosWindowDiv").data("kendoWindow").center();
                },
            });
            customerInfosWindowDivKendo = $("#customerInfosWindowDiv").data("kendoWindow");
        }
        customerInfosWindowDivKendo.refresh({
            url: CustomerCommonValues.GetCustomerInfoByResId,
            data: { resId: resIdValue, rnd: Math.random() },
            type: "get",
            iframe: false
        }).center().open();
        return true;
    }
    return false;
}
//会员余额提示
function profileComboBox_Tooltip() {
    $("[name='profileComboBox_input']").attr("title", "鼠标移上“会员”两字可查会员账务余额。");
    $("[for='profileComboBox']").kendoTooltip({
        content: { url: CustomerCommonValues.GetMbrCardBlance },
        width: 120,
        height: 127,
        position: "top",
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
//熟客信息展示
function guestDetailInfo() {
    var url = CustomerCommonValues.GuestManageDetail;
    if (url == null || url == undefined || url.length <= 0) {
        return;
    }
    var cerType = $("#cerType").data("kendoDropDownList").value();
    var cerId = $.trim($("#cerId").val());
    if (cerType == null || cerType == undefined || cerType.length <= 0) {
        jAlert("请选择证件类型！");return;
    }
    if (cerId == null || cerId == undefined || cerId.length <= 0) {
        jAlert("请输入证件号！"); return;
    }
    url = url + "?cerType=" + cerType + "&cerId=" + cerId + "&hasButton=false";
    top.openKendoWindow("客历详情", url, null);
}
//更新备注
function customerUpdateRemarkWindow_Initialization() {
    var obj = $("#customerUpdateRemarkWindow").data("kendoWindow");
    if (obj == null || obj == undefined) {
        $("#customerUpdateRemarkWindow").kendoWindow({
            width: "500px",
            title: "修改备注",
            visible: false,
            modal: true,
            actions: ["Close"],
            close: function (e) {
                $("#customerUpdateRemarkWindow_info").text("");
                $("#customerUpdateRemarkWindow_remark").text("");
            }
        });
        obj = $("#customerUpdateRemarkWindow").data("kendoWindow");
    }
    return obj;
}
function remark_dblclick(e) {
    var resDetail = OrderList.GetSelected();
    if (resDetail != null && resDetail != undefined) {
        if (resDetail.Status == "C") {
            var obj = customerUpdateRemarkWindow_Initialization();
            $("#customerUpdateRemarkWindow_regid").val(resDetail.Regid);
            $("#customerUpdateRemarkWindow_info").text(("房号：{0}，客人名：{1}，账号：{2}").replace("{0}", resDetail.RoomNo).replace("{1}", resDetail.Guestname).replace("{2}", resDetail.Regid.replace(resDetail.Hid, "")));
            $("#customerUpdateRemarkWindow_remark").val(resDetail.Remark);
            obj.center().open();
        }
    }
}
function customerUpdateRemarkWindow_SubmitButton() {
    var regid = $("#customerUpdateRemarkWindow_regid").val();
    var remark = $("#customerUpdateRemarkWindow_remark").val();
    $.post(CustomerCommonValues.UpdateRemark, { regid: regid, remark:remark }, function (result) {
        if (result.Success) {
            jAlert("备注保存成功！");
            OrderCustomer.RefreshData(result, "OrderCustomer.UpdateRemark");
        } else {
            ajaxErrorHandle(result);
        }
        customerUpdateRemarkWindow_CancelButton();
    }, 'json');
}
function customerUpdateRemarkWindow_CancelButton() {
    var obj = customerUpdateRemarkWindow_Initialization();
    obj.close();
}
//显示扫描身份证上的照片
function cerIdPhoto_Tooltip() {
    $("[for='openPhoto']").kendoTooltip({
        content: kendo.template($("#template").html()),
        width: 130,
        height: 150,
        position: "top",
        show: function () {
            var obj = $("[for='openPhoto']");
            var objKendoTooltip = obj.data("kendoTooltip");
            objKendoTooltip.content.html("");
            objKendoTooltip.refresh();
        }
    });
}
//合约单位提示
function cttidDropDownList_Tooltip() {
    $("[for='Cttid']").kendoTooltip({
        content: { url: CustomerCommonValues.GetCommpanyBlance },
        width: 130,
        height: 95,
        position: "top",
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
//显示禁用的房型
function showDisabledRoomType(roomTypeId) {
    var grid = $("#grid").data("kendoGrid");
    if (grid == null || grid == undefined) {
        return;
    }
    var row = grid.select();
    if (row == null || row == undefined || row.length <= 0) {
        return;
    }
    var dataItem = grid.dataItem(row[0]);
    if (dataItem == null || dataItem == undefined) {
        return;
    }
    if($.trim($("#regId").val()) == "" || $.trim(dataItem.Regid) == ""){
        return;
    }
    if ($("#regId").val() != dataItem.Regid) {
        return;
    }
    var roomTypeKendo = $("#roomType").data("kendoDropDownList");
    if (roomTypeKendo == null || roomTypeKendo == undefined) {
        return;
    }
    if (roomTypeKendo.dataSource == null || roomTypeKendo.dataSource == undefined) {
        return;
    }
    var isExists = false;
    var datas = roomTypeKendo.dataSource.data();
    if (datas != null && datas != undefined) {
        $.each(datas, function (index, item) {
            if (item.id == dataItem.RoomTypeId) {
                isExists = true;
                return false;
            }
        });
    }
    if (!isExists) {
        roomTypeKendo.dataSource.add({ id: dataItem.RoomTypeId, name: dataItem.RoomTypeName, roomqty: null, seqid: -444 });
        if (roomTypeId == dataItem.RoomTypeId) {
            roomTypeKendo.value(dataItem.RoomTypeId);
        }
    }
}
function checkDisabledRoomType() {
    var roomTypeKendo = $("#roomType").data("kendoDropDownList");
    if (roomTypeKendo == null || roomTypeKendo == undefined) {
        return;
    }
    if (roomTypeKendo.dataSource == null || roomTypeKendo.dataSource == undefined) {
        return;
    }
    var removeItems = [];
    var datas = roomTypeKendo.dataSource.data();
    if (datas != null && datas != undefined) {
        $.each(datas, function (index, item) {
            if (item.seqid == -444) {
                removeItems.push(item);
            }
        });
    }
    if (removeItems != null && removeItems != undefined && removeItems.length > 0) {
        $.each(removeItems, function (index, item) {
            roomTypeKendo.dataSource.remove(item);
        });
        roomTypeKendo.select(0);
    }
}
//根据证件类型和证件号获取会员信息
function getProfileInfoByCerId() {
    //如果已经选择了会员，则返回
    var profileComboBoxValue = $("#profileComboBox").data("kendoComboBox").value();
    if ($.trim(profileComboBoxValue) != "") { return; }
    //获取会员信息
    var cerType = $("#cerType").data("kendoDropDownList").text();
    var cerId = $("#cerId").val();
    if ($.trim(cerType).length > 0 && $.trim(cerId).length > 0) {
        $.post(CustomerCommonValues.GetProfileInfoByCerId, { cerType: cerType, cerId: cerId }, function (result) {
            if (result != null && result != undefined && result.Success && result.Data != null && result.Data != undefined) {
                var profileComboBox = $("#profileComboBox").data("kendoComboBox");
                if ($.trim(profileComboBox.value()) == "") {
                    profileComboBox.dataSource.data([]);
                    profileComboBox.dataSource.add({ GuestName: result.Data.ProfileName, Id: result.Data.Profileid, MbrCardNo: result.Data.ProfileNo, Mobile: result.Data.ProfileMobile });
                    var liObj = profileComboBox.ul.find("li");
                    if (liObj.length > 0) {
                        liObj[0].click();
                    }
                }
            }
        }, 'json');
    }
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
function profileTransDetailInfo(){
    var url = CustomerCommonValues.ProfileTransDetail;
    if (url == null || url == undefined || url.length <= 0) {
        return;
    }
    var profileid = $("#profileComboBox").data("kendoComboBox").value();
    if (profileid == null || profileid == undefined || profileid.length <= 0) {
        jAlert("请选择会员！"); return;
    }
    top.openIframeKendoWindow("会员消费记录", url, { profileId: profileid }, "30001_17179869184", "消费记录");
}
//天数改变事件
function days_depDate_arriveDate_change(changeid) {
    if (changeid == "dateDays" && $("#depDate").attr("disabled") == "disabled") { return; }

    var daysObj = $("#dateDays").data("kendoNumericTextBox");
    var arrDate = kendo.date.getDate($("#arriveDate").data("kendoDateTimePicker").value());
    var depDateObj = $("#depDate").data("kendoDateTimePicker");
    var depDate = depDateObj.value();
    var days = daysObj.value();
    if (changeid == "dateDays") {
        //天数修改
        if (arrDate == null || arrDate == undefined || days == null || days == undefined) { daysObj.value(null); return; }
        if (depDate == null || depDate == undefined) { depDate = arrDate; }
        arrDate.setDate((arrDate.getDate() + days));
        $("#depDate").data("kendoDateTimePicker").value((arrDate.ToDateString() + " " + depDate.ToTimeString()));
        editControl_onChange("depDate", true);
    } else {
        //离店时间修改
        if (arrDate == null || arrDate == undefined || depDate == null || depDate == undefined) { daysObj.value(null); return; }
        var num = parseInt((kendo.date.getDate(depDate) - arrDate) / (1000 * 60 * 60 * 24));
        daysObj.value(num < 1 ? null : num);
    }
}
//刷卡自动绑定
function profileComboBox_databound(e)
{
    if ($("#profileComboBox").data("kendoComboBox").dataSource.data().length > 0 && showMbr)
    {
        $("#profileComboBox").data("kendoComboBox").open();
        showMbr = false;
    }
}
//团体/散客单选按钮
function isGroup_clicked(e){
    if($("#rateCode").attr("disabled") == "disabled") { return; }
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
//设置自动打印RC单
function setAutoOpenRC(selectRegIdIsNewCheckIn) {
    if (selectRegIdIsNewCheckIn == true && CustomerCommonValues.IsCheckInAutoPrintRC == "True") {
        setTimeout(function () { openReportWindow("btnResRCReoprt"); }, 1000);
    }
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
//RefreshData方法后续
function RefreshData_End() {
    //拖动换房
    if ($.trim(CustomerCommonValues.ChangeRoomTypeId).length > 0 && $.trim(CustomerCommonValues.ChangeRoomId).length > 0) {
        try {
            setTimeout(function() {
                ChangeRoomWindow.AutoSetRoom(CustomerCommonValues.ChangeRoomTypeId, CustomerCommonValues.ChangeRoomId);
                CustomerCommonValues.ChangeRoomTypeId = "";
                CustomerCommonValues.ChangeRoomId = "";
            }, 1000);
        } catch (e) { }
    }
}