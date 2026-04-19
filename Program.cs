var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var appName = builder.Configuration["App:Name"] ?? "IsLabApp";
var appVersion = builder.Configuration["App:Version"] ?? "0.1.0";

// Эндпоинт /health - проверка здоровья
app.MapGet("/health", () => new {
    status = "ok",
    timestamp = DateTime.UtcNow
});

// Эндпоинт /version - версия приложения
app.MapGet("/version", () => new {
    name = appName,
    version = appVersion
});

// Эндпоинт /db/ping - проверка БД (пока заглушка)
app.MapGet("/db/ping", () => new {
    status = "not_configured",
    message = "Database not configured yet"
});

// Хранилище заметок (в памяти)
var notes = new List<Note>();
var counter = 1;

// POST /api/notes - создать заметку
app.MapPost("/api/notes", (CreateNoteDto dto) => {
    var note = new Note
    {
        Id = counter++,
        Title = dto.Title,
        Text = dto.Text,
        CreatedAt = DateTime.UtcNow
    };
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

// GET /api/notes - получить все заметки
app.MapGet("/api/notes", () => notes);

// GET /api/notes/{id} - получить одну заметку
app.MapGet("/api/notes/{id}", (int id) => {
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note is not null ? Results.Ok(note) : Results.NotFound();
});

// DELETE /api/notes/{id} - удалить заметку
app.MapDelete("/api/notes/{id}", (int id) => {
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note is not null) notes.Remove(note);
    return Results.NoContent();
});

app.Run();

// Классы для работы с заметками
class Note
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}

class CreateNoteDto
{
    public string Title { get; set; }
    public string Text { get; set; }
}