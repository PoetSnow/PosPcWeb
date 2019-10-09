using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Gemstar.BSPMS.Common.Tools
{
    public class HttpHelper
    {
        /// <summary>
        /// 为WebClient增加超时时间
        /// <para>从WebClient派生一个新的类，重载GetWebRequest方法</para>
        /// </summary>
        public class NewWebClient : WebClient
        {
            private int _timeout;

            /// <summary>
            /// 超时时间(毫秒)
            /// </summary>
            public int Timeout
            {
                get
                {
                    return _timeout;
                }
                set
                {
                    _timeout = value;
                }
            }

            public NewWebClient()
            {
                this._timeout = 60000;
            }

            public NewWebClient(int timeout)
            {
                this._timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var result = base.GetWebRequest(address);
                result.Timeout = this._timeout;
                return result;
            }
        }
    }

    

}
