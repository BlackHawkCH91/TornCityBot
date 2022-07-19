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
            ExplicitProxyEndPoint explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8080, true);

            proxyServer.Start();
            proxyServer.AddEndPoint(explicitEndPoint);
            //proxyServer.SetAsSystemProxy(httpProxy, ProxyProtocolType.AllHttp);
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

            //proxyServer.BeforeRequest += OnBeforeRequest;
            proxyServer.BeforeResponse += OnBeforeResponse;
            Console.ReadLine();
        }
        
        public static void StopProxy()
        {
            proxyServer.Stop();
        }

        /*static async Task OnBeforeResponse(object sender, SessionEventArgs e) => await Task.Run(() => {
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            { 
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    byte[] bodyBytes = e.GetResponseBody().Result;
                    e.SetResponseBody(bodyBytes);
                    string body = e.GetResponseBodyAsString().Result;
                    body = body.Replace("<title>Chrome Headless Detection (Round II)</title>", "<title>My Example Domain</title>");
                    e.SetResponseBodyString(body);
                    *//*if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        
                    }*//*
                }
            }
        });*/

        static async Task OnBeforeRequest(object sender, SessionEventArgs e) => await Task.Run(() => {
            var method = e.HttpClient.Request.Method.ToUpper();
            if ((method == "POST" || method == "PUT" || method == "PATCH" || method == "GET")) {
                //Get/Set request body bytes
                byte[] bodyBytes = e.GetRequestBody().Result;
                e.SetRequestBody(bodyBytes);
                //Get/Set request body as string
                string body = e.GetRequestBodyAsString().Result;
                body = body.Replace("<title>Chrome Headless Detection (Round II)</title>", "<title>My Example Domain</title>");
                e.SetRequestBodyString(body);
            }
        });

        static async Task OnBeforeResponse(object sender, SessionEventArgs ev)
        {
            var request = ev.HttpClient.Request;
            var response = ev.HttpClient.Response;

            if (String.Equals(ev.HttpClient.Request.RequestUri.Host, "www.example.com", StringComparison.OrdinalIgnoreCase))
            {
                var body = await ev.GetResponseBodyAsString();

                body = body.Replace("<title>Example Domain</title>", "<title>My Example Domain</title>");

                ev.SetResponseBodyString(body);
            }
        }
    }
}
