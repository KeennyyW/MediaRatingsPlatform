using System;
using System.Net;
using System.Security;
using System.Text.Json;
using Microsoft.VisualBasic;
using Mrp.Business;

namespace Mrp.Api.Controller;

using System.Security.Authentication;
using Mrp.Api.RequestModels;

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
                var response = new ApiResponse
                {
                    Error = e.Message
                };

                await JSONhelper.SendJSON(resp, 401, response);  
            }
            catch (ArgumentException e)
            {
                var response = new ApiResponse
                {
                    Error = e.Message
                };

                await JSONhelper.SendJSON(resp, 400, response);  
            }
            catch (JsonException e)
            {
                var response = new ApiResponse
                {
                    Error = e.Message
                };

                await JSONhelper.SendJSON(resp, 400, response);                  
            }

        }
        
    }
}
