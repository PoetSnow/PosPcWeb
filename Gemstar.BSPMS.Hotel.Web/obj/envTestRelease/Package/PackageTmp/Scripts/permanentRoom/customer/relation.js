//关联房
var permanentRoom_RelationWindow = {
    Initialization: function () {
        if (!$("#relationWindowTabStrip").data("kendoSplitter")) {
            $("#relationWindowTabStrip").kendoTabStrip({
                scrollable: false
            });
        }
        $("#relationWindow").kendoWindow({
            width: "850px",
            title: "关联房",
            visible: false,
            modal: true,
            actions: ["Close"]
        });
        $("#relationWindow_search").unbind("click").click(function () { permanentRoom_RelationWindow.Search(); });
    },
    Open: function () {
        permanentRoom_customerMoreDivClose();
        if (!$("#relationWindow").data("kendoWindow")) { permanentRoom_RelationWindow.Initialization(); }
        $("#relationTableDiv").find("tbody").empty();
        permanentRoom_RelationWindow.CancelTabRefresh();
        $("#relationWindow").data("kendoWindow").center().open();
    },
    Search: function () {
        var tBody = $("#addRelationTbody");
        tBody.empty();
        $.post(CustomerCommonValues.GetRelationList, { notResId: $("#Resid").val(), roomNo: $("#relationWindow_roomNo").val(), guestName: $("#relationWindow_GuestName").val(), guestMobile: $("#relationWindow_GuestMobile").val(), status: $("#relationWindow_status").data("kendoDropDownList").value() }, function (result) {
            if (result.Success) {
                var trHtml = [];
                var statusList = $("#relationWindow_status").data("kendoDropDownList").dataItems();
                $.each(result.Data, function (index, item) {
                    var arrDateTime = item.ArrDate != null ? new Date(parseInt(item.ArrDate.replace('/Date(', '').replace(')/', ''))).ToDateTimeWithoutSecondString() : "";
                    var depDateTime = item.DepDate != null ? new Date(parseInt(item.DepDate.replace('/Date(', '').replace(')/', ''))).ToDateTimeWithoutSecondString() : "";
                    var regid = item.Regid.replace(item.Hid, "");
                    $.each(statusList, function (indexStatus, itemStatus) {
                        if (itemStatus.Value == item.Status) {
                            item.StatusName = itemStatus.Text;
                            return false;
                        }
                    });
                    trHtml.push("<tr class=\"" + (index % 2 == 0 ? "" : "k-alt") + "\" data-regid=\"" + item.Regid + "\" ><td>" + regid + "</td><td>" + item.Resno + "</td><td>" + (item.RoomNo == null ? "" : item.RoomNo) + "</td><td>" + item.Guestname + "</td><td>" + (item.GuestMobile != null ? item.GuestMobile : "") + "</td><td>" + arrDateTime + "</td><td>" + depDateTime + "</td><td>" + item.StatusName + "</td><td><input class=\"k-button\" type=\"button\" value=\"关联\" onclick=\"permanentRoom_RelationWindow.AddRelation('" + item.Regid + "')\"></td></tr>");
                });
                tBody.html(trHtml.join(""));
            } else {
                //jAlert(result.Data, "知道了");
                ajaxErrorHandle(result);
            }
        }, 'json');
    },
    AddRelation: function (regId) {
        $.post(CustomerCommonValues.GetRelationResInfo, { regId: regId }, function (existsResult) {
            if (existsResult.Success) {
                if (existsResult.Data != null && existsResult.Data != undefined && existsResult.Data.length > 0) {
                    var html = [];
                    html.push("<p>所选主单[" + existsResult.Data[0].resNo + "]：<p>");
                    html.push("<div class=\"k-widget k-grid\"><table class=\"k-selectable\"><thead class=\"k-grid-header\"><tr><th class=\"k-header\">账号</th><th class=\"k-header\">房间号</th><th class=\"k-header\">客人名</th></tr></thead>");
                    $.each(existsResult.Data, function (index, item) {
                        html.push("<tr "+ (item.OriginRegId == regId ? "class=\"k-state-selected\"" : "" ) +">");
                        html.push(("<td>{1}</td>").replace("{1}", item.RegId));
                        html.push(("<td>{2}</td>").replace("{2}", item.RoomNo));
                        html.push(("<td>{3}</td>").replace("{3}", item.GuestName));
                        html.push("</tr>");
                    });
                    html.push("</table></div>");
                    html.push("<p>是否把主单内的所有客单增加为关联房？<p>");
                    jConfirm(html.join(""), "关联所有", "关联当前", function (value) {
                        if (value) {
                            permanentRoom_RelationWindow.AddRelationHandle(regId, existsResult.Data[0].resId);
                        } else {
                            permanentRoom_RelationWindow.AddRelationHandle(regId);
                        }
                    }, "关联房提示", "取消", false);
                }
                else {
                    permanentRoom_RelationWindow.AddRelationHandle(regId);
                }
            } else {
                ajaxErrorHandle(existsResult);
            }
        }, 'json');
    },
    AddRelationHandle: function (regId, resId) {
        var isRes = (resId != null && resId != undefined && resId.length > 0);
        $.post(CustomerCommonValues.AddRelation, { addRegId: (isRes ? resId : regId), toResId: $("#Resid").val(), isRes: isRes }, function (result) {
            if (result.Success) {
                var msg = "增加关联房成功！";
                var trRow = $("#addRelationTbody tr[data-regid='" + regId + "']");
                if (trRow != null && trRow != undefined) {
                    if (isRes) {
                        msg += "\n主单[{0}]内的所有客单，已与主单号{3}关联";
                        msg = msg.replace("{0}", trRow.find("td:eq(1)").text());
                        msg = msg.replace("{3}", $("#Resno").val());
                    }
                    else {
                        msg += "\n房间号:{0},账号:{1},客人名:{2}\n已与主单号{3}关联";
                        msg = msg.replace("{0}", trRow.find("td:eq(2)").text());
                        msg = msg.replace("{1}", trRow.find("td:eq(0)").text());
                        msg = msg.replace("{2}", trRow.find("td:eq(3)").text());
                        msg = msg.replace("{3}", $("#Resno").val());
                    }
                }
                jAlert(msg);
                permanentRoom_OrderCustomer.RefreshData(result, "permanentRoom_RelationWindow.AddRelation");
                permanentRoom_RelationWindow.Search();
                permanentRoom_RelationWindow.CancelTabRefresh();
            } else {
                //jAlert(result.Data, "知道了");
                ajaxErrorHandle(result);
            }
        }, 'json');
    },
    CancelRelation: function (regId) {
        $.post(CustomerCommonValues.RemoveRelation, { regId: regId }, function (result) {
            if (result.Success) {
                var msg = "取消关联房成功！";
                var trRow = $("#cancelRelationTbody tr[data-regid='" + regId + "']");
                if (trRow != null && trRow != undefined) {
                    msg += "\n房间号:{0},账号:{1},客人名:{2}\n已与主单号{3}取消关联";
                    msg = msg.replace("{0}", trRow.find("td:eq(3)").text());
                    msg = msg.replace("{1}", trRow.find("td:eq(0)").text());
                    msg = msg.replace("{2}", trRow.find("td:eq(2)").text());
                    msg = msg.replace("{3}", $("#Resno").val());
                    trRow.remove();
                }
                jAlert(msg);
                permanentRoom_OrderCustomer.RefreshData(result, "permanentRoom_RelationWindow.CancelRelation");
                permanentRoom_RelationWindow.CancelTabRefresh();
            } else {
                //jAlert(result.Data, "知道了");
                ajaxErrorHandle(result);
            }
        }, 'json');
    },
    CancelTabRefresh: function () {
        var tBody = $("#cancelRelationTbody");
        tBody.empty();
        var list = permanentRoom_OrderList.Get();
        if (list != null && list != undefined && list.length > 0) {
            var trHtml = [];
            $.each(list, function (index, item) {
                var arrDateTime = item.ArrDate != null ? new Date(item.ArrDate).ToDateTimeWithoutSecondString() : "";
                var depDateTime = item.DepDate != null ? new Date(item.DepDate).ToDateTimeWithoutSecondString() : "";
                var regid = item.Regid.replace(item.Hid, "");
                trHtml.push("<tr data-regid=\"" + item.Regid + "\" class=\"" + (index % 2 == 0 ? "" : "k-alt") + "\" ><td>" + regid + "</td><td>" + item.RoomTypeName + "</td><td>" + item.Guestname + "</td><td>" + (item.RoomNo == null ? "" : item.RoomNo) + "</td><td>" + item.RoomQty + "</td><td>" + item.StatusName + "</td><td><input class=\"k-button\" type=\"button\" value=\"取消关联\" onclick=\"permanentRoom_RelationWindow.CancelRelation('" + item.Regid + "')\"></td></tr>");
            });
            tBody.html(trHtml.join(""));
        }
    },
};