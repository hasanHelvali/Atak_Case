using Atak_API.Abstraction;
using Atak_API.Context;
using Atak_API.Service;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Add DbContext for PostgreSQL
builder.Services.AddDbContext<APIDbContext>();

// Add Kafka consumer and producer configuration
var kafkaConfig = new ConsumerConfig
{
    GroupId = "consumer-group",
    BootstrapServers = "localhost:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};
//builder.Services.AddSingleton(kafkaConfig);
// Register the Kafka consumer service
//builder.Services.AddHostedService<KafkaConsumerService>();
// Register the recommendation service
//builder.Services.AddScoped<IRecommendationService, RecommendationService>();    

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBrowsingHistoryService, BrowsingHistoryService>();
builder.Services.AddScoped<IBestSellerService, BestSellerService>();
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
