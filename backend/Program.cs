using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// application services
builder.Services.AddScoped<PreClear.Api.Interfaces.IAuthService, PreClear.Api.Services.AuthService>();

// Connection
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException("Please set DefaultConnection in appsettings.json");

// EF Core 8 + Pomelo (MySQL)
builder.Services.AddDbContext<PreclearDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

// Log which connection string is being used (mask password) to help troubleshooting
var effectiveConn = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(effectiveConn))
{
    try
    {
        // mask password value for logging
        var masked = System.Text.RegularExpressions.Regex.Replace(effectiveConn, "(Password=)([^;]+)", "$1****", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        builder.Logging.AddConsole();
        Console.WriteLine($"Using DB connection: {masked}");
    }
    catch { }
}

// CORS (dev)
builder.Services.AddCors(p => p.AddDefaultPolicy(q => q.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Apply migrations automatically in Development only
using (var scope = app.Services.CreateScope())
{
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    if (env.IsDevelopment())
    {
        var db = scope.ServiceProvider.GetRequiredService<PreclearDbContext>();
        db.Database.Migrate();
        PreClear.Api.Services.DbSeeder.Seed(db);

    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();
