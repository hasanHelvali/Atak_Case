using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using StreamReaderApp.Models;
using System.Text.Json;
using StreamReaderApp.Context;
using System.Reflection.Metadata;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace StreamReaderApp
{
    public class Program
        //Bu uygulama Producer'dan kafka sunucusuna gonderilen verileri okumak ve bu verileri uygun bir sekilde db sunucusuna kaydetmek icin bir Cunsomer uygulamadır.
    {
        //private const string KafkaBootstrapServers = "localhost:9092";//Kafka lokal adresi 
        //private const string KafkaTopic = "product-views";//Kafka topic
        //private static readonly string ConnectionString = "Host=localhost;Port=5433;Username=postgres;Password=123456;Database=data-db";//connectionString

        //KONFIGURASYON VERILERI KOD ICERISINE GOMULMEMELIDIR
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            // Konfigurasyon verileri okunuyor.
            var kafkaBootstrapServers = configuration["Kafka:BootstrapServers"];
            var kafkaTopic = configuration["Kafka:Topic"];
            var kafkaGroupId = configuration["Kafka:GroupId"];
            var connectionString = configuration["PostgreSql:ConnectionString"];
            //Appsettings.json dosyasından ihtiyac duyulan bilgiler alındı.

            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ProductViewContext>();

                var config = new ConsumerConfig
                {
                    BootstrapServers = kafkaBootstrapServers,
                    GroupId = kafkaGroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                    //Kafka tüketicisi olusturuldu
                {
                    consumer.Subscribe(kafkaTopic);
                    Console.WriteLine("Consuming messages...");
                    var cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, e) =>
                    {
                        e.Cancel = true; // Ctrl+C veya diğer kesme sinyallerine yanıt verir.
                        cts.Cancel(); // Tüketici işlemini iptal eder.
                    };

                    while (true)//Sonsuz dongu
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            // Kafka'dan mesajları tüketir.

                            var productViewMessage = JsonSerializer.Deserialize<ProductViewJsonMessage>(cr.Value, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                PropertyNameCaseInsensitive = true
                            });// Mesajı deserialize eder. Modele cast edilir.

                            if (productViewMessage != null && !string.IsNullOrEmpty(productViewMessage.Events))//icerik kontrolu
                            {
                                var user = dbContext.Users.FirstOrDefault(u => u.UserId == productViewMessage.UserId);
                                //Kullanıcı alınır.
                                if (user == null)//Enity kontrol edilir. Yoksa eklenir
                                {
                                    user = new User { UserId = productViewMessage.UserId };
                                    dbContext.Users.Add(user);
                                    await dbContext.SaveChangesAsync();
                                }

                                var product = dbContext.Products.FirstOrDefault(p => p.ProductId == productViewMessage.ProductId.ProductId);
                                //Urun alınır
                                if (product == null)//Enity kontrol edilir. Yoksa eklenir.
                                {
                                    product = new Product { ProductId = productViewMessage.ProductId.ProductId };
                                    dbContext.Products.Add(product);
                                    await dbContext.SaveChangesAsync();
                                }
                                var contextEntity = dbContext.Contexts.FirstOrDefault(c => c.Source == productViewMessage.Source.Source);
                                //Source alınır.
                                if (contextEntity == null)//Enity Kontrol edilir. Yoksa eklenir.
                                {
                                    contextEntity = new Models.Context { Source = productViewMessage.Source.Source };
                                    dbContext.Contexts.Add(contextEntity);
                                    await dbContext.SaveChangesAsync();
                                }

                                var productViewEntity = new ProductView//Alınan veri tümüyle entity modelimize cast edilir
                                {
                                    Event = productViewMessage.Events,
                                    MessageId = productViewMessage.MessageId,
                                    Timestamp = productViewMessage.Timestamp,
                                    ProductId = product.ProductId,
                                    ContextId = contextEntity.Id,
                                    UserId = user.UserId
                                };
                                dbContext.ProductViews.Add(productViewEntity);//Dbye eklenir.

                            };
                               
                                await dbContext.SaveChangesAsync();//degisiklikler db'ye execute edilir.

                                Console.WriteLine($"Mesaj alındı ve kaydedildi: {cr.Value}");//Bilgi amaclı konsol kaydı olusturulur.
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Hata: {e.Error.Reason}");//Hata alındııgnda cosnole a loglama yapılır.
                        }
                        catch (OperationCanceledException)
                        {
                            consumer.Close();//Operasyonel bir hata olursa ilgili tüketici nesne kapatılır.
                            break;
                        }
                    }
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Konfigürasyonu al
                    var configuration = hostContext.Configuration;

                    // PostgreSQL bağlantı dizesini al
                    var connectionString = configuration["PostgreSql:ConnectionString"];

                    // NOT  -- ConnectionString parametrik olarak da alınabilirdi.. -------

                    //dbcontext yapılandırılır.
                    services.AddDbContext<ProductViewContext>(options =>
                        options.UseNpgsql(connectionString));
                });
    }
}
