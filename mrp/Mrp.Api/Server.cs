using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;
using Mrp.Api.Controller;

namespace Mrp.Api;

public class Server
{
    private readonly HttpListener Listener = new HttpListener();
    public string Port {get; set;}
    public static int RequestCount = 0;

    private UserController _userController;
    private readonly MediaController _mediaController;
    public Server(string port, UserController userController, MediaController mediaController)
    {
        Listener.Prefixes.Add($"http://localhost:{port}/");

        _userController = userController;
        _mediaController = mediaController;
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

            // Console.WriteLine("Request #: {0}", ++RequestCount);
            // Console.WriteLine(req.Url.ToString());
            // Console.WriteLine(req.HttpMethod);
            // Console.WriteLine(req.UserHostName);
            // Console.WriteLine(req.UserAgent);
            string path = context.Request.Url.AbsolutePath;
            string[] parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);



            if (req.HttpMethod == "GET")
            {

                if (parts[0] == "users")
                {
                    await _userController.Handle(context, parts);
                }
                else if (parts[0] == "media")
                {
                    await _mediaController.Handle(context, parts);
                }

                else
                {
                    var response = new ApiResponse
                    {
                        Error = "not found",
                    };

                    await JSONhelper.SendJSON(resp, 404, response);

                }
            }
            else if (req.HttpMethod == "POST")
            {
                if (parts[0] == "users")
                {
                    await _userController.Handle(context, parts);
                }
                else if (parts[0] == "media")
                {
                    await _mediaController.Handle(context, parts);
                }
            }

            else if (req.HttpMethod == "PUT")
            {
                if (parts[0] == "media")
                {
                    await _mediaController.Handle(context, parts);
                }
            }

            else if (req.HttpMethod == "DELETE")
            {
                if (parts[0] == "media")
                {
                    await _mediaController.Handle(context, parts);
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


            await JSONhelper.SendJSON(resp, 500, response);
        }

        finally
        {
            context.Response.Close();
        }
    }


}
