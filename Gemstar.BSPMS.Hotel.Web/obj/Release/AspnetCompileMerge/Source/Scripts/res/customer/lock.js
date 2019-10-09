//门卡
var LockWindow = {
    //初始化
    Initialization: function () {
        $("#lockWindow").kendoWindow({
            width: "920px",
            title: "门卡管理",
            visible: false,
            modal: true,
            actions: ["Close"]
        });
        if (!$("#lockWindow_roomNo").data("kendoDropDownList")) {
            $("#lockWindow_roomNo").kendoDropDownList({
                dataTextField: "roomNo",
                dataValueField: "regid",
                template: "#= (hid != '定位') ? ((roomNo != '全部') ? ('<span style=\"display:inline-block;width:80px;\">房号：'+ roomNo + '</span><span data-id=\"'+ regid +'\">账号：' + regid.replace(hid,'') + '</span>') : '全部') : ('定位到房号：'+ roomNo) #",
                dataSource: [{ roomNo: "全部", regid: "", hid: "" }],
                index: 0,
                change: function (e) { LockWindow.RoomNoChange(e); },
                filter: "contains",
                filtering: function (e) {
                    if (e == null || e == undefined || e.sender == null || e.sender == undefined || e.sender.dataSource == null || e.sender.dataSource == undefined) {return;}
                    //初始化
                    var list = e.sender.dataSource.data();
                    for (var i = 0; i < list.length; i++) {
                        var item = list[i];
                        if (item.hid == "定位") {
                            list.remove(item);
                            i--;
                        }
                    }
                    //获取
                    if (e.filter == null || e.filter == undefined) {return;}
                    var value = e.filter.value;
                    if ($.trim(value).length <= 0) { return; }
                    var field = e.filter.field;
                    //加
                    var dataSource = new kendo.data.DataSource({data: list});
                    dataSource.filter({ field: field, operator: "eq", value: value});
                    var view = dataSource.view();
                    if (view.length <= 0) {
                        e.sender.dataSource.add({ roomNo: value, regid: value + "|" + Math.random(), hid: "定位" });
                    }
                },
            });
        }
        $("#lockWindow_roomNo").data("kendoDropDownList").list.width(180);
        $("#readLockCardButton").unbind("click").click(function (e) { LockWindow.ReadLockCard(e); });
        $("#logoutLockCardButton").unbind("click").click(function (e) { LockWindow.LogoutLockCard(e); });
    },
    //门锁弹框
    Open: function () {
        if (!$("#lockWindow").data("kendoWindow")) { LockWindow.Initialization(); }
        $("#lockResId").val($("#Resid").val());
        $("#lockWindow").data("kendoWindow").center().open();
        LockWindow.GetRemote();
    },
    //从服务器获取门卡最新信息
    GetRemote: function (regid, successCallBack) {
        //加上房号列表，可以分开展示单个房号的门卡信息。
        $("#lockTbody").empty();
        $("#lockWindow_roomNo").data("kendoDropDownList").setDataSource([{ roomNo: "全部", regid: "", hid: "" }]);
        var resId = $("#lockResId").val();
        if (resId == null || resId == undefined || resId.length <= 0) { return; }
        $.ajax({
            type: "POST",
            url: CustomerCommonValues.GetLockInfo,
            data: { resId: resId },
            dataType: "json",
            success: function (result) {
                if (result.Success) {
                    if (result.Data != null && result.Data.length > 0) {
                        var template = kendo.template($("#lockTemplet").html());
                        var trHtml = [];
                        var length = result.Data.length;
                        var roomNoList = [{ roomNo: "全部", regid: "", hid: "" }];
                        for (var i = 0; i < length; i++) {
                            var tmpplateHtml = template({ Data: result.Data[i], useWithBlock: false });
                            trHtml.push(i % 2 == 0 ? tmpplateHtml : tmpplateHtml.replace("k-master-row", "k-master-row k-alt"));
                            roomNoList.push({ roomNo: (result.Data[i].RoomNo != null && result.Data[i].RoomNo.length > 0 ? result.Data[i].RoomNo:''), regid: result.Data[i].RegId, hid: result.Data[i].Hid});
                        }
                        $("#lockTbody").html(trHtml.join(""));
                        $("#lockWindow_roomNo").data("kendoDropDownList").setDataSource(roomNoList);
                        $("#lockTbody a.k-icon").on("click", function (e) { LockWindow.IconClick(this); });
                        if (LockWindow.IsOpenResWindow() == false) {
                            $("#lockTbody a.k-lock-regid").on("click", function (e) { LockWindow.OpenResOrderWindow(this); });
                            $("#lockTbody a.k-lock-regid").css("text-decoration", "underline");
                        }
                        LockWindow.Select(regid);
                        if (successCallBack != null && successCallBack != undefined) {
                            successCallBack();
                        }
                    }
                } else {
                    jAlert(result.Data, "知道了");
                }
            },
            error: function (xhr, msg, ex) {
                jAlert(msg, "知道了");
            }
        });
    },
    //扫描门锁卡
    ScanLock: function (operate, para) {
        var lockType = CustomerCommonValues.lockType;
        var lockCode = CustomerCommonValues.lockCode;
        var lockEditionName = CustomerCommonValues.lockEditionName;
        if (!lockType || !lockCode) {
            jAlert("没有开通门锁卡扫描接口", "知道了");
            return;
        }
        var hardwareCallPara = { Type: lockType, ID: lockCode, Operate: operate, Para: para, EditionName: lockEditionName };
        callHardware(hardwareCallPara);
    },
    //读卡
    ReadLockCard: function () {
        LockWindow.Status.EnableBtn(false);
        LockWindow.ScanLock('ReadCard', { LockId: '', LockInfo: '', BeginTime: '', EndTime: '', GuestName: '', IsNew: '', SeqNo: '', UserName: '' });
    },
    //写卡
    WriteLockCard: function (regId, cardNo, seqId) {
        LockWindow.Status.EnableBtn(false);
        if (regId == null || regId == undefined || regId.length <= 0) { return; }
        $("#writeRegId").val(regId);
        $("#writeSeqId").val(seqId);
        if (cardNo == null || cardNo == undefined || cardNo.length <= 0) {
            $.ajax({//发卡时检测此房间号是否有在住或者预定的发过卡
                type: "POST",
                url: CustomerCommonValues.QueryLockWriteOpt,
                data: { regId: regId },
                dataType: "json",
                async: false,
                success: function (result) {
                    if (result.Success) {
                        jConfirm(result.Data, "返回", "继续发卡", function (confirmed) {
                            if (!confirmed) {
                                WriteCardBef(cardNo, regId);
                            }
                        });
                    }
                    else
                    {
                        WriteCardBef(cardNo, regId);
                    }
                },
                error: function (xhr, msg, ex)
                {
                    WriteCardBef(cardNo, regId);
                }
            });
        }
        else
        {
            WriteCardBef(cardNo, regId);
        }
    },
    WriteOnlineLockCard:function(regId)
    {
        LockWindow.Status.EnableBtn(false);
        $("#writeRegId").val(regId);
        hardwareCallback_01_WriteCard({ CardNo: "1", SeqNo: "", OnLockType: "lockstar" });
    },
    //重写卡
    ReWriteLockCard: function (regId, cardNo, seqId) {
        if (regId == null || regId == undefined || regId.length <= 0) { return; }
        if (cardNo == null || cardNo == undefined || cardNo.length <= 0) { return; }
        if (seqId == null || seqId == undefined || seqId.length <= 0) { return; }
        LockWindow.WriteLockCard(regId, cardNo, seqId);
    },
    //注销卡
    LogoutLockCard: function () {
        LockWindow.Status.EnableBtn(false);
        LockWindow.ScanLock('LogoutCard', { LockId: '', LockInfo: '', BeginTime: '', EndTime: '', GuestName: '', IsNew: '', SeqNo: '', UserName: '', CardNo: '' });
    },
    //无卡注销
    NoCardLogoutLockCard: function (regId, cardNo, seqId) {
        LockWindow.Status.EnableBtn(false);
        if (cardNo == null || cardNo == undefined || cardNo.length <= 0) { return; }
        hardwareCallback_01_LogoutCard({ RegId: regId, CardNo: cardNo ,SeqId:seqId}, 22);
    },
    //房号改变选择事件
    RoomNoChange: function (e) {
        //定位服务器订单
        var selectItem = $("#lockWindow_roomNo").data("kendoDropDownList").dataItem();
        if (selectItem != null && selectItem != undefined) {
            if (selectItem.hid == '定位' && $.trim(selectItem.roomNo).length > 0) {
                LockWindow.GetResIdByLockRoomNo(selectItem.roomNo);
                return;
            }
        }
        //定位本地订单
        $("#lockTbody a.k-icon").each(function () {
            var obj = $(this);
            if (obj.hasClass("k-minus")) {
                obj.click();
            }
            var regid = obj.parent().parent().attr("data-regid");
            var selectRegid = $("#lockWindow_roomNo").val();
            if (regid == selectRegid) {
                if (obj.hasClass("k-plus")) {
                    obj.click();
                }
            }
        });
    },
    //定位
    Select: function (regid) {
        if (regid == null || regid == undefined || regid.length <= 0) {
            $("#lockWindow_roomNo").data("kendoDropDownList").select(0);
            var selectRegId = $("#regId").val();
            if (selectRegId != null && selectRegId != undefined && selectRegId.length > 0) {
                regid = selectRegId;
            } else {
                return;
            }
        }
        var roomNoDropDownList = $("#lockWindow_roomNo").data("kendoDropDownList").items();
        var length = roomNoDropDownList.length;
        for (var i = 0; i < length; i++) {
            var item_regid = $(roomNoDropDownList[i]).find("[data-id]").attr("data-id");
            if (regid == item_regid) {
                roomNoDropDownList[0].click();
                roomNoDropDownList[i].click();
                break;
            }
        }
    },
    //展开或关闭
    IconClick: function (thisObj) {
        var obj = $(thisObj);
        var regid = obj.parent().parent().attr("data-regid");
        var tr = $("#lockTbody tr[data-regid='" + regid + "'].k-detail-row");
        if (tr != null || tr == undefined || tr.length > 0) {
            var display = tr.css("display");
            tr.css("display", display == "none" ? "table-row" : "none");
            obj.removeClass(display == "none" ? "k-plus" : "k-minus");
            obj.addClass(display == "none" ? "k-minus" : "k-plus");
        }
    },
    //根据门锁信息获取主单ID resid
    GetResIdByLockInfo: function (cardNo, successCallback, errorCallback) {
        $.ajax({
            type: "POST",
            url: CustomerCommonValues.GetResIdByLockInfo,
            data: { cardNo: cardNo },
            dataType: "json",
            success: function (result) {
                successCallback(result);
            },
            error: function (xhr, msg, ex) {
                errorCallback(xhr, msg, ex);
            }
        });
    },
    //检查账单内是否有未注销的门卡
    CheckLockInfoByRegIds: function (regids) {
        if (regids == null || regids == undefined || regids.length <= 0) { return;}
        $.ajax({
            type: "POST",
            url: CustomerCommonValues.CheckLockInfoByRegIds,
            data: { regids: regids },
            dataType: "json",
            success: function (result) {
                if (result != null && result != undefined && result.Success) {
                    $("#lockResId").val($("#Resid").val());
                    $("#lockWindow").data("kendoWindow").center().open();
                    LockWindow.GetRemote(result.Data);
                }
            }
        });
    },
    //跳转穿透 打开对应的客单
    OpenResOrderWindow: function (thisObj) {
        if (thisObj == null || thisObj == undefined) { return;}
        var regid = $(thisObj).parents("tr").attr("data-regid");
        if ($.trim(regid) == "") { return; }

        if (LockWindow.IsOpenResWindow() == true) {
            return;
        }

        if ($.trim(CustomerCommonValues.ResOrderAdd) == "") { return; }

        var url = CustomerCommonValues.ResOrderAdd + "?type=I&id=" + regid + "&IsRoomStatus=4";
        top.openResKendoWindow("客单", url, null, "20020", "订单维护");
    },
    //客单弹框是否已打开
    IsOpenResWindow: function () {
        var resWindow = $("#resKendoWindow").data("kendoWindow");
        if (resWindow != null && resWindow != undefined && resWindow.options != null && resWindow.options != undefined) {
            if (resWindow.options.visible == true) { return true; }
        }
        return false;
    },
    //状态
    Status: {
        setTimeoutId: 0,
        //启用禁用
        EnableBtn: function (toggle) {
            var btnlist = $("#lockWindow").find("#lockTableDiv").find(".k-button");
            if (toggle == false) {
                btnlist.attr("disabled", "disabled");
                LockWindow.Status.IsComplete = false;
                LockWindow.Status.setTimeoutId = setTimeout(function () {
                    if (LockWindow.Status.IsComplete == false) {
                        LockWindow.Status.EnableBtn(true);
                    }
                }, LockWindow.Status.Timeout);
            }
            else {
                clearTimeout(LockWindow.Status.setTimeoutId);
                btnlist.removeAttr("disabled", "disabled");
                LockWindow.Status.IsComplete = true;
            }
        },
        IsComplete: false,//是否完成
        Timeout: (1000 * 5),//超时时间
    },
    //返回错误信息
    CallBackError: function (result) {
        if (result != null && result != undefined) {
            if (!result.Success) {
                jAlert(result.Data, "知道了");
                LockWindow.Status.EnableBtn(true);
            }
        }
    },
    //根据房号获取主单ID resid
    GetResIdByLockRoomNo: function (roomNo) {
        $.ajax({
            type: "POST",
            url: CustomerCommonValues.GetResIdByLockRoomNo,
            data: { roomNo: roomNo },
            dataType: "json",
            success: function (result) {
                if (result.Success) {
                    $("#lockResId").val(result.Data.ResId);
                    LockWindow.GetRemote(result.Data.RegId, function () {
                        //成功执行
                    });
                }
                else {
                    jAlert(result.Data, "知道了");
                }
            },
            error: function (xhr, msg, ex) {
                jAlert(msg, "知道了");
            }
        });
    },
};

//读卡回调函数
function hardwareCallback_01_ReadCard(data) {
    LockWindow.Status.EnableBtn(true);
    //清除标记
    $("[data-CardNo]").each(function () {
        var obj = $(this);
        if (obj.css("color") == "rgb(255, 255, 255)" && obj.css("background-color") == "rgb(66, 139, 202)") {
            obj.css("color", "#333");
            obj.css("background-color", "#fff");
        }
    });
    //验证
    if (data == null || data == undefined || data.CardNo == null || data.CardNo == undefined || data.CardNo.length <= 0) {
        jAlert("读卡失败", "知道了");
        return;
    }
    //添加标记
    var tr = $("[data-CardNo='" + data.CardNo + "']");
    if (tr != null && tr != undefined && tr.length > 0) {
        tr.css("color", "#fff");
        tr.css("background-color", "#428bca");
        var regid = tr.parents(".k-detail-row").attr("data-regid");
        LockWindow.Select(regid);
    } else {
        jAlert("读卡成功，卡号：" + data.CardNo + "。当前订单中没有找到此卡号，正在定位其他订单。", "知道了");
        LockWindow.GetResIdByLockInfo(data.CardNo, function (result) {
            if (result.Success) {
                $("#lockResId").val(result.Data);
                LockWindow.GetRemote(null, function () {
                    var tr = $("[data-CardNo='" + data.CardNo + "']");
                    if (tr != null && tr != undefined && tr.length > 0) {
                        tr.css("color", "#fff");
                        tr.css("background-color", "#428bca");
                        var regid = tr.parents(".k-detail-row").attr("data-regid");
                        LockWindow.Select(regid);
                    }
                });
            } else {
                jAlert(result.Data, "知道了");
            }
        }, function (xhr, msg, ex) {
            jAlert(result.Data, "知道了");
        });
    }
}
//写卡回调函数
function hardwareCallback_01_WriteCard(data) {
    if (data == null || data == undefined || data.CardNo == null || data.CardNo == undefined || data.CardNo.length <= 0) {
        jAlert("写卡失败", "知道了");
        return;
    }
    var type = null;
    if (data.OnLockType == null || data.OnLockType == undefined)
    {
        type = "";
    }
    else
    {
        type = data.OnLockType;
    }
    var regId = $("#writeRegId").val();
    var seqId = $("#writeSeqId").val();
    if (regId == null || regId == undefined || regId.length <= 0) { return; }

    $.post(CustomerCommonValues.WriteLock, { regId: regId, seqId: seqId, cardNo: data.CardNo, seqNo: data.SeqNo,onlockType: type }, function (result) {
        if (result.Success) {
            jAlert("写卡成功", "知道了");
            $("#writeRegId").val("");
            $("#writeSeqId").val("");
            LockWindow.GetRemote(regId);
        } else {
            //jAlert(result.Data, "知道了");
            ajaxErrorHandle(result);
            $("#writeRegId").val("");
            $("#writeSeqId").val("");
        }
        LockWindow.Status.EnableBtn(true);
    }, 'json');
}
//注销卡回调函数
function hardwareCallback_01_LogoutCard(data, status) {
    if (data == null || data == undefined || data.CardNo == null || data.CardNo == undefined || data.CardNo.length <= 0) {
        jAlert("注销卡失败", "知道了");
        return;
    }
    var seqId = null;
    if (data == null || data == undefined || data.SeqId == null || data.SeqId == undefined || data.SeqId.length <= 0) {
        seqId = "";
    }
    else
    {
        seqId = data.SeqId;
    }
    if (status == null || status == undefined || isNaN(status)) {
        status = 21;//默认有卡注销
    }
    var regid = null;
    var tr = $("[data-CardNo='" + data.CardNo + "']");
    if (tr != null && tr != undefined && tr.length > 0) {
        regid = tr.parents(".k-detail-row").attr("data-regid");
    }
    $.post(CustomerCommonValues.CancelLock, { cardNo: data.CardNo, status: status, seqId: seqId }, function (result) {
        if (result.Success) {
            jAlert(result.Data, "知道了");
            LockWindow.GetRemote(regid);
        } else {
            //jAlert(result.Data, "知道了");
            ajaxErrorHandle(result);
        }
        LockWindow.Status.EnableBtn(true);
    }, 'json');
}
function WriteCardBef(cardNo,regId)
{    
    $.ajax({
        type: "POST",
        url: CustomerCommonValues.GetLockWriteCardPara,
        data: { regId: regId, cardNo: cardNo },
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.Success) {
                var seqId = $("#writeSeqId").val();
                if (result.Data.OnLockType == null || result.Data.OnLockType == undefined || result.Data.OnLockType != "lockstar")
                {
                    LockWindow.ScanLock('WriteCard', result.Data);
                }
                else
                {
                    hardwareCallback_01_WriteCard({ CardNo: cardNo, SeqNo: "",SeqId:seqId, OnLockType: "lockstar" });
                }
            } else {
                jAlert(result.Data, "知道了");
                $("#writeRegId").val("");
                $("#writeSeqId").val("");
            }
        },
        error: function (xhr, msg, ex) {
            jAlert(msg, "知道了");
            $("#writeRegId").val("");
            $("#writeSeqId").val("");
        }
    });
}