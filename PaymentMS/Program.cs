using Application.Interfaces.Gateway;
using Application.Interfaces.ICommand;
using Application.Interfaces.IServices;
using Application.Services;
using Application.ExternalService;
using Application.Configuration;
using Domain.Entities;
using Infrastructure.Command;
using Infrastructure.Persistence;
using Infrastructure.Query;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MercadoPagoSettings>(
       builder.Configuration.GetSection("MercadoPago"));

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient
builder.Services.AddHttpClient<IPaymentGateway, MercadoPagoService>();

// Register services
builder.Services.AddScoped<IPaymentCommand, PaymentCommand>();
builder.Services.AddScoped<IPaymentQuery, PaymentQuery>();
builder.Services.AddScoped<ICreatePaymentService, CreatePaymentService>();
builder.Services.AddScoped<IGetPaymentService, GetPaymentService>();
builder.Services.AddScoped<IUpdatePaymentStatusService, UpdatePaymentStatusService>();
builder.Services.AddScoped<IPaymentGateway, MercadoPagoService>();

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