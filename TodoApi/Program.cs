using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("ToDoList"));

var app = builder.Build();

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) => 
    await db.Todos.FindAsync(id));

app.MapPost("/todoitems", async (ToDoItem item, TodoDb db) =>
{
    db.Todos.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{item.Id}", item);
});

app.MapPut("/todoitems/{id}", async (int id, ToDoItem item, TodoDb db) =>
{
    var itemToBeUpdated = await db.Todos.FindAsync(id);
    if (itemToBeUpdated == null) return Results.NotFound();
    itemToBeUpdated.Name = item.Name;
    itemToBeUpdated.IsComplete = item.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var itemToBeDeleted = await db.Todos.FindAsync(id);
    if (itemToBeDeleted == null) return Results.NotFound();
    db.Todos.Remove(itemToBeDeleted);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/", () => "Hello World!");

app.Run();
