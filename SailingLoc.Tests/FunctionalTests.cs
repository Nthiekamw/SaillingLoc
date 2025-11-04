// using System.Net;
// using System.Net.Http;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using FluentAssertions;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Xunit;



// namespace SailingLoc.Tests
// {
//    public class FunctionalTests : IClassFixture<WebApplicationFactory<SaillingLoc.Program>>


//     {
//         private readonly HttpClient _client;

//         public FunctionalTests(WebApplicationFactory<SailingLoc.Program> factory)
//         {
//             _client = factory.CreateClient();
//         }

//         [Fact]
//         public async Task Get_HomePage_ShouldReturnSuccess()
//         {
//             var response = await _client.GetAsync("/");

//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//             var content = await response.Content.ReadAsStringAsync();
//             content.Should().Contain("Catégories des Bateaux");
//         }

//         [Fact]
//         public async Task Get_LoginPage_ShouldReturnSuccess()
//         {
//             var response = await _client.GetAsync("/Account/Login");

//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//             var content = await response.Content.ReadAsStringAsync();
//             content.Should().Contain("Email");
//         }

//         [Fact]
//         public async Task Post_Login_WithValidCredentials_ShouldRedirect()
//         {
//             var postData = new FormUrlEncodedContent(new[]
//             {
//                 new KeyValuePair<string, string>("Input.Email", "test@example.com"),
//                 new KeyValuePair<string, string>("Input.Password", "Password123!")
//             });

//             var response = await _client.PostAsync("/Account/Login", postData);

//             response.StatusCode.Should().Be(HttpStatusCode.Redirect);
//             response.Headers.Location.Should().NotBeNull();
//             response.Headers.Location!.ToString().Should().Be("/");
//         }

//         [Fact]
//         public async Task Post_Login_WithInvalidCredentials_ShouldStayOnPage()
//         {
//             var postData = new FormUrlEncodedContent(new[]
//             {
//                 new KeyValuePair<string, string>("Input.Email", "test@example.com"),
//                 new KeyValuePair<string, string>("Input.Password", "WrongPassword!")
//             });

//             var response = await _client.PostAsync("/Account/Login", postData);

//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//             var content = await response.Content.ReadAsStringAsync();
//             content.Should().Contain("Email");
//         }
//     }
// }
