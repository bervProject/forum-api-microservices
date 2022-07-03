using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Redis.OM;

using UserService.HostedServices;
using UserService.Model;
using UserService.Repository;
using UserService.Service;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration["RedisConnectionString"]));
builder.Services.Configure<ForumApiDatabaseSettings>(builder.Configuration.GetSection("ForumApiDatabase"));
builder.Services.Configure<AuthServiceSettings>(builder.Configuration.GetSection("AuthServiceSettings"));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddHostedService<IndexCreationService>();
// add Automapper
builder.Services.AddAutoMapper(typeof(UserProfile));
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

app.MapPost("/users", async ([FromServices] IUserServices userServices, [FromServices] IMapper mapper, [FromBody] UserCreation userCreation) =>
{
    var user = mapper.Map<Users>(userCreation);
    return await userServices.NewUser(user);
})
.WithName("CreateUser");

app.MapPut("/users", async ([FromServices] IUserServices userServices, [FromBody] UserUpdate user, HttpRequest req) =>
{
    return await userServices.UpdateUser(user.Id, user, req);
})
.WithName("UpdateUser");

app.MapPut("/users/password", async ([FromServices] IUserServices userServices, [FromBody] UserPassword user, HttpRequest req) =>
{
    return await userServices.UpdateUserPassword(user.Id, user, req);
})
.WithName("UpdateUserPassword");

app.MapDelete("/users", async ([FromServices] IUserServices userServices, [FromBody] ById data, HttpRequest req) =>
{
    return await userServices.DeleteUser(data.Id, req);
})
.WithName("DeleteUser");

app.Run();