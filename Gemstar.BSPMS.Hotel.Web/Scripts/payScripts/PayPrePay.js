//定金支付
function PayPrePay() {

    PayBase.apply(this);
}
PayPrePay.prototype = Object.create(PayBase.prototype);
PayPrePay.prototype.constructor = PayPrePay;

//初始化界面的方法
PayPrePay.prototype.PayInit = function () {
    //var htmlStr = '<tr class="folioAddPayment"><td class="textright">单号：</td><td colspan="3"><input type="text" class="k-textbox" id="folioPrePayBillNo" name="folioPrePayBillNo" onkeydown="getPrePay(this,0)" onblur="getPrePay(this,1)" style="width:80%" /><input id="lblPrePayId" type="hidden" value="" /></td></tr>'

    //    + '<tr class="folioAddPayment"><td class="textright">姓名：</td><td><input type="text" readonly="true" style="width:100%;background-color:rgb(245, 245, 245);" class="k-textbox" id="txtGuestCName"/></td>'
    //    + '<td class="textright">电话：</td><td><input type="text" readonly="true" style="width:100%;background-color:rgb(245, 245, 245);" class="k-textbox" id="txtMobile"/></td></tr>'
    //    + '<tr class="folioAddPayment"><td class="textright" > 余额：</td > <td><span id="lblBalance"></span></td> <td class="textright">营业日：</td><td><span id="lbldBusiness"></span></td></tr>'
    //    + '<tr class="folioAddPayment"><td class="textright" > 班次：</td > <td><span id="lblshift"></span></td> <td class="textright">收据号码：</td><td><span id="lblhandBillno"></span></td></tr>'
    //    + '<tr class="folioAddPayment"><td class="textright" > 使用时间：</td > <td><span id="lblusedDate"></span></td> <td class="textright">使用说明：</td><td><span id="lblusedDesc"></span></td></tr>'
    //    + '<tr class="folioAddPayment"><td class="textright" > 备注：</td > <td colspan="3"><span id="lblRemark"></span></td> </tr>';
    var _selectorForInit = $(this.settings.selectorForInit);
    $.ajax({
        url: '/PosManage/Shared/_PayPrePay',
        type: "post",
        data: {},
        dataType: "html",
        success: function (data) {
            $(data).insertAfter(_selectorForInit);
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
    //$(htmlStr).insertAfter($(this.settings.selectorForInit));
};

PayPrePay.prototype.isRefund = 0;
//提交前的检测方法
PayPrePay.prototype.PayCheck = function () {
    var prePayId = $("#PrePayId").val();
    if (prePayId == '' || $("#lblBalance").text()=='') {
        enableSubmit(); //启用按钮
        layer.alert('请选择押金 ', { title: "快点云Pos提示", skin: 'err' });
        return false;
    }
    //var payAmount = $("#txtMoneyPayment").val();    //付款金额
    //var balanceAmount = $("#lblBalance").text();    //押金余额
    //if (Number(balanceAmount) > Number(payAmount)) {
    //    var conIndex = layer.confirm("是否退押金？", {
    //        btn: ['继续', '取消'] //按钮
    //        , title: '快点云Pos提示'
    //        , shade: 'rgba(0,0,0,0)'
    //    }, function () {
    //        PayPrePay.prototype.isRefund = 1;
    //    }, function () {
    //        PayPrePay.prototype.isRefund = 0;
    //    });
       
    //}   
    return true;
};
//设置各自的支付参数方法
PayPrePay.prototype.PaySetPara = function (model) {
    model.IsRefund = PayPrePay.prototype.isRefund;
    model.PrePayId = $("#PrePayId").val();
};