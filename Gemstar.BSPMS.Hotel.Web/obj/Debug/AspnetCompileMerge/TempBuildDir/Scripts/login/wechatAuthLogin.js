//微信授权扫码登录
wechatAuthLogin = {
    type: 0,
    employeeId: 0,
    qrcodeSessionId: '',
    setIntervalId: 0,
    fingetFailCount: 0,
    getQrcodeStateFailCount: 0,
    MAX_GET_QRCODE_STATE_FAIL_COUNT: 3,
    GET_QRCODE_STATE_TIME: 3,
    isOnGetQrcode: false,
    JS_QRCODE_EMPTY_ACCOUNT: -1,
    JS_QRCODE_INVALID: -2,
    JS_QRCODE_SCANED: 100,
    JS_QRCODE_LOGIN_SUCCESS: 101,
    JS_QRCODE_REFUSE_LOGIN: 102,
};
wechatAuthLogin.init = function () {
    that = wechatAuthLogin;
    that.bindEvent();
    try {
        if (typeof qrcodeType != 'undefined') {
            that.type = qrcodeType;
        }
        if (typeof qrcodeEmployeeId != 'undefined') {
            that.employeeId = qrcodeEmployeeId;
        }
        if (that.qrcodeSessionId.length <= 0) {
            that.getQrcode();
        }
    } catch (e) {}
};
wechatAuthLogin.redirectPost = function (location, args) {
    var form = '';
    $.each(args,
        function (key, value) {
            form += '<input type="hidden" name="' + key + '" value="' + value + '">';
        });
    $('<form action="' + location + '" method="POST">' + form + '</form>').appendTo('body').submit();
};
wechatAuthLogin.loginSuccess = function (jump) {
    var token_name = "__RequestVerificationToken";
    var loginType = $("input[name='login_type']").val();
    var hash = $("[name='__RequestVerificationToken']").val();
    var para = (getMac()).replace(/"/g, "\'");
    var postData = {
        login_type: loginType,
        jump: jump,
        jsonPara:para
    };
    var returnUrls = location.search.split('return_url=');
    if (returnUrls.length >= 2) {
        postData['return_url'] = returnUrls[1];
    }
    postData[token_name] = hash;
    that.redirectPost('/Account/loginJumpForQrcode', postData);
};
wechatAuthLogin.bindEvent = function () {   
    $('#IDQrcodeInvalid').on('click',
    function () {
        that.refreshQrcode();
    });
    $('#IDRefuseLoginReturn').on('click',
    function () {
        $('#IDRefuseLogin').addClass('hide');
        $('#IDQrcode').removeClass('hide');
        that.getQrcode();
    });
};
wechatAuthLogin.refreshQrcode = function () {
    if ($('#IDQrcodeInvalid').hasClass('hide')) {
        return false;
    }
    if (that.isOnGetQrcode) {
        return false;
    }
    that.isOnGetQrcode = true;
    that.getQrcode();
};
wechatAuthLogin.getQrcode = function () {
    var token_name = "__RequestVerificationToken";
    var hash = $("[name='__RequestVerificationToken']").val();
    var AuthCode = $("#AuthCode").val();
    if (!AuthCode) {
        that.showErrorMsg("请输入授权码后点击刷新来生成登录二维码");
        return;
    }
    var postData = {
        type: that.type,
        employeeId: that.employeeId,
        loginType: 2,
        authCode: AuthCode
    };
    postData[token_name] = hash;
    $.ajax({
        url: '/Account/getQrcode',
        type: 'post',
        data: postData,
        dataType: 'json',
        async: true,
        success: function (resp) {
            that.isOnGetQrcode = false;
            if (resp.Success) {
                that.qrcodeSessionId = resp.Data.qrcodeSessionId;
                if (resp.Data.genQrcodeType == 0) {
                    $('#IDQrcodeImg').html('');
                    $("#IDQrcodeImg").qrcode({ text: resp.Data.qrcodeUrl, width: 185, height: 185, correctLevel: QRErrorCorrectLevel.M });
                } else {
                    $('#IDQrcodeImg').html('<img src="' + resp.Data.qrcodeUrl + '"/>');
                }
                $('#IDQrcodeInvalid').addClass('hide');
                that.getQrcodeStateFailCount = 0;
                that.setIntervalId = setInterval(that.getQrcodeState, that.GET_QRCODE_STATE_TIME * 1000);
                return true;
            }
            that.showErrorMsg(resp.Data);
        },
        error: function (xhr) {
            that.showErrorMsg('网络错误 ' + xhr.statusText + '');
        }
    });
};
wechatAuthLogin.getQrcodeState = function () {
    if (that.getQrcodeStateFailCount > that.MAX_GET_QRCODE_STATE_FAIL_COUNT) {
        clearInterval(that.setIntervalId);
        $('#IDQrcodeScaned').addClass('hide');
        $('#IDQrcodeInvalid').removeClass('hide');
        $('#IDQrcode').removeClass('hide');
        return;
    }
    var token_name = "__RequestVerificationToken";
    var hash = $("[name='__RequestVerificationToken']").val();
    var postData = {
        type: that.type,
        employeeId: that.employeeId,
        sign: that.qrcodeSessionId
    };
    postData[token_name] = hash;
    $.ajax({
        url: '/Account/getQrcodeState?nowTime=' + new Date().getTime(),
        type: 'post',
        data: postData,
        dataType: 'json',
        async: true,
        success: function (resp) {
            that.closeErrorMsg();
            switch (resp.Data.errorcode) {
                case 0:
                    return true;
                case that.JS_QRCODE_LOGIN_SUCCESS:
                    clearInterval(that.setIntervalId);
                    that.loginSuccess(resp.Data.jump);
                    return true;
                case that.JS_QRCODE_EMPTY_ACCOUNT:
                    clearInterval(that.setIntervalId);
                    $('#IDQrcode').addClass('hide');
                    $('#IDHasNotBind').removeClass('hide');
                    return true;
                case that.JS_QRCODE_INVALID:
                    clearInterval(that.setIntervalId);
                    $('#IDQrcodeScaned').addClass('hide');
                    $('#IDQrcodeInvalid').removeClass('hide');
                    $('#IDQrcode').removeClass('hide');
                    return true;
                case that.JS_QRCODE_SCANED:
                    $('#IDQrcode').addClass('hide');
                    $('#IDQrcodeScaned').removeClass('hide');
                    return true;
                case that.JS_QRCODE_REFUSE_LOGIN:
                    clearInterval(that.setIntervalId);
                    $('#IDQrcode').addClass('hide');
                    $('#IDQrcodeScaned').addClass('hide');
                    $('#IDRefuseLogin').removeClass('hide');
                    return true;
                default:
                    that.getQrcodeStateFailCount++;
                    that.showErrorMsg(resp.Data.msg);
                    return true;
            }
        },
        error: function (xhr) {
            that.getQrcodeStateFailCount++;
            that.showErrorMsg('网络错误 ' + xhr.statusText + '');
        }
    });
};
wechatAuthLogin.showErrorMsg = function (msg) {
    $('#IDQrcodeError').removeClass('hide').text(msg);
};
wechatAuthLogin.closeErrorMsg = function () {
    $('#IDQrcodeError').addClass('hide').text('');
};
function getMac() {
    //使用授权登录时，直接返回一个固定的mac地址
    return "wechatAuthLoginMac";
}
wechatAuthLogin.init();