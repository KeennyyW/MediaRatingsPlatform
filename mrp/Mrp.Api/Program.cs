
using System.ComponentModel.DataAnnotations.Schema;
using Mrp.Api;
using Mrp.Data;
DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();


var db = new Database();
await using var conn = await db.OpenConnection();

Server server = new Server("4534");
await server.Start();



