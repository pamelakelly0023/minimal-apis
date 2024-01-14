using Microsoft.AspNetCore.Mvc;

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

app.Run();
record Todo(int Id, string Title);
