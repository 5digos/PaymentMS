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


var builder = WebApplication.CreateBuilder(args);

//Add services to the container
builder.Services.AddControllers();

#if DEBUG
builder.Configuration.AddUserSecrets<Program>();
#endif

//Database Context
var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

//Infrastructure services
builder.Services.AddScoped<IPaymentQuery, PaymentQuery>(); 
builder.Services.AddScoped<IPaymentCommand, PaymentCommand>();

//Application services
builder.Services.AddScoped<ICreatePaymentService, CreatePaymentService>(); 
builder.Services.AddScoped<IGetPaymentService, GetPaymentService>();
builder.Services.AddScoped<IUpdatePaymentStatusService, UpdatePaymentStatusService>();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentMS", Version = "1.0" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


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

//app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
