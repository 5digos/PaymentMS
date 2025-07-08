using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Interfaces;
using Application.UseCase;
using Infrastructure.Command;
using Infrastructure.Query;
using Application.Interfaces.IServices;
using Application.UseCase.Payments;
using Application.Interfaces.ICommand;
using Domain.Entities;
using MercadoPago.Config;
using Application.Interfaces.IServices.IReservationServices;
using Infrastructure.HttpClients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PaymentMS.Handlers;
using System.Net.Http.Headers;


var builder = WebApplication.CreateBuilder(args);

// Esto debería estar por defecto
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // Nivel mínimo



//Add services to the container
builder.Services.AddControllers();

#if DEBUG
builder.Configuration.AddUserSecrets<Program>();
#endif

//Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString
, sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure();
    sqlOptions.MigrationsAssembly("Infrastructure");
}));

//MercadoPago
builder.Services.AddScoped<MercadoPagoService>();


//Infrastructure services
builder.Services.AddScoped<IPaymentQuery, PaymentQuery>();
builder.Services.AddScoped<IPaymentCommand, PaymentCommand>();

//Application services
builder.Services.AddScoped<ICreatePaymentService, CreatePaymentService>();
builder.Services.AddScoped<IGetPaymentService, GetPaymentService>();
builder.Services.AddScoped<IUpdatePaymentStatusService, UpdatePaymentStatusService>();
builder.Services.AddScoped<IPaymentCalculationService, PaymentCalculationService>();

var reservationServiceUrl = builder.Configuration["ReservationService:BaseUrl"];
builder.Services.AddHttpClient<IReservationServiceClient, ReservationServiceClient>(client =>
{
    client.BaseAddress = new Uri(reservationServiceUrl);
});


//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentMS", Version = "1.0" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

//TokenConfiguration
var jwtKey = builder.Configuration["JwtSettings:key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("No se encontró 'JwtSettings:key'. Configúralo en User Secrets o Variables de Entorno.");
}

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ActiveUser", policy => policy.RequireClaim("IsActive", "True"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<BearerTokenHandler>();

//Configurar el HttpClient y asociarle el handler
builder.Services
    .AddHttpClient<IReservationServiceClient, ReservationServiceClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ReservationService:BaseUrl"]);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddHttpMessageHandler<BearerTokenHandler>();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("La aplicación ha arrancado correctamente");


app.Use(async (context, next) =>
{
    //continúa con la solicitud
    await next();

    //si el estado de la respuesta es 401 (No autorizado), añade los encabezados CORS
    if (context.Response.StatusCode == 401)
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, Content-Type");

    }
});

//configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
