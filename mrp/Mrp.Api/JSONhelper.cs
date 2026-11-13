using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Mrp.Api;

public static class JSONhelper
{
    public static async Task SendJSON(HttpListenerResponse resp, int statusCode,  ApiResponse data)
    {

        string json = JsonSerializer.Serialize(data);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        resp.StatusCode = statusCode;
        resp.ContentType = "application/json; charset=utf-8";
        resp.ContentLength64 = bytes.Length;

        await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
    }
}
