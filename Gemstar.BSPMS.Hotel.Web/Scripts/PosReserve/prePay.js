function PayModeNo_change() {
    var payMomo = $("#PayModeNo").data("kendoDropDownList").value();

    var payType = payMomo.split('_')[1];

    if (payType == "no") {
        $("#tr_card").css("display", "none");
        $("#tr_Wx").css("display", "none");
    }
    else if (payType == "AliBarcode" || payType == "WxBarcode") {
        $("#tr_card").css("display", "none");
        $("#tr_Wx").css("display", "block");
    }
    else if (payType == "mbrCard" || payType == "mbrLargess" || payType == "mbrCardAndLargess") {
        $("#tr_card").css("display", "block");
        $("#tr_Wx").css("display", "none");
    }
    else {
        $("#tr_card").css("display", "none");
        $("#tr_Wx").css("display", "none");
        layer.alert("不支持此付款方式", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
}

var payIndex;

function addPrepay() {

  
  
    //增加验证
    var amount = $("#Amount").val();
    if (Number(amount) <= 0) {
        layer.alert("金额必须大于0！", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    var payMomo = $("#PayModeNo").data("kendoDropDownList").value();
    if (payMomo == "") {
        layer.alert("请选择付款方式！", { title: "快点云Pos提示", skin: "err" });
        return false;
    }

    var payId = payMomo.split('_')[0];//付款方式Id
    var payType = payMomo.split('_')[1];//处理动作
    
    if (payType == "AliBarcode" || payType == "WxBarcode") {
        var barCode = $("#BarCode").val();  //微信，支付宝付款条码
        if (barCode=="") {
            layer.alert("付款码不能为空！", { title: "快点云Pos提示", skin: "err" });
            return false;
        }
        $("#folioWxBarcode").val(barCode);
    }
    if (payType == "mbrCard" || payType == "mbrLargess" || payType == "mbrCardAndLargess") {
        var cardNo = $("#CardId").data("kendoComboBox").text(); //会员卡号
        var cardId = $("#CardId").val();
        if (cardId == "") {
            layer.alert("请选择会员信息！", { title: "快点云Pos提示", skin: "err" });
            return false;
        }
        $("#folioMbrCardNo").val(cardNo);
    }

    //设置支付参数
    var model = {
        FolioAmount: $("#Amount").val(),
        FolioItemAction: payType,
        PayItemId: payId,
        PayBody: $("#PosName").val(),
        MbrCardNo: $("#folioMbrCardNo").val(),
        ProfileId: $("#CardId").val()        
    };
    try {
        var payAction = createPayByAction(payType);
        payAction.PaySetPara(model);
    } catch (e) { }
    $("#FolioItemActionJsonPara").val(model.FolioItemActionJsonPara);
    //提交表单
    var f = $("#fromAddPrepay");
    var validator = $(f).validate();
    if (validator.form()) {
        $.post(
            $(f).attr("action"),
            $(f).serialize(),
            function (data) {
                if (data.Success) { 
                    payIndex = layer.alert('添加成功', {
                        skin: 'err',
                        closeBtn: 1,
                        btn: ['确定'],
                        title: "快点云Pos提示",
                        yes: function (index, layero) {                           
                            if ($("#OpenFlag").val() == "B") {
                                layer.closeAll();
                                top.closeKendoWindow();
                                var iframe = $('iframe[data-contenttype="thirdmenu"]')[0];
                                iframe.contentWindow.location.reload(true);

                            }
                            else {
                                layer.closeAll();
                            }
                        }
                    })
                }
                else {
                    if (data.ErrorCode == "2") {
                        PayWxBarcode.prototype.wxbarcodeQueryData = data;
                        payIndex=layer.alert('正在支付请稍候。。。', {
                            icon: 16,
                            skin: 'err',
                            closeBtn: 1,
                            btn: ['确定'],
                            title: "快点云Pos提示",
                            yes: function (index, layero) {
                                return false;
                            }
                        })

                     
                        //启动定时器进行定时刷新支付状态
                        PayWxBarcode.prototype.folioWxbarcodeQueryTimes = 0;
                        PayWxBarcode.prototype.queryIntervalId = setInterval(function () { _folio_WxBarcode_queryPayStatus(); }, 5 * 1000);

                        //加入全局定时器数组，便于清除
                        queryInterval.push(PayWxBarcode.prototype.queryIntervalId);
                    }
                    else {
                        layer.alert("添加失败：" + data.Data, { title: "快点云Pos提示", skin: "err" });
                    }
                    
                }
            },
            "json");
    }
}

function selectHYInfo()
{
    layer.alert("请先选择会员", { title: "快点云Pos提示", skin: "err" });
    return false;
}

//function PayWxBarcode() {
//    PayBase.apply(this);
//    PayWxBarcode.prototype.queryIntervalId = undefined;
//    PayWxBarcode.prototype.wxbarcodeQueryData = undefined;
//}

function _folio_WxBarcode_queryPayStatus()
{
    var that = this;
    if (PayWxBarcode.prototype.wxbarcodeQueryData != undefined) {
        $.post("/PayManage/Pay/QueryFolioPayStatu", { folioPayInfoId: PayWxBarcode.prototype.wxbarcodeQueryData.Data.QueryTransId }, function (data) {
            console.log(data);
            _folio_WxBarcode_handleQueryResult(data);
        }, 'json');
    }
}
function _folio_WxBarcode_handleQueryResult(data)
{
    if (isNaN(PayWxBarcode.prototype.folioWxbarcodeQueryTimes)) {
        PayWxBarcode.prototype.folioWxbarcodeQueryTimes = 0;
    }
    PayWxBarcode.prototype.folioWxbarcodeQueryTimes = PayWxBarcode.prototype.folioWxbarcodeQueryTimes + 1;
    if (data.Success) {
        layer.close(payIndex);
        PayWxBarcode.prototype.folioClearQueryInterval();
        layer.alert("支付成功", { title: "快点云Pos提示", skin: "err" });

    }
}
