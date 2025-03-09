using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTO
{
    public class TaskCreateDTO 
    {
          public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string AdditionalData { get; set; }
        public int UserId { get; set; }
    }
}
