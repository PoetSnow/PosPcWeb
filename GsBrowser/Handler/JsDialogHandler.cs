using System.Windows.Forms;

namespace GsBrowser
{
    class JsDialogHandler : CefSharp.IJsDialogHandler
    {
        public void OnDialogClosed(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
        {

        }

        public bool OnJSBeforeUnload(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string message,
bool isReload, CefSharp.IJsDialogCallback callback)
        {
            return true;
        }

        public bool OnJSDialog(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser,
string originUrl, CefSharp.CefJsDialogType dialogType, string messageText, string defaultPromptText,
 CefSharp.IJsDialogCallback callback, ref bool suppressMessage)
        {
            switch (dialogType)
            {
                case CefSharp.CefJsDialogType.Alert:
                    MessageBox.Show(messageText, "快点云Pos提示");
                    suppressMessage = true;
                    return false;
                case CefSharp.CefJsDialogType.Confirm:
                    var dr = MessageBox.Show(messageText, "快点云Pos提示", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        callback.Continue(true, string.Empty);
                        suppressMessage = false;
                        return true;
                    }
                    else
                    {
                        callback.Continue(false, string.Empty);
                        suppressMessage = false;
                        return true;
                    }
                case CefSharp.CefJsDialogType.Prompt:
                    MessageBox.Show("系统不支持prompt形式的提示框", "提示");
                    break;
                default:
                    break;
            }
            return false;
        }

        public void OnResetDialogState(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
        {

        }
    }
}
