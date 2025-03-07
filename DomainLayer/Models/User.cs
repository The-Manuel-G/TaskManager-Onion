namespace DomainLayer.Models
{
    public class User
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }

        // Contraseña encriptada (hash)
        public string? PasswordHash { get; set; }

        public bool IsVerified { get; set; }
        public string? PhoneNumber { get; set; }

        // Lista de roles
        public List<string>? Roles { get; set; }
    }
}
