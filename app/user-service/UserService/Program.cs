using Microsoft.AspNetCore.Mvc;
using UserService.Model;
using UserService.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForumApiDatabaseSettings>(builder.Configuration.GetSection("ForumApiDatabase"));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
// Add services to the container.
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


app.MapGet("/users", async ([FromServices] IUserRepository userRepository) =>
{
    return await userRepository.GetUsers();
})
.WithName("GetUsers");

app.MapPost("/users", async ([FromServices] IUserRepository userRepository, [FromBody] Users user) =>
{
    var existing = await userRepository.GetUserByEmail(user.Email);
    if (existing != null) {
        return Results.BadRequest(new {Message = "User Exists"});
    }
    var createdId = await userRepository.NewUser(user);
    return Results.Json(new { Message = "Created", UserId = createdId });
})
.WithName("CreateUser");

app.MapPut("/users", async ([FromServices] IUserRepository userRepository, [FromBody] UserUpdate user) =>
{
    var existing = await userRepository.GetUserById(user.Id);
    if (existing == null) {
        return Results.NotFound(new {Message = "User Not Found"});
    }
    existing.Name = user.Name;
    await userRepository.UpdateUser(user.Id, existing);
    return Results.Json(new { Message = "Updated" });
})
.WithName("UpdateUser");

app.MapPut("/users/password", async ([FromServices] IUserRepository userRepository, [FromBody] UserPassword user) =>
{
    var existing = await userRepository.GetUserById(user.Id);
    if (existing == null) {
        return Results.NotFound(new {Message = "User Not Found"});
    }
    existing.Password = user.Password;
    await userRepository.UpdateUserPassword(user.Id, existing);
    return Results.Json(new { Message = "Updated" });
})
.WithName("UpdateUserPassword");

app.MapDelete("/users", async ([FromServices] IUserRepository userRepository, [FromBody] ById data) =>
{
    var existing = await userRepository.GetUserById(data.Id);
    if (existing == null) {
        return Results.NotFound(new {Message = "User Not Found"});
    }
    await userRepository.DeleteUser(data.Id);
    return Results.Json(new { Message = "Deleted" });
})
.WithName("DeleteUser");

app.Run();