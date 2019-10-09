//主单
var OrderMain = {
    //获取主单
    Get: function () {
        return {
            Resid: $("#Resid").val(),
            ResTime: $("#ResTime").val(),
            SaveContinue: $("#SaveContinue").val(),
            AuthorizationSaveContinue: $("#authorizationSaveContinue").val(),
            OriginResMainInfoJsonData: $("#OriginResMainInfoJsonData").val(),
            UseScoreSaveContinue: $("#UseScoreSaveContinue").val(),

            Name: $.trim($("#Name").val()),
            Resno: $("#Resno").val(),
            ResNoExt: $.trim($("#ResNoExt").val()),
            ResCustName: $.trim($("#ResCustName").val()),
            ResCustMobile: $.trim($("#ResCustMobile").val()),
            Cttid: $("#Cttid").data("kendoDropDownList").value(),
            IsGroup: $("[name='IsGroup']:checked").val(),
            IsCustemSecret: $("[name='IsCustemSecret']")[0].checked?1:0,
            IsHidePrice: $("[name='IsHidePrice']")[0].checked ? 1 : 0,
            ResInvoiceInfo: null,//OrderMainInvoiceInfoWindow.Get(),
        };
    },
    //设置主单
    Set: function (data) {
        if (data != null && data != undefined) {
            $("#Resid").val(data.Resid);
            $("#ResTime").val(data.ResTime);
            $("#SaveContinue").val(data.SaveContinue);
            $("#authorizationSaveContinue").val(data.AuthorizationSaveContinue);
            $("#OriginResMainInfoJsonData").val(data.OriginResMainInfoJsonData);
            $("#UseScoreSaveContinue").val(data.UseScoreSaveContinue);

            $("#Name").val(data.Name);
            $("#Resno").val(data.Resno);
            $("#ResNoExt").val(data.ResNoExt);
            $("#ResNoExt").attr("title", data.ResNoExt);
            $("#ResCustName").val(data.ResCustName);
            $("#ResCustMobile").val(data.ResCustMobile);
            $("#Cttid").data("kendoDropDownList").value(data.Cttid);
            if (data.IsGroup == 1) {
                $("[name='IsGroup'][value='1']")[0].checked = true;
            } else {
                $("[name='IsGroup'][value='0']")[0].checked = true;
            }
            if (data.IsCustemSecret == 1) {
                $("[name='IsCustemSecret']")[0].checked = true;
            } else {
                $("[name='IsCustemSecret']")[0].checked = false;
            }
            if (data.IsHidePrice == 1) {
                $("[name='IsHidePrice']")[0].checked = true;
            } else {
                $("[name='IsHidePrice']")[0].checked = false;
            }
            var extType = data.ExtType;
            if (extType == "普通订单") {
                $("#ExtType").css("color", "#333");
                if (data.ResDetailInfos != null && data.ResDetailInfos.length > 0 && data.ResDetailInfos[0] != null) {
                    var resStatus = data.ResDetailInfos[0].ResStatus;
                    var recStatus = data.ResDetailInfos[0].RecStatus;
                    var status = data.ResDetailInfos[0].Status;
                    if (resStatus == "R" || resStatus == "N" || resStatus == "X") {
                        extType = "预订客单";
                    }
                    else {
                        extType = "直接入住";
                    }
                }
            } else {
                $("#ExtType").css("color", "green");
            }
            $("#ExtType").val(extType);
            //OrderMainInvoiceInfoWindow.Set(data.ResInvoiceInfo);
        }
        else {
            $("#Resid").val("");
            $("#ResTime").val(new Date().ToDateTimeWithoutSecondString());
            $("#SaveContinue").val("");
            $("#OriginResMainInfoJsonData").val("");

            $("#Name").val("");
            $("#Resno").val("");
            $("#ResNoExt").val("");
            $("#ResNoExt").attr("title","");
            $("#ResCustName").val("");
            $("#ResCustMobile").val("");
            $("#Cttid").data("kendoDropDownList").select(0);
            $("[name='IsGroup'][value='0']")[0].checked = true;
            $("[name='IsCustemSecret']")[0].checked = false;
            $("[name='IsHidePrice']")[0].checked = false;
            $("#ExtType").val("");
            //OrderMainInvoiceInfoWindow.Set(null);
        }
    },
};