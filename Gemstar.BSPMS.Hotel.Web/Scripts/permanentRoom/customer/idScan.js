//身份证扫描
function scanIdButton_clicked(e) {
    if (e) { e.preventDefault(); }
    var idType = CustomerCommonValues.idType;
    var idCode = CustomerCommonValues.idCode;
    var idEditionName = CustomerCommonValues.idEditionName;
    if (!idType || !idCode) { jAlert("没有开通身份证扫描接口", "手工输入"); return; }
    //先固定写硬件交互参数值，等后面再修改为从数据库读取酒店设置的硬件型号
    var hardwareCallPara = { Type: idType, ID: idCode, Operate: 'ReadCard', Para: null, EditionName: idEditionName };
    callHardware(hardwareCallPara);
}
//调用成功后的回调函数
function hardwareCallback_02_ReadCard(data) {
    if (data == null || data == undefined) { return; }
    var type = $("#certificatesScan").val();
    try {
        var func = eval(type + "ScanCallBack");
        if (typeof (func) == "function") {
            func(data);
        }
    } catch (e) { }
}
function showPhoto(data) {
    try {

        var IsScanSavePhoto = CustomerCommonValues.IsScanSavePhoto;
        if (IsScanSavePhoto != null && IsScanSavePhoto != undefined && IsScanSavePhoto) {
            $("[for='openPhoto']").data("src", "");
            if (data.ImageBase64 != "" && data.ImageBase64 != null) {
                var data = "data:image/bmp;base64," + data.ImageBase64;
                $("[for='openPhoto']").data("src", data);
            }
        }

    } catch (e) { }
}
function customerScanCallBack(data) {
    $("#guestName").val(data.Name);
    $("#cerId").val(data.Number);
    showPhoto(data);
    if (data.Sex == "F") {
        $("[name='gender'][value='F']")[0].checked = true;
    }
    else {
        $("[name='gender'][value='M']")[0].checked = true;
    }
    $("#birthday").data("kendoDatePicker").value(data.Birthday);
    $("#address").val(data.Address);
    cerId_changed();
}
//会员证件扫描回调
function mbrIDFuncScanCallBack(data) {
    $("#GuestName").val(data.Name);
    $("#CerId").val(data.Number);
    if (data.Sex == "F") {
        $("#Gender").data("kendoDropDownList").value("F");
    }
    else {
        $("#Gender").data("kendoDropDownList").value("M");
    }
    $("#BirthDay").data("kendoDatePicker").value(data.Birthday);
    $("#Address").val(data.Address);
    var cerType = $("#CerType").data("kendoDropDownList").text();
    if (cerType == "居民身份证") {
        var cerId = $("#CerId").val();
        getCity(cerId, $("#City"));
    }
}