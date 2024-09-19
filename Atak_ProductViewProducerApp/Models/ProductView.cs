using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atak_ProductViewProducerApp.Models
{
    public class ProductView
    {
        public string Event { get; set; }
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public Properties? Properties { get; set; }
        public Context? Context { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
