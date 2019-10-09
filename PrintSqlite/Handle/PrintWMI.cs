namespace PrintSqlite
{
    using System;
    using System.Management;

    public class PrintWMI
    {
        private String _name;
        private ManagementObject _printer = null;

        public PrintWMI(String printerName)
        {
            this._name = printerName;

            //Find the Win32_Printer which is a Network Printer of this name

            //Declare WMI Variables
            ManagementObjectCollection MgmtCollection;
            ManagementObjectSearcher MgmtSearcher;

            //Perform the search for printers and return the listing as a collection
            MgmtSearcher = new ManagementObjectSearcher("Select * from Win32_Printer");
            MgmtCollection = MgmtSearcher.Get();

            foreach (ManagementObject objWMI in MgmtCollection)
            {
                if (objWMI.Properties["DeviceId"].Value.ToString().Equals(this._name,StringComparison.CurrentCultureIgnoreCase))
                {
                    this._printer = objWMI;
                    break;
                }
            }

            if (this._printer == null)
            {
                throw new Exception("选定的打印机未连接到此计算机");
            }
        }

        public void PrintTestPage()
        {
            this.InvokeWMIMethod("PrintTestPage");
        }

        /// <summary>
        /// Helper Method which Invokes WMI Methods on this Printer
        /// </summary>
        /// <param name="method">The name of the WMI Method to Invoke</param>
        /// <remarks></remarks>
        private void InvokeWMIMethod(String method)
        {
            if (this._printer == null)
            {
                throw new Exception("无法在未连接到计算机的打印机上打印测试页");
            }

            Object[] objTemp = new Object[0] { };
            try
            {
//Invoke the WMI Method
            this._printer.InvokeMethod(method, objTemp);
            }
            catch(Exception ex)
            {
                LogHelp.CreateLog(ex);
                //throw new Exception(ex.Message);
            }
        }
    }
}
