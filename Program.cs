var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Эндпоинт /health
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        time = DateTime.UtcNow.ToString("o")
    });
});

// Эндпоинт /version
app.MapGet("/version", (IConfiguration config) =>
{
    var name = config["App:Name"] ?? "IsLabApp";
    var version = config["App:Version"] ?? "1.0-lab";
    return Results.Ok(new { name, version });
});

// Эндпоинт /db/ping
app.MapGet("/db/ping", async (IConfiguration config) =>
{
    var conn = config.GetConnectionString("MsSql");
    try
    {
        await using var connection = new Microsoft.Data.SqlClient.SqlConnection(conn);
        await connection.OpenAsync();
        return Results.Ok(new { status = "ok", message = "Connected to MS SQL" });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { status = "error", message = ex.Message });
    }
});

// Данные для /api/notes
var notes = new List<Note>();
int nextId = 1;

// GET /api/notes
app.MapGet("/api/notes", () => notes);

// GET /api/notes/{id}
app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note is not null ? Results.Ok(note) : Results.NotFound();
});

// POST /api/notes
app.MapPost("/api/notes", (NoteCreate input) =>
{
    if (string.IsNullOrWhiteSpace(input.Title))
        return Results.BadRequest(new { error = "Title is required" });

    var note = new Note(nextId++, input.Title, input.Text?.Trim() ?? "", DateTime.Now);
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

// DELETE /api/notes/{id}
app.MapDelete("/api/notes/{id}", (int id) =>
{
    var index = notes.FindIndex(n => n.Id == id);
    if (index == -1) return Results.NotFound();
    notes.RemoveAt(index);
    return Results.NoContent();
});

app.Run();

// Модели
record Note(int Id, string Title, string Text, DateTime CreatedAt);
record NoteCreate(string Title, string? Text);