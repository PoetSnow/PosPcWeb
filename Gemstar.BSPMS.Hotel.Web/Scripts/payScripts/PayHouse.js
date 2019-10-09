function PayHouse() {
    PayBase.apply(this);
}
PayHouse.prototype = Object.create(PayBase.prototype);
PayHouse.prototype.constructor = PayHouse;
//初始化界面的方法
PayHouse.prototype.PayInit = function () {
    //{"Row":{"RegId":"00025035","RoomNo":"0709","GuestCname":"测试","ArrDate":"2018-01-03 15:00:00","Payment":"","isTransfer":"1","ExcutiveRate":"51","Balance":"-353","EnableAmount":"-353","WqPersons":"0"}}

    var htmlStr = '<tr class="folioAddPayment">' +
        '<td class="textright">房间号：</td>' +
        '<td class="textright"><input type="text" style="width:100%" class="k-textbox" id="roomNo" name="roomNo" onkeydown="getRommInfo(this,0)" onblur="getRommInfo(this,1)" /></td>' +
        '<td class="textright">客人名：</td>' +
        '<td class="textright"><input type="text" readonly="true" style="width:100%;background-color:rgb(245, 245, 245);" class="k-textbox" id="labelRoom"/></td></tr>' +
        '<tr class="folioAddPayment"><td class="textright">抵店日期：</td><td><span id="lblArrDate"></span></td><td class="textright">余　额：</td><td><span id="lblBalance"></span></td></tr>' +
        '<tr class="folioAddPayment"><td class="textright">授权金额：</td><td><span id="lblApprovalAmt"></span></td><td class="textright">信用调节额：</td><td><span id="lblApprovalAdj"></span></td></tr>' +
        '<tr class="folioAddPayment"><td class="textright">可用余额：</td><td><span id="lblAvailableBalance"></span></td><td class="textright">挂账限额：</td><td><span id="lblLimitAmount"></span></td></tr>' +
        '<tr class="folioAddPayment"><td class="textright">付款方式：</td><td><span id="lblPayment"></span></td><td class="textright">已挂账金额：</td><td><span id="lblChargeamt"></span></td></tr>' +
        '<tr class="folioAddPayment"><td class="textright">房　价：</td><td><span id="lblExcutiveRate"></span></td><td class="textright">可用限额：</td><td><span id="lblEnableAmount"></span></td></tr>' +
        '<tr class="folioAddPayment"><td class="textright">备　注：</td><td><span id="lblRemark"></span></td><td class="textright">收银说明：</td><td><span id="lblCashRemark"></span></td></tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};

//提交前的检测方法
PayHouse.prototype.PayCheck = function () {
    var roomNoValue = $("#roomNo").val();
    if (!roomNoValue) {
        layer.alert("请输入需要会员卡费转房账的房间号", { title: "快点云Pos提示", skin: 'err' });
        return false;
    }
    return true;
};
function roomNoChange()
{
    var roomNoValue = $("#roomNo").val();
    if (roomNoValue == "")
    {
        $("#labelRoom").val("");
        return;
    }
    $.ajax({
        type: "POST",
        url: "/MbrCardCenter/MbrCardManage/CheckRoomNo",
        data: { roomNo: roomNoValue },
        async: false,
        success: function (data) {
            $("#labelRoom").val(data.Data);
            if (data.Success) {
                $("#labelRoom").css("color", "");
            }
            else {
                $("#labelRoom").css("color", "red");
            }
        }
    });
}
//设置各自的支付参数方法
PayHouse.prototype.PaySetPara = function (model) {
    var para = { roomNo: $("#roomNo").val(), amount: model.FolioAmount, invno: model.InvNo, remark: model.Remark, Id: model.Id, itemid: model.PayItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
