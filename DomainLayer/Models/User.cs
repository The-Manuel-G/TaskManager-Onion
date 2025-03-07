using System.Collections.Generic;

namespace DomainLayer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsVerified { get; set; }
        public string? PhoneNumber { get; set; }

        // Lista de roles (Serializado en la BD como JSON)
        public List<string>? Roles { get; set; }

        // Relación uno a muchos: Un usuario puede tener muchas tareas
        public ICollection<Tareas> Tareas { get; set; } = new List<Tareas>();
    }
}
