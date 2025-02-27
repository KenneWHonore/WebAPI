using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyApi.Data;
using MyApi.Models;
using System.Linq;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Route pour enregistrer un utilisateur
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            // Vérifier si le rôle est bien défini (ex: "admin" ou "client")
            if (string.IsNullOrEmpty(user.Role) ||
                (user.Role != "admin" && user.Role != "client"))
            {
                return BadRequest(new { message = "Rôle invalide. Utilisez 'admin' ou 'client'." });
            }

            // Ajouter l'utilisateur à la base de données
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new { message = "Utilisateur créé avec succès" });
        }

        // Route pour se connecter et obtenir un JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            // Recherche de l'utilisateur par username et password
            var user = _context.Users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Identifiants invalides" });
            }

            // Création du token JWT
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = Convert.ToInt32(jwtSettings["ExpiryMinutes"]);

            // Ajout des claims pour le token (y compris le rôle sous forme de string)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),  // Role en tant que string
                new Claim("UserId", user.Id.ToString())
            };

            // Création de la clé et des informations d'authentification
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            // Retourner le token JWT généré
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
