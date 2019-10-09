window.onmessage = function (e) {
    e = e || event;
    location.href = e.data;
};
$(function () {
    $("#HotelId").val($.cookie('hotelCode'));
    $("#doLogin").click(function (e) {
        $("#loadingDiv").remove();
        e.preventDefault(this);
        doLogin();
    });
    $("#tryButton").click(function (e) {
        e.preventDefault();
        tryButton_click();
    });
    $("#showPassword").click(function (e) {
        e.preventDefault();
        showPassword('showPassword',1);
    });
    checkcdStyle();
    if (window.navigator.userAgent.indexOf("Chrome") > -1) {//谷歌浏览器
        $("#PromptLi").css("display", "none")
        $("#PromptLi").next().css("margin-top","20px")
    }
});
function checkcdStyle() {
    var checkcd = $("#checkcd").css("display");
    if (checkcd != "none") {
        $(".re-password").css("margin-top", "10px");
    }
}
function doLogin() {
    showPassword('showPassword', 0);
    //数据有效性检测
    var hid = $.trim($("#HotelId").val());
    if (hid.length == 0) {
        jAlert("请输入酒店代码");
        return;
    }
    var uid = $.trim($("#Username").val());
    if (uid.length == 0) {
        jAlert("请输入用户名");
        return;
    }
    var pwd = $.trim($("#Password").val());
    if (pwd.length == 0) {
        jAlert("请输入密码");
        return;
    }
    var check = $.trim($("#CheckCode").val());
    if (checkcd.style.display != "none" && check.length == 0) {
        jAlert("请输入验证码");
        return;
    }
    var hascheckcode = 0;
    if (checkcd.style.display != "none") {
        hascheckcode = 1;
    }
    var f = $("#doLogin")[0].form;
    var validator = $(f).validate();
    if (validator.form()) {
        $(".login-box").append("<div id=\"loadingDiv\"></div>");
        $.post(
            $(f).attr("action"),
            $(f).serialize() + "&hascheckcode=" + hascheckcode,
            function (data) {
                if (data.Success) {
                    var hotelCode = "hotelCode";
                    if ($.cookie(hotelCode) != hid) {
                        $.removeCookie(hotelCode);
                        $.cookie(hotelCode, hid, { expires: 365 });
                    }

                    if (data.Data.indexOf("TryInfo") > 0) {
                        location.href = data.Data;
                        return;
                    }

                    if (data.Data.indexOf("ChangePassword") == -1) {                                              
                        location.href = data.Data + "/Home/Index";
                    }
                    else {                   
                        location.href = data.Data;
                    }                 
                } else {
                    if (hascheckcode == "0") {
                        jAlert(data.Data.replace("验证码不匹配", "").replace("\n", ""),"知道了");
                    } else {
                        //jAlert(data.Data);
                        ajaxErrorHandle(data);
                    }
                    checkcd.style.display = "";
                    checkcdStyle();
                    _reloadMvcCaptchaImage();
                }
            },
            "json");
    }
}
//我要体验
function tryButton_click() {
    var hid = CommonValues.TryHotelId;//"@Model.TryHotelId";
    var uid = CommonValues.TryUserName;//"@Model.TryUserName";
    var pwd = CommonValues.TryUserPass;//"@Model.TryUserPass";
    if (!hid || !uid) {
        jAlert("未正确配置体验参数，暂时不能体验");
        return;
    }
    $("#HotelId").val(hid);
    $("#Username").val(uid);
    $("#Password").val(pwd);
    jAlert("体验账户已自动填写，请点击登录即可");
}
//眼睛
function showPassword(btnId, changeStatu) {
    var $btn = $("#" + btnId);
    var isHidePassword = $btn.data("statu");
    var passwordId = $btn.data("password");
    var textId = $btn.data("text");

    if (isHidePassword == "1") {
        if (changeStatu == 1) {
            $btn.data("statu", "0");
            $btn.removeClass("k-eye-open");
            $btn.addClass("k-eye-close");
            $("#" + passwordId).hide();
            $("#" + textId).show();
        }
        $("#" + textId).val($("#" + passwordId).val());
    } else {
        if (changeStatu == 1) {
            $btn.data("statu", "1");
            $btn.removeClass("k-eye-close");
            $btn.addClass("k-eye-open");
            $("#" + textId).hide();
            $("#" + passwordId).show();
        }
        $("#" + passwordId).val($("#" + textId).val());
    }
}
function go2authLogin() {
    location.href = CommonValues.AuthLogin;//"@Url.Action("AuthLogin")";
}
window.onload = function () {
    $("#slider-wrapper").css("width", $(window).width());
    var offset = (2000 - parseInt($(document).width())) / 2;
    $("#slider").css("right", offset);
    function getBanner() {
        $.ajax({
            type: 'POST',
            url: CommonValues.GetBanner,//'@Url.Action("GetBanner", "Account")',
            dataType: "json",
            success: function (result) {
                if (result != null && result.Success && result.Data != null && result.Data.length > 0) {
                    var arrayContent = new Array();
                    $.each(result.Data, function (i, item) {
                        arrayContent[i] = "<a target=\"_blank\" href=\"" + item.Link + "\"><img src=\"" + item.PicLink + "\" data-thumb=\"" + item.PicLink + "\" alt=\"\" /></a>";
                    });
                    $(".banner").css("background", "none");
                    $("#slider").html(arrayContent.join(""));
                    $('#slider').nivoSlider({ controlNav: false });
                    $(".nivo-prevNav").css("left", offset + 15);
                    $(".nivo-nextNav").css("left", parseInt(2000 - offset - 45));
                } else {
                    $("#slider-wrapper").css("display", "none");
                }
            },
            error: function () {
                $("#slider-wrapper").css("display", "none");
            }
        });
    }
    getBanner();
}