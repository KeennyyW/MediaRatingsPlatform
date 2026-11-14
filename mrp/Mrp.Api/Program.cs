
using System.ComponentModel.DataAnnotations.Schema;
using Mrp.Api;
using Mrp.Api.Controller;
using Mrp.Business;
using Mrp.Business.Services;
using Mrp.Data;
using Mrp.Data.Repositories;
using Mrp.Models;
DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();


var db = new Database();
var userRepository = new AdoNetUserRepository(db.GetConnectionString());

var userService = new UserService(userRepository);
var userController = new UserController(userService);

var mediaRepository = new AdoNetMediaRepository(db.GetConnectionString());
var mediaService = new MediaService(mediaRepository);
var mediaController = new MediaController(mediaService, userService);

Server server = new Server("4534", userController, mediaController);
await server.Start();



