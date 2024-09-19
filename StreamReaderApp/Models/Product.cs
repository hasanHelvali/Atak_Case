using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamReaderApp.Models
{
    public class Product
    {
        public string ?ProductId { get; set; }

        // Navigation properties
        public ICollection<ProductView>? ProductViews { get; set; }
    }
}
