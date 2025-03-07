using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
   
        public static class UserRoles
        {
            public const string Admin = "Admin";
            public const string Dev = "Dev";
            public const string Client = "Client";

            public static readonly HashSet<string> AllowedRoles = new HashSet<string> { Admin, Dev, Client };
        }
    
}
