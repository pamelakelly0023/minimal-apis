using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MinimalApi.Models;
using MinimalApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Adiciona DBContext
builder.Services.AddDbContext<MinimalApiDbContext>(options => options.UseInMemoryDatabase("Todos"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("todos", (MinimalApiDbContext dbContext) 
    => TypedResults.Ok(dbContext.Todos.ToList()))
    .WithName("TodoList"); 

app.MapGet("todos/{id:int}", Results<Ok<Todo>, NotFound> (int id, MinimalApiDbContext dbContext)
    => dbContext.Todos.FirstOrDefault(x => x.Id == id) is Todo todo ?
        TypedResults.Ok(todo) : TypedResults.NotFound()
    ).WithName("TodoDetails");

app.MapPost("todos", (TodoApiInput input, MinimalApiDbContext dbContext) 
    => {
        var todo = new Todo(){ Description = input.Description };
        dbContext.Todos.Add(todo);
        dbContext.SaveChanges();
        return TypedResults.CreatedAtRoute("TodoDetails", todo);
    })
    .WithName("TodoList");

app.MapDelete("todos/{id:int}", Results<NoContent, NotFound> (int id, MinimalApiDbContext dbContext)
    => {
        var todo = dbContext.Todos.FirstOrDefault(x => x.Id == id);
        if(todo is null)
            return TypedResults.NotFound();
        dbContext.Todos.Remove(todo);
        dbContext.SaveChanges();
        return TypedResults.NoContent();
    }).WithName("TodoDelete");

app.MapPut("todos/{id:int}", Results<Ok<Todo>, NotFound> (TodoApiInput input, int id, MinimalApiDbContext dbContext)
    => {
        var todo = dbContext.Todos.FirstOrDefault(x => x.Id == id);
        if(todo is null)
            return TypedResults.NotFound();
        todo.Description = input.Description;
        dbContext.Todos.Update(todo);
        dbContext.SaveChanges();
        return TypedResults.Ok(todo);
    }).WithName("TodoUpdate");


app.Run();

record TodoApiInput(string Description = "");