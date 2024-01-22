using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usando binding automático
app.MapGet("/todos/{project}", (
    string project, 
    int page
) => 
    new { Greetings = $"Show: {project}, page {page}"}
); 

// Usando binding explícito 
app.MapGet("/todos/explicito/{project}", (
    [FromBody] string project, 
    [FromQuery] int page
) => 
    new { Greetings = $"Show: {project}, page {page}"}
);

//Usando binding http request
app.MapGet("/todos/http", async(HttpRequest req, HttpResponse res) =>
{
    var name = req.Query["name"];
    await res.WriteAsync($"hello, { name }");
});

//Custom binding
app.MapPost("/todos/custom", (TodoCustom todo) => todo);

// Results

app.MapGet("/results", (int id)
    => id > 10 ? 
        Results.Ok(new Resp() { Message = "Ok", IsSuccess = true })
        : Results.NotFound()
)
    .Produces<Resp>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.Run();

// Responses
record Resp {
    public bool IsSuccess {get ; set ;}
    public string Message {get ; set ;}
};
record Todo(int Id, string Title);

record TodoCustom(int Id, string Title)
{
    public static bool TryParse(string todoEncoded, out TodoCustom? todo)
    {
        try{
            var parts = todoEncoded.Split(",");
            todo = new TodoCustom(int.Parse(parts[0]), parts[1]);
            return true;
        }catch(Exception ex)
        { 
            todo = null;
            return false;
        }
    }
}


