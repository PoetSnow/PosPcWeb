function PayAliCredit() {
    PayBase.apply(this);
}
PayAliCredit.prototype = Object.create(PayBase.prototype);
PayAliCredit.prototype.constructor = PayAliCredit;
//初始化界面的方法
PayAliCredit.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">' +
                '<td colspan="4" style="text-align:right;">注意：提交后会等待阿里信用住支付，请勿关闭此窗口。</td>' +
                '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};

//提交前的检测方法
PayAliCredit.prototype.PayCheck = function () {
    return true;
};

//设置各自的支付参数方法
PayAliCredit.prototype.PaySetPara = function (model) {
    var para = {amount: model.FolioAmount, regid: model.FolioRegId, itemid: model.FolioItemId, ischeck: model.IsCheckout};
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
