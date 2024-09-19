using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamReaderApp.Models
{
    public class User
    {
        //public int ?Id { get; set; }
        public string ?UserId { get; set; }

        // Navigation properties
        public ICollection<ProductView> ?ProductViews { get; set; }
    }
}
