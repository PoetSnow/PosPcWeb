//价格计划 修改房价 模板
var permanentRoom_OrderDetailRatePlanEditWindow = {
    SetConstructor: function (rateCodeId, roomTypeId, arriveDateId, depDateId, roomPriceId, roomPriceJsonId, arrBsnsDateId) {
        //设置构造函数
        var constructor = $("#txtConstructor");
        constructor.val("");
        var para = {
            rateCodeId: rateCodeId,
            roomTypeId: roomTypeId,
            arriveDateId: arriveDateId,
            depDateId: depDateId,
            roomPriceId: roomPriceId,
            roomPriceJsonId: roomPriceJsonId,
            arrBsnsDateId: arrBsnsDateId,
        };
        if (para.rateCodeId == null || para.rateCodeId == undefined || para.rateCodeId.length <= 0) {
            return false;
        }
        if (para.roomTypeId == null || para.roomTypeId == undefined || para.roomTypeId.length <= 0) {
            return false;
        }
        if (para.arriveDateId == null || para.arriveDateId == undefined || para.arriveDateId.length <= 0) {
            return false;
        }
        if (para.depDateId == null || para.depDateId == undefined || para.depDateId.length <= 0) {
            return false;
        }
        if (para.roomPriceId == null || para.roomPriceId == undefined || para.roomPriceId.length <= 0) {
            return false;
        }
        if (para.roomPriceJsonId == null || para.roomPriceJsonId == undefined || para.roomPriceJsonId.length <= 0) {
            return false;
        }
        if (para.arrBsnsDateId == null || para.arrBsnsDateId == undefined || para.arrBsnsDateId.length <= 0) {
            return false;
        }
        constructor.val(JSON.stringify(para));
        return true;
    },
    GetConstructor: function () {
        //获取构造函数
        var constructorJson = $.trim($("#txtConstructor").val());
        if (constructorJson != null && constructorJson != undefined && constructorJson.length > 0) {
            var para = JSON.parse(constructorJson);
            if (para != null && para != undefined) {
                try {
                    $(para.rateCodeId).data("kendoDropDownList").value();
                    $(para.roomTypeId).data("kendoDropDownList").value();
                    $(para.arriveDateId).data("kendoDatePicker").value();
                    $(para.depDateId).data("kendoDatePicker").value();
                } catch (e) {
                    return null;
                }
                var roomPrice = $(para.roomPriceId);
                if (!(roomPrice != null && roomPrice != undefined && roomPrice.length > 0)) {
                    return null;
                }
                var roomPriceJson = $(para.roomPriceJsonId);
                if (!(roomPriceJson != null && roomPriceJson != undefined && roomPriceJson.length > 0)) {
                    return null;
                }
                var arrBsnsDate = $(para.arrBsnsDateId);
                if (!(arrBsnsDate != null && arrBsnsDate != undefined && arrBsnsDate.length > 0)) {
                    return null;
                }
                return para;
            }
        }
        return null;
    },
    Initialization: function () {
        //初始化弹框
        $("#orderDetailRatePlanEditWindow").kendoWindow({
            width: "427px",
            title: "调整价格",
            visible: false,
            modal: true,
            actions: ["Close"],
            close: function () { document.getElementById("checkAllRatePlan").checked = false; $("#txtConstructor").val(""); }
        });
        $("#editPriceButton").unbind("click").click(function (e) { permanentRoom_OrderDetailRatePlanEditWindow.Edit(e); });
        $("#savePriceButton").unbind("click").click(function (e) { permanentRoom_OrderDetailRatePlanEditWindow.Save(e); });
    },
    Open: function () {
        if (!$("#orderDetailRatePlanEditWindow").data("kendoWindow")) { permanentRoom_OrderDetailRatePlanEditWindow.Initialization(); }
        var para = permanentRoom_OrderDetailRatePlanEditWindow.GetConstructor();
        if (para == null || para == undefined) { console.log("请先调用SetConstructor函数"); return; }
        //打开房间弹框
        var arriveDateValue = $(para.arriveDateId).data("kendoDatePicker").value();
        var depDateValue = $(para.depDateId).data("kendoDatePicker").value();
        if (!(arriveDateValue != null && depDateValue != null)) { jAlert("请先选择抵店时间和离店时间", "选择时间"); return; }
        if (depDateValue <= arriveDateValue) { jAlert("离店时间必须大于抵店时间"); return; }
        var arrBsnsDateValue = $(para.arrBsnsDateId).val();

        var nowDate = new Date(CustomerCommonValues.BusinessDate.replace(/-/g, "/") + " 00:00:00");
        var beginDate = ($.trim(arrBsnsDateValue) == "") ? new Date(arriveDateValue.ToDateString().replace(/-/g, "/") + " 00:00:00") : new Date(arrBsnsDateValue.replace(/-/g, "/") + " 00:00:00");
        var endDate = new Date(depDateValue.ToDateString().replace(/-/g, "/") + " 00:00:00");
        var trHtml = "";
        var ratePlanInfos = [];
        var roomPriceJson = $.trim($(para.roomPriceJsonId).val());
        if (roomPriceJson.length > 0) {
            ratePlanInfos = JSON.parse(roomPriceJson);
        }
        var index = 0;
        var isSameDay = (beginDate.valueOf() == endDate.valueOf());
        while (beginDate <= endDate) {
            var dateStr = beginDate.ToDateString();
            var priceStr = ""; var originPriceStr = "";
            if (ratePlanInfos != null && ratePlanInfos.length > 0) {
                var length = ratePlanInfos.length;
                for (var i = 0; i < length; i++) {
                    if (ratePlanInfos[i].Ratedate == dateStr) {
                        priceStr = ratePlanInfos[i].Price;
                        originPriceStr = ratePlanInfos[i].OriginPrice;
                        break;
                    }
                }
            }
            var disabled = (beginDate < nowDate) ? " disabled=\"disabled\" style=\"background-color: #f5f5f5;\" " : " ";
            var checkId = "checkRatePlan_" + index;
            if ((beginDate.valueOf() <= endDate.valueOf()) || isSameDay) {
                trHtml += "<tr><td><input type=\"checkbox\" class=\"k-checkbox\" name=\"checkRatePlan\" id=\"" + checkId + "\"" + disabled + "/><label class=\"k-checkbox-label\" for=\"" + checkId + "\"></label></td><td class=\"ratePlanDate\">" + dateStr + "</td><td class=\"ratePlanOriginPrice\">" + ((originPriceStr == null || originPriceStr == undefined) ? "" : originPriceStr) + "</td><td><input type=\"text\" id=\"ratePlanPrice\" value=\"" + priceStr + "\" class=\"k-textbox\"" + disabled + "/></td></tr>";
            }
            beginDate = new Date(beginDate.valueOf() + 1 * 24 * 60 * 60 * 1000);
            index++;
        }
        $("#ratePlanTbody").empty().html(trHtml);
        if (trHtml.length > 0) {
            var disabledTrLast = $("#ratePlanTbody tr td input[disabled='disabled']:last").parents("tr");
            if (disabledTrLast != null && disabledTrLast != undefined && disabledTrLast.length == 1) {
                disabledTrLast.after("<tr id=\"ratePlanTbodyTrSeparator\"><td colspan=\"4\" style=\"background-color: #f5f5f5;text-align: center;\">以上早期营业日房价不允调整</td></tr>");
            }
        }
        //关联更新
        var IsRelationUpdateAllRoonTypeRatePlanP = $("#IsRelationUpdateAllRoonTypeRatePlanP");
        var IsRelationUpdateAllRoonTypeRatePlan = $("#IsRelationUpdateAllRoonTypeRatePlan")[0];
        IsRelationUpdateAllRoonTypeRatePlanP.css("display", "none"); IsRelationUpdateAllRoonTypeRatePlan.checked = false;
        if (para.roomPriceJsonId == "#roomPriceJson") {
            var roomTypeName = $(para.roomTypeId).data("kendoDropDownList").text();
            var rateCodeName = $(para.rateCodeId).data("kendoDropDownList").text();
            $("label[for='IsRelationUpdateAllRoonTypeRatePlan']").attr("title", (("关联更新主单内所有（价格代码={0} 并且 房型={1} 并且 状态=在住或预订）订单的价格").replace("{0}", rateCodeName).replace("{1}", roomTypeName)));
            IsRelationUpdateAllRoonTypeRatePlanP.css("display", "block"); IsRelationUpdateAllRoonTypeRatePlan.checked = true;
        }
        $("#orderDetailRatePlanEditWindow").data("kendoWindow").center().open();
    },
    Save: function () {
        //保存房价
        var prices = [];
        var isReturn = false;
        $("#ratePlanTbody").find("tr").each(function (index, tr) {
            var $tr = $(tr);
            if ($tr.attr("id") == "ratePlanTbodyTrSeparator") {
                return true;
            }
            var priceDate = $.trim($tr.find("td.ratePlanDate").text());
            var price = $.trim($tr.find("input#ratePlanPrice").val());
            var orginPrice = $.trim($tr.find("td.ratePlanOriginPrice").text());

            if (priceDate.length <= 0) { jAlert("请选择时间"); isReturn = true; return false; }
            var priceDateTimeStamp = Date.parse(priceDate);
            if (!(priceDateTimeStamp != null && priceDateTimeStamp != undefined && !isNaN(priceDateTimeStamp) && priceDateTimeStamp > 0)) { jAlert("请选择时间"); isReturn = true; return false; }
            if (price.length < 0 || isNaN(price)) { jAlert("请输入价格"); isReturn = true; return false; }
            var priceFloat = parseFloat(price);
            if (!(priceFloat != null && priceFloat != undefined && !isNaN(priceFloat) && priceFloat >= 0)) { jAlert("请输入价格"); isReturn = true; return false; }
            var originPriceFloat = parseFloat(orginPrice);
            if (originPriceFloat == null || originPriceFloat == undefined || isNaN(originPriceFloat) || originPriceFloat < 0) { orginPrice = null; }

            prices.push({ Ratedate: priceDate, Price: price, OriginPrice: orginPrice });
        });
        if (!isReturn) {
            permanentRoom_OrderDetailRatePlanEditWindow.Set(prices, true);
            $("#btnSave").attr("data-IsRelationUpdateAllRoonTypeRatePlan", $("#IsRelationUpdateAllRoonTypeRatePlan")[0].checked);
            $("#orderDetailRatePlanEditWindow").data("kendoWindow").close();
        }
    },
    Edit: function () {
        //统一修改房价
        var $checked = $("#ratePlanTbody").find("input[name='checkRatePlan']:checked");
        if ($checked.length == 0) { jAlert("请先选择要调整价格记录", "前往选择"); return; }
        var editedPriceValue = $("#editedPrice").data("kendoNumericTextBox").value();
        if (!(editedPriceValue != null && editedPriceValue > 0)) { jAlert("请先输入要调整为的价格", "前往输入"); return; }

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
        var para = permanentRoom_OrderDetailRatePlanEditWindow.GetConstructor();
        if (para == null || para == undefined) { console.log("请先调用SetConstructor函数"); return; }
        //显示当前的价格计划价格
        if (isEdit == true && para.roomPriceId == "#roomPrice") {
            hasChanged = true;
        }
        var oldData = permanentRoom_OrderDetailRatePlanEditWindow.Get();
        if (oldData != null && oldData != undefined && oldData.length > 0) {
            if (CustomerCommonValues.BusinessDate != null && CustomerCommonValues.BusinessDate != undefined && CustomerCommonValues.BusinessDate.length > 0) {
                var businessDate = new Date(CustomerCommonValues.BusinessDate.replace(/-/g, "/") + " 00:00:00");
                var newData = [];//当前营业日之前的价格不能被修改
                $.each(oldData, function (index, item) {
                    if (new Date(item.Ratedate.replace(/-/g, "/") + " 00:00:00") < businessDate) {
                        newData.push({ Ratedate: item.Ratedate, Price: item.Price, OriginPrice: item.OriginPrice });
                    }
                });
                $.each(data, function (index, item) {
                    if (new Date(item.Ratedate.replace(/-/g, "/") + " 00:00:00") >= businessDate) {
                        newData.push({ Ratedate: item.Ratedate, Price: item.Price, OriginPrice: item.OriginPrice });
                    }
                });
                data = newData;
            }
        }
        if (data != null && data.length > 0) {
            var prices = [];
            var length = data.length;
            for (var i = 0; i < length; i++) {
                if (data[i].Ratedate == CustomerCommonValues.BusinessDate) {
                    prices.push(data[i].Price);
                    break;
                }
            }
            if (prices.length <= 0) {
                try {
                    var rate = JSON.parse(permanentRoom_OrderList.GetSelected().OriginResDetailInfo).Rate;
                    if (rate != null && rate != undefined && !isNaN(rate) && rate >= 0) { prices.push(rate); }
                } catch (e) { }
            }
            $(para.roomPriceId).val(prices.join(","));
            $(para.roomPriceJsonId).val(JSON.stringify(data));
        } else {
            $(para.roomPriceId).val("");
            $(para.roomPriceJsonId).val("");
        }
    },
    Get: function () {
        var para = permanentRoom_OrderDetailRatePlanEditWindow.GetConstructor();
        if (para == null || para == undefined) { console.log("请先调用SetConstructor函数"); return; }
        //返回房价信息
        var ratePlanInfos = null;
        var roomPriceJson = $.trim($(para.roomPriceJsonId).val());
        if (roomPriceJson.length > 0) {
            ratePlanInfos = JSON.parse(roomPriceJson);
        }
        return ratePlanInfos;
    },
    GetRemote: function () {
        var para = permanentRoom_OrderDetailRatePlanEditWindow.GetConstructor();
        if (para == null || para == undefined) { console.log("请先调用SetConstructor函数"); return; }
        //根据当前选择的价格码，房型，抵店时间，离店时间来刷新房价
        var rateCodeValue = $.trim($(para.rateCodeId).data("kendoDropDownList").value());
        var roomTypeValue = $.trim($(para.roomTypeId).data("kendoDropDownList").value());
        var arriveDateValue = $(para.arriveDateId).data("kendoDatePicker").value();
        var depDateValue = $(para.depDateId).data("kendoDatePicker").value();
        if (!(rateCodeValue.length > 0 && roomTypeValue.length > 0 && arriveDateValue != null && depDateValue != null)) { return; }
        var arrBsnsDateValue = $(para.arrBsnsDateId).val();
        var beginDate = ($.trim(arrBsnsDateValue) == "") ? arriveDateValue : new Date(arrBsnsDateValue.replace(/-/g, "/") + " 00:00:00");

        var depDateTimeNew = new Date(depDateValue.valueOf());
        depDateTimeNew.setDate(depDateTimeNew.getDate() + 1);
        if (depDateTimeNew == null) { return; }

        $.post(CustomerCommonValues.GetRateDetailPrices, { rateId: rateCodeValue, roomTypeId: roomTypeValue, beginDate: beginDate.ToDateString(), endDate: depDateTimeNew.ToDateString() }, function (data) {
            if (data.Success && data.Data != null && data.Data.length > 0) {
                var noPrice = 0;
                var orderDetailPlans = [];
                var length = data.Data.length;
                for (var i = 0; i < length; i++) {
                    if (data.Data[i].Price != null) {
                        orderDetailPlans.push({ Ratedate: data.Data[i].Ratedate, Price: data.Data[i].Price, OriginPrice: data.Data[i].Price });
                    } else {
                        noPrice++;
                    }
                }
                permanentRoom_OrderDetailRatePlanEditWindow.Set(orderDetailPlans);
                if (noPrice > 0) {
                    jAlert("选中的价格代码在住店期间没有当前房型的明细价格，请手工调整价格", "知道了");
                }
            } else {
                permanentRoom_OrderDetailRatePlanEditWindow.Set(null);
                jAlert("选中的价格代码在住店期间没有当前房型的明细价格，请手工调整价格", "知道了");
            }
        }, 'json');
    },
    Button: {
        Enabled: function () { $("#editPriceDiv").css("display", "block"); $("#editPriceDivMsg").css("display", "none"); },
        Disabled: function (isShowMsg) { $("#editPriceDiv").css("display", "none"); $("#editPriceDivMsg").css("display", (isShowMsg == true) ? "block" : "none"); },
        Status: function () {
            if ($("#editPriceDiv").css("display") == "block") {
                return true;
            }
            return false;
        },
    },
};