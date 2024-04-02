using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var dbContext = services.GetRequiredService<ToDoDbContext>();

app.UseCors();
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () =>
{
    var todos = dbContext.Items.ToList();
    return JsonSerializer.Serialize(todos);
});

app.MapGet("/{id}", (int id) =>
{
    var item = dbContext.Items.FirstOrDefault(i => i.Id == id);
    if (item != null)
    {
        return JsonSerializer.Serialize(item);
    }

    return "Item with ID " + id + " not found";
});

app.MapPost("/", (Item item) =>
{
    dbContext.Items.Add(item);
    dbContext.SaveChanges();
    return "Item added successfully";
});

app.MapPut("/{id}", (int id, Item item) =>
{
    var existingItem = dbContext.Items.FirstOrDefault(i => i.Id == id);
    if (existingItem != null)
    {
        existingItem.Name = item.Name;
        existingItem.IsComplete = item.IsComplete;
        dbContext.SaveChanges();
    }

    return "Item updated successfully";
});

app.MapDelete("/{id}", (int id) =>
{
    var itemToDelete = dbContext.Items.FirstOrDefault(i => i.Id == id);
    if (itemToDelete != null)
    {
        dbContext.Items.Remove(itemToDelete);
        dbContext.SaveChanges();
        return "Item deleted successfully";
    }

    return "Item not found";
});

app.Run();