using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamReaderApp.Models
{
    public class ProductView
    {
        public int ?Id { get; set; }
        public string ?Event { get; set; }
        public string ?MessageId { get; set; }
        public DateTime? Timestamp { get; set; }

        // Foreign keys
        public string ?ProductId { get; set; }
        public Product ?Product { get; set; }//Nav Prop

        // Foreign keys
        public string ?UserId { get; set; }
        public User ?User { get; set; }//Nav Prop

        // Foreign keys
        public int ?ContextId { get; set; }
        public Context? Context { get; set; }//Nav Prop
    }
}
