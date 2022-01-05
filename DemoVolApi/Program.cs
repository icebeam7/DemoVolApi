var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LibraryContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("LibraryConnectionSqlite"));
});

var app = builder.Build();

var factory = app.Services.GetService<IServiceScopeFactory>();

if (factory != null)
{
    using var scope = factory.CreateScope();
    var service = scope.ServiceProvider;

    if (service != null)
    {
        try
        {
            var db = service.GetRequiredService<LibraryContext>();
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = service.GetRequiredService<ILogger<Program>>();
            logger?.LogError(ex, "Database Creation/Migration failed");
        }
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Get All Books
app.MapGet("/api/books", async (LibraryContext db) =>
    await db.Books.ToListAsync()
)
.Produces<List<Book>>(StatusCodes.Status200OK)
.WithName("GetAllBooks").WithTags("Books");

//Get Books by ID 
app.MapGet("/books/{id}", async (LibraryContext db, int id) =>
{
    try
    {
        var x = await db.Books.FindAsync(id) is Book mybook ? Results.Ok(mybook) : Results.NotFound();
        return x;
    }
    catch (System.Exception ex)
    {
        Console.WriteLine(ex.Message);
        return Results.NotFound();
    }
}
)
.Produces<Book>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetBookByID").WithTags("Books");

app.MapPost("/api/books",
    async ([FromBody] Book newBook, [FromServices] LibraryContext db, HttpResponse response) =>
    {
        db.Books.Add(newBook);
        await db.SaveChangesAsync();
        return Results.Ok(newBook);
    })
.Accepts<Book>("application/json")
.Produces<Book>(StatusCodes.Status201Created)
.WithName("AddNewBook").WithTags("Books");

// Update existing book 
app.MapPut("/books/{id}", async (int id, [FromBody] Book updatedBook, [FromServices] LibraryContext db, HttpResponse response) =>
{
    if (id != updatedBook.Id)
        return Results.BadRequest();

    db.Entry(updatedBook).State = EntityState.Modified;

    try
    {
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.NotFound();
    }
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest)
.WithName("UpdateBook").WithTags("Books");

// Delete existing book 
app.MapDelete("/books/{id}", async (int id, [FromServices] LibraryContext db, HttpResponse response) =>
{
    var myBook = await db.Books.FindAsync(id);

    if (myBook == null)
        return Results.NotFound();

    db.Books.Remove(myBook);

    await db.SaveChangesAsync();
    return Results.Ok(myBook);
})
.Produces<Book>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("DeleteBook").WithTags("Books");


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}