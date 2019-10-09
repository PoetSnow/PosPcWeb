var queryInterval = [];

function PayBase() {

}
//创建具体付款动作实例的工厂方法
createPayByAction = function (action, options) {
    if (!action) {
        return undefined;
    }
    if (action == "no") {
        return undefined;
    }
    var firstLetter = action.charAt(0);
    var actionValue = firstLetter.toUpperCase() + action.slice(1);

    var objStr = "new Pay" + actionValue + "();";
    var obj = eval(objStr);

    var _defaultOptions = {
        selectorForInit: "tr.folioAddAmountForC"//初始化界面时的定位元素，新元素会加在此元素的后面
        , selectorForItem: "#folioAddItem"//付款方式项目元素，用于获取付款方式信息
        , selectorForSaveButton: "#folioAddSave"//保存按钮元素
        , selectorForResultDiv: "#folioPayResultDiv"//支付结果显示元素
        , selectorForWindow: "#folioAddFolioDiv"//支付窗口元素
        , urlForQueryPayStatus: "/PayManage/Pay/QueryFolioPayStatu"//查询支付结果的url地址
        , urlForAutoCompleteCorp: "/PayManage/Pay/AutoCompleteCorp"//合约单位自动完成
    };
    var empty = {};
    obj.settings = jQuery.extend(empty, _defaultOptions, options);
    return obj;
};
//初始化界面的方法
PayBase.prototype.PayInit = function () { };
//提交前的检测方法
PayBase.prototype.PayCheck = function () { return true; };
//设置各自的支付参数方法
PayBase.prototype.PaySetPara = function (model) { };
//支付提交保存成功后的后续方法
PayBase.prototype.PayAfterSave = function (data) { };

//获取支付产品说明，优先取传递参数中的支付产品说明，如果没有值，则取付款方式的名称
PayBase.prototype.PayGetBody = function (obj, model) {
    var result = model.PayBody;
    if (!model.PayBody) {
        var kendoAddItem = $(obj.settings.selectorForItem).data("kendoDropDownList");
        if (kendoAddItem) {
            result = kendoAddItem.text();
        }
    }
    return result;
}
//禁用保存按钮
PayBase.prototype.PayDisableSaveBtn = function (obj) {
    var kendoSaveBtn = $(obj.settings.selectorForSaveButton).data("kendoButton");
    if (kendoSaveBtn) {
        kendoSaveBtn.enable(false);
    } else {
        $(obj.settings.selectorForSaveButton).attr("disabled", "disabled");
    }
}
//重新设置窗口居中
PayBase.prototype.PayCenterWindow = function (obj) {
    var kendoWindow = $(obj.settings.selectorForWindow).data("kendoWindow");
    if (kendoWindow) {
        kendoWindow.center();
    }
};

//清除支付查询定时器
PayBase.prototype.CannelPayWindow = function (data) { };



