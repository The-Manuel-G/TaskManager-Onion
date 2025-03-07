using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  namespace DomainLayer.Models
    {
        public class RefreshToken
        {
            public int Id { get; set; }
            public string? Token { get; set; }
            public DateTime Expires { get; set; }
            public bool IsExpired => DateTime.UtcNow >= Expires;

            // Relación con User
            public int UserId { get; set; }
            public User? User { get; set; }
        }
    }


