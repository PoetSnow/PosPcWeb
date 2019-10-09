function PayBankCard() {
    PayBase.apply(this);
}
PayBankCard.prototype = Object.create(PayBase.prototype);
PayBankCard.prototype.constructor = PayBankCard;
//初始化界面的方法
PayBankCard.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">'+
        '<td class="textright">银联卡号</td>'+
        '<td colspan="3"><input type="text" style="width:100%" class="k-textbox" id="folioBankCardNo" name="folioBankCardNo" /></td>'+
        '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};

//提交前的检测方法
PayBankCard.prototype.PayCheck = function () {
    var cardNoValue = $("#folioBankCardNo").val();
    if (!cardNoValue) {
        //jAlert("请输入银联卡卡号", "确定");
        layer.alert("请输入银联卡卡号", { title: "快点云Pos提示", skin: 'err'  });
        return false;
    }
    return true;
};

//设置各自的支付参数方法
PayBankCard.prototype.PaySetPara = function (model) {
    var para = { cardNo: $("#folioBankCardNo").val() };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};

