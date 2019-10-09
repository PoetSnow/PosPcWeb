using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.PayManage.WxProviderPay
{
    /// <summary>
    /// http连接基础类，负责底层的http通信
    /// </summary>
    public class HttpService
    {

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        public static string Post(string xml, string url, bool isUseCert, int timeout, IWxHttpServicePara paraInfo, IPayLogService logService, string hid, bool provider,string contentType = "text/xml")
        {
            GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 500;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            CheckValidationResult;
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = timeout * 1000;
                request.ServicePoint.Expect100Continue = false;

                //设置代理服务器
                if (!string.IsNullOrWhiteSpace(paraInfo.ProxyUrl))
                {
                    WebProxy proxy = new WebProxy { Address = new Uri(paraInfo.ProxyUrl) }; //定义一个网关对象
                    request.Proxy = proxy;
                }

                //设置POST的数据类型和长度
                request.ContentType = contentType;
                byte[] data = Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    ///服务商
                    if (provider)
                    {
                        //优先从本地计算机证书存储中读取证书，如果不成功则从文件中读取
                        X509Certificate2 cert;
                        try {
                            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                            cert = store.Certificates.Find(X509FindType.FindBySubjectName, "深圳市捷信达电子有限公司", false)[0];
                        } catch {
                            var clientAssembly = Assembly.GetAssembly(typeof(HttpService));
                            var stream = clientAssembly.GetManifestResourceStream("Gemstar.BSPMS.Common.PayManage.WxProviderPay.apiclient_cert.p12");
                            var certData = new byte[stream.Length];
                            stream.Read(certData, 0, certData.Length);
                            cert = new X509Certificate2(certData, paraInfo.CertKey);
                            stream.Close();
                        }
                        
                        request.ClientCertificates.Add(cert);
                    }
                    ///普通商户
                    else
                    {
                        var array = ReadCertificate(paraInfo.CertResourceName);
                        var streamCert = new X509Certificate2(array, paraInfo.CertKey);
                        request.ClientCertificates.Add(streamCert);
                    }
                    //将请求的证书信息写到日志中，以便对比 
                    var certLogBuilder = new StringBuilder();
                    certLogBuilder.AppendFormat("证书数量:{0}\n",request.ClientCertificates.Count);
                    foreach(var cert in request.ClientCertificates) {
                        certLogBuilder.AppendFormat("证书字符串：{0}\n", cert.ToString(true));
                        var cert2 = cert as X509Certificate2;
                        if(cert2 != null && cert2.HasPrivateKey) {
                            certLogBuilder.AppendFormat("证书私钥：{0}\n", cert2.PrivateKey.ToXmlString(true));
                        }
                    }
                    logService.Info(hid, "HttpService_Cert", certLogBuilder.ToString());
                }
                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                logService.Error(hid, "HttpService", "Thread - caught ThreadAbortException - resetting.");
                logService.Error(hid, "Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                logService.Error(hid, "HttpService", e.ToString());
                //取出本地ip记录下来
                var localName = Dns.GetHostName();
                var ipAddresses = Dns.GetHostAddresses(localName);
                var ipLogBuilder = new StringBuilder();
                ipLogBuilder.AppendFormat("机器名:{0}\n", localName);
                foreach(var ip in ipAddresses) {
                    if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                        ipLogBuilder.AppendFormat("ip:{0}\n", ip.ToString());
                    }
                }
                logService.Error(hid, "HttpService_WebException", ipLogBuilder.ToString());

                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    logService.Error(hid, "HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    logService.Error(hid, "HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new WxPayException(e.ToString());
            }
            catch (Exception e)
            {
                logService.Error(hid, "HttpService", e.ToString());
                throw new WxPayException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 读取微信证书文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] ReadCertificate(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = CredentialCache.DefaultCredentials;
            //以数组的形式下载指定文件  
            byte[] byteData = webClient.DownloadData(url);
            webClient.Dispose();
            return byteData;
        }
        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="logService">日志记录服务实例</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url, IWxHttpServicePara paraInfo, IPayLogService logService, string hid)
        {
            GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 500;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            CheckValidationResult;
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //设置代理
                if (!string.IsNullOrWhiteSpace(paraInfo.ProxyUrl))
                {
                    WebProxy proxy = new WebProxy();
                    proxy.Address = new Uri(paraInfo.ProxyUrl);
                    request.Proxy = proxy;
                }

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                logService.Error(hid, "HttpService", "Thread - caught ThreadAbortException - resetting.");
                logService.Error(hid, "Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                logService.Error(hid, "HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    logService.Error(hid, "HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    logService.Error(hid, "HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new WxPayException(e.ToString());
            }
            catch (Exception e)
            {
                logService.Error(hid, "HttpService", e.ToString());
                throw new WxPayException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
    }
}