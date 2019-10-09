//子单客人
//客人列表
var CustomerList = {
    //客人列表ID
    Id: "#orderDetailCustomersListView",
    //获取客人列表
    Get: function () {
        var customerList = [];
        var customerDataItems = $(this.Id).data("kendoListView").dataItems();
        var customerDataItemsLength = customerDataItems.length;
        if (customerDataItemsLength > 0) {
            for (var i = 0; i < customerDataItemsLength; i++) {
                customerList.push({
                    Id: customerDataItems[i].Id,
                    OriginRegInfoJsonData: customerDataItems[i].OriginRegInfoJsonData,
                    Address: customerDataItems[i].Address,
                    Birthday: customerDataItems[i].Birthday,
                    CarNo: customerDataItems[i].CarNo,
                    CerId: customerDataItems[i].CerId,
                    CerType: customerDataItems[i].CerType,
                    City: customerDataItems[i].City,
                    Email: customerDataItems[i].Email,
                    Gender: customerDataItems[i].Gender,
                    GuestName: customerDataItems[i].GuestName,
                    Interest: customerDataItems[i].Interest,
                    Mobile: customerDataItems[i].Mobile,
                    Qq: customerDataItems[i].Qq,
                    Guestid: customerDataItems[i].Guestid,
                    Nation: customerDataItems[i].Nation,
                    PhotoUrl: customerDataItems[i].PhotoUrl,
                });
            }
        }
        return customerList;
    },
    //设置客人列表
    Set: function (data, selectid) {
        var listView = $(this.Id).data("kendoListView");
        listView.dataSource.data((data != null && data.length > 0) ? data : []);
        listView.clearSelection();
        this.Customer.Set(null);
        this.Select(selectid);
    },
    //在客人列表中选择一项
    Select: function (customerid) {
        if ($.trim($("#guestName").val()).length > 0) { this.Add(); }

        var obj = $(this.Id);
        var listView = obj.data("kendoListView");
        listView.clearSelection();

        var li = null;
        if (customerid != null && customerid != undefined && customerid.length > 0) {
            li = obj.find("li[id='" + customerid.toLowerCase() + "']");
        } else {
            li = obj.find("li:eq(0)");
        }

        if (li != null && li != undefined && li.length > 0) {
            listView.select(li[0]);
        }

        var row = listView.select();
        this.Customer.Set((row != null && row != undefined && row.length > 0) ? listView.dataItem(row) : null);
    },
    //把客人信息添加或更新到客人列表中
    Add: function () {
        var returnId = null;
        if ($.trim($("#guestName").val()).length <= 0) {
            jAlert("请先输入当前客人的客人名"); return returnId;
        }
        var entity = this.Customer.Get();
        if (entity == null) {
            return returnId;
        }
        if (entity.Email.length > 0) {
            var reg = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
            if (!reg.test(entity.Email)) {
                jAlert("邮箱格式不正确"); return returnId;
            }
        }
        var listView = $(this.Id).data("kendoListView");
        var dataItem = listView.dataSource.get(entity.Id);
        if (dataItem != null && dataItem != undefined && entity.Id == dataItem.Id) {
            if (dataItem.GuestName != entity.GuestName) { dataItem.GuestName = entity.GuestName; }
            if (dataItem.Mobile != entity.Mobile) { dataItem.Mobile = entity.Mobile; }
            if (dataItem.CerType != entity.CerType) { dataItem.CerType = entity.CerType; }
            if (dataItem.CerId != entity.CerId) { dataItem.CerId = entity.CerId; }
            if (dataItem.Gender != entity.Gender) { dataItem.Gender = entity.Gender; }
            if (dataItem.Birthday != entity.Birthday) { dataItem.Birthday = entity.Birthday; }
            if (dataItem.City != entity.City) { dataItem.City = entity.City; }
            if (dataItem.Address != entity.Address) { dataItem.Address = entity.Address; }
            if (dataItem.Qq != entity.Qq) { dataItem.Qq = entity.Qq; }
            if (dataItem.Email != entity.Email) { dataItem.Email = entity.Email; }
            if (dataItem.CarNo != entity.CarNo) { dataItem.CarNo = entity.CarNo; }
            if (dataItem.Interest != entity.Interest) { dataItem.Interest = entity.Interest; }
            if (dataItem.Guestid != entity.Guestid) { dataItem.Guestid = entity.Guestid; }
            if (dataItem.Nation != entity.Nation) { dataItem.Nation = entity.Nation; }
            if (dataItem.PhotoUrl != entity.PhotoUrl) { dataItem.PhotoUrl = entity.PhotoUrl; }
            returnId = entity.Id;
        } else {
            entity.Id = Math.floor(Math.random() * 999999999 + 1) + "";
            var customerList = this.Get();
            customerList.push(entity);
            listView.dataSource.data(customerList);
            returnId = entity.Id;
        }
        listView.refresh();
        this.Customer.Set(null);
        return returnId;
    },
    //在客人列表中移除选定项
    Remove: function () {
        var listView = $(this.Id).data("kendoListView");
        var row = listView.select();
        if (row == null || row == undefined || row.length <= 0) {
            jAlert("请先选择要移除的客人"); return null;
        }
        var removeId = row[0].id;
        var customerList = this.Get();
        var customerListLength = customerList.length;
        if (customerListLength > 0) {
            for (var i = 0; i < customerListLength; i++) {
                if (customerList[i].Id == removeId) {
                    customerList.splice(i, 1);
                    break;
                }
            }
        }
        listView.dataSource.data(customerList);
        this.Customer.Set(null);
        return removeId;
    },
    //客人
    Customer: {
        //获取客人信息
        Get: function () {
            var birthday = $("#birthday").data("kendoDatePicker").value();
            return {
                Id: $.trim($("#customerInfoId").val()),
                GuestName: $.trim($("#guestName").val()),
                Mobile: $.trim($("#guestMobile").val()),
                CerType: $("#cerType").data("kendoDropDownList").value(),
                CerId: $.trim($("#cerId").val()),
                Gender: $("[name='gender']:checked").val(),
                Birthday: birthday != null ? birthday.ToDateString() : null,
                City: $.trim($("#city").val()),
                Address: $.trim($("#address").val()),
                Qq: $.trim($("#qq").val()),
                Email: $.trim($("#email").val()),
                CarNo: $.trim($("#carNo").val()),
                Interest: $.trim($("#love").val()),
                Guestid: $.trim($("#guestid").val()),
                OriginRegInfoJsonData: null,
                Nation: $("#nation").data("kendoDropDownList").value(),
                PhotoUrl: $("[for='openPhoto']").data("src"),
            };
        },
        //设置客人信息
        Set: function (data) {
            if (data != null && data != undefined) {
                $("#customerInfoId").val(data.Id);
                $("#guestName").val(data.GuestName);
                $("#guestMobile").val(data.Mobile);
                $("#cerType").data("kendoDropDownList").value(data.CerType);
                $("#cerId").val(data.CerId);
                if (data.Gender == "F") {
                    $("[name='gender'][value='F']")[0].checked = true;
                }
                else {
                    $("[name='gender'][value='M']")[0].checked = true;
                }
                if (data.Birthday != null && data.Birthday != undefined && data.Birthday.length > 0) {
                    $("#birthday").data("kendoDatePicker").value(data.Birthday);
                } else {
                    $("#birthday").data("kendoDatePicker").value("");
                }
                $("#city").val(data.City);
                $("#address").val(data.Address);
                $("#qq").val(data.Qq);
                $("#email").val(data.Email);
                $("#carNo").val(data.CarNo);
                $("#love").val(data.Interest);
                $("#guestid").val(data.Guestid);
                $("#nation").data("kendoDropDownList").value(data.Nation);
                $("[for='openPhoto']").data("src", data.PhotoUrl);
            } else {
                $("#customerInfoId").val("");
                $("#guestName").val("");
                $("#guestMobile").val("");
                $("#cerType").data("kendoDropDownList").select(0);
                $("#cerId").val("");
                $("[name='gender'][value='M']")[0].checked = true;
                $("#birthday").data("kendoDatePicker").value("");
                $("#city").val("");
                $("#address").val("");
                $("#qq").val("");
                $("#email").val("");
                $("#carNo").val("");
                $("#love").val("");
                $("#guestid").val("");
                $("#nation").data("kendoDropDownList").select(0);
                $("[for='openPhoto']").data("src", "");
            }
        },
    },
};