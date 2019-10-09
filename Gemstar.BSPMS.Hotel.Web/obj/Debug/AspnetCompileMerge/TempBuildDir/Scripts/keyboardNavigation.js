//监听ESC键
$(document).on("keydown.examples", function (e) {
    if (e.keyCode === 27 /* Esc */) {
        try{
            window.top.closeWindowByESC(document.activeElement);
        }catch(e){}
    }
});
//执行ESC关闭Windos弹框
function closeWindowByESC(activeElement) {
    if ($("#popup_container").css("display") == "block") { return; }
    if (activeElement == null || activeElement == undefined || activeElement.length <= 0) { return; }
    var activeObj = $(activeElement);
    if (activeObj == null || activeObj == undefined || activeObj.length <= 0) { return;}
    if (activeObj.attr("data-role") != "window") {//焦点在此div上，则焦点在弹框窗口上，按ESC会自动关闭。无需继续执行

        var divWindow = activeObj.parents("[data-role='window']");
        if (divWindow != null && divWindow != undefined && divWindow.length > 0) {//找到当前焦点控件，所在弹框窗口的关闭按钮

            var kWindow = divWindow.parent(".k-widget.k-window");
            if (kWindow != null && kWindow != undefined && kWindow.length > 0 && kWindow.css("display") == "block") {

                var windowHeader = kWindow.find(".k-window-titlebar.k-header");
                if (windowHeader != null && windowHeader != undefined && windowHeader.length > 0) {

                    var closeObj = windowHeader.find(".k-i-close");
                    if (closeObj != null && closeObj != undefined && closeObj.length > 0) {

                        closeObj[0].click();
                    }
                }
            }
        }
        else {//在有弹框中，找到排在最前的弹框
            var maxIndex = -1;
            var list = [];
            $(".k-widget.k-window").each(function (index, item) {
                var obj = $(item);
                if (obj != null && obj != undefined && obj.length > 0) {
                    if (obj.css("display") == "block") {
                        var zIndex = parseInt(obj.css("z-index"));
                        if (!isNaN(zIndex)) {
                            if (zIndex > maxIndex) {
                                maxIndex = zIndex;
                                list.push({ index: index, zIndex: maxIndex });
                            }
                        }
                    }
                }
            });
            var closeIndex = -1;
            if (maxIndex > -1) {
                if (list != null && list != undefined && list.length > 0) {
                    $.each(list, function (index, item) {
                        if (item.zIndex == maxIndex) {
                            closeIndex = item.index;
                            return false;
                        }
                    });
                }
            }
            if (closeIndex > -1) {
                var kWindow = $(".k-widget.k-window")[closeIndex];
                if (kWindow != null && kWindow != undefined) {
                    var closeIcon = $(kWindow).find(".k-window-titlebar.k-header .k-i-close");
                    if (closeIcon != null && closeIcon != undefined && closeIcon.length > 0) {
                        closeIcon[0].click();
                    }
                }
            }
        }

    }
}