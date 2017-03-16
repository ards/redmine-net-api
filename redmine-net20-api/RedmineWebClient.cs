/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class RedmineWebClient : WebClient
    {
        const string UA = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:8.0) Gecko/20100101 Firefox/8.0";
        //  private readonly CookieContainer container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var wr = base.GetWebRequest(address);
            var httpWebRequest = wr as HttpWebRequest;

            if (httpWebRequest != null)
            {

                if (UseCookies)
                {
                    httpWebRequest.Headers.Add(HttpRequestHeader.Cookie, "redmineCookie");
                    httpWebRequest.CookieContainer = CookieContainer;
                }
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;
                httpWebRequest.PreAuthenticate = PreAuthenticate;
                httpWebRequest.KeepAlive = KeepAlive;
                httpWebRequest.UseDefaultCredentials = UseDefaultCredentials;
                httpWebRequest.Credentials = Credentials;
                httpWebRequest.UserAgent = UA;
                httpWebRequest.CachePolicy = CachePolicy;
                httpWebRequest.ClientCertificates = ClientCertificates;

                if (UseProxy)
                {
                    if (Proxy != null)
                    {
                        Proxy.Credentials = Credentials;
                    }
                    httpWebRequest.Proxy = Proxy;
                }

                if (Timeout != null)
                    httpWebRequest.Timeout = Timeout.Value.Milliseconds;

                return httpWebRequest;
            }

            return base.GetWebRequest(address);
        }

        public bool UseProxy { get; set; }
        public bool UseCookies { get; set; }

        /// <summary>
        /// in miliseconds
        /// </summary>
        public TimeSpan? Timeout { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public bool PreAuthenticate { get; set; }
        public bool KeepAlive { get; set; }
        public  X509CertificateCollection ClientCertificates { get; set; }

    }
}