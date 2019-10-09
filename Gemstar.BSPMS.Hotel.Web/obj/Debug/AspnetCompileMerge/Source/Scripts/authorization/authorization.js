//授权
var authorizationWindow = {
    //初始化
    Initialization: function (authType) {//授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4日租半日租调价授权）
        var windowTitle = "授权";
        var windowWidth = "427px";
        labelReason = $("[for='authorizationWindow_Reason']");
        if (authType == 1) {
            windowTitle = "调价授权";
            windowWidth = "427px";
            labelReason.text("调价原因");
        } else if (authType == 2) {
            windowTitle = "减免授权";
            windowWidth = "427px";
            labelReason.text("减免原因");
        } else if (authType == 3) {
            windowTitle = "冲销授权";
            windowWidth = "427px";
            labelReason.text("冲销原因");
        } else if (authType == 4) {
            windowTitle = "房租加收修改授权";
            windowWidth = "427px";
            labelReason.text("修改原因");
        } else {
            authorizationWindow.Clear();
            jAlert("授权类型错误！","知道了"); return;
        }
        //弹框
        var authorizationWindowObj = $("#authorizationWindow");
        var authorizationWindowObjKendo = authorizationWindowObj.data("kendoWindow");
        if (authorizationWindowObjKendo == null || authorizationWindowObjKendo == undefined) {
            authorizationWindowObj.kendoWindow({
                title: windowTitle,
                width: windowWidth,
                visible: false,
                modal: true,
                actions: ["Close"],
                close: function () { authorizationWindow.IsWait = false; authorizationWindow.Clear(); authorizationWindow.RevokeAuthorization(); },
                open: function () {
                    $("#authorizationWindow_LoginPassword").change(function () {
                        if ($.trim($("#authorizationWindow_LoginPassword").val()).length > 0) {
                            $("#authorizationWindow_LoginPassword").attr("type", "password");
                        }
                    });
                },
            });
        } else {
            authorizationWindowObjKendo.title(windowTitle);
            authorizationWindowObjKendo.setOptions({ width: windowWidth });
        }
        //选项卡
        var authorizationWindow_TabstripObj = $("#authorizationWindow_Tabstrip");
        var authorizationWindow_TabstripObjKendo = authorizationWindow_TabstripObj.data("kendoTabStrip");
        if (authorizationWindow_TabstripObjKendo == null || authorizationWindow_TabstripObjKendo == undefined) {
            authorizationWindow_TabstripObj.kendoTabStrip();
        }
        //微信授权人
        var authorizationWindow_UserIdObj = $("#authorizationWindow_UserId");
        var authorizationWindow_UserIdObjKendo = authorizationWindow_UserIdObj.data("kendoDropDownList");
        if (authorizationWindow_UserIdObjKendo == null || authorizationWindow_UserIdObjKendo == undefined) {
            authorizationWindow_UserIdObj.kendoDropDownList({
                optionLabel: "请选择",
                dataTextField: "Value",
                dataValueField: "Key",
                dataSource: {
                    transport: {
                        read: {
                            type: "POST",
                            dataType: "json",
                            url: "/Home/GetWeiXinAuthorizationUsersToList?authType=" + authType,
                        }
                    }
                }
            });
        } else {
            var dataSource = new kendo.data.DataSource({
                transport: {
                    read: {
                        type: "POST",
                        dataType: "json",
                        url: "/Home/GetWeiXinAuthorizationUsersToList?authType=" + authType,
                    }
                }
            });
            authorizationWindow_UserIdObjKendo.setDataSource(dataSource);
        }
    },
    //打开
    Open: function (authType, data, successCallBackFunction) {
        authorizationWindow.Clear();
        if ($.trim(data).length <= 0) { jAlert("授权内容不能为空！", "知道了"); return; }
        if ($.trim(successCallBackFunction).length <= 0) { jAlert("授权成功执行操作不能为空！", "知道了"); return; }
        authorizationWindow.Initialization(authType);
        $("#authorizationWindow_ContentDiv").html(data);
        $("#authorizationWindow_SuccessCallBackFunction").val(successCallBackFunction);
        if ($.trim($("#authorizationWindow_Url").val()).length <= 0) { jAlert("授权地址不能为空！", "知道了"); return; }
        if ($.trim($("#authorizationWindow_UrlCallBack").val()).length <= 0) { jAlert("授权地址不能为空！", "知道了"); return; }
        if ($.trim($("#authorizationWindow_Type").val()).length <= 0) { jAlert("授权类型不能为空！", "知道了"); return; }
        if (authType != $.trim($("#authorizationWindow_Type").val())) { jAlert("授权类型错误！", "知道了"); return; }
        if ($.trim($("#authorizationWindow_Content").val()).length <= 0) { jAlert("授权内容不能为空！", "知道了"); return; }
        $("#authorizationWindow").data("kendoWindow").center().open();
    },
    //关闭
    Close: function () {
        var authorizationWindowObjKendo = $("#authorizationWindow").data("kendoWindow");
        if (authorizationWindowObjKendo != null && authorizationWindowObjKendo != undefined) {
            authorizationWindowObjKendo.close();
        }
        authorizationWindow.IsWait = false; authorizationWindow.Clear();
    },
    //提交
    Submit: function () {
        //参数
        var data = {
            AuthType: $("#authorizationWindow_Type").val(),
            AuthContent: $("#authorizationWindow_Content").val(),
            AuthReason: $("#authorizationWindow_Reason").val(),
        };
        var tabid = ""; try { tabid = $("#authorizationWindow_Tabstrip").data("kendoTabStrip").select().attr("id"); } catch (e) { }
        if (tabid == "authorizationWindow_LoginTab") {
            data.AuthMode = 1;
            data.LoginName = $("#authorizationWindow_LoginName").val();
            data.LoginPassword = $("#authorizationWindow_LoginPassword").val();
        } else if (tabid == "authorizationWindow_WeiXinTab") {
            data.AuthMode = 2;
            data.Userid = $("#authorizationWindow_UserId").val();
        }
        var url = $("#authorizationWindow_Url").val();
        var urlCallback = $("#authorizationWindow_UrlCallBack").val();
        //验证
        if ($.trim(url).length <= 0 || $.trim(urlCallback).length <= 0) {
            authorizationWindow.ShowMessage("授权地址不能为空！"); return;
        }
        if ($.trim(data.AuthType) != "1" && $.trim(data.AuthType) != "2" && $.trim(data.AuthType) != "3" && $.trim(data.AuthType) != "4") {
            authorizationWindow.ShowMessage("授权类型错误！"); return;
        }
        if ($.trim(data.AuthContent).length <= 0) {
            authorizationWindow.ShowMessage("授权内容不能为空！"); return;
        }
        if ($.trim(data.AuthReason).length <= 0) {
            var labelReason = "授权"; if (data.AuthType == 1) { labelReason = "调价"; } else if (data.AuthType == 2) { labelReason = "减免"; } else if (data.AuthType == 2) { labelReason = "冲销"; }
            authorizationWindow.ShowMessage("请输入" + labelReason + "原因！"); return;
        }
        if (data.AuthMode == 1) {
            if ($.trim(data.LoginName).length <= 0) {
                authorizationWindow.ShowMessage("请输入登录名！"); return;
            }
            if ($.trim(data.LoginPassword).length <= 0) {
                authorizationWindow.ShowMessage("请输入登录密码！"); return;
            }
        } else if (data.AuthMode == 2) {
            if ($.trim(data.Userid).length <= 0) {
                authorizationWindow.ShowMessage("请选择授权人！"); return;
            }
        }
        else {
            authorizationWindow.ShowMessage("请选择授权模式！"); return;
        }
        //提交
        $("#authorizationWindow_Submit").attr("disabled", "disabled");
        $.post(url, data, function (result) {
            if (result.Success && result.Data != null && result.Data.length > 0) {
                authorizationWindow.ShowMessage("授权请求已发送，正在等待结果。。。<br/>请不要关闭当前窗口，否则视为放弃授权！");
                authorizationWindow.IsWait = true;
                authorizationWindow.AuthorizationId = result.Data;
                authorizationWindow.GetAuthorizationResult(result.Data);
            } else {
                authorizationWindow.ShowMessage(result.Data);
                $("#authorizationWindow_Submit").removeAttr("disabled");
            }
        }, 'json');
    },
    //获取授权结果
    GetAuthorizationResult: function (id) {
        if (authorizationWindow.IsWait != true) { return;}
        var urlCallback = $("#authorizationWindow_UrlCallBack").val();
        $.post(urlCallback, { id: id }, function (result) {
            if (result != null && result.Success && result.Data != null && result.Data != undefined) {
                if (result.Data.Status == -2) {
                    //微信失败
                    authorizationWindow.ShowMessage(result.Data.Message);
                } else if (result.Data.Status == -1) {
                    //授权失败
                    authorizationWindow.ShowMessage("授权失败，授权人拒绝授权！");
                } else if (result.Data.Status == 0) {
                    //继续循环
                    authorizationWindow.ShowMessage("授权请求已发送，正在等待结果。。。<br/>请不要关闭当前窗口，否则视为放弃授权！<br/>" + result.Data.Message);
                    authorizationWindow.TimeoutId = setTimeout(function () { authorizationWindow.GetAuthorizationResult(id); }, 4000);
                } else if (result.Data.Status > 0) {
                    //授权成功
                    if (authorizationWindow.IsWait != true) { return; }
                    $("#authorizationSaveContinue").val(id + "-" + result.Data.Status);
                    var funcName = $("#authorizationWindow_SuccessCallBackFunction").val();
                    try { var func = eval(funcName); if (typeof (func) == "function") { func(); } } catch (e) { }
                    authorizationWindow.ShowMessage("授权成功，授权人同意授权！<br />当前操作已执行，请确认是否成功！<br />10秒后自动关闭当前窗口！");
                    authorizationWindow.TimeoutId = setTimeout(function () { authorizationWindow.Close(); }, 10000);
                }
                else {
                    authorizationWindow.ShowMessage(result.Data);
                }
            } else {
                authorizationWindow.ShowMessage(result.Data);
            }
        }, 'json');
    },
    //显示消息
    ShowMessage: function (value) {
        var authorizationWindow_MessageObj = $("#authorizationWindow_Message");
        authorizationWindow_MessageObj.html(value);
        if ($.trim(value).length <= 0) {
            authorizationWindow_MessageObj.css("margin-top","0px");
        } else {
            authorizationWindow_MessageObj.css("margin-top", "6.96px");
        }
    },
    //清除内容
    Clear: function () {
        //清除授权信息
        $("#authorizationWindow_Url").val("");
        $("#authorizationWindow_UrlCallBack").val("");
        $("#authorizationWindow_Type").val("");
        $("#authorizationWindow_Content").val("");
        $("#authorizationWindow_ContentDiv").empty();
        //清除授权原因
        $("#authorizationWindow_Reason").val("");
        //清除授权模式
        $("#authorizationWindow_LoginName").val("");
        $("#authorizationWindow_LoginPassword").val(""); $("#authorizationWindow_LoginPassword").attr("type", "text");
        var authorizationWindow_UserIdObjKendo = $("#authorizationWindow_UserId").data("kendoDropDownList");
        if (authorizationWindow_UserIdObjKendo != null && authorizationWindow_UserIdObjKendo != undefined) {
            authorizationWindow_UserIdObjKendo.setDataSource({});
        }
        $("#authorizationWindow_UserId").val();
        //清除其他
        $("#authorizationWindow_SuccessCallBackFunction").val("");
        $("#authorizationWindow_Message").empty();
        $("#authorizationSaveContinue").val("");
        if ($.trim(authorizationWindow.TimeoutId) != "") { clearTimeout(authorizationWindow.TimeoutId); }
        authorizationWindow.TimeoutId = null;
        authorizationWindow.IsWait = false;
        $("#authorizationWindow_Submit").removeAttr("disabled");
    },
    //循环ID
    TimeoutId: null,
    //是否等待
    IsWait: false,
    //授权ID
    AuthorizationId: null,
    //撤销授权申请
    RevokeAuthorization: function () {
        if (authorizationWindow.AuthorizationId != null && authorizationWindow.AuthorizationId != undefined && authorizationWindow.AuthorizationId.length == 36) {
            var url = "/Home/RevokeAuthorization/";
            var data = { id : authorizationWindow.AuthorizationId };
            $.post(url, data, function (result) {
                if(result.Success) {
                    //authorizationWindow.ShowMessage("已放弃授权！");
                } else {
                    //authorizationWindow.ShowMessage("已放弃授权！");
                }
            }, 'json');
        }
        authorizationWindow.AuthorizationId = null;
    },
};