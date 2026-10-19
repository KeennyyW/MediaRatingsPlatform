using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace Mrp.Api;

public class Server
{
    private HttpListener listener = new HttpListener();
    public string prefix {get; set;}
    private HttpListenerContext context;
    Server()
    {
        listener.Prefixes.Add(prefix);
    }

    public void Start()
    {
        try
        {
            listener.Start();
            Console.WriteLine("HttpListener listening..."); 
        }
        catch (HttpListenerException)
        {
            Console.WriteLine("Error starting HttpListener");
        }
        finally
        {
            listener.Close();
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        
    }
}
