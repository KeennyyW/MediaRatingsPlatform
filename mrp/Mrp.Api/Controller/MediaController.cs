using System;
using System.Net;
using System.Text.Json;
using Mrp.Business;
using Mrp.Business.Services;
using Mrp.Models;

namespace Mrp.Api.Controller;

public class MediaController
{
    private readonly MediaService _mediaService;
    private readonly UserService _userService;

    public MediaController(MediaService mediaService, UserService userService)
    {
        _mediaService = mediaService;
        _userService = userService;
    }

    public async Task Handle(HttpListenerContext context, string[] parts)
    {
        var resp = context.Response;
        string method = context.Request.HttpMethod;

        try
        {
            User? user = GetCurrentUser(context);
            if (user == null)
            {
                await SendError(resp, 401, "Invalid Token");
                return;
            }

            
            if (parts.Length == 1)
            {
                if (method == "POST") // Create
                {
                    MediaEntry? entry = ReadBody(context);
                    if (entry == null)
                    {
                        await SendError(resp, 400, "Missing Body");
                        return;
                    }
                    MediaEntry newMedia = _mediaService.Create(entry, user.Id);
                    ApiResponse response = new ApiResponse
                    {
                        Data = newMedia
                    };

                    await JSONhelper.SendJSON(resp, 201, response);
                }

                else if (method == "GET") // Get all Entries
                {
                    ApiResponse response = new ApiResponse
                    {
                        Data = _mediaService.GetAll()
                    };
                    await JSONhelper.SendJSON(resp, 200, response);
                }

                else
                {
                    await SendError(resp, 405, "Method not allowed");
                }
            }
       
            else if (parts.Length == 2)
            {
                int id = 0;       
                                           
                bool checkInt = int.TryParse(parts[1], out id);     
                if (checkInt == false)                         
                {
                    await SendError(resp, 400, "Id must be a number");
                    return;
                }

                if (method == "GET") // Get specific entry by id
                {
                    MediaEntry found = _mediaService.GetById(id);
                    ApiResponse response = new ApiResponse
                    {
                        Data = found
                    };
                    await JSONhelper.SendJSON(resp, 200, response);
                }

                else if (method == "PUT") // Update entry
                {
                    MediaEntry? entry = ReadBody(context);
                    if (entry == null)
                    {
                        await SendError(resp, 400, "Missing Body");
                        return;
                    }
                    MediaEntry updated = _mediaService.Update(id, entry, user.Id);
                    ApiResponse response = new ApiResponse
                    {
                        Data = updated
                    };
                    await JSONhelper.SendJSON(resp, 200, response);
                }

                else if (method == "DELETE") // Delete Entry
                {
                    _mediaService.Delete(id, user.Id);
                    resp.StatusCode = 204;
                }

                else
                {
                    await SendError(resp, 405, "Method not allowed");
                }
            }
            else
            {
                await SendError(resp, 404, "not found");
            }
        }

        catch (ArgumentException e)
        {
            await SendError(resp, 400, e.Message);
        }
        
        catch (JsonException e)
        {
            await SendError(resp, 400, e.Message);
        }

        catch (UnauthorizedAccessException e)
        {
            await SendError(resp, 403, e.Message);
        }

        catch (KeyNotFoundException e)
        {
            await SendError(resp, 404, e.Message);
        }
    }

    private User? GetCurrentUser(HttpListenerContext context)
    {
        string? header = context.Request.Headers["Authorization"];
        if (header == null || !header.StartsWith("Bearer "))
        {
            return null;
        }
        string token = header.Substring(7);
        User? currentUser = _userService.GetUserByToken(token);

        return currentUser;
    }

    private async Task SendError(HttpListenerResponse resp, int statusCode, string message)
    {
        var response = new ApiResponse
        {
            Error = message
        };

        await JSONhelper.SendJSON(resp, statusCode, response);
    }

    private MediaEntry? ReadBody(HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);
        string body = reader.ReadToEnd();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<MediaEntry>(body, options);
    }
}