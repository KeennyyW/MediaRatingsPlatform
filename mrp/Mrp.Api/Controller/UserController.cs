using System;
using System.Net;
using System.Security;
using System.Text.Json;
using Microsoft.VisualBasic;
using Mrp.Business;

namespace Mrp.Api.Controller;

using System.Security.Authentication;
using Mrp.Api.RequestModels;
using Mrp.Models;

public class UserController
{
    private UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    public async Task Handle(HttpListenerContext context, string[] parts)
    {   
        if (parts[1] == "register")
        {
            
            using var reader = new StreamReader(context.Request.InputStream);
            string body = reader.ReadToEnd();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            RegisterRequest? request = JsonSerializer.Deserialize<RegisterRequest>(body, options);
            string username = request.Username;
            string password = request.Password;

            Console.WriteLine(username);
            Console.WriteLine(password);

            _userService.Register(username, password);
        }

        else if (parts[1] == "login")
        {
            var resp = context.Response;

            try
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = reader.ReadToEnd();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                RegisterRequest? request = JsonSerializer.Deserialize<RegisterRequest>(body, options);
                string username = request.Username;
                string password = request.Password;         

                string token = _userService.Login(username, password);

                var response = new ApiResponse
                {
                    Data = token,
                    Error = null
                };

                await JSONhelper.SendJSON(resp, 200, response);                
            }

            catch (InvalidCredentialException e)
            {
                await SendError(resp, 401, e.Message);
            }
            catch (ArgumentException e)
            {
                await SendError(resp, 400, e.Message); 
            }
            catch (JsonException e)
            {
                await SendError(resp, 400, e.Message);                 
            }

        }
        else if (parts[1] == "me")
        {
            string? header = context.Request.Headers["Authorization"];

            var resp = context.Response;
            if (header == null || !header.StartsWith("Bearer "))
            {
                await SendError(resp, 401, "No bearer token");  
                return;
            }

            string token = header.Substring(7);
            User? user = _userService.GetUserByToken(token);
            
            if (user == null)
            {
                await SendError(resp, 401, "user is null");
                return;
            }

            var response =  new ApiResponse
            {
                Data = user.Username
            };

            await JSONhelper.SendJSON(resp, 200, response);
        }
        
    }

private async Task SendError(HttpListenerResponse resp, int statusCode, string message)
{
    var response = new ApiResponse 
    { 
        Error = message 
    };

    await JSONhelper.SendJSON(resp, statusCode, response);
}
}
