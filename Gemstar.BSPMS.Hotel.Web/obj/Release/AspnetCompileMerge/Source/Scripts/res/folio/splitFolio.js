//拆账JS
var orderSplitFolioWindow = {
    Initialization: function () {
        var windowObj = $("#orderSplitFolioWindow");
        var windowObjKendo = windowObj.data("kendoWindow");
        if (windowObjKendo == null || windowObjKendo == undefined) {
            windowObj.kendoWindow({
                title: "拆账",
                width: "850px",
                visible: false,
                modal: true,
                actions: ["Close"],
                close: function () {
                    orderSplitFolioWindow.Others.ClearTbody();
                    $("#orderSplitFolioWindow_Tbody").empty();
                    folioMoreDivClose();
                }
            });
            windowObjKendo = windowObj.data("kendoWindow");
        }
        $("#orderSplitFolio_submit").unbind("click").click(function (e) { orderSplitFolioWindow.Save(e); });
        $("#orderSplitFolio_cancel").unbind("click").click(function (e) { orderSplitFolioWindow.Cancel(e); });
        $("[name='orderSplitFolio_quantity']").unbind("click").click(function (e) { orderSplitFolioWindow.Others.SetorQuantity(e); });
        $("#orderSplitFolio_clearAmount").unbind("click").click(function (e) { orderSplitFolioWindow.Others.ClearAmount(e); });
        $("#orderSplitFolio_averageAmount").unbind("click").click(function (e) { orderSplitFolioWindow.Others.AverageAmount(e); });
        return windowObjKendo;
    },
    Open: function () {
        //1.客账列表选项
        var selectedState = $("input[name='folioStatu']:checked").val();
        if (selectedState != "1") {
            folioMoreDivClose();
            jAlert("只有在未结状态下才可以进行拆账操作");
            return;
        }
        //2.勾选账务
        var selectedFolioIds = [];
        $("input.folioFolioRowCheck:checked").each(function (index, obj) {
            selectedFolioIds.push($(obj).val());
        });
        if (selectedFolioIds.length == 0) {
            folioMoreDivClose();
            jAlert("请勾选要拆账的明细账");
            return;
        }
        //3.获取账务信息
        var select_list = orderSplitFolioWindow.Others.GetFolioList(selectedFolioIds);
        if (select_list == null || select_list == undefined || select_list.length <= 0) { return; }
        //4.清空内容
        orderSplitFolioWindow.Others.ClearTbody();
        //5.生成HTML
        var trHtml = [];
        var td = "<td>{序号}</td><td>{房号}</td><td>{项目}</td><td>{金额}</td>";
        var tdTextBox = "<td><input data-col=\"{列}\" style=\"width:100px;\" /></td>";
        var rowNumber = 1;
        $.each(select_list, function (index, item) {
            var amount = 0;
            if ($.trim(item.DebitAmount).length > 0) { amount = item.DebitAmount; }
            if ($.trim(item.CreditAmount).length > 0) { amount = item.CreditAmount; }
            if (amount > 0) {
                var tr = ("<tr data-transid=\"{0}\" data-amount=\"{1}\">").replace("{0}", item.Transid).replace("{1}", amount);
                tr += td.replace("{序号}", rowNumber).replace("{房号}", item.RoomNo).replace("{项目}", item.ItemName).replace("{金额}", amount);
                
                tr += tdTextBox.replace("{列}", 1);
                tr += tdTextBox.replace("{列}", 2);
                tr += tdTextBox.replace("{列}", 3);
                tr += tdTextBox.replace("{列}", 4);
                tr += "</tr>";
                trHtml.push(tr);
                rowNumber++;
            }
        });
        $("#orderSplitFolioWindow_Tbody").empty().html(trHtml.join(""));
        //6.生成kendo
        orderSplitFolioWindow.Others.SetTbody();
        //7.使用份数
        orderSplitFolioWindow.Others.SetorQuantity();
        //8.弹框
        var windowObjKendo = orderSplitFolioWindow.Initialization();
        windowObjKendo.center().open();
    },
    Save: function () {
        var isCheck = true;
        var datalist = [];

        var selectedQuantity = parseInt($("input[name='orderSplitFolio_quantity']:checked").val());
        $("#orderSplitFolioWindow_Tbody tr").each(function (index, item) {
            var trObj = $(item);
            var transid = trObj.attr("data-transid")
            var amount = parseFloat(trObj.attr("data-amount"));
            var amount1 = trObj.find("[data-col='1']:input").data("kendoNumericTextBox").value();
            var amount2 = trObj.find("[data-col='2']:input").data("kendoNumericTextBox").value();
            var amount3 = trObj.find("[data-col='3']:input").data("kendoNumericTextBox").value();
            var amount4 = trObj.find("[data-col='4']:input").data("kendoNumericTextBox").value();

            var title = "第" + (index + 1) + "行数据，";
            if ($.trim(transid).length <= 0) {
                isCheck = false;
                jAlert(title + "账务异常，请联系管理员！");
                return false;
            }
            if (amount == null || amount == undefined || amount == 0) {
                isCheck = false;
                jAlert(title + "金额异常，请联系管理员！");
                return false;
            }
            if (amount1 == null || amount1 == 0) {
                isCheck = false;
                jAlert(title + "请填写金额1！");
                return false;
            }
            if (amount2 == null || amount2 == 0) {
                isCheck = false;
                jAlert(title + "请填写金额2！");
                return false;
            }
            if (selectedQuantity >= 3) {
                if (amount3 == null || amount3 == 0) {
                    isCheck = false;
                    jAlert(title + "请填写金额3！");
                    return false;
                }
            }
            if (selectedQuantity >= 4) {
                if (amount4 == null || amount4 == 0) {
                    isCheck = false;
                    jAlert(title + "请填写金额4！");
                    return false;
                }
            }

            var amountSum = 0;
            if (selectedQuantity == 2) {
                amountSum = amount1 + amount2;
            }
            else if (selectedQuantity == 3) {
                amountSum = amount1 + amount2 + amount3;
            }
            else if (selectedQuantity == 4) {
                amountSum = amount1 + amount2 + amount3 + amount4;
            }
            else {
                isCheck = false;
                jAlert("选择份数不正确，请联系管理人员！");
                return false;
            }
            amountSum = amountSum.toFixed(2);
            if (amount != amountSum || amountSum == 0) {
                isCheck = false;
                jAlert(title + "请检查并修改金额！");
                return false;
            }

            datalist.push({
                TransId: transid,
                Amount1: amount1,
                Amount2: amount2,
                Amount3: selectedQuantity >= 3 ? amount3 : 0,
                Amount4: selectedQuantity >= 4 ? amount4 : 0,
            });
        });
        if (isCheck == false) {
            return;
        }
        if (datalist == null || datalist.length <= 0) {
            jAlert("没有要拆分的记录！");
            return;
        }
        var bill = {
            Amount1: $("#orderSplitFolio_bill1").data("kendoDropDownList").value(),
            Amount2: $("#orderSplitFolio_bill2").data("kendoDropDownList").value(),
            Amount3: $("#orderSplitFolio_bill3").data("kendoDropDownList").value(),
            Amount4: $("#orderSplitFolio_bill4").data("kendoDropDownList").value(),
        };
        var msg = ("共{0}条账务，每条账务拆分{1}份，确定拆账？").replace("{0}", datalist.length).replace("{1}", selectedQuantity);
        jConfirm(msg, "确定", "返回", function (confirmed) {
            if (confirmed) {
                orderSplitFolioWindow.Post(datalist, bill, selectedQuantity);
            }
        });
    },
    Post: function (folioList, bill, splitCount) {
        $.post(FolioCommonValues.SplitFolio, { FolioList: folioList, Bill: bill, SplitCount: splitCount }, function (result) {
            if (result != null && result.Success) {
                folioQueryButton_clicked();
                orderSplitFolioWindow.Cancel();
                jAlert("拆账成功！");
            }
            else {
                ajaxErrorHandle(result);
            }
        });
    },
    Cancel: function () {
        //取消
        $("#orderSplitFolioWindow").data("kendoWindow").close();
    },
    Others: {
        ClearBill: function () {
            $("#orderSplitFolio_quantity2")[0].checked = true;
        },
        SetBill: function () {
            var obj1 = $("#orderSplitFolio_bill1");
            if (obj1 == null || obj1 == undefined) { return; }
            var objKendo1 = obj1.data("kendoDropDownList");
            if (objKendo1 == null && objKendo1 == undefined) {
                var obj = $(".orderSplitFolio_bill");
                if (obj == null || obj == undefined) { return; }
                obj.kendoDropDownList({
                    dataSource: [
                      { id: "A", name: "A账单" },
                      { id: "B", name: "B账单" },
                      { id: "C", name: "C账单" },
                      { id: "D", name: "D账单" },
                      { id: "E", name: "E账单" },
                      { id: "F", name: "F账单" },
                      { id: "G", name: "G账单" },
                      { id: "H", name: "H账单" },
                      { id: "I", name: "I账单" },
                      { id: "J", name: "J账单" },
                      { id: "K", name: "K账单" },
                      { id: "L", name: "L账单" },
                      { id: "M", name: "M账单" },
                      { id: "N", name: "N账单" },
                      { id: "O", name: "O账单" },
                      { id: "P", name: "P账单" },
                      { id: "Q", name: "Q账单" },
                      { id: "R", name: "R账单" },
                      { id: "S", name: "S账单" },
                      { id: "T", name: "T账单" },
                      { id: "U", name: "U账单" },
                      { id: "V", name: "V账单" },
                      { id: "W", name: "W账单" },
                      { id: "X", name: "X账单" },
                      { id: "Y", name: "Y账单" },
                      { id: "Z", name: "Z账单" }
                    ],
                    autoWidth: true,
                    dataTextField: "name",
                    dataValueField: "id",
                    optionLabel: " "
                });
            }
            else {
                $("#orderSplitFolio_bill1").data("kendoDropDownList").select(0);
                $("#orderSplitFolio_bill2").data("kendoDropDownList").select(0);
                $("#orderSplitFolio_bill3").data("kendoDropDownList").select(0);
                $("#orderSplitFolio_bill4").data("kendoDropDownList").select(0);
            }
        },
        ClearTbody: function () {
            orderSplitFolioWindow.Others.ClearBill();
            var list = $("#orderSplitFolioWindow_Tbody [data-col]:input");
            if (list == null || list == undefined || list.length <= 0) { return; }
            $.each(list, function (index, item) {
                var itemObj = $(item).data("kendoNumericTextBox");
                if (itemObj != null && itemObj != undefined) {
                    itemObj.destroy();
                }
            });
        },
        SetTbody: function () {
            orderSplitFolioWindow.Others.SetBill();
            $("#orderSplitFolioWindow_Tbody [data-col]:input").kendoNumericTextBox();
        },
        SetorQuantity: function () {
            var selectedQuantity = $("input[name='orderSplitFolio_quantity']:checked").val();
            if (selectedQuantity == 2) {
                orderSplitFolioWindow.Others.EnableCol(4, false);
                orderSplitFolioWindow.Others.EnableCol(3, false);
            }
            else if (selectedQuantity == 3) {
                orderSplitFolioWindow.Others.EnableCol(4, false);
                orderSplitFolioWindow.Others.EnableCol(3, true);
            }
            else if (selectedQuantity == 4) {
                orderSplitFolioWindow.Others.EnableCol(4, true);
                orderSplitFolioWindow.Others.EnableCol(3, true);
            }
            orderSplitFolioWindow.Others.AverageAmount();
        },
        EnableCol: function (colNumber, enable) {
            var list = $("#orderSplitFolioWindow_Tbody [data-col='" + colNumber + "']:input");
            if (list == null || list == undefined || list.length <= 0) { return; }
            $.each(list, function (index, item) {
                var itemObj = $(item).data("kendoNumericTextBox");
                if (itemObj != null && itemObj != undefined) {
                    if (enable == false) {
                        itemObj.value(null);
                    }
                    itemObj.enable(enable);
                }
            });
            var obj = $("#orderSplitFolio_bill" + colNumber);
            if (obj == null || obj == undefined) { return; }
            var objKendo = obj.data("kendoDropDownList");
            if (objKendo == null || objKendo == undefined) { return; }
            if (enable == false) {
                objKendo.select(0);
            }
            objKendo.enable(enable);
        },
        AverageAmount: function (e) {
            var selectedQuantity = parseInt($("input[name='orderSplitFolio_quantity']:checked").val());
            $("#orderSplitFolioWindow_Tbody tr").each(function (index, item) {
                var trObj = $(item);
                var amount1 = null;
                var amount2 = null;
                var amount3 = null;
                var amount4 = null;
                
                var amount = parseFloat(trObj.attr("data-amount"));
                var temp = parseInt((amount / selectedQuantity) * 100) / 100;
                if (selectedQuantity >= 2) {
                    amount1 = temp;
                    amount2 = temp;
                }
                if (selectedQuantity >= 3) {
                    amount3 = temp;
                }
                if (selectedQuantity >= 4) {
                    amount4 = temp;
                }
                var amount1_add = (amount - (amount1 == null ? 0 : amount1) - (amount2 == null ? 0 : amount2) - (amount3 == null ? 0 : amount3) - (amount4 == null ? 0 : amount4));

                trObj.find("[data-col='1']:input").data("kendoNumericTextBox").value(amount1 + amount1_add);
                trObj.find("[data-col='2']:input").data("kendoNumericTextBox").value(amount2);
                trObj.find("[data-col='3']:input").data("kendoNumericTextBox").value(amount3);
                trObj.find("[data-col='4']:input").data("kendoNumericTextBox").value(amount4);
            });
        },
        ClearAmount: function (e) {
            var list = $("#orderSplitFolioWindow_Tbody [data-col]:input");
            if (list == null || list == undefined || list.length <= 0) { return; }
            $.each(list, function (index, item) {
                var itemObj = $(item).data("kendoNumericTextBox");
                if (itemObj != null && itemObj != undefined) {
                    itemObj.value(null);
                }
            });
        },
        GetFolioList: function (selectedFolioIds) {
            var resultlist = [];
            if (selectedFolioIds == null || selectedFolioIds == undefined || selectedFolioIds.length <= 0) {
                return resultlist;
            }
            //获取所有账务信息
            var grid = $("#gridFolioFolio").data("kendoGrid");
            if (grid == null || grid == undefined) { return resultlist; }
            var grid_list = grid.dataItems();
            if (grid_list == null || grid_list == undefined || grid_list.length <= 0) { return resultlist; }
            //获取勾选账务信息
            $.each(selectedFolioIds, function (index, transid) {
                var info = orderSplitFolioWindow.Others.GetFolioInfo(grid_list, transid);
                if (info != null && info != undefined) {
                    resultlist.push(info);
                }
            });
            return resultlist;
        },
        GetFolioInfo: function (list, transid) {
            var resultInfo = null;
            $.each(list, function (index, item) {
                if (transid == item.Transid) {
                    resultInfo = item;
                    return true;
                }
            });
            return resultInfo;
        },
    },
};