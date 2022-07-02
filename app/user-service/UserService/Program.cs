using Microsoft.AspNetCore.Mvc;

using Redis.OM;

using UserService.HostedServices;
using UserService.Model;
using UserService.Repository;
using UserService.Service;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration["RedisConnectionString"]));
builder.Services.Configure<ForumApiDatabaseSettings>(builder.Configuration.GetSection("ForumApiDatabase"));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddHostedService<IndexCreationService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/users", async ([FromServices] IUserServices userServices) =>
{
    return await userServices.GetUsers();
})
.WithName("GetUsers");

app.MapGet("/users/{id}", async ([FromServices] IUserServices userServices, Guid id) =>
{
    return await userServices.GetUserById(id);
})
.WithName("GetUserById");

app.MapGet("/userByEmail/{email}", async ([FromServices] IUserServices userServices, string email) =>
{
    return await userServices.GetUserByEmail(email);
})
.WithName("GetUserByEmail");

app.MapPost("/users", async ([FromServices] IUserServices userServices, [FromBody] Users user) =>
{
    var createdUser = await userServices.NewUser(user);
    if (createdUser == null)
    {
        return Results.BadRequest(new { Message = "User Exists" });
    }
    return Results.Json(new { Message = "Created", User = createdUser });
})
.WithName("CreateUser");

app.MapPut("/users", async ([FromServices] IUserServices userServices, [FromBody] UserUpdate user) =>
{
    var result = await userServices.UpdateUser(user.Id, user);
    if (result == null)
    {
        return Results.NotFound(new { Message = "User Not Found" });
    }
    return Results.Json(new { Message = "Updated", User = result });
})
.WithName("UpdateUser");

app.MapPut("/users/password", async ([FromServices] IUserServices userServices, [FromBody] UserPassword user) =>
{
    var result = await userServices.UpdateUserPassword(user.Id, user);
    if (result == null)
    {
        return Results.NotFound(new { Message = "User Not Found" });
    }
    return Results.Json(new { Message = "Updated", User = result });
})
.WithName("UpdateUserPassword");

app.MapDelete("/users", async ([FromServices] IUserServices userServices, [FromBody] ById data) =>
{
    var result = await userServices.DeleteUser(data.Id);
    if (!result)
    {
        return Results.NotFound(new { Message = "User Not Found" });
    }
    return Results.Json(new { Message = "Deleted" });
})
.WithName("DeleteUser");

app.Run();