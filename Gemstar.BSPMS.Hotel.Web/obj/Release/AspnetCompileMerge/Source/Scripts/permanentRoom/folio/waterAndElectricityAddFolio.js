//水电费入账
var waterAndElectricityAddFolio = {
    //在预结 窗口打开
    GetHtmlForCheckout: function () {
        var selectedRegIds = folioGetSelectedRegIdArray();
        if (selectedRegIds == null || selectedRegIds == undefined || selectedRegIds.length <= 0) {
            return "";
        }
        var html = [];
        html.push('<div class="k-widget k-grid"><table class="k-selectable"><thead class="k-grid-header"><tr><th class="k-header">账号</th><th class="k-header">房号</th><th class="k-header">水表</th><th class="k-header">电表</th><th class="k-header">燃气</th></tr></thead><tbody id="waterAndElectricityAddFolio_tbody">');
        var gridFolioGuest = $("#gridFolioGuest").data("kendoGrid");
        $.each(selectedRegIds, function (index, regid) {
            var dataItem = gridFolioGuest.dataSource.get(regid);
            if (dataItem != null && dataItem != undefined && dataItem.StatuName == "在住") {
                html.push(waterAndElectricityAddFolio.GetHtmlTr(dataItem.RegId, dataItem.RoomNo));
            }
        });
        html.push('</tbody></table></div>');
        return html.join("");
    },
    GetHtmlTr: function (regid, roomno) {
        var trHtml = [];
        trHtml.push("<tr data-regid=\"" + regid + "\">");
        trHtml.push("<td>" + replaceHid(regid, FolioCommonValues.HotelId) +"</td>");
        trHtml.push("<td>" + roomno + "</td>");
        trHtml.push("<td><input name=\"input_water\" id=\"input_water_" + regid + "\" class=\"k-textbox\" style=\"width:80px;\" /></td>");
        trHtml.push("<td><input name=\"input_electric\" id=\"input_electric_" + regid + "\" class=\"k-textbox\" style=\"width:80px;\" /></td>");
        trHtml.push("<td><input name=\"input_gas\" id=\"input_gas_" + regid + "\" class=\"k-textbox\" style=\"width:80px;\" /></td>");
        trHtml.push("</tr>");
        return trHtml.join("");
    },
    Submit: function () {
        var tbody = $("#waterAndElectricityAddFolio_tbody");
        if (tbody == null || tbody == undefined || tbody.length <= 0) { return true; }


        //Regid账号, Action处理方式（51水费,52电费,53燃气费）, ThisTimeMeterReading本次读表数
        var list = [];
        tbody.find("tr").each(function (index, element) {
            var item = $(element);
            var regid = item.attr("data-regid");
            var water = item.find("input[name='input_water']").val();
            var electric = item.find("input[name='input_electric']").val();
            var gas = item.find("input[name='input_gas']").val();
            if (parseInt(water) > 0) {
                list.push({ Regid: regid, Action: "51", ThisTimeMeterReading: water });
            }
            if (parseInt(electric) > 0) {
                list.push({ Regid: regid, Action: "52", ThisTimeMeterReading: electric });
            }
            if (parseInt(gas) > 0) {
                list.push({ Regid: regid, Action: "53", ThisTimeMeterReading: gas });
            }
        });

        if (list == null || list.length <= 0) { return true;}

        var result = false;
        $.ajax({
            async: false,
            type: "POST",
            url: FolioCommonValues.WaterAndElectricity_AddFolioDebit,
            data: { list: list },
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    result = true;
                } else {
                    jAlert(data.Data, "知道了");
                }
            },
            error: function (xhr, msg, ex) {
                jAlert(msg, "知道了");
            },
            complete: function () {

            },
        });

        return result;
        //return false;//水电费入账失败
        //return true;//水电费入账成功
    },



    //在换房 窗口打开
    GetHtmlForChangeRoom_Submit: function (regid) {
        if ($.trim(regid).length <= 0) { return true; }

        //Regid账号, Action处理方式（51水费,52电费,53燃气费）, ThisTimeMeterReading本次读表数
        var list = [];
        var item = $("#changeRoomDiv");
        var water = item.find("input[id='input_water_origin']").val();
        var electric = item.find("input[id='input_electric_origin']").val();
        var gas = item.find("input[id='input_gas_origin']").val();
        if (parseInt(water) > 0) {
            list.push({ Regid: regid, Action: "51", ThisTimeMeterReading: water });
        }
        if (parseInt(electric) > 0) {
            list.push({ Regid: regid, Action: "52", ThisTimeMeterReading: electric });
        }
        if (parseInt(gas) > 0) {
            list.push({ Regid: regid, Action: "53", ThisTimeMeterReading: gas });
        }

        if (list == null || list.length <= 0) { return true; }

        var result = false;
        $.ajax({
            async: false,
            type: "POST",
            url: FolioCommonValues.WaterAndElectricity_AddFolioDebit,
            data: { list: list },
            dataType: "json",
            success: function (data) {
                if (data.Success) {
                    result = true;
                } else {
                    jAlert(data.Data, "知道了");
                }
            },
            error: function (xhr, msg, ex) {
                jAlert(msg, "知道了");
            },
            complete: function () {

            },
        });

        return result;
        //return false;//水电费入账失败
        //return true;//水电费入账成功


    },
};