
using System.ComponentModel.DataAnnotations.Schema;
using Mrp.Api;
using Mrp.Business;
using Mrp.Data;
using Mrp.Models;
DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();


var db = new Database();
var userRepository = new AdoNetUserRepository(db.GetConnectionString());

var UserService = new UserService(userRepository);

Server server = new Server("4534");
await server.Start();



