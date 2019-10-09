function advanced(funcCode) {
    if (funcCode == "002") {
        UpdateItemPrice();  //修改单价
    }
    else if (funcCode == "001") {
        CancelPrint();  //取消打单
    }
    else if (funcCode == "003") {
        HandClearTable();  //手工清台
    }
    else if (funcCode == "005") {
        CancelServiceRate();  //取消服务费
    }
    else if (funcCode == "006") {    //换台
        ChangeTable('A');
    }
    else if (funcCode == "004") {    //取消折扣
        CancelDiscount();
    }
    else if (funcCode == "007") {    //修改服务费
        ServiceRate();
    }
    else if (funcCode == "008") {    //并台
        _MergeTab();
    }
    else if (funcCode == "009") {    //整单取消
        CancelAllItem();
    }
    else if (funcCode == "010") {    //转菜
        _ChangeItem();
    }
    else if (funcCode == "011") {    //取消低消
        CancelMinConsume();
    }
    else if (funcCode == "012") {    //复制菜式
        CopyTabBillDetail();
    }
    else if (funcCode == "999") {
        if ("undefined" != typeof jsObject) //如果是封装程序
        {
            jsObject.SetLocalParameters();
        }
        else {
            layer.alert("浏览器不支持本地配置", { title: "快点云Pos提示", skin: "err" });
        }
    }
}

//修改单价
function UpdateItemPrice() {

    $.ajax({
        url: 'PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {

            if (dataJson.Success) {
                var grid = $("#grid").data("kendoGrid");
                var selectedRows = grid.select();
                if (selectedRows.length > 0) {
                    var row = selectedRows[0];
                    var data = grid.dataItem(row);
                    console.log(data);
                    //取消的或者赠送的不修改单价
                    if (Number(data["Status"]) == 51 || Number(data["Status"]) == 52 || Number(data["Status"]) == 54) {
                        return false;
                    }

                    //var isCurrent = data["isCurrent"];
                    //if (isCurrent == "0" || isCurrent == null) {  
                    //    layer.alert("选择的项目不支持修改价格", { title: "快点云Pos提示", skin: "err" });
                    //    return false;
                    //}                  //注释原因：因为isCurrent时价菜字段不能满足 需求。所以添加 isModiPrice字段，来判断是否能够修改单价。时价菜字段暂时不用
                    // 2019-3-15 杨澹

                    var theKeyID = data["Itemid"];

                    $.ajax({
                        url: 'PosInSingAdvanceFunc/_PriceNumber',
                        type: "post",
                        data: { Id: data["Id"], keyID: data["Itemid"], BillId: $("#billid").val() },
                        datatype: "html",
                        success: function (dataResult) {
                            //debugger


                            //返回请求报错原因的json数据
                            var boolJson = isJson(dataResult);
                            if (boolJson) {
                                if (dataResult.Success == false) {
                                    layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: "err" });
                                    return false;
                                }
                            }
                            //请求成功
                            layer.open({
                                type: 1,
                                title: false,
                                closeBtn: 0, //不显示关闭按钮
                                area: ['320px', '400px'], //宽高
                                content: dataResult,
                                shadeClose: true
                            });
                        },
                        error: function (dataResult) {
                            layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: "err" });
                        }
                    });
                }
            }
            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//取消打单
function CancelPrint() {

    var billid = $("#billid").val()
    $.ajax({
        url: 'PosInSingAdvanceFunc/CancelPrint',
        type: "post",
        data: { billId: billid },
        datatype: "josn",
        success: function (data) {
            if (data.Success) {
                $("#grid").data("kendoGrid").dataSource.read();
                getStatistics(1);
            }
            else {
                layer.alert(data.Data, { title: "快点云pos提示", skin: "err" });
            }
        },
        error: function (data) {
            layer.alert(data.responsetext, { title: "快点云pos提示", skin: "err" });
        }
    });
}

//手工清台
function HandClearTable() {
    var billid = $("#billid").val();
    $.ajax({
        url: 'PosInSingAdvanceFunc/HandClearTable',
        type: "post",
        data: { billId: billid },
        datatype: "josn",
        success: function (data) {
            if (data.Success) {
                if (openFlag == "A") {
                    location.href = "/PosManage/PosTabStatus?rnd=" + Math.random();
                }
                else if (openFlag == "B") {
                    location.href = "/PosManage/PosCashier/Index";
                }
                else {
                    if (window.history.length > 1) {
                        window.history.back();
                    }
                    else {
                        location.href = "../Account/Index";
                    }
                }
            }
            else {
                layer.alert(data.Data, { title: "快点云pos提示", skin: "err" });
            }
        },
        error: function (data) {
            layer.alert(data.responsetext, { title: "快点云pos提示", skin: "err" });
        }
    });
}

//取消服务费
function CancelServiceRate() {

    $.ajax({
        url: 'PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {
            if (dataJson.Success) {
                layer.confirm("是否取消服务费", {
                    btn: ['是', '否'] //按钮
                    , title: '快点云Pos提示'
                    , shade: 'rgba(0,0,0,0)'
                }, function () {
                    $.ajax({
                        url: 'PosInSingAdvanceFunc/CancelServiceRate',
                        type: "post",
                        data: { billId: $("#billid").val() },
                        datatype: "josn",
                        success: function (data) {
                            if (data.Success) {
                                layer.alert("操作成功", { title: "快点云pos提示", skin: "err" });
                            }
                            else {
                                layer.alert(data.Data, { title: "快点云pos提示", skin: "err" });
                            }
                        },
                        error: function (data) {
                            layer.alert(data.responsetext, { title: "快点云pos提示", skin: "err" });
                        }
                    });
                }, function () {
                    layer.closeAll();
                });
            }
            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//取消折扣
function CancelDiscount() {

    $.ajax({
        url: 'PosInSingAdvanceFunc/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {
            if (dataJson.Success) {
                layer.confirm("是否取消折扣", {
                    btn: ['是', '否'] //按钮
                    , title: '快点云Pos提示'
                    , shade: 'rgba(0,0,0,0)'
                }, function () {
                    $.ajax({
                        url: 'PosInSingAdvanceFunc/CancelDiscount',
                        type: "post",
                        data: { billId: $("#billid").val() },
                        datatype: "josn",
                        success: function (data) {
                            if (data.Success) {
                                layer.alert("操作成功", { title: "快点云pos提示", skin: "err" });
                            }
                            else {
                                layer.alert(data.Data, { title: "快点云pos提示", skin: "err" });
                            }
                        },
                        error: function (data) {
                            layer.alert(data.responsetext, { title: "快点云pos提示", skin: "err" });
                        }
                    });
                }, function () {
                    layer.closeAll();
                });
            }
            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//修改服务费
function ServiceRate() {
    $.ajax({
        url: 'PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {
            if (dataJson.Success) {
                var model = {
                    Title: "服务费：",
                    Message: "请输入服务费率",
                    ID: $("#billid").val(),
                    Callback: "UpdateServiceRate($('#txtNumberValue').val());",
                    Flag: "A"
                };

                $.ajax({
                    url: 'PosInSingAdvanceFunc/_NumberInputByServiceRate',
                    type: "post",
                    data: model,
                    dataType: "html",
                    success: function (data) {
                        var boolJson = isJson(data);    //判断是否为json 格式
                        if (!boolJson) {
                            layer.open({
                                type: 1,
                                scrollbar: false,//禁止滚动条
                                closeBtn: 0, //不显示关闭按钮
                                shadeClose: true, //开启遮罩关闭
                                area: ['20rem', '25rem'], //宽高
                                maxWidth: "75%",
                                maxHeight: "75%",
                                content: data,
                                title: false
                            });

                            $('#txtNumberValue').focus();
                            $("#txtNumberValue").css({ "margin-top": "6px" });
                            $("#letterInput").css({ "margin-top": "6px" });
                            $(".wrap").css({ "margin": 0 });
                            $(".layui-layer-tips .layui-layer-content").css({ "overflow": "hidden", "margin": "0", "padding": "0", "background": "rgba(150, 150, 150, 0.9)" });
                        }
                        else {
                            var jsonData = JSON.parse(data);
                            if (jsonData.Success == false) {
                                layer.alert(jsonData.Data, { title: "快点云Pos提示", skin: "err" });
                                return false;
                            }
                        }
                    },
                    error: function (data) {
                        layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
                    }
                });
            }
            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}
//修改服务费
function UpdateServiceRate(val) {
    if (val == "") {
        layer.alert("服务费不能为空", { title: "快点云Pos提示", skin: "err" });
        return false;
    }
    $.ajax({
        url: 'PosInSingAdvanceFunc/UpdateServiceRate',
        type: "post",
        data: { billId: $("#billid").val(), ServiceRate: val },
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                layer.confirm("操作成功", {
                    btn: ['确定', '关闭'] //按钮
                    , title: '快点云Pos提示'
                    , shade: 'rgba(0,0,0,0)'
                }, function () {
                    layer.closeAll();
                }, function () {
                    layer.closeAll();
                });
                //   layer.alert("操作成功", { title: "快点云Pos提示", btn: ['确定', '关闭'] });
                //     layer.closeAll();
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//并台
function _MergeTab() {
    $.ajax({
        url: 'PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {
            if (dataJson.Success) {
                $.ajax({
                    url: 'PosInSingAdvanceFunc/_MergeTab',
                    type: "post",
                    data: {},
                    datatype: "html",
                    success: function (dataResult) {
                        var boolJson = isJson(dataResult);    //判断是否为json 格式
                        if (boolJson) {
                            if (dataResult.Success == false) {
                                layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: "err" });
                                return false;
                            }
                        }
                        layer.open({
                            type: 1,
                            title: "并台",
                            closebtn: 0, //不显示关闭按钮
                            area: ['660px', '510px'], //宽高
                            content: dataResult,
                            id: "TabStatusListByMergeTabList"
                        });
                    },
                    error: function (dataResult) {
                        layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: "err" });
                    }
                });
            }

            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//整单取消
function CancelAllItem() {
    $.ajax({
        url: 'PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {
            if (dataJson.Success) {
                var model = {
                    PageIndex: 1,
                    PageSize: 12,
                    Istagtype: 0
                };
                $.ajax({
                    url: 'PosInSingAdvanceFunc/_ReasonListByAll',
                    type: "post",
                    data: model,
                    datatype: "html",
                    success: function (dataResult) {
                        var boolJson = isJson(dataResult);    //判断是否为json 格式
                        if (boolJson) {
                            if (dataResult.Success == false) {
                                layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: "err" });
                                return false;
                            }
                        }
                        layer.open({
                            type: 1,
                            title: "取消原因",
                            closebtn: 0, //不显示关闭按钮
                            area: ['34rem', '23rem'], //宽高
                            content: dataResult,
                            id: "ReasonListByAll"
                        });
                    },
                    error: function (dataResult) {
                        layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: "err" });
                    }
                });
                getListTotal(model, 'PosInSingle/GetReasonTotal');
            }
            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//整单取消
function CancelAllItemByNum(obj) {
    var model;
    //是字符串
    if (typeof (obj) == 'string') {
        model = {
            billId: $("#billid").val(),
            canReason: obj,
            isreuse: "false"
        }
    }
    else {
        model = {
            billId: $("#billid").val(),
            canReason: $(obj).attr("data-name"),
            isreuse: $(obj).attr("data-isreuse")
        }
    }

    $.ajax({
        url: 'PosInSingAdvanceFunc/CancelAllItemByNum',
        type: "post",
        data: model,
        dataType: "json",
        success: function (data) {
            if (data.Success) {
                if ("undefined" != typeof jsObject) {   //封装程序
                    if (data.Data.length > 0 && data.Data != "") {
                        jsObject.UserName = $("#userName").val();
                        jsObject.isCanItemPrint = "1";
                        jsObject.PrintReport("PosTheMenu", "TheMenu", JSON.stringify(data.Data), false, false);
                    }
                }

                if (openFlag == "A") {
                    location.href = "/PosManage/PosTabStatus?rnd=" + Math.random();
                }
                else if (openFlag == "B") {
                    location.href = "/PosManage/PosCashier/Index";
                }
                else {
                    if (window.history.length > 1) {
                        window.history.back();
                    }
                    else {
                        location.href = "../Account/Index";
                    }
                }
            }
            else {
                layer.alert(data.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

function getListPageByReason(obj, operation, listUrl, totalUrl) {
    var index = parseInt($("#pageIndexItem").val());
    var total = parseInt($("#pageTotalItem").val());
    if (Number(operation) === 1) {
        index -= 1;
    }
    else {
        index += 1;
    }
    if (index > 1) {
        var size = 13;
    }
    else {
        size = 12;
    }
    var number = (total % size) > 0 ? parseInt(total / size) + 1 : parseInt(total / size);
    if (index < 1 || index > number) {
        return false;
    }

    var model = {
        PageIndex: index,
        PageSize: size,
        Istagtype: 1
    };
    $.ajax({
        url: listUrl,
        type: "post",
        data: model,
        datatype: "html",
        success: function (dataResult) {
            $("#ReasonListByAll").html(dataResult);
            $("#pageIndexItem").val(index)
            getListTotal(model, totalUrl);
        },
        error: function (dataResult) {
            layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: "err" });
        }
    });
}

function handReasonAll() {
    var model = {
        Title: "手写原因：",
        Message: "请输入原因",
        ID: $("#billid").val(),
        Callback: "CancelAllItemByNum($('#txtInputValue').val());",
    };
    $.ajax({
        url: 'PosInSingle/_LetterInput',
        type: "post",
        data: model,
        dataType: "html",
        success: function (data) {
            layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                scrollbar: false,
                shadeClose: true,
                area: ['30rem', 'auto'],

                content: data
            });
            //layer.open({
            //    type: 1,
            //    title: "手写原因",
            //    scrollbar: false,//禁止滚动条
            //    closeBtn: 0, //不显示关闭按钮
            //    shadeClose: true, //开启遮罩关闭
            //    area: ['70rem', '40rem'], //宽高
            //    maxWidth: "75%",
            //    maxHeight: "75%",
            //    content: data
            //});
        },
        error: function (data) {
            layer.alert(data.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//转菜
function _ChangeItem() {
    $.ajax({
        url: 'PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {
            if (dataJson.Success) {
                $.ajax({
                    url: 'PosInSingAdvanceFunc/_MergeTabB',
                    type: "post",
                    data: { Flag: "B" },
                    datatype: "html",
                    success: function (dataResult) {
                        var boolJson = isJson(dataResult);    //判断是否为json 格式
                        if (boolJson) {
                            if (dataResult.Success == false) {
                                layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: "err" });
                                return false;
                            }
                        }
                        layer.open({
                            type: 1,
                            title: "转菜",
                            closebtn: 0, //不显示关闭按钮
                            area: ['660px', '510px'], //宽高
                            content: dataResult,
                            id: "TabStatusListByMergeTabList"
                        });
                    },
                    error: function (dataResult) {
                        layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: "err" });
                    }
                });
            }
            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}

//是否计低消(原 取消最低消费）
function CancelMinConsume() {
    layer.confirm('是否计低消？', {
        btn: ['确定', '取消'], //按钮
        content: '<script  type="text/javascript">$(function(){ $("#CancelMinConsume").kendoNumericTextBox({ '
                 +'  format: "n0",'
                 +'  min: 1'
            + '});'
            + 'var fanxiBox = $("input:checkbox");'
            + ' fanxiBox.click(function() {'
            + '     if(this.checked || this.checked=="checked"){ '
            + '            fanxiBox.removeAttr("checked");'
            + '            $(this).prop("checked", true);'
            + '     }'
            + '})'
            + ';})'
            + '</script>'
            + '<span>是否计低消？</span><input type="checkbox" name="islimit" value="true" checked="true" />是  <input type="checkbox" name="islimit" value="false" />否<br /><span>当前最低消费:<span/><input id="CancelMinConsume" value="' + $("#LimitValue").val()+'"/>'
    }, function () { //确定
        $.ajax({
            url: 'PosInSingAdvanceFunc/CancelMinConsume',
            type: "post",
            data: { billid: $("#billid").val(), islimit: $('input[type=checkbox]:checked').val(), limit: $("#CancelMinConsume").val() },
            dataType: "json",
            success: function (dataJson) {
                if (dataJson.Success) {
                    layer.alert("设置成功", { title: "快点云Pos提示", skin: "err" }, function () { window.location.reload(); }, function () { window.location.reload();});
                }
                else {
                    layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" }, function () { window.location.reload(); }, function () { window.location.reload(); });
                }
            },
            error: function (dataJson) {
                layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" }, function () { window.location.reload(); }, function () { window.location.reload(); });
            }
        });
        }, function () { //取消
            layer.closeAll();
    });
}

//复制菜式
function CopyTabBillDetail() {
    $.ajax({
        url: 'PosInSingle/CheckEditForBillRefe',
        type: "post",
        data: { BillId: $("#billid").val() },
        dataType: "json",
        success: function (dataJson) {
            if (dataJson.Success) {
                $.ajax({
                    url: 'PosInSingAdvanceFunc/_CopyTabList',
                    type: "post",
                    datatype: "html",
                    success: function (dataResult) {

                        console.log(dataResult)
                        var boolJson = isJson(dataResult);    //判断是否为json 格式
                        if (boolJson) {
                            if (dataResult.Success == false) {
                                layer.alert(dataResult.Data, { title: "快点云Pos提示", skin: "err" });
                                return false;
                            }
                        }
                        layer.open({
                            type: 1,
                            title: "复制菜式",
                            closebtn: 0, //不显示关闭按钮
                            area: ['660px', '510px'], //宽高
                            content: dataResult,
                            id: "TabStatusListByCopyTabList"
                        });
                    },
                    error: function (dataResult) {
                        layer.alert(dataResult.responsetext, { title: "快点云pos提示", skin: "err" });
                    }
                });
            }
            else {
                layer.alert(dataJson.Data, { title: "快点云Pos提示", skin: "err" });
            }
        },
        error: function (dataJson) {
            layer.alert(dataJson.responseText, { title: "快点云Pos提示", skin: "err" });
        }
    });
}
