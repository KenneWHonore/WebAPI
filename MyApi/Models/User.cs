using System.ComponentModel.DataAnnotations;

namespace MyApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // En production, stocker un hash !

        public string Role { get; set; }  // Changer en string

        // Autres informations utilisateur (ex. email, nom...)
        public string Email { get; set; }
    }
}
