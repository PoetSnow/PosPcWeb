function PayRoomFolio() {
    PayBase.apply(this);
}
PayRoomFolio.prototype = Object.create(PayBase.prototype);
PayRoomFolio.prototype.constructor = PayRoomFolio;
//初始化界面的方法
PayRoomFolio.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">' +
        '<td class="textright">房间号</td>' +
        '<td><input type="text" style="width:100%" class="k-textbox" id="roomNo" name="roomNo" oninput="roomNoChange()" onchange="roomNoChange()" /></td>' +
         '<td class="textright">客人名</td>' +
        '<td><input type="text" readonly="true" style="width:100%;background-color:rgb(245, 245, 245);" class="k-textbox" id="labelRoom"/></td>' +
        '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};

//提交前的检测方法
PayRoomFolio.prototype.PayCheck = function () {
    var roomNoValue = $("#roomNo").val();
    if (!roomNoValue) {
     //   jAlert("请输入需要会员卡费转房账的房间号", "确定");
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
PayRoomFolio.prototype.PaySetPara = function (model) {
    var para = { roomNo: $("#roomNo").val(), amount: model.FolioAmount, invno: model.InvNo, remark: model.Remark, Id: model.Id, itemid: model.PayItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
