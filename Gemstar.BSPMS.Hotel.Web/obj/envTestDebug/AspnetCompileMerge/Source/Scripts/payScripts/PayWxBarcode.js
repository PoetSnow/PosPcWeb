function PayWxBarcode() {
    PayBase.apply(this);
    PayWxBarcode.prototype.queryIntervalId = undefined;
    PayWxBarcode.prototype.wxbarcodeQueryData = undefined;
}
PayWxBarcode.prototype = Object.create(PayBase.prototype);
PayWxBarcode.prototype.constructor = PayWxBarcode;

//初始化界面的方法
PayWxBarcode.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">'
        + '<td class="textright">付款条码：</td>'
        + '<td colspan="3"><input type="text" style="width:100%;" class="k-textbox" id="folioWxBarcode" name="folioWxBarcode" /></td>'
        + '</tr>'
        + '<tr class="folioAddPayment">'
        + '<td class="textright">注意：</td>'
        + '<td colspan="3">付款条码只有一分钟有效期，所以扫描后请尽快点击确定提交</td>'
        + '</tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
};
//提交前的检测方法
PayWxBarcode.prototype.PayCheck = function () {
    var cardNoValue = $("#folioWxBarcode").val();
    if (!cardNoValue) {
        // jAlert("请输入付款条码");
        layer.alert("请输入付款条码", { title: "快点云Pos提示", skin: 'err'  });
        return false;
    }
    return true;
};
//设置各自的支付参数方法
PayWxBarcode.prototype.PaySetPara = function (model) {
    var para = { barcode: $("#folioWxBarcode").val(), amount: model.FolioAmount, body: this.PayGetBody(this, model), itemId: model.PayItemId };
    model.FolioItemActionJsonPara = JSON.stringify(para);
};
//支付提交保存成功后的后续方法
PayWxBarcode.prototype.PayAfterSave = function (data) {
    

    PayWxBarcode.prototype.wxbarcodeQueryData = data;
    //禁用确定按钮
    this.PayDisableSaveBtn(this);
    var that = this;
    //显示提示信息
    var htmlWaitPay = '<div id="folioWxbarcodeWaitpayDiv">' +
        ' <p style="color: red;line-height: 16px;">用户正在输入密码中，请不要关闭此窗口。<br/>程序将会自动查询实际的支付状态；如关闭此窗口，则需要手动查询支付状态。</p>' +
        ' <textarea id="folioWxbarcodeQueryResultShow" style="color: #ff5555;border:solid 1px #ccc ;height:85px;width:410px;">此处将会显示查询结果，请稍候</textarea>' +
        ' <br /><button class="k-button" id="folioWxbarcodeCloseQueryButton" style="display:none;">关闭窗口，稍后手动查询</button>' +
        '</div>';
    $(htmlWaitPay).appendTo($(this.settings.selectorForResultDiv));
    //绑定按钮点击事件
    $("#folioWxbarcodeCloseQueryButton").click(function (e) {
        e.preventDefault();
        data.Data.Callback();
    });
    //启动定时器进行定时刷新支付状态
    PayWxBarcode.prototype.folioWxbarcodeQueryTimes = 0;
    PayWxBarcode.prototype.queryIntervalId = setInterval(function () { that.folio_WxBarcode_queryPayStatus.call(that); }, 5 * 1000);

    //加入全局定时器数组，便于清除
    queryInterval.push(PayWxBarcode.prototype.queryIntervalId);

};

//定时查询方法
PayWxBarcode.prototype.folio_WxBarcode_queryPayStatus = function () {
    var that = this;
    
    if (PayWxBarcode.prototype.wxbarcodeQueryData != undefined) {
        $.post(this.settings.urlForQueryPayStatus, { folioPayInfoId: PayWxBarcode.prototype.wxbarcodeQueryData.Data.QueryTransId }, function (data) {
            that.folio_WxBarcode_handleQueryResult(data);
        }, 'json');
    }
};
PayWxBarcode.prototype.folio_WxBarcode_handleQueryResult = function (data) {
    if (isNaN(PayWxBarcode.prototype.folioWxbarcodeQueryTimes)) {
        PayWxBarcode.prototype.folioWxbarcodeQueryTimes = 0;
    }
    PayWxBarcode.prototype.folioWxbarcodeQueryTimes = PayWxBarcode.prototype.folioWxbarcodeQueryTimes + 1;
    if (data.Success) {

        enableSubmit(); //启用按钮
        $("#gridPayment").data("kendoGrid").dataSource.read();
        if ($("#gridPayment") != undefined) {
            RefreshAmount();
            $("#gridPayment").data("kendoGrid").dataSource.read();
            setTimeout(function () {
                if (parseFloat($("#txtMoneyPayment").val()) <= 0 || parseFloat($("#isSuccess").val()) == 1) {
                    layer.alert('买单成功！', {
                        time: 0 //不自动关闭
                        , title: "快点云Pos提示"
                        , btn: ['确定']
                        , yes: function () {
                            PayWxBarcode.prototype.wxbarcodeQueryData.Data.Callback();
                            layer.closeAll();
                        }
                    });
                }
            }, 500);
        }
        PayWxBarcode.prototype.folioClearQueryInterval();
        $("#folioWxbarcodeCloseQueryButton").text("支付成功，关闭");
       
    }
    var text = $("#folioWxbarcodeQueryResultShow").val();
    text = "" + PayWxBarcode.prototype.folioWxbarcodeQueryTimes + data.Data + "\r\n" + text;
    $("#folioWxbarcodeQueryResultShow").val(text);
};
//取消定时查询
PayWxBarcode.prototype.folioClearQueryInterval = function () {
    if (PayWxBarcode.prototype.queryIntervalId) {
        clearInterval(PayWxBarcode.prototype.queryIntervalId);
        PayWxBarcode.prototype.queryIntervalId = undefined;
    }
};
//关闭查询按钮点击
PayWxBarcode.prototype.folioWxbarcodeCloseQueryButton_clicked = function () {
    this.folioClearQueryInterval();
    folioAfterSave(PayWxBarcode.prototype.wxbarcodeQueryData, 0);//默认为0
};


