//using Confluent.Kafka;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using StreamReaderApp.Context;
//using StreamReaderApp.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//namespace StreamReaderApp.Services
//{
//    //public class KafkaCunsomerService:IHostedService
//    //{
//    //    private readonly IConfiguration _configuration;
//    //    private readonly IServiceProvider _serviceProvider;
//    //    private IConsumer<Ignore, string> _consumer;
//    //    private CancellationTokenSource _cts;

//    //    public KafkaCunsomerService(IConfiguration configuration, IServiceProvider serviceProvider)
//    //    {
//    //        _configuration = configuration;
//    //        _serviceProvider = serviceProvider;
//    //    }

//    //    //public KafkaConsumerService(IConfiguration configuration, IServiceProvider serviceProvider)
//    //    //{
//    //    //    _configuration = configuration;
//    //    //    _serviceProvider = serviceProvider;
//    //    //}

//    //    public Task StartAsync(CancellationToken cancellationToken)
//    //    {
//    //        var config = new ConsumerConfig
//    //        {
//    //            GroupId = _configuration["Kafka:GroupId"],
//    //            BootstrapServers = _configuration["Kafka:BootstrapServers"],
//    //            AutoOffsetReset = AutoOffsetReset.Earliest
//    //        };

//    //        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
//    //        _consumer.Subscribe(_configuration["Kafka:Topic"]);

//    //        _cts = new CancellationTokenSource();
//    //        Task.Run(() => ConsumeMessagesAsync(_cts.Token), cancellationToken);

//    //        return Task.CompletedTask;
//    //    }
//    //    private async Task ConsumeMessagesAsync(CancellationToken token)
//    //    {
//    //        try
//    //        {
//    //            while (!token.IsCancellationRequested)
//    //            {
//    //                var consumeResult = _consumer.Consume(token);

//    //                Console.WriteLine($"Mesaj alındı: {consumeResult.Message.Value}");

//    //                var productView = ParseProductView(consumeResult.Message.Value);

//    //                using (var scope = _serviceProvider.CreateScope())
//    //                {
//    //                    var dbContext = scope.ServiceProvider.GetRequiredService<ProductViewContext>();
//    //                    dbContext.ProductViews.Add(productView);
//    //                    await dbContext.SaveChangesAsync();
//    //                }

//    //                Console.WriteLine("Veritabanına kaydedildi.");
//    //            }
//    //        }
//    //        catch (OperationCanceledException)
//    //        {
//    //            _consumer.Close();
//    //        }
//    //    }
//    //    public Task StopAsync(CancellationToken cancellationToken)
//    //    {
//    //        _cts.Cancel();
//    //        _consumer?.Close();
//    //        return Task.CompletedTask;
//    //    }
//    //     private ProductView ParseProductView(string message)
//    //    {
//    //        var viewEvent = System.Text.Json.JsonSerializer.Deserialize<ProductViewEvent>(message);

//    //        return new ProductView
//    //        {
//    //            UserId = viewEvent.UserId,
//    //            ProductId = viewEvent.Properties?.ProductId,
//    //            ViewedAt = viewEvent.Timestamp
//    //        };
//    //    }
//    //}


//    //public class KafkaCunsomerService : BackgroundService
//    //{
//    //    private readonly IConfiguration _configuration;
//    //    private IConsumer<Ignore, string> _consumer;

//    //    public KafkaCunsomerService(IConfiguration configuration)
//    //    {
//    //        _configuration = configuration;
//    //        InitializeConsumer();
//    //    }

//    //    private void InitializeConsumer()
//    //    {
//    //        var config = new ConsumerConfig
//    //        {
//    //            BootstrapServers = _configuration.GetValue<string>("Kafka:BootstrapServers"),
//    //            GroupId = _configuration.GetValue<string>("Kafka:GroupId"), // Add this line
//    //            AutoOffsetReset = AutoOffsetReset.Earliest,
//    //            EnableAutoCommit = false
//    //        };

//    //        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
//    //        _consumer.Subscribe(_configuration.GetValue<string>("Kafka:Topic"));
//    //    }

//    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    //    {
//    //        while (!stoppingToken.IsCancellationRequested)
//    //        {
//    //            try
//    //            {
//    //                var cr = _consumer.Consume(stoppingToken);
//    //                // Handle the message
//    //                Console.WriteLine($"Message received: {cr.Value}");
//    //                // Process the message
//    //            }
//    //            catch (ConsumeException e)
//    //            {
//    //                Console.WriteLine($"Error occurred: {e.Error.Reason}");
//    //            }
//    //        }
//    //    }

//    //    public override Task StopAsync(CancellationToken cancellationToken)
//    //    {
//    //        _consumer.Close();
//    //        return base.StopAsync(cancellationToken);
//    //    }
//    //}

//    public class KafkaCunsomerService
//    {
//        private readonly ProductViewContext _dbContext;
//        private readonly string _kafkaBootstrapServers;
//        private readonly string _kafkaTopic;

//        public KafkaCunsomerService(ProductViewContext dbContext, string kafkaBootstrapServers, string kafkaTopic)
//        {
//            _dbContext = dbContext;
//            _kafkaBootstrapServers = kafkaBootstrapServers;
//            _kafkaTopic = kafkaTopic;
//        }

//        public async Task StartAsync(CancellationToken cancellationToken)
//        {
//            var config = new ConsumerConfig
//            {
//                BootstrapServers = _kafkaBootstrapServers,
//                GroupId = "product-view-consumer-group",
//                AutoOffsetReset = AutoOffsetReset.Earliest,
//            };

//            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
//            consumer.Subscribe(_kafkaTopic);

//            await Task.Run(() =>
//            {
//                while (!cancellationToken.IsCancellationRequested)
//                {
//                    try
//                    {
//                        var cr = consumer.Consume(cancellationToken);
//                        var message = cr.Value;

//                        Console.WriteLine($"Consumed message '{message}' from topic '{cr.Topic}'.");

//                        SaveMessageToDatabase(message);
//                    }   
//                    catch (ConsumeException e)
//                    {
//                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
//                    }
//                }
//            }, cancellationToken);
//        }

//        private void SaveMessageToDatabase(string message)
//        {
//            var productView = ParseMessage(message);

//            if (productView != null)
//            {
//                _dbContext.ProductViews.Add(productView);
//                _dbContext.SaveChanges();
//            }
//        }
//        private ProductView ParseMessage(string message)
//        {
//            try
//            {
//                using var jsonDoc = JsonDocument.Parse(message);
//                var root = jsonDoc.RootElement;

//                return new ProductView
//                {
//                    MessageId = Guid.Parse(root.GetProperty("messageid").GetString()),
//                    UserId = root.GetProperty("userid").GetString(),
//                    ProductId = root.GetProperty("properties").GetProperty("productid").GetString(),
//                    ContextSource = root.GetProperty("context").GetProperty("source").GetString(),
//                    Timestamp = DateTime.UtcNow // Mesajın timestamp'ını kullanıyorsanız, burayı değiştirebilirsiniz
//                };
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error parsing message: {ex.Message}");
//                return null;
//            }
//        }
//    }




//}
