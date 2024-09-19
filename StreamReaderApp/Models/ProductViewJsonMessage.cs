using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StreamReaderApp.Models
{
    public class ProductViewJsonMessage
    {
        public string Events { get; set; } // Event türü
        public string MessageId { get; set; } // Mesaj kimliği
        public string UserId { get; set; } // Kullanıcı kimliği

        // İç içe geçmiş nesneleri doğrudan burada tanımlıyoruz
        public ProductIdDetails ProductId { get; set; } // Ürün kimliği
        public SourceDetails Source { get; set; } // Kaynak

        public DateTime Timestamp { get; set; } // Zaman damgası

        // İç içe geçmiş nesneleri doğrudan burada tanımlıyoruz
        public class ProductIdDetails
        {
            public string ProductId { get; set; } // Ürün kimliği
        }

        public class SourceDetails
        {
            public string Source { get; set; } // Kaynak
        }
    }

    //public class ProductId
    //{
    //    public int ID { get; set; }
    //    public string? ProductId { get; set; }
    //}

    //public class _Context
    //{
    //    public int ID { get; set; }
    //    public string? Source { get; set; }
    //}
}
