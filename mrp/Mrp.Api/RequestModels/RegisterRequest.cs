using System;

namespace Mrp.Api.RequestModels;

public class RegisterRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
