//发票
var OrderMainInvoiceInfoWindow = {
    Initialization: function () {
        //发票弹框初始化
        $("#orderMainInvoiceInfoWindow").kendoWindow({
            width: "450px",
            title: "发票信息",
            visible: false,
            modal: true,
            actions: ["Close"],
            close: function () {
                var invoiceInfoTemporary = $.trim($("#InvoiceInfoTemporary").val());
                if (invoiceInfoTemporary.length > 0) {
                    //改为上一次保存的信息
                    var obj = JSON.parse(invoiceInfoTemporary);
                    $("#ResInvoiceInfo_TaxName").val(obj.TaxName);
                    $("#ResInvoiceInfo_TaxNo").val(obj.TaxNo);
                    $("#ResInvoiceInfo_TaxAddTel").val(obj.TaxAddTel);
                    $("#ResInvoiceInfo_TaxBankAccount").val(obj.TaxBankAccount);
                    if (obj.InvoiceType == null || obj.InvoiceType == undefined) {
                        document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = false;
                        document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = false;
                    } else if (obj.InvoiceType == true) {
                        document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = true;
                    } else if (obj.InvoiceType == false) {
                        document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = true;
                    }
                } else {
                    //改为源信息
                    var originResInvoiceInfo = $.trim($("#ResInvoiceInfo_OriginResInvoiceInfoJsonData").val());
                    if (originResInvoiceInfo.length <= 0) {
                        $("#ResInvoiceInfo_Id").val("");
                        $("#ResInvoiceInfo_TaxName").val("");
                        $("#ResInvoiceInfo_TaxNo").val("");
                        $("#ResInvoiceInfo_TaxAddTel").val("");
                        $("#ResInvoiceInfo_TaxBankAccount").val("");
                        $("#ResInvoiceInfo_OriginResInvoiceInfoJsonData").val("");
                        document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = false;
                        document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = false;
                    } else {
                        var obj = JSON.parse(originResInvoiceInfo);
                        $("#ResInvoiceInfo_Id").val(obj.Id);
                        $("#ResInvoiceInfo_TaxName").val(obj.TaxName);
                        $("#ResInvoiceInfo_TaxNo").val(obj.TaxNo);
                        $("#ResInvoiceInfo_TaxAddTel").val(obj.TaxAddTel);
                        $("#ResInvoiceInfo_TaxBankAccount").val(obj.TaxBankAccount);
                        if (obj.InvoiceType == null || obj.InvoiceType == undefined) {
                            document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = false;
                            document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = false;
                        } else if (obj.InvoiceType == true) {
                            document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = true;
                        } else if (obj.InvoiceType == false) {
                            document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = true;
                        }
                    }
                }
            }
        });
        $("#orderMainInvoiceInfoWindow_submit").click(function (e) { OrderMainInvoiceInfoWindow.Save(); });
        $("#ResInvoiceInfo_InvoiceType_True").click(function (e) { OrderMainInvoiceInfoWindow.RadioChange(); });
        $("#ResInvoiceInfo_InvoiceType_False").click(function (e) { OrderMainInvoiceInfoWindow.RadioChange(); });
        $("#ResInvoiceInfo_InvoiceType_True").dblclick(function (e) { OrderMainInvoiceInfoWindow.ClearRadioStatus(); });
        $("#ResInvoiceInfo_InvoiceType_False").dblclick(function (e) { OrderMainInvoiceInfoWindow.ClearRadioStatus(); });
        $("[for='ResInvoiceInfo_InvoiceType']").dblclick(function (e) { OrderMainInvoiceInfoWindow.ClearRadioStatus(); });
        $("[for='ResInvoiceInfo_InvoiceType_True']").dblclick(function (e) { OrderMainInvoiceInfoWindow.ClearRadioStatus(); });
        $("[for='ResInvoiceInfo_InvoiceType_False']").dblclick(function (e) { OrderMainInvoiceInfoWindow.ClearRadioStatus(); });
    },
    Open: function () {
        //发票弹框
        $("#orderMainInvoiceInfoWindow").data("kendoWindow").center().open();
        var InvoiceType = $("[name='ResInvoiceInfo.InvoiceType']:checked").val();
        if (InvoiceType != "True" && InvoiceType != "False") {
            document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = true;
        }
        OrderMainInvoiceInfoWindow.RadioChange();
    },
    Save: function () {
        //发票保存
        var InvoiceType = $("[name='ResInvoiceInfo.InvoiceType']:checked").val();
        var TaxName = $.trim($("#ResInvoiceInfo_TaxName").val());
        var TaxNo = $.trim($("#ResInvoiceInfo_TaxNo").val());
        var TaxAddTel = $.trim($("#ResInvoiceInfo_TaxAddTel").val());
        var TaxBankAccount = $.trim($("#ResInvoiceInfo_TaxBankAccount").val());
        if (InvoiceType == "True") { InvoiceType = true; } else if (InvoiceType == "False") { InvoiceType = false; } else { InvoiceType == null; }
        if (InvoiceType != null) {
            if (TaxName.length <= 0) { jAlert("请填写发票抬头！"); return; }
        }
        if (InvoiceType == true) {
            if (TaxNo.length <= 0) { jAlert("请填写税务登记号！"); return; }
            if (TaxAddTel.length <= 0) { jAlert("请填写地址和电话！"); return; }
            if (TaxBankAccount.length <= 0) { jAlert("请填写开户银行和账号！"); return; }
        }
        if (InvoiceType == null && (TaxName.length > 0 || TaxNo.length > 0 || TaxAddTel.length > 0 || TaxBankAccount.length > 0)) { jAlert("请选择发票类型！"); return; }
        $("#InvoiceInfoTemporary").val(JSON.stringify({ InvoiceType: InvoiceType, TaxName: TaxName, TaxNo: TaxNo, TaxAddTel: TaxAddTel, TaxBankAccount: TaxBankAccount }));//暂存数据
        $("#orderMainInvoiceInfoWindow").data("kendoWindow").close();
        hasChangedMain = true;
    },
    ClearRadioStatus: function () {
        //发票类型 取消radio选中状态
        document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = false;
        document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = false;
        $("#orderMainInvoiceInfoWindow span.field-validation-error").css("display", "none");
    },
    Get: function () {
        return {
            Id: $("#ResInvoiceInfo_Id").val(),
            InvoiceType: $("[name='ResInvoiceInfo.InvoiceType']:checked").val(),
            TaxName: $.trim($("#ResInvoiceInfo_TaxName").val()),
            TaxNo: $.trim($("#ResInvoiceInfo_TaxNo").val()),
            TaxAddTel: $.trim($("#ResInvoiceInfo_TaxAddTel").val()),
            TaxBankAccount: $.trim($("#ResInvoiceInfo_TaxBankAccount").val()),
            OriginResInvoiceInfoJsonData: $.trim($("#ResInvoiceInfo_OriginResInvoiceInfoJsonData").val()),
        };
    },
    Set: function (data) {
        if (data != null && data != undefined) {
            $("#ResInvoiceInfo_Id").val(data.Id);
            $("#ResInvoiceInfo_TaxName").val(data.TaxName);
            $("#ResInvoiceInfo_TaxNo").val(data.TaxNo);
            $("#ResInvoiceInfo_TaxAddTel").val(data.TaxAddTel);
            $("#ResInvoiceInfo_TaxBankAccount").val(data.TaxBankAccount);
            $("#ResInvoiceInfo_OriginResInvoiceInfoJsonData").val(data.OriginResInvoiceInfoJsonData);
            var invoiceType = data.InvoiceType;
            if (invoiceType == null || invoiceType == undefined) {
                document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = false;
                document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = false;
            } else if (invoiceType == true) {
                document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = true;
            } else if (invoiceType == false) {
                document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = true;
            }
        }
        else {
            $("#ResInvoiceInfo_Id").val("");
            $("#ResInvoiceInfo_TaxName").val("");
            $("#ResInvoiceInfo_TaxNo").val("");
            $("#ResInvoiceInfo_TaxAddTel").val("");
            $("#ResInvoiceInfo_TaxBankAccount").val("");
            $("#ResInvoiceInfo_OriginResInvoiceInfoJsonData").val("");
            document.getElementById("ResInvoiceInfo_InvoiceType_True").checked = false;
            document.getElementById("ResInvoiceInfo_InvoiceType_False").checked = false;
        }
    },
    RadioChange: function () {
        var InvoiceType = $("[name='ResInvoiceInfo.InvoiceType']:checked").val();
        if (InvoiceType == "True") {
            $("#orderMainInvoiceInfoWindow span.field-validation-error").css("display", "inline");
        } else if (InvoiceType == "False") {
            $("#orderMainInvoiceInfoWindow span.field-validation-error").css("display", "none");
            $("[data-valmsg-for='ResInvoiceInfo.TaxName']").css("display", "inline");
        } else {
            $("#orderMainInvoiceInfoWindow span.field-validation-error").css("display", "none");
        }
    },
};