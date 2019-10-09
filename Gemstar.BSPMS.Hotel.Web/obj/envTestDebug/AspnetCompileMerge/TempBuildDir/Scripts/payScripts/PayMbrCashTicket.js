function PayMbrCashTicket() {
    PayBase.apply(this);
}
PayMbrCashTicket.prototype = Object.create(PayBase.prototype);
PayMbrCashTicket.prototype.constructor = PayMbrCashTicket;
//初始化界面的方法
PayMbrCashTicket.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">' +
        '<td class="textright">现金券号</td>' +
        '<td colspan="3"><input type="text" style="width:100%" class="k-textbox" id="folioTicketNo" onblur="check()" name="folioTicketNo" />' +
        '<input type=hidden id="folioAmount"></td>' +
        '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};

//提交前的检测方法
PayMbrCashTicket.prototype.PayCheck = function () {
    var cardNoValue = $("#folioTicketNo").val();
    
    if (!cardNoValue) {
        //jAlert("请输入现金券号", "确定");
        layer.alert("请输入现金券号", { title: "快点云Pos提示", skin: 'err'  });
        return false;
    }
    //if (cardNoValue.trim().lenth != 12) {
    //    jAlert("现金券号应是12位数字", "确定");
    //    return false;
    //}
    return true;
};
function check()
{
    var ticketNo = $("#folioTicketNo").val();
    var money = $("#folioAddItemAmountC").data("kendoNumericTextBox").value();
    $.post("/MbrCardCenter/MbrCardManage/CheckTicketNo", { ticketNo: ticketNo, money: money }, function (data) {
        if (data.Success) {            
            if ($.trim(money) == "")
            {
                $("#folioAddItemAmountOriC").data("kendoNumericTextBox").value(data.Data);
                $("#folioAddItemAmountC").data("kendoNumericTextBox").value(data.Data);
            }
            return true;
        }
        else {
            ajaxErrorHandle(data);
            return false;
        }

    }, 'json');
    
    //jAlert(, "确定");
}
//设置各自的支付参数方法
PayMbrCashTicket.prototype.PaySetPara = function (model) {
    var para = { ticketNo: $("#folioTicketNo").val(), remark: model.FolioRemark,amount: model.FolioAmount, regid: model.FolioRegId, itemid: model.FolioItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
