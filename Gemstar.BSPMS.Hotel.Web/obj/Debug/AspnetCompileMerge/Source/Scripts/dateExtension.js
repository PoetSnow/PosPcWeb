// 对Date的扩展，将 Date 转化为指定格式的String 
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1,                 //月份 
        "d+": this.getDate(),                    //日 
        "h+": this.getHours(),                   //小时 
        "m+": this.getMinutes(),                 //分 
        "s+": this.getSeconds(),                 //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds()             //毫秒 
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
Date.prototype.ToDateString = function () {
    return this.Format("yyyy-MM-dd");
}
Date.prototype.ToDateTimeString = function () {
    return this.Format("yyyy-MM-dd hh:mm:ss");
}
Date.prototype.ToDateTimeWithoutSecondString = function () {
    return this.Format("yyyy-MM-dd hh:mm");
}
Date.prototype.ToTimeString = function () {
    return this.Format("hh:mm:ss");
}
function convertJsonDate(value, formate) {
    if (value == null || value == undefined || $.trim(value).length <= 0) {
        return "";
    }
    value = value.replace('/Date(', '').replace(')/', '');
    var valueInt = parseInt(value);
    if (valueInt <= 0 || isNaN(valueInt)) {
        return "";
    }
    var newDate = new Date(valueInt);
    if (newDate == 'Invalid Date') {
        return "";
    }
    if (formate == "ToDateTimeString") {
        return newDate.ToDateTimeString();
    }
    else if (formate == "ToDateTimeWithoutSecondString") {
        return newDate.ToDateTimeWithoutSecondString();
    }
    else if (formate == "ToTimeString") {
        return newDate.ToTimeString();
    }
    else{
        return newDate.ToDateString();
    }
}
/*
先将通用的clone方法也放到此处
*/
function clone(obj,withoutFunction,without_) {
    var o;
    switch (typeof obj) {
        case 'undefined': break;
        case 'string': o = obj + ''; break;
        case 'number': o = obj - 0; break;
        case 'boolean': o = obj; break;
        case 'object':
            if (obj === null) {
                o = null;
            } else {
                //因为kendo绑定table生成的array有问题，这里要判断他的长度是否大于0
                if (obj instanceof Array||obj.length>0) {
                    o = [];
                    for (var i = 0, len = obj.length; i < len; i++) {
                        o.push(clone(obj[i], withoutFunction, without_));
                    }
                } else {
                        o = {};
                        for (var k in obj) {
                            if (k.indexOf('_') == 0) {
                                if (!without_) {
                                    o[k] = clone(obj[k], withoutFunction, without_);
                                }
                            } else {
                                o[k] = clone(obj[k], withoutFunction, without_);
                            }
                        }
                    }
            }
            break;
        default:
            if (!withoutFunction) {
                o = obj; 
            }
            break;
    }
    return o;
}