using Serilog;
using MongoDB.Driver;
using Store.Core.Contracts;
using Store.Core.Repositories;
using Store.Core.Services;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Main DataBase
builder.Services.AddTransient<IUserRepository, UserDbRepository>();
builder.Services.AddTransient<IProductRepository, ProductDbRepository>();
builder.Services.AddTransient<IOrderRepository, OrderDbRepository>();

// Cache
builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient("mongodb+srv://EdvinasPocius:st83OA8OyHilnaNS@cluster.pjckb.mongodb.net/?retryWrites=true&w=majority&appName=Cluster"));
builder.Services.AddTransient<IUserCacheRepository, UserCacheRepository>();
builder.Services.AddTransient<IProductCacheRepository, ProductCacheRepository>();
builder.Services.AddTransient<IOrderCacheRepository, OrderCacheRepository>();

// Services
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IOrderService, OrderService>();

builder.Services.Configure<JsonOptions>(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var cacheDelete1 = app.Services.GetRequiredService<IUserService>().DeleteCache();
var cacheDelete2 = app.Services.GetRequiredService<IProductService>().DeleteCache();
var cacheDelete3 = app.Services.GetRequiredService<IOrderService>().DeleteCache();

app.Run();
