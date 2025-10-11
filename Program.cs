using xp_bank.Data;
using xp_bank.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// LiteDB Context
builder.Services.AddSingleton<LiteDbContext>();

// HttpClient for ReceitaWS
builder.Services.AddHttpClient<ReceitaWsService>();

// Application Services
builder.Services.AddScoped<MerchantService>();
builder.Services.AddScoped<PixService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SafePIX API",
        Version = "v1",
        Description = "Sistema de Bloqueio de PIX para Casas de Apostas - LiteDB Local"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SafePIX API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
