using Helpdesk.Infrastructure.Data;
using Helpdesk.Infrastructure.Repositories;
using Helpdesk.Application.Interfaces;
using Helpdesk.Application.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using Helpdesk.Application.Interfaces.Security;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} {Properties} {NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // DATABASE
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // DEPENDENCY INJECTION
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ITicketRepository, TicketRepository>();
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<TicketService>();

    // JWT CONFIG
    var jwtKey = builder.Configuration["JwtToken"];

    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey!)
                ),

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        });

    // CONTROLLERS
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // BUILD APP
    var app = builder.Build();

    // MIDDLEWARE
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCorrelationId();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    File.WriteAllText("crash.txt", ex.ToString());
    Log.Fatal(ex, "Aplicação encerrou inesperadamente");
    Console.WriteLine(ex);
    Console.ReadLine();
}
finally
{
    Log.CloseAndFlush();
}