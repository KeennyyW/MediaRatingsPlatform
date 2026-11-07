using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace Mrp.Api;

public class Server
{
    private readonly HttpListener Listener = new HttpListener();
    public string Port {get; set;}
    public static int RequestCount = 0;
    public Server(string port)
    {
        Listener.Prefixes.Add($"http://localhost:{port}/");
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



            if (req.HttpMethod == "GET")
            {
                if (req.Url.AbsolutePath == "/ping")
                {
                    var response = new ApiResponse
                    {
                        Data = "ok",
                    };

                    await SendJSON(resp, 200, response);

                }
                else
                {
                    var response = new ApiResponse
                    {
                        Error = "not found",
                    };

                    await SendJSON(resp, 404, response);

                }
            }
            else
            {
                var body = $"Still needs to be implemented You requested: {req.HttpMethod} {req.Url}\n";
                var bytes = Encoding.UTF8.GetBytes(body);
                resp.StatusCode = 200;
                resp.ContentType = "text/plain; charset=utf-8";
                resp.ContentLength64 = bytes.Length;
                await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);

            }



        }
        catch (HttpListenerException)
        {
            Console.WriteLine("Error handling Request.");
        }
        catch (NullReferenceException)
        {
            var resp = context.Response;

            Console.WriteLine("Error handling Request.");

            var response = new ApiResponse
            {
                Error = "not found",
            };

            await SendJSON(resp, 500, response);
        }

        finally
        {
            context.Response.Close();
        }
    }

    private async Task SendJSON(HttpListenerResponse resp, int statusCode,  ApiResponse data)
    {

        string json = JsonSerializer.Serialize(data);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        resp.StatusCode = statusCode;
        resp.ContentType = "application/json; charset=utf-8";
        resp.ContentLength64 = bytes.Length;

        await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
    }
}
