using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using MyApi.Models;
using System.Linq;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context)
        {
            _context = context;
        }

        // Seuls les admins peuvent consulter la liste de tous les utilisateurs
        [HttpGet]
        [Authorize(Roles = "admin")]  // L'admin peut voir tous les utilisateurs
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        // Modification d'un utilisateur (admin ou client)
        [HttpPut("{id}")]
        [Authorize]  // L'utilisateur doit être authentifié pour modifier son compte
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            var currentUser = _context.Users.Find(id);
            if (currentUser == null)
                return NotFound();

            // Si l'utilisateur n'est pas l'admin et essaie de modifier un autre utilisateur
            if (User.Identity.Name != currentUser.Username && !User.IsInRole("admin"))
            {
                return Forbid();  // L'accès est interdit
            }

            // Si c'est l'admin ou l'utilisateur lui-même, on met à jour
            currentUser.Username = user.Username;
            currentUser.Password = user.Password; 
            currentUser.Email = user.Email;

            _context.SaveChanges();
            return Ok(new { message = "Utilisateur mis à jour" });
        }

        // Suppression d'un utilisateur (admin ou client)
        [HttpDelete("{id}")]
        [Authorize]  // L'utilisateur doit être authentifié pour supprimer son compte
        public IActionResult DeleteUser(int id)
        {
            var currentUser = _context.Users.Find(id);
            if (currentUser == null)
                return NotFound();

            // Si l'utilisateur n'est pas l'admin et essaie de supprimer un autre utilisateur
            if (User.Identity.Name != currentUser.Username && !User.IsInRole("admin"))
            {
                return Forbid();  // L'accès est interdit
            }

            _context.Users.Remove(currentUser);
            _context.SaveChanges();
            return Ok(new { message = "Utilisateur supprimé" });
        }
    }
}
