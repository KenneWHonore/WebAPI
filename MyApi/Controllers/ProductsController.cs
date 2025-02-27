using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using MyApi.Models;
using Microsoft.AspNetCore.Http;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProductsController(ApiDbContext context)
        {
            _context = context;
        }

        // Tout le monde authentifié peut consulter la liste des produits
        [HttpGet]
        [Authorize]  // Tous les utilisateurs authentifiés peuvent voir les produits
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        // Seuls les admins peuvent ajouter un produit
        [HttpPost]
        [Authorize(Roles = "admin")] // Seulement l'admin peut ajouter un produit
        public IActionResult AddProduct([FromBody] Product product)
        {
            // Vérification que le produit n'est pas null
            if (product == null)
            {
                return BadRequest(new { message = "Produit invalide" });
            }

            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(new { message = "Produit ajouté" });
        }

        // Seuls les admins peuvent modifier un produit
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")] // Seulement l'admin peut modifier un produit
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            // Vérification que le produit n'est pas null
            if (product == null)
            {
                return BadRequest(new { message = "Produit invalide" });
            }

            var existing = _context.Products.Find(id);
            if (existing == null)
                return NotFound(new { message = "Produit non trouvé" });

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            _context.SaveChanges();
            return Ok(new { message = "Produit mis à jour" });
        }

        // Seuls les admins peuvent supprimer un produit
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // Seulement l'admin peut supprimer un produit
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound(new { message = "Produit non trouvé" });

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok(new { message = "Produit supprimé" });
        }
    }
}
