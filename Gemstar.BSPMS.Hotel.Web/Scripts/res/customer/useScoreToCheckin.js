//积分换房
var useScoreToCheckinWindow = {
    Initialization: function () {
        var windowObj = $("#orderDetailUseScoreToCheckinWindow");
        var windowObjKendo = windowObj.data("kendoWindow");
        if (windowObjKendo == null || windowObjKendo == undefined) {
            windowObj.kendoWindow({
                title: "积分换房",
                width: "427px",
                visible: false,
                modal: true,
                actions: ["Close"],
            });
            windowObjKendo = windowObj.data("kendoWindow");
        }
        $("#orderDetailUseScoreToCheckinWindow_submit").unbind("click").click(function (e) { useScoreToCheckinWindow.Save(e); });
        $("#orderDetailUseScoreToCheckinWindow_cancel").unbind("click").click(function (e) { useScoreToCheckinWindow.Cancel(e); });
        return windowObjKendo;
    },
    Open: function (data, successCallBackFunction) {
        //验证
        if (data == null || data == undefined && data.length > 0) { jAlert("积分换房内容不能为空！", "知道了"); return; }
        if ($.trim(successCallBackFunction).length <= 0) { jAlert("积分换房确认后执行操作不能为空！", "知道了"); return; }
        //赋值
        var trList = [];
        var topTr = "<tr><td style=\"background-color: #cce4ff;\">{0}</td><td style=\"background-color: #cce4ff;\">{1}</td><tr>";
        var tr = "<tr><td>{0}</td><td>{1}</td><tr>";
        var isTrue = true;
        $.each(data, function (index, item) {
            var leftTitle = "房号：{0}<br/>价格代码：{1}<br/>入住所需积分：{2}<br/>";
            leftTitle = leftTitle.replace("{0}", item.RoomNo);
            leftTitle = leftTitle.replace("{1}", item.RateCodeName);
            leftTitle = leftTitle.replace("{2}", item.RateScore);
            var rightTitle = "会员：{3} {5}<br/>会员卡号：{4}<br/>会员可用积分：{6}<br/>";
            rightTitle = rightTitle.replace("{3}", item.ProfileName);
            rightTitle = rightTitle.replace("{4}", item.ProfileNo);
            rightTitle = rightTitle.replace("{5}", $.trim(item.ProfileMobile));
            rightTitle = rightTitle.replace("{6}", item.ProfileScore);
            trList.push(topTr.replace("{0}", leftTitle).replace("{1}", rightTitle));
            $.each(item.RatePlans, function (index, rateItem) {
                trList.push(tr.replace("{0}", convertJsonDate(rateItem.Ratedate)).replace("{1}", rateItem.Price));
            });
            if (item.ProfileScore < item.RateScore) { isTrue = false; }
        });
        $("#orderDetailUseScoreToCheckinWindow_Tbody").html(trList.join(""));
        $("#orderDetailUseScoreToCheckinWindow_SuccessCallBackFunction").val(successCallBackFunction);

        var editPriceDiv = $("#orderDetailUseScoreToCheckinWindow_editPriceDiv");
        var editPriceDivMsg = $("#orderDetailUseScoreToCheckinWindow_editPriceDivMsg");
        editPriceDiv.css("display", "block");
        editPriceDivMsg.css("display", "none");
        if (isTrue == false) {
            editPriceDiv.css("display", "none");
            editPriceDivMsg.css("display", "block");
        }
        //弹框
        var windowObjKendo = useScoreToCheckinWindow.Initialization();
        windowObjKendo.center().open();
    },
    Save: function () {
        $("#UseScoreSaveContinue").val("1");
        var funcName = $("#orderDetailUseScoreToCheckinWindow_SuccessCallBackFunction").val();
        try { var func = eval(funcName); if (typeof (func) == "function") { func(); } } catch (e) { }
        useScoreToCheckinWindow.Cancel();
        $("#UseScoreSaveContinue").val("0");
    },
    Cancel: function () {
        //取消
        $("#orderDetailUseScoreToCheckinWindow").data("kendoWindow").close();
    },
};