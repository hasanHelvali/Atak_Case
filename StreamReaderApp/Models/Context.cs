using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamReaderApp.Models
{
    public class Context
    {
        public int ? Id { get; set; }
        public string ?Source { get; set; }

        // Navigation properties
        public ICollection<ProductView> ?ProductViews { get; set; }
    }
}
