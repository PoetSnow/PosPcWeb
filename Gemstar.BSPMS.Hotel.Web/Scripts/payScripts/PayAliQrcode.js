function PayAliQrcode() {
    PayBase.apply(this);
    PayAliQrcode.prototype.AliQrcodeQueryData = undefined;
}
PayAliQrcode.prototype = Object.create(PayBase.prototype);
PayAliQrcode.prototype.constructor = PayAliQrcode;
PayAliQrcode.prototype.AliQrcodeQueryData = undefined;
//初始化界面的方法
PayAliQrcode.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">'
        + '<td style="text-align:right;">提示：</td>'
        + '<td colspan="3">请输入金额后点确定提交，提交后程序会生成支付订单并显示二维码</td>'
        + '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};
//设置各自的支付参数方法
PayAliQrcode.prototype.PaySetPara = function (model) {
    var para = { amount: model.FolioAmount, subject: this.PayGetBody(this, model), itemId: model.PayItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
//支付提交保存成功后的后续方法
PayAliQrcode.prototype.PayAfterSave = function (data) {
    PayAliQrcode.prototype.AliQrcodeQueryData = data;
    //禁用确定按钮
    this.PayDisableSaveBtn(this);
    //显示提示信息
    var htmlWaitPay = '<div id="folioAliQrcodeWaitpayDiv" style="text-align:center;">'+
        '<div id="folioAliQrcodeImg"></div>'+
        '<button class="k-button" id="folioAliQrcodeCloseQueryButton">关闭并打印</button>' +
        '</div>';
    $(htmlWaitPay).appendTo(this.settings.selectorForResultDiv);
    //显示二维码内容
    $("#folioAliQrcodeImg").qrcode({ text: data.Data.QrCodeUrl, width: 180, height: 180 });
    //重新显示窗口居中
    this.PayCenterWindow(this);
    //绑定按钮点击事件
    $("#folioAliQrcodeCloseQueryButton").click(function (e) {
        e.preventDefault();
        var data = PayAliQrcode.prototype.AliQrcodeQueryData;
        console.log(data);
        debugger;
        if ($("#folioAddItemAmountOriC").length > 0)
        {
            folioAfterSave(data, 0);
        }
        else
        {
            rechargeFolioAfterSave(data);
        }        
        PayAliQrcode.prototype.AliQrcodeQueryData = undefined;
    });
};
