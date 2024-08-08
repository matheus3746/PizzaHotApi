
using Microsoft.EntityFrameworkCore;
using PizzaHotApi.Data;
using PizzaHotApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("pizzadb"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/pizzas", async (AppDbContext db) => await db.Pizzas.ToListAsync());

app.MapGet("/pizzas/{id}", async (AppDbContext db, int id) => await db.Pizzas.FindAsync(id));

app.MapPost("/pizzas", async (AppDbContext db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();    
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

app.MapPut("/pizza/{id}", async (AppDbContext db, Pizza updatepizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    pizza.Nome = updatepizza.Nome;
    pizza.Descricao = updatepizza.Descricao;
    pizza.Quantidade = updatepizza.Quantidade;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/pizza/{id}", async (AppDbContext db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if(pizza is null)
    {
        return Results.NotFound();
    }
    db.Pizzas.Remove(pizza);
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();

