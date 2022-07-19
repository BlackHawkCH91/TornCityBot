using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace TornCityBot
{
    public static class Proxy
    {
        static ProxyServer proxyServer = new ProxyServer(userTrustRootCertificate: true);
        public static void StartProxy()
        {
            ExplicitProxyEndPoint httpProxy = new ExplicitProxyEndPoint(IPAddress.Any, 8080, true);

            proxyServer.Start();
            proxyServer.AddEndPoint(httpProxy);
            //proxyServer.SetAsSystemProxy(httpProxy, ProxyProtocolType.AllHttp);
            proxyServer.SetAsSystemHttpProxy(httpProxy);
            proxyServer.SetAsSystemHttpsProxy(httpProxy);

            proxyServer.BeforeResponse += OnBeforeResponse;
        }
        
        public static void StopProxy()
        {
            proxyServer.Stop();
        }

        static async Task OnBeforeResponse(object sender, SessionEventArgs e) => await Task.Run(() => {
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        byte[] bodyBytes = e.GetResponseBody().Result;
                        e.SetResponseBody(bodyBytes);
                        string body = e.GetResponseBodyAsString().Result;
                        e.SetResponseBodyString(body);
                    }
                }
            }
        });

        /*static async Task OnBeforeResponse(object sender, SessionEventArgs ev)
        {
            var request = ev.HttpClient.Request;
            var response = ev.HttpClient.Response;

            var body = await ev.GetResponseBodyAsString();
            body = body.Replace("<title>Chrome Headless Detection (Round II)</title>", "<title>My Example Domain</title>");
            ev.SetResponseBodyString(body);

            //if (String.Equals(ev.HttpClient.Request.RequestUri.Host, "www.example.com", StringComparison.OrdinalIgnoreCase)){}
        }*/
    }
}
