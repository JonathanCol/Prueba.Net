using RegistroPersonas.Negocio;
using RegistroUsuarios.Infrastructure.Repositories.SQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(_ =>
    new PersonaRepository(builder.Configuration.GetConnectionString("DB")
        ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DB'.")));
builder.Services.AddScoped<PersonaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
