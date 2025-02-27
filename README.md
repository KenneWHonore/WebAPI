# WebAPI
Mise sur pied des API pour la gestion des autorisations et authentifications .

# 🚀 Création d'une Web API avec Swagger et Tokens  

Ce projet est une **API RESTful** développée avec **ASP.NET Core**, intégrant **JWT (JSON Web Token)** pour l’authentification et l’autorisation. Il permet la gestion des utilisateurs (**Admin, Client**) ainsi que des produits.

---

## 🛠️ Étapes de création  

### 1️⃣ **Création du projet dans Visual Studio**  
1. Ouvrir **Visual Studio**  
2. Cliquer sur **Créer un nouveau projet**  
3. Sélectionner **ASP.NET Core Web API** et cliquer sur **Suivant**  
4. Choisir un nom pour le projet (ex: `MyApi`), définir l’emplacement et cliquer sur **Créer**  
5. Sélectionner **.NET 8.0** et cocher **Activer OpenAPI (Swagger)**  

---

### 2️⃣ **Ajout des packages nécessaires**  
Dans le **Terminal** de Visual Studio, exécuter :  
```sh
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Swashbuckle.AspNetCore


3️⃣ Configuration de l’authentification avec JWT
Dans Program.cs, ajouter :

csharp
Copier
Modifier
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ClientPolicy", policy => policy.RequireRole("Client"));
});
Ajouter ensuite la configuration dans appsettings.json :

json
Copier
Modifier
"Jwt": {
  "Key": "MaSuperCleSecrete123456789",
  "Issuer": "https://localhost:5001",
  "Audience": "https://localhost:5001"
}
4️⃣ Mise en place du modèle Utilisateur et Authentification
Dans Models/User.cs :

csharp
Copier
Modifier
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
Dans Controllers/AuthController.cs :

csharp
Copier
Modifier
[HttpPost("login")]
public IActionResult Login([FromBody] UserLoginModel login)
{
    var user = _userService.ValidateUser(login.Username, login.Password);
    if (user == null) return Unauthorized();

    var token = _jwtService.GenerateToken(user);
    return Ok(new { Token = token });
}
5️⃣ Création de la gestion des produits
Dans Models/Product.cs :

csharp
Copier
Modifier
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
Dans Controllers/ProductsController.cs :

csharp
Copier
Modifier
[Authorize(Policy = "AdminPolicy")]
[HttpPost]
public IActionResult AddProduct([FromBody] Product product)
{
    _context.Products.Add(product);
    _context.SaveChanges();
    return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
}
6️⃣ Ajout de Swagger pour la documentation de l’API
Dans Program.cs, ajouter :

csharp
Copier
Modifier
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Activer Swagger dans Program.cs :

csharp
Copier
Modifier
app.UseSwagger();
app.UseSwaggerUI();
L’interface de Swagger sera disponible à l’adresse :
📌 https://localhost:5001/swagger/index.html

7️⃣ Tester l’API avec Swagger
Lancer l’application avec dotnet run
Ouvrir Swagger et tester les endpoints :
POST /auth/login → Obtenir un token
GET /products (nécessite un token Admin)
POST /products (ajouter un produit)
🎯 Conclusion
Cette Web API permet une gestion sécurisée des utilisateurs et des produits, avec une séparation des rôles Admin et Client. Swagger facilite la documentation et les tests.