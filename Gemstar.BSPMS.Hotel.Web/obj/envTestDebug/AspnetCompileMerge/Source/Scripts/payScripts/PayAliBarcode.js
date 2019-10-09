function PayAliBarcode() {
    PayBase.apply(this);
}
PayAliBarcode.prototype = Object.create(PayBase.prototype);
PayAliBarcode.prototype.constructor = PayAliBarcode;

//初始化界面的方法
PayAliBarcode.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">'
        + '<td class="textright">付款条码：</td>'
        + '<td colspan="3"><input type="text" style="width:100%;" class="k-textbox" id="folioAliBarcode" name="folioAliBarcode" /></td>'
        + '</tr>'
        + '<tr class="folioAddPayment">'
        + '<td class="textright">注意：</td>'
        + '<td colspan="3">付款条码只有一分钟有效期，所以扫描后请尽快点击确定提交</td>'
        + '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};
//提交前的检测方法
PayAliBarcode.prototype.PayCheck = function () {
    var cardNoValue = $("#folioAliBarcode").val();
    if (!cardNoValue) {
       // jAlert("请输入付款条码");
        layer.alert("请输入付款条码", { title: "快点云Pos提示", skin: 'err'  });
        return false;
    }
    return true;
};
//设置各自的支付参数方法
PayAliBarcode.prototype.PaySetPara = function (model) {
    var para = { authCode: $("#folioAliBarcode").val(), amount: model.FolioAmount, subject: this.PayGetBody(this, model), itemId: model.PayItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};

