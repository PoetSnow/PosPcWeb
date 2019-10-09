function PayCredit() {
    PayBase.apply(this);
}
PayCredit.prototype = Object.create(PayBase.prototype);
PayCredit.prototype.constructor = PayCredit;
//初始化界面的方法
PayCredit.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">' +
        '<td class="textright">信用卡号</td>' +
        '<td><input type="text" class="k-textbox" id="folioCreditCardNo" name="folioCreditCardNo" style="width:100%"/></td>' +
        '<td class="textright">有效期</td>' +
        '<td><input type="text" id="folioCreditCardExpire" name="folioCreditCardExpire"  style="width:100%"/></td>'+
        '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
    $("#folioCreditCardExpire").kendoDatePicker({
        animation: false,
        format: "MM/yy"
    });
};

//提交前的检测方法
PayCredit.prototype.PayCheck = function () {
    var cardNoValue = $("#folioCreditCardNo").val();
    if (!cardNoValue) {
       // jAlert("请输入信用卡卡号", "确定");
        layer.alert("请输入信用卡卡号", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }
    var expireDate = $("#folioCreditCardExpire").data("kendoDatePicker").value();
    if (!expireDate) {
       // jAlert("请输入信用卡有效期", "确定");
        layer.alert("请输入信用卡有效期", { title: "快点云Pos提示", skin: 'err'  });
        return false;
    }
    return true;
};

//设置各自的支付参数方法
PayCredit.prototype.PaySetPara = function (model) {
    var expireDate = $("#folioCreditCardExpire").data("kendoDatePicker").value();
    if (expireDate) {
        expireDate = expireDate.ToDateString().substr(0, 7);
    } else {
        expireDate = "";
    }
    var para = { cardNo: $("#folioCreditCardNo").val(), expire: expireDate };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
