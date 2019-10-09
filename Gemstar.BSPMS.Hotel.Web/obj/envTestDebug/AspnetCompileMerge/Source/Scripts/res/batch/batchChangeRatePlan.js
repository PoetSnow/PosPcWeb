//价格计划 修改房价 模板
var batchChangeRatePlanWindow = {
    para: {
        arriveDateId: "#arriveDate",
        depDateId: "#depDate",
        rateCodeId: "#rateCode"
    },
    selectedRoomTypeDataItem:function(){
        var grid = $("#grid").data("kendoGrid");
        var selectedRows = grid.select();
        if (selectedRows.length > 0) {
            var dataItem = grid.dataItem(selectedRows[0]);
            return dataItem;
        }
        return null;
    },
    Initialization: function () {
        //初始化弹框
        $("#batchChangeRatePlanWindow").kendoWindow({
            width: "427px",
            title: "调整价格",
            visible: false,
            modal: true,
            actions: ["Close"],
            close: function () { document.getElementById("checkAllRatePlan").checked = false; $("#txtConstructor").val(""); }
        });
        $("#editPriceButton").click(function (e) { batchChangeRatePlanWindow.Edit(e); });
        $("#savePriceButton").click(function (e) { batchChangeRatePlanWindow.Save(e); });
    },
    Open: function () {
        //打开房间弹框
        var dataItem = batchChangeRatePlanWindow.selectedRoomTypeDataItem();
        if (dataItem == null) { jAlert("请先选择房间类型"); return; }
        var arriveDateValue = $(batchChangeRatePlanWindow.para.arriveDateId).data("kendoDateTimePicker").value();
        var depDateValue = $(batchChangeRatePlanWindow.para.depDateId).data("kendoDateTimePicker").value();
        if (!(arriveDateValue != null && depDateValue != null)) { jAlert("请先选择抵店时间和离店时间", "选择时间"); return; }
        if (depDateValue <= arriveDateValue) { jAlert("离店时间必须大于抵店时间"); return; }

        var nowDate = new Date(CustomerCommonValues.businessDate.replace(/-/g, "/") + " 00:00:00");
        var beginDate = (CustomerCommonValues.isCheckIn == "1") ? nowDate : new Date(arriveDateValue.ToDateString().replace(/-/g, "/") + " 00:00:00");
        var endDate = new Date(depDateValue.ToDateString().replace(/-/g, "/") + " 00:00:00");
        var trHtml = "";
        var ratePlanInfos = dataItem.rate.split(',');
        var index = 0;
        var isSameDay = (beginDate.valueOf() == endDate.valueOf());
        while (beginDate <= endDate) {
            var dateStr = beginDate.ToDateString();
            var priceStr = "";
            if (ratePlanInfos != null && ratePlanInfos.length > 0) {
                var length = ratePlanInfos.length;
                if (index < ratePlanInfos.length) {
                    priceStr = ratePlanInfos[index];
                }
            }
            var disabled = (beginDate < nowDate) ? " disabled=\"disabled\" style=\"background-color: #f5f5f5;\" " : " ";
            var checkId = "checkRatePlan_" + index;
            if ((beginDate.valueOf() <= endDate.valueOf()) || isSameDay) {
                trHtml += "<tr><td><input type=\"checkbox\" class=\"k-checkbox\" name=\"checkRatePlan\" id=\"" + checkId + "\"" + disabled + "/><label class=\"k-checkbox-label\" for=\"" + checkId + "\"></label></td><td class=\"ratePlanDate\">" + dateStr + "</td><td><input type=\"text\" id=\"ratePlanPrice\" value=\"" + priceStr + "\" class=\"k-textbox\"" + disabled + "/></td></tr>";
            }
            beginDate = new Date(beginDate.valueOf() + 1 * 24 * 60 * 60 * 1000);
            index++;
        }
        $("#ratePlanTbody").empty().html(trHtml);
        $("#batchChangeRatePlanWindow").data("kendoWindow").center().open();
        batchChangeRatePlanWindow.Integral.Chcek();
    },
    Save: function () {
        var priceOrIntegral = batchChangeRatePlanWindow.Integral.GetText();
        //保存房价
        var prices = [];
        var isReturn = false;
        $("#ratePlanTbody").find("tr").each(function (index, tr) {
            var $tr = $(tr);
            var priceDate = $.trim($tr.find("td.ratePlanDate").text());
            var price = $.trim($tr.find("input#ratePlanPrice").val());

            if (priceDate.length <= 0) { jAlert("请选择时间"); isReturn = true; return false; }
            var priceDateTimeStamp = Date.parse(priceDate);
            if (!(priceDateTimeStamp != null && priceDateTimeStamp != undefined && !isNaN(priceDateTimeStamp) && priceDateTimeStamp > 0)) { jAlert("请选择时间"); isReturn = true; return false; }
            if (price.length < 0 || isNaN(price)) { jAlert("请输入" + priceOrIntegral); isReturn = true; return false; }
            var priceFloat = parseFloat(price);
            if (!(priceFloat != null && priceFloat != undefined && !isNaN(priceFloat) && priceFloat >= 0)) { jAlert("请输入" + priceOrIntegral); isReturn = true; return false; }

            prices.push(price);
        });
        if (!isReturn) {
            batchChangeRatePlanWindow.Set(prices, true);
            $("#batchChangeRatePlanWindow").data("kendoWindow").close();
        }
    },
    Edit: function () {
        var priceOrIntegral = batchChangeRatePlanWindow.Integral.GetText();
        //统一修改房价
        var $checked = $("#ratePlanTbody").find("input[name='checkRatePlan']:checked");
        if ($checked.length == 0) { jAlert("请先选择要调整" + priceOrIntegral + "记录", "前往选择"); return; }
        var editedPriceValue = $("#editedPrice").data("kendoNumericTextBox").value();
        if (!(editedPriceValue != null && editedPriceValue > 0)) { jAlert("请先输入要调整为的" + priceOrIntegral, "前往输入"); return; }

        $checked.each(function (index, item) {
            var $tr = $(item).parents("tr").find("input#ratePlanPrice").val(editedPriceValue);
        });
    },
    SelectAll: function () {
        //全选
        var checked = $("input#checkAllRatePlan")[0].checked;
        $("#ratePlanTbody").find("input[name='checkRatePlan']").each(function (index, item) {
            if (!item.disabled) {
                item.checked = checked;
            }
        });
    },
    Set: function (data, isEdit) {
        var para = batchChangeRatePlanWindow.para;
        var roomTypeDataItem = batchChangeRatePlanWindow.selectedRoomTypeDataItem();
        var priceStr = data.join(',');
        roomTypeDataItem.rate = priceStr;

        var grid = $("#grid").data("kendoGrid");
        var selectedRows = grid.select();
        if (selectedRows.length > 0) {
            var tr = selectedRows[0];
            $(tr).find(".batchRoomTypeRate").text(priceStr);
        }
    },
    Integral: {//积分换房功能，此处替换文字信息和提示信息
        Chcek: function () {
            var isIntegral = batchChangeRatePlanWindow.Integral.Get();
            batchChangeRatePlanWindow.Integral.Set(isIntegral);
        },
        Set: function (isIntegral) {
            var oldText = "积分";
            var newText = "价格";
            if (isIntegral == true) {
                oldText = "价格";
                newText = "积分";
            }
            var obj = $("#batchChangeRatePlanWindow");
            obj.find("th.k-header:contains('" + oldText + "')").each(function () {
                $(this).text($(this).text().replace(oldText, newText));//原价格,执行价格
            });

            //var relationUpdateRate = obj.find("[for='IsRelationUpdateAllRoonTypeRatePlan']");
            //relationUpdateRate.text(relationUpdateRate.text().replace(oldText, newText));//关联更新价格
            //relationUpdateRate.attr("title", relationUpdateRate.attr("title").replace("订单的" + oldText, "订单的" + newText));//title 订单的价格

            obj.find("[for='editedPrice']").text(obj.find("[for='editedPrice']").text().replace(oldText, newText));//选中的价格调整为
            obj.find("#editPriceDivMsg").text(obj.find("#editPriceDivMsg").text().replace(isIntegral ? "调价" : "调积分", isIntegral ? "调积分" : "调价"));//本价格代码不允许调价
            obj.find("#ratePlanTbodyTrSeparator td").text(obj.find("#ratePlanTbodyTrSeparator td").text().replace(isIntegral ? "房价" : "积分", isIntegral ? "积分" : "房价"));//以上早期营业日房价不允调整
            $("#batchChangeRatePlanWindow_wnd_title").text($("#batchChangeRatePlanWindow_wnd_title").text().replace(oldText, newText));
        },
        Get: function () {
            var rateCodeValue = $("#rateCode").data("kendoDropDownList").value();
            if ($.trim(rateCodeValue).length > 0) {
                var rateCodeIsUseScore = $("#rateCodeIsUseScore").val();
                if ($.trim(rateCodeIsUseScore).length > 0 && rateCodeIsUseScore.toLowerCase() == "true") {
                    return true;
                }
            }
            return false
        },
        GetText: function () {
            var isIntegral = batchChangeRatePlanWindow.Integral.Get();
            if (isIntegral == true) {
                return "积分";
            }
            return "价格";
        },
    },
};