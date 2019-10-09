var s="";
if(navigator.userAgent.indexOf("Chrome")>0 || navigator.userAgent.indexOf("Firefox")>0){
	s = "<object id='HWPostil1' type='application/x-itst-activex' align='baseline' border='0'"
		+ "style='LEFT: 0px; WIDTH: 100%; TOP: 0px; HEIGHT: 100%'"
		+ "clsid='{FF1FE7A0-0578-4FEE-A34E-FB21B277D561}' "
		+ "event_NotifyCtrlReady='HWPostil1_NotifyCtrlReady' "
		+ "event_NotifyChangePage='HWPostil1_NotifyChangePage'>"
		+ "</object>";

}else {
	s = "<OBJECT id='HWPostil1' align='middle' style='LEFT: 0px; WIDTH: 100%; TOP: 0px; HEIGHT: 100%'"
		+ "classid=clsid:FF1FE7A0-0578-4FEE-A34E-FB21B277D561 "
		s += "</OBJECT>";
}
document.write(s)
