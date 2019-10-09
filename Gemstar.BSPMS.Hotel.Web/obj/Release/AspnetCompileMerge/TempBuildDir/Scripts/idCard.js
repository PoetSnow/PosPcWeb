/*身份证号码JS类*/
//获取籍贯
function getCity(idCard, obj)
{
    if (idCard != null && idCard != undefined && idCard.length > 0 && obj != null && obj != undefined) {
        $.post("/Home/GetCity", { idCard: idCard }, function (result) {
            if (result.Success) {
                obj.val(result.Data);
            } else {
                obj.val("");
            }
        }, 'json');
    } else {
        obj.val("");
    }
}
//获取生日
function getBirthDay(idCard) {
    if (idCard != null && idCard != undefined && idCard.length >= 14) {
        var yyyy = idCard.substring(6, 10);
        var mm = idCard.substring(10, 12);
        var dd = idCard.substring(12, 14);
        var birthDayDate = new Date(yyyy + "/" + mm + "/" + dd);
        if (birthDayDate != "Invalid Date") {
            return birthDayDate.ToDateString();
        }
    }
    return null;
}
//获取性别（男M，女F）
function getGender(idCard) {
    if (idCard != null && idCard != undefined && idCard.length >= 17) {
        var gender = parseInt(idCard.substring(16, 17));
        if (!isNaN(gender) && gender % 2 == 0) {
            return 'F';
        }
    }
    return 'M';
}