using Microsoft.AspNetCore.Mvc;

using ThreadService.HostedServices;
using ThreadService.Model;
using ThreadService.Repository;
using ThreadService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                      });
});
builder.Services.AddHttpClient();
builder.Services.Configure<ForumApiDatabaseSettings>(builder.Configuration.GetSection("ForumApiDatabase"));
builder.Services.Configure<AuthServiceSettings>(builder.Configuration.GetSection("AuthServiceSettings"));
builder.Services.AddSingleton<IThreadRepository, ThreadRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddScoped<IThreadServices, ThreadServices>();
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
app.UseCors();

app.MapGet("/threads", async ([FromServices] IThreadServices threadServices, [FromQuery] int limit, [FromQuery] int page) =>
{
    var paginated = new Paginated
    {
        Limit = limit,
        Page = page,
    };
    return await threadServices.GetThreads(paginated);
})
.WithName("GetThreads");

app.MapGet("/threads/{id}", async ([FromServices] IThreadServices threadServices, Guid id) =>
{
    return await threadServices.GetThreadById(id);
})
.WithName("GetThreadById");

app.MapGet("/threads/search", async ([FromServices] IThreadServices threadServices, string keyword) =>
{
    return await threadServices.SearchThreads(keyword);
})
.WithName("SearchThreads");

app.MapGet("/threads/user/{id}", async ([FromServices] IThreadServices threadServices, Guid id) =>
{
    return await threadServices.GetThreadsByUserId(id);
})
.WithName("GetThreadByUserId");

app.MapPost("/threads", async ([FromServices] IThreadServices threadServices, Threads thread, HttpRequest request) =>
{
    return await threadServices.Insert(thread, request);
})
.WithName("CreateThread");

app.MapPut("/threads/{id}", async ([FromServices] IThreadServices threadServices, Guid id, [FromBody] Threads thread) =>
{
    return await threadServices.Update(id, thread);
})
.WithName("UpdateThread");

app.MapDelete("/threads/{id}", async ([FromServices] IThreadServices threadServices, Guid id) =>
{
    await threadServices.Delete(id);
    return Results.Ok();
})
.WithName("DeleteThread");

app.Run();