using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalAPI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>(Options => Options.UseInMemoryDatabase("ToDoItems"));
await using var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", (Func<string>)(() => "Hello World!"));


app.MapGet("/todoitems", async (http) =>
{
    var dbContext = http.RequestServices.GetService<ToDoDbContext>();
    var todoItems = await dbContext.ToDoItems.ToListAsync();
    await http.Response.WriteAsJsonAsync(todoItems);
});

app.MapPost("/todoitems", async (http) =>
{
    var todoItem = await http.Request.ReadFromJsonAsync<Items>();
    var dbContext = http.RequestServices.GetService<ToDoDbContext>();
    dbContext.ToDoItems.Add(todoItem);
    await dbContext.SaveChangesAsync();
    http.Response.StatusCode = 204;

});

app.MapPut("todoitems/{id}", async (http) =>
{
    if(!http.Request.RouteValues.TryGetValue("id", out var id))
    {
        http.Response.StatusCode = 400;
        return;
    }

    var dbContext = http.RequestServices.GetService<ToDoDbContext>();
    var todoItem = await dbContext.ToDoItems.FindAsync(int.Parse(id.ToString()));
    if (todoItem == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    var inputToDoItem = await http.Request.ReadFromJsonAsync<Items>();
    todoItem.IsCompeleted = inputToDoItem.IsCompeleted;
    await dbContext.SaveChangesAsync();
    http.Response.StatusCode = 204;
});

app.MapDelete("/todoitems/{id}", async (http) =>
{
    if (!http.Request.RouteValues.TryGetValue("id", out var id))
    {
        http.Response.StatusCode = 400;
        return;
    }

    var dbContext = http.RequestServices.GetService<ToDoDbContext>();
    var todoItem = await dbContext.ToDoItems.FindAsync(int.Parse(id.ToString()));
    if (todoItem == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    dbContext.Remove(todoItem);
    await dbContext.SaveChangesAsync();
    http.Response.StatusCode = 204;

});
await app.RunAsync();


