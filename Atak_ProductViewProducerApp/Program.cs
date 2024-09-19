using Atak_ProductViewProducerApp.Models;
using Confluent.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atak_ProductViewProducerApp
{
    public class Program
    {
        //Bu uygulama verilen json dosyasındaki verilen kafka sunucusunu aktarmakla yukumludur.
        private const string KafkaBootstrapServers = "localhost:9092";//kafka nın ayaga kaldırıldıgı lokal adres.
        private const string KafkaTopic = "product-views";//kafkaya gonderilen message'lar bu topic üzerinden akışa alınır.
        static async Task Main(string[] args)
        {
            var config = new ProducerConfig//Kafka adresinin konfigurasyonu yapıldı
            {
                BootstrapServers = KafkaBootstrapServers
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
                //ProducerBuilder yapılandırıldı. Key null verildi ve aktarılan message lar string olarak tanımlandı.
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //Uygulamanın calıstıgı temel dizin alındı.
                string parentDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
                //Üç seviyeyi yukarıdaki dizini alır (proje dizinine ulaşmak için).
                string filePath = Path.Combine(parentDirectory, "Data", "product-views.json");
                // JSON dosyasının tam yolunu olusturur.

                //Console.WriteLine(filePath);

                var jsonLines = File.ReadAllLines(filePath);//Json daki butun satırlar okunur ve bir dizi icerisine alınır. Bunu ıterasyon ıcın yapıyoruz.
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };//Json Serialize ve Deserialize islemi yapılandırıldı. Modeller ile okunan verinin map edilmesi için gerekli...

                foreach (var line in jsonLines)//Okunan json dosyası için iterasyon baslatılıyor. Satır satır okuma yapıyoruz.
                {
                    var productView = JsonSerializer.Deserialize<ProductView>(line, options);
                    //Her bir json satırı ilgili yapılandırmayla ProductView modeline donusturuldu.

                    var timeStampedMessage = new
                    {
                        events = productView.Event,
                        messageid = productView.MessageId,
                        userid = productView.UserId,
                        productId = new { ProductId = productView.Properties?.ProductId },
                        source = new { source = productView.Context?.Source },
                        timestamp = DateTime.UtcNow.ToString("o")
                    };
                    //Okunan json verisini zaman damgası ekleyerek kullanabilecegim bir formata donusturdum.

                    var message = JsonSerializer.Serialize(timeStampedMessage);
                    //Ilgili yapılandırılmış veri kafkaya sunulacak. Json'a cast edildi.

                    await producer.ProduceAsync(KafkaTopic, new Message<Null, string> { Value = message });
                    //Olusturulan timeStampedMessage verisi Kafka tarafına gonderiliyor.

                    Console.WriteLine($"Produced message '{message}'");//Gonderilen verinin konsol ekranında goruntulenmesi saglandı.

                    //await Task.Delay(100);//Akışı yavaslatabiliriz.
                }
            }
        }
    }
}
