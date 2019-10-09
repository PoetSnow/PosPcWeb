/////////////////////////////////////////////////////////////////////////////////////////
// Super Tables v0.30 - MIT Style License
// Copyright (c) 2008 Matt Murphy --- www.matts411.com
//
// Contributors:
// Joe Gallo
/////////////////////////////////////////////////////////////////////////////////////////
////// TO CALL: 
// new superTable([string] tableId, [object] options);
//
////// OPTIONS: (order does not matter )
// cssSkin : string ( eg. "sDefault", "sSky", "sOrange", "sDark" )
// headerRows : integer ( default is 1 )
// fixedCols : integer ( default is 0 )
// colWidths : integer array ( use -1 for auto sizing )
// onStart : function ( any this.variableNameHere variables you create here can be used later ( eg. onFinish function ) )
// onFinish : function ( all this.variableNameHere variables created in this script can be used in this function )
//
////// EXAMPLES:
// var myST = new superTable("myTableId");
//
// var myST = new superTable("myTableId", {
//		cssSkin : "sDefault",
//		headerRows : 1,
//		fixedCols : 2,
//		colWidths : [100, 230, 220, -1, 120, -1, -1, 120],
//		onStart : function () {
//			this.start = new Date();
//		},
//		onFinish : function () {
//			alert("Finished... " + ((new Date()) - this.start) + "ms.");
//		}
// });
//
////// ISSUES / NOTES:
// 1. No quirksmode support (officially, but still should work)
// 2. Element id's may be duplicated when fixedCols > 0, causing getElementById() issues
// 3. Safari will render the header row incorrectly if the fixed header row count is 1 and there is a colspan > 1 in one 
//		or more of the cells (fix available)
/////////////////////////////////////////////////////////////////////////////////////////

var superTable = function (tableId, options) {
/////* Initialize */
	options = options || {};
	this.cssSkin = options.cssSkin || "";
	this.headerRows = parseInt(options.headerRows || "1");
	this.fixedCols = parseInt(options.fixedCols || "0");
	this.colWidths = options.colWidths || [];
	this.initFunc = options.onStart || null;
	this.callbackFunc = options.onFinish || null;
	this.initFunc && this.initFunc();
	
/////* Create the framework dom */
	this.sBase = document.createElement("DIV");
	this.sFHeader = this.sBase.cloneNode(false);
	this.sHeader = this.sBase.cloneNode(false);
	this.sHeaderInner = this.sBase.cloneNode(false);
	this.sFData = this.sBase.cloneNode(false);
	this.sFDataInner = this.sBase.cloneNode(false);
	this.sData = this.sBase.cloneNode(false);
	this.sColGroup = document.createElement("COLGROUP");
	
	this.sDataTable = document.getElementById(tableId);
	this.sDataTable.style.margin = "0px"; /* Otherwise looks bad */
	if (this.cssSkin !== "") {
		this.sDataTable.className += " " + this.cssSkin;
	}
	if (this.sDataTable.getElementsByTagName("COLGROUP").length > 0) {
		this.sDataTable.removeChild(this.sDataTable.getElementsByTagName("COLGROUP")[0]); /* Making our own */
	}
	this.sParent = this.sDataTable.parentNode;
	this.sParentHeight = this.sParent.offsetHeight;
	this.sParentWidth = this.sParent.offsetWidth;
	
/////* Attach the required classNames */
	this.sBase.className = "sBase";
	this.sFHeader.className = "sFHeader";
	this.sHeader.className = "sHeader";
	this.sHeaderInner.className = "sHeaderInner";
	this.sFData.className = "sFData";
	this.sFDataInner.className = "sFDataInner";
	this.sData.className = "sData";
	
/////* Clone parts of the data table for the new header table */
	var alpha, beta, touched, clean, cleanRow, i, j, k, m, n, p;
	this.sHeaderTable = this.sDataTable.cloneNode(false);
	if (this.sDataTable.tHead) {
		alpha = this.sDataTable.tHead;
		this.sHeaderTable.appendChild(alpha.cloneNode(false));
		beta = this.sHeaderTable.tHead;
	} else {
		alpha = this.sDataTable.tBodies[0];
		this.sHeaderTable.appendChild(alpha.cloneNode(false));
		beta = this.sHeaderTable.tBodies[0];
	}
	alpha = alpha.rows;
	for (i=0; i<this.headerRows; i++) {
		beta.appendChild(alpha[i].cloneNode(true));
	}
	this.sHeaderInner.appendChild(this.sHeaderTable);
	
	if (this.fixedCols > 0) {
		this.sFHeaderTable = this.sHeaderTable.cloneNode(true);
		this.sFHeader.appendChild(this.sFHeaderTable);
		this.sFDataTable = this.sDataTable.cloneNode(true);
		this.sFDataInner.appendChild(this.sFDataTable);
	}
	
/////* Set up the colGroup */
	alpha = this.sDataTable.tBodies[0].rows;
	for (i=0, j=alpha.length; i<j; i++) {
		clean = true;
		for (k=0, m=alpha[i].cells.length; k<m; k++) {
			if (alpha[i].cells[k].colSpan !== 1 || alpha[i].cells[k].rowSpan !== 1) {
				i += alpha[i].cells[k].rowSpan - 1;
				clean = false;
				break;
			}
		}
		if (clean === true) break; /* A row with no cells of colSpan > 1 || rowSpan > 1 has been found */
	}
	cleanRow = (clean === true) ? i : 0; /* Use this row index to calculate the column widths */
	try{
		for (i=0, j=alpha[cleanRow].cells.length; i<j; i++) {
		if (i === this.colWidths.length || this.colWidths[i] === -1) {
			this.colWidths[i] = alpha[cleanRow].cells[i].offsetWidth;
		}
	}
	}catch (msg) {

	}
	
	
	for (i=0, j=this.colWidths.length; i<j; i++) {
		this.sColGroup.appendChild(document.createElement("COL"));
		this.sColGroup.lastChild.setAttribute("width", this.colWidths[i]);
	}
	this.sDataTable.insertBefore(this.sColGroup.cloneNode(true), this.sDataTable.firstChild);
	this.sHeaderTable.insertBefore(this.sColGroup.cloneNode(true), this.sHeaderTable.firstChild);
	if (this.fixedCols > 0) {
		this.sFDataTable.insertBefore(this.sColGroup.cloneNode(true), this.sFDataTable.firstChild);
		this.sFHeaderTable.insertBefore(this.sColGroup.cloneNode(true), this.sFHeaderTable.firstChild);
	}
	
/////* Style the tables individually if applicable */
	if (this.cssSkin !== "") {
		this.sDataTable.className += " " + this.cssSkin + "-Main";
		this.sHeaderTable.className += " " + this.cssSkin + "-Headers";
		if (this.fixedCols > 0) {
			this.sFDataTable.className += " " + this.cssSkin + "-Fixed";
			this.sFHeaderTable.className += " " + this.cssSkin + "-FixedHeaders";
		}
	}
	
/////* Throw everything into sBase */
	if (this.fixedCols > 0) {
		this.sBase.appendChild(this.sFHeader);
	}
	this.sHeader.appendChild(this.sHeaderInner);
	this.sBase.appendChild(this.sHeader);
	if (this.fixedCols > 0) {
		this.sFData.appendChild(this.sFDataInner);
		this.sBase.appendChild(this.sFData);
	}
	this.sBase.appendChild(this.sData);
	this.sParent.insertBefore(this.sBase, this.sDataTable);
	this.sData.appendChild(this.sDataTable);
	
/////* Align the tables */
	var sDataStyles, sDataTableStyles;
	try {//有数据的时候执行的代码
    	this.sHeaderHeight = this.sDataTable.tBodies[0].rows[(this.sDataTable.tHead) ? 0 : this.headerRows].offsetTop;
    	$(".fakeContainer").removeClass("my_width");   	
		sDataStyles += "height: " + (this.sParentHeight-53) + "px;";//更改滚动条的位置
	} catch (e) {//无数据或者无权限的时候执行的代码
		$(".fakeContainer").removeClass("fakeContainer");
		//$(".first_div").addClass("my_width");
		$("#my_div div").removeClass();
		$("#my_div div div:nth-child(1)").remove();
		$("#my_div div div:nth-child(2)").remove();
		$("#my_div div div:nth-child(3)").remove();
		$("#my_div div div:nth-child(4)").remove();
		$("#demoTable th").css("min-width","0");
		$("#sj").css("display","block");						
		$("#my_div").css("overflow-x","scroll");
		$(".page-header").css("display","none");	
				
		$("#qx").css("display","block");
		//$("#qx").addClass("my_width");
		var id = $(".my_table").attr('id');
		//console.log(id);					 
		var width = $("#my_div").width();
		$("#" + id ).css("width",width+"px ");	//加上此处解决ie浏览器查看问题	
		sDataStyles += "height: " + (this.sParentHeight+17) + "px;";//更改滚动条的位置
	}	
	sDataTableStyles = "margin-top: " + (this.sHeaderHeight * -1) + "px;";
	sDataStyles = "margin-top: " + this.sHeaderHeight + "px;";
		
	if (this.fixedCols > 0) {		
		/* A collapsed table's cell's offsetLeft is calculated differently (w/ or w/out border included) across broswers - adjust: */
		this.sFHeaderWidth = this.sDataTable.tBodies[0].rows[cleanRow].cells[this.fixedCols].offsetLeft;
		if (window.getComputedStyle) {
			alpha = document.defaultView;
			beta = this.sDataTable.tBodies[0].rows[0].cells[0];
			if (navigator.taintEnabled) { /* If not Safari */
				this.sFHeaderWidth += Math.ceil(parseInt(alpha.getComputedStyle(beta, null).getPropertyValue("border-right-width")) / 2);
			} else {
				this.sFHeaderWidth += parseInt(alpha.getComputedStyle(beta, null).getPropertyValue("border-right-width"));
			}
		} else if (/*@cc_on!@*/0) { /* Internet Explorer */
			alpha = this.sDataTable.tBodies[0].rows[0].cells[0];
			beta = [alpha.currentStyle["borderRightWidth"], alpha.currentStyle["borderLeftWidth"]];
			if(/px/i.test(beta[0]) && /px/i.test(beta[1])) {
				beta = [parseInt(beta[0]), parseInt(beta[1])].sort();
				this.sFHeaderWidth += Math.ceil(parseInt(beta[1]) / 2);
			}
		}
		
		/* Opera 9.5 issue - a sizeable data table may cause the document scrollbars to appear without this: */
		if (window.opera) {
			this.sFData.style.height = this.sParentHeight + "px";
		}
		
		this.sFHeader.style.width = this.sFHeaderWidth + "px";
		sDataTableStyles += "margin-left: " + (this.sFHeaderWidth * -1) + "px;";
		sDataStyles += "margin-left: " + this.sFHeaderWidth + "px;";
		sDataStyles += "width: " + (this.sParentWidth - this.sFHeaderWidth) + "px;";
	} else {
		sDataStyles += "width: " + this.sParentWidth + "px;";
	}
	this.sData.style.cssText = sDataStyles;
	this.sDataTable.style.cssText = sDataTableStyles;
	
/////* Set up table scrolling and IE's onunload event for garbage collection */
	(function (st) {
		if (st.fixedCols > 0) {
			st.sData.onscroll = function () {
				st.sHeaderInner.style.right = st.sData.scrollLeft + "px";
				st.sFDataInner.style.top = (st.sData.scrollTop * -1) + "px";
			};
		} else {
			st.sData.onscroll = function () {
				st.sHeaderInner.style.right = st.sData.scrollLeft + "px";
			};
		}
		if (/*@cc_on!@*/0) { /* Internet Explorer */
			window.attachEvent("onunload", function () {
				st.sData.onscroll = null;
				st = null;
			});
		}
	})(this);
	
	this.callbackFunc && this.callbackFunc();
};


/////////////////////////////////////////////////////////////////////////////////////////

// Super Tables Plugin for jQuery - MIT Style License

// Copyright (c) 2009 Jeffrey Lee --- blog2.darkthread.net

//

// A wrapper for Matt Murphy's Super Tables http://www.matts411.com/post/super_tables/

//

// Contributors:

//

/////////////////////////////////////////////////////////////////////////////////////////

////// TO CALL: 

// $("...").toSuperTable(options)

//

////// OPTIONS: (order does not matter )

// cssSkin : string ( eg. "sDefault", "sSky", "sOrange", "sDark" )

// headerRows : integer ( default is 1 )

// fixedCols : integer ( default is 0 )

// colWidths : integer array ( use -1 for auto sizing )

// onStart : function ( any this.variableNameHere variables you create here can be used later ( eg. onFinish function ) )

// onFinish : function ( all this.variableNameHere variables created in this script can be used in this function )

// margin, padding, width, height, overflow...: Styles for "fakeContainer"

//

////// Example:

// $("#GridView1").toSuperTable(

//              { width: "640px", height: "480px", fixedCols: 2,

//                onFinish: function() { alert('Done!'); } })



(function($) {

    $.fn.extend(

            {

                toSuperTable: function(options) {

                    var setting = $.extend(

                    {

                        width: "640px", height: "320px",

                        margin: "10px", padding: "0px",

                        overflow: "hidden", colWidths: undefined,

                        fixedCols: 0, headerRows: 1,

                        onStart: function() { },

                        onFinish: function() { },

                        cssSkin: "sSky"

                    }, options);

                    return this.each(function() {

                        var q = $(this);

                        var id = q.attr("id");

                        q.removeAttr("style").wrap("<div id='" + id + "_box'></div>");



                        var nonCssProps = ["fixedCols", "headerRows", "onStart", "onFinish", "cssSkin", "colWidths"];

                        var container = $("#" + id + "_box");



                        for (var p in setting) {

                            if ($.inArray(p, nonCssProps) == -1) {

                                container.css(p, setting[p]);

                                delete setting[p];

                            }

                        }



                        var mySt = new superTable(id, setting);



                    });

                }

            });

})(jQuery);


(function($) {
	$.fn.extend({
		Scroll: function(opt, callback) {
			//参数初始化
			if (!opt) var opt = {};
			var _btnUp = $("#" + opt.up); //Shawphy:向上按钮
			var _btnDown = $("#" + opt.down); //Shawphy:向下按钮
			var timerID;
			var _this = this.eq(0).find("ul:first");
			var lineH = _this.find("li:first").height(), //获取行高
				line = opt.line ? parseInt(opt.line, 10) : parseInt(this.height() / lineH, 10), //每次滚动的行数，默认为一屏，即父容器高度
				speed = opt.speed ? parseInt(opt.speed, 10) : 500; //卷动速度，数值越大，速度越慢（毫秒）
			timer = opt.timer //?parseInt(opt.timer,10):3000; //滚动的时间间隔（毫秒）
			if (line == 0) line = 1;
			var upHeight = 0 - line * lineH;
			//滚动函数
			var scrollUp = function() {
				_btnUp.unbind("click", scrollUp); //Shawphy:取消向上按钮的函数绑定
				_this.animate({
					marginTop: upHeight
				}, speed, function() {
					for (i = 1; i <= line; i++) {
						_this.find("li:first").appendTo(_this);
					}
					_this.css({
						marginTop: 0
					});
					_btnUp.bind("click", scrollUp); //Shawphy:绑定向上按钮的点击事件
				});

			}
			//Shawphy:向下翻页函数
			var scrollDown = function() {
				_btnDown.unbind("click", scrollDown);
				for (i = 1; i <= line; i++) {
					_this.find("li:last").show().prependTo(_this);
				}
				_this.css({
					marginTop: upHeight
				});
				_this.animate({
					marginTop: 0
				}, speed, function() {
					_btnDown.bind("click", scrollDown);
				});
			}
			//Shawphy:自动播放
			var autoPlay = function() {
				if (timer) timerID = window.setInterval(scrollUp, timer);
			};
			var autoStop = function() {
				if (timer) window.clearInterval(timerID);
			};
			//鼠标事件绑定
			_this.hover(autoStop, autoPlay).mouseout();
			_btnUp.css("cursor", "pointer").click(scrollUp).hover(autoStop, autoPlay); //Shawphy:向上向下鼠标事件绑定
			_btnDown.css("cursor", "pointer").click(scrollDown).hover(autoStop, autoPlay);

		}
	})
})(jQuery);