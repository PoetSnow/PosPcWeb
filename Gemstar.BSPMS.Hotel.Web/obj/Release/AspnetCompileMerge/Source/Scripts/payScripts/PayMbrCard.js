function PayMbrCard() {
    PayBase.apply(this);
}
PayMbrCard.prototype = Object.create(PayBase.prototype);
PayMbrCard.prototype.constructor = PayMbrCard;
//初始化界面的方法
PayMbrCard.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment"><td class="textright">卡号/手机：</td><td><input type="text" class="k-textbox" id="folioMbrCardNo" name="folioMbrCardNo" onkeydown="getMbrCard(this,0)" onblur="getMbrCard(this,1)"/><input id="lblProfileId" type="hidden" value="" /></td>'
        + '<td class="textright">客户名：</td><td><input type="text" readonly="true" style="width:100%;background-color:rgb(245, 245, 245);" class="k-textbox" id="txtGuestCName"/></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">储值余额：</td><td><span id="lblBalance"></span></td><td class="textright">卡类型：</td>'
        + '<td><span id="lblMbrCardTypeName"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">状　态：</td><td><span id="lblStatus"></span></td><td class="textright">用户类型：</td><td><span id="lblGuestType"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">有效期：</td><td><span id="lblMbrExpired"></span></td><td class="textright">联系人：</td><td><span id="lblContactor"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">性　别：</td><td><span id="lblGender"></span></td><td class="textright">生　日：</td><td><span id="lblBirthday"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">可用积分：</td><td><span id="lblValidScore"></span></td><td class="textright">已用金额：</td><td><span id="lblTotalUsedBalance"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">本金余额：</td><td><span id="lblBaseAmtBalance"></span></td><td class="textright">赠券余额：</td><td><span id="lblIncamount"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">市场类型：</td><td><span id="lblMarket"></span></td><td class="textright">公司名称：</td><td><span id="lblCompanyName"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">消费次数：</td><td><span id="lblTimes"></span></td></tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
}; 

//提交前的检测方法
PayMbrCard.prototype.PayCheck = function () {
    var cardNoValue = $("#folioMbrCardNo").val();
    if (!cardNoValue) {
        //jAlert("请输入会员卡卡号", "确定");
        layer.alert("请输入需要会员卡费转房账的房间号", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }
    return true;
};

//设置各自的支付参数方法
PayMbrCard.prototype.PaySetPara = function (model) {
    var para = { cardNo: $("#folioMbrCardNo").val(), amount: model.FolioAmount, outletCode: "01", invno: model.FolioInvoNo, remark: model.FolioRemark, regid: model.FolioRegId, itemid: model.FolioItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};

PayMbrCard.prototype.PayAfterSave = function (model) {

    
};
