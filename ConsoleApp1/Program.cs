using System.Text.Json;
using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading;
namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                GroupId = "product-view-consumers",
                BootstrapServers = "kafka:9092", // Kafka konteyner adı
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("product-views"); // Kafka konusuna abone ol

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    Console.WriteLine("Kafka'dan mesajlar bekleniyor...");
                    while (true)
                    {
                        try
                        {
                            // Kafka mesajını tüket
                            var consumeResult = consumer.Consume(cts.Token);

                            // Gelen mesajı konsola yazdır
                            Console.WriteLine($"Mesaj alındı: {consumeResult.Message.Value}");

                            // Mesajı deserialize et
                            var productView = JsonSerializer.Deserialize<ProductViewEvent>(consumeResult.Message.Value);

                            // Mesajın içeriğini işle (örneğin, veritabanına kaydetme veya analiz yapma)
                            Console.WriteLine($"Event: {productView.Event}, ProductId: {productView.Properties.ProductId}, Timestamp: {productView.Timestamp}");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Mesaj tüketme hatası: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close(); // Tüketici düzgün bir şekilde kapatılıyor
                    Console.WriteLine("Tüketici kapatıldı.");
                }
            }
        }

        public class ProductViewEvent
        {
            public string Event { get; set; }
            public string MessageId { get; set; }
            public string UserId { get; set; }
            public Properties Properties { get; set; }
            public Context Context { get; set; }
            public string Timestamp { get; set; } // ISO 8601 zaman damgası
        }

        public class Properties
        {
            public string ProductId { get; set; }
        }

        public class Context
        {
            public string Source { get; set; }
        }
    }
}