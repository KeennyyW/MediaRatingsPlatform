using System;
using System.Net;
using System.Security;
using System.Text.Json;
using Microsoft.VisualBasic;
using Mrp.Business;

namespace Mrp.Api.Controller;
using Mrp.Api.RequestModels;

public class UserController
{
    private UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    public void Handle(HttpListenerContext context, string[] parts)
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
        
    }
}
