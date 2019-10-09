function PayWxQrcode() {
    PayBase.apply(this);
    PayWxQrcode.prototype.wxqrcodeQueryData = undefined;
}
PayWxQrcode.prototype = Object.create(PayBase.prototype);
PayWxQrcode.prototype.constructor = PayWxQrcode;
PayWxQrcode.prototype.wxqrcodeQueryData = undefined;
//初始化界面的方法
PayWxQrcode.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">'
        + '<td style="text-align:right;">提示：</td>'
        + '<td colspan="3">请输入金额后点确定提交，提交后程序会生成支付订单并显示二维码</td>'
        + '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};
//设置各自的支付参数方法
PayWxQrcode.prototype.PaySetPara = function (model) {
    var para = { amount: model.FolioAmount, body: this.PayGetBody(this, model), itemId: model.PayItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
//支付提交保存成功后的后续方法
PayWxQrcode.prototype.PayAfterSave = function (data) {
    PayWxQrcode.prototype.wxqrcodeQueryData = data;
    //禁用确定按钮
    this.PayDisableSaveBtn(this);
    //显示提示信息
    var htmlWaitPay = '<div id="folioWxQrcodeWaitpayDiv" style="text-align:center;">' +
        '<div id="folioWxQrcodeImg"></div>' +
        '<button class="k-button" id="folioWxQrcodeCloseQueryButton">关闭并打印</button>' +
        '</div>';
    $(htmlWaitPay).appendTo(this.settings.selectorForResultDiv);
    //显示二维码内容
    $("#folioWxQrcodeImg").qrcode({ text: data.Data.QrCodeUrl, width: 180, height: 180 });
    //重新显示窗口居中
    this.PayCenterWindow(this);
    //绑定按钮点击事件
    $("#folioWxQrcodeCloseQueryButton").click(function (e) {
        e.preventDefault();
        var data = PayWxQrcode.prototype.wxqrcodeQueryData;
        console.log(data);
        if ($("#folioAddItemAmountOriC").length > 0) {
            folioAfterSave(data, 0);
        }
        else {
            rechargeFolioAfterSave(data);
        }
        PayWxQrcode.prototype.wxqrcodeQueryData = undefined;
    });
};

