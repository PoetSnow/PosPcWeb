function PayCorp() {
    PayBase.apply(this);
    //合约单位自动完成查询参数
    this.folioPayCorp_autoCompleteSetPara = function () {
        return { keyword: $("#folioCorpAutoComplete").val() };
    }
    //合约单位自动完成选择项目
    this.folioPayCorp_autoCompleteSelected = function (e) {
        var selectedLi = e.item[0];
        var index = -1;
        var ul_lis = e.sender.ul[0].childNodes;
        var count = ul_lis.length;
        for (var i = 0; i < count; i++) {
            var li = ul_lis[i];
            if (li == selectedLi) {
                index = i;
                break;
            }
        }
        var itemId = "";
        var signers = [];
        if (index >= 0) {
            var dataItem = e.sender.dataItem(index);
            itemId = dataItem["CompanyId"];
            signers = dataItem["Signers"];
        }
        $("#lblProfileId").val(itemId);
        PayCorpSigners(signers);
    }
}
function PayCorpSigners(signers) {
    //判断是否有签单人，如果有，则只能从已经维护好的签单人中下拉选择，否则可以随意输入
    if (signers && signers.length > 0) {
        $("#folioCorpSignInput").val("0");
        $("#corpSignInputTr").hide();
        $("#corpSignSelectTr").show();
        var signerData = [];
        for (var i = 0; i < signers.length; i++) {
            signerData.push(signers[i]["Name"]);
        }
        var dataSource = new kendo.data.DataSource({
            data: signerData
        });
        $("#folioCorpSignPersonSelect").data("kendoDropDownList").setDataSource(dataSource);
    } else {
        $("#folioCorpSignInput").val("1");
        $("#corpSignInputTr").show();
        $("#corpSignSelectTr").hide();
        var dataSource = new kendo.data.DataSource({
            data: []
        });
        $("#folioCorpSignPersonSelect").data("kendoDropDownList").setDataSource(dataSource);
    }
}
PayCorp.prototype = Object.create(PayBase.prototype);
PayCorp.prototype.constructor = PayCorp;
//初始化界面的方法
PayCorp.prototype.PayInit = function () {
    var htmlStr = '<tr class="folioAddPayment">' +
        '<td class="textright">合约单位</td>' +
        '<td colspan="3"><input type="text" style="width:100%" class="k-textbox" id="folioCorpAutoComplete" name="folioCorpAutoComplete" placeholder="输入拼音码、代码、名称来查询" onchange="getContractUnit(this,1)"/><input type="hidden" id="lblProfileId" /><input type="hidden" id="folioCorpSignInput" value="1" /></td>' +
        '</tr>' +
        '<tr class="folioAddPayment" id="corpSignInputTr">' +
        '<td class="textright">签单人</td>' +
        '<td colspan="3"><input type="text" style="width:100%" class="k-textbox" id="folioCorpSignPerson" name="folioCorpSignPerson" /></td>' +
        '</tr>' +
        '<tr class="folioAddPayment" id="corpSignSelectTr" style="display:none;">' +
        '<td class="textright">签单人</td>' +
        '<td colspan="3"><input id="folioCorpSignPersonSelect" style="width:100%" /></td>' +
        '</tr>'
        + '<tr class="folioAddPayment"><td class="textright">　联系人：</td><td><span id="lblContactor"></span></td><td class="textright">联系电话：</td><td><span id="lblTel"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">　合同号：</td><td><span id="lblCttno"></span></td><td class="textright">　业务员：</td><td><span id="lblSales"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">　有效期：</td><td><span id="lblMbrexpired"></span></td><td class="textright">　余　额：</td><td><span id="lblBalance"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">信用等级：</td><td><span id="lblCreditlevel"></span></td><td class="textright">信用金额：</td><td><span id="lblApprovalAmt"></span></td></tr>'
        + '<tr class="folioAddPayment"><td class="textright">　备　注：</td><td><span id="lblRemark"></span></td><td class="textright"></td><td><span id="lbl"></span></td></tr>';
    $(htmlStr).insertAfter($(this.settings.selectorForInit));
    $("#folioCorpAutoComplete").kendoAutoComplete({
        "select": this.folioPayCorp_autoCompleteSelected,
        "dataSource": { "transport": { "read": { "url": this.settings.urlForAutoCompleteCorp, "data": this.folioPayCorp_autoCompleteSetPara }, "prefix": "" }, "serverFiltering": true, "filter": [], "schema": { "errors": "Errors" } },
        "dataTextField": "Name",
        "dataBound": function () { $("#lblProfileId").val(""); }
    });
    $("#folioCorpSignPersonSelect").kendoDropDownList({
        dataSource: {
            data: []
        },
        animation: false
    });
};

//提交前的检测方法
PayCorp.prototype.PayCheck = function () {
    var folioCorpIdValue = $("#lblProfileId").val();
    if (!folioCorpIdValue) {
        layer.alert("请选择要挂账的合约单位", { title: "快点云Pos提示", skin: 'err'  });
        return false;
    }
    var signInput = $("#folioCorpSignInput").val();
    if (signInput == "1") {
        var folioCorpSignPersonValue = $("#folioCorpSignPerson").val();
        if (!folioCorpSignPersonValue) {
            layer.alert("请输入签单人姓名", { title: "快点云Pos提示", skin: 'err'  });
            return false;
        }
    } else {
        var folioCorpSignPersonValue = $("#folioCorpSignPersonSelect").data("kendoDropDownList").value();
        if (!folioCorpSignPersonValue) {
            layer.alert("请选择签单人", { title: "快点云Pos提示", skin: 'err'  });
            return false;
        }
    }
    return true;
};

//设置各自的支付参数方法
PayCorp.prototype.PaySetPara = function (model) {
    var signInput = $("#folioCorpSignInput").val();
    var signPerson = $("#folioCorpSignPerson").val();
    if (signInput == "0") {
        signPerson = $("#folioCorpSignPersonSelect").data("kendoDropDownList").value();
    }
    var para = { corpId: $("#lblProfileId").val(), signPerson: signPerson, amount: model.FolioAmount, outletCode: "01", invno: model.FolioInvoNo, remark: model.FolioRemark, regid: model.FolioRegId };
    model.FolioItemActionJsonPara = JSON.stringify(para);

    var folioCorpAutoCompleteObj = $("#folioCorpAutoComplete").data("kendoAutoComplete");
    if (folioCorpAutoCompleteObj != null && folioCorpAutoCompleteObj != undefined) {
        var entity = folioCorpAutoCompleteObj.dataItem();
        if (entity != null && entity != undefined) {
            if (entity.CompanyId == para.corpId) {
                model.FolioRemark += "[合约单位:" + $.trim(entity.Name) + "]";
            }
        } else {
            if ($.trim(folioCorpAutoCompleteObj.value()) != "") {
                model.FolioRemark += "[合约单位:" + $.trim(folioCorpAutoCompleteObj.value()) + "]";
            }
        }
    }
};
