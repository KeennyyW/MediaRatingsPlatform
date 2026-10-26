using System;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;

namespace Mrp.Api;

public class Server
{
    private readonly HttpListener Listener = new HttpListener();
    public string Prefix {get; set;}
    public static int RequestCount = 0;
    public Server(string prefix)
    {
        Listener.Prefixes.Add(prefix);
    }

    public async Task Start()
    {
        try
        {
            Listener.Start();
            
            Console.WriteLine("HttpListener listening..."); 

            while(true)
            {
                var context = Listener.GetContext();
                await HandleRequest(context);
            }
        }
        catch (HttpListenerException)
        {
            Console.WriteLine("Error starting HttpListener");
        }
        finally
        {
            Listener.Close();
        }
    }

    private async Task HandleRequest(HttpListenerContext context)
    {
        try
        {
            var req = context.Request;
            var resp = context.Response;

            Console.WriteLine("Request #: {0}", ++RequestCount);
            Console.WriteLine(req.Url.ToString());
            Console.WriteLine(req.HttpMethod);
            Console.WriteLine(req.UserHostName);
            Console.WriteLine(req.UserAgent);
        

            var body = $"Hello from HttpListener! You requested: {req.HttpMethod} {req.Url}\n";
            var bytes = Encoding.UTF8.GetBytes(body);

            resp.StatusCode = 200;
            resp.ContentType = "text/plain; charset=utf-8";
            resp.ContentLength64 = bytes.Length;

            await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            resp.Close();
            
        }
        catch (HttpListenerException)
        {
            Console.WriteLine("Error handling Request.");
        }
        finally
        {
            Listener.Stop();
            Listener.Close();
        }
    }
}
