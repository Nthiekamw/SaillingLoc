// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using System.Threading.Tasks;
// using SaillingLoc; // Namespace exact du projet principal

// namespace SailingLoc.Tests.Functional
// {
//     [TestClass]
//     public class HomePageTests
//     {
//         private readonly WebApplicationFactory<Program> _factory;

//         // Constructeur pour initialiser la factory correctement
//         public HomePageTests()
//         {
//             _factory = new WebApplicationFactory<Program>();
//         }

//         [TestMethod]
//         public async Task HomePage_ShouldReturnSuccessStatusCode()
//         {
//             // Crée un client HTTP pour simuler les requêtes
//             var client = _factory.CreateClient();

//             // Appel de la page d'accueil
//             var response = await client.GetAsync("/");

//             // Vérifie que la réponse HTTP est 200 OK
//             response.EnsureSuccessStatusCode();
//         }
//     }
// }
