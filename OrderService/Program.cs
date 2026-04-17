using OrderService.Services;
using OrderService.Repository;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using static OrderService.Repository.IOrderRepository;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// MongoDB
builder.Services.AddSingleton<IMongoClient>(s => 
    new MongoClient("mongodb://localhost:27017"));

builder.Services.AddSingleton<IMongoCollection<OrderService.Models.Order>>(s =>
{
    var client = s.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase("orderdb");
    return database.GetCollection<OrderService.Models.Order>("orders");
});

// Repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ProductCatalogService>();

// CatalogService HTTP klient
builder.Services.AddHttpClient("Catalog", client =>
{
    client.BaseAddress = new Uri("http://localhost:5125");
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();