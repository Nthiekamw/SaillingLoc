// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Xunit;
// using FluentAssertions;
// using SaillingLoc.Models;
// using SaillingLoc.Pages.Account;
// using Microsoft.AspNetCore.Mvc.RazorPages;

// namespace SailingLoc.Tests
// {
//     // Alias pour éviter l'ambiguïté avec Microsoft.AspNetCore.Identity.SignInResult
//     using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

//     // Fake UserStore pour UserManager
//     public class FakeUserStore : IUserStore<User>
//     {
//         public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
//         public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
//         public void Dispose() { }
//         public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<User?>(null);
//         public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<User?>(null);
//         public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.UserName ?? string.Empty);
//         public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.UserName ?? string.Empty);
//         public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.UserName ?? string.Empty);
//         public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
//         public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken) => Task.CompletedTask;
//         public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
//     }

//     // Fake UserManager
//     public class FakeUserManager : UserManager<User>
//     {
//         public FakeUserManager()
//             : base(new FakeUserStore(), null, null, null, null, null, null, null, null) { }

//         public override Task<User?> FindByEmailAsync(string email)
//         {
//             if (email == "test@example.com")
//                 return Task.FromResult<User?>(new User { UserName = "testuser", Email = email });

//             return Task.FromResult<User?>(null);
//         }
//     }

//     // Fake ClaimsPrincipalFactory
//     public class FakeUserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<User>
//     {
//         public Task<System.Security.Claims.ClaimsPrincipal> CreateAsync(User user)
//         {
//             var identity = new System.Security.Claims.ClaimsIdentity();
//             var principal = new System.Security.Claims.ClaimsPrincipal(identity);
//             return Task.FromResult(principal);
//         }
//     }

//     // Fake SignInManager
//     public class FakeSignInManager : SignInManager<User>
//     {
//         public FakeSignInManager(UserManager<User> userManager)
//             : base(userManager,
//                   new Microsoft.AspNetCore.Http.HttpContextAccessor(),
//                   new FakeUserClaimsPrincipalFactory(),
//                   null, null, null, null)
//         { }

//         public override Task<IdentitySignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
//         {
//             if (userName == "testuser" && password == "Password123!")
//                 return Task.FromResult(IdentitySignInResult.Success);

//             return Task.FromResult(IdentitySignInResult.Failed);
//         }
//     }

//     // Tests unitaires pour LoginModel
//     public class LoginModelTests
//     {
//         private readonly FakeUserManager _userManager;
//         private readonly FakeSignInManager _signInManager;

//         public LoginModelTests()
//         {
//             _userManager = new FakeUserManager();
//             _signInManager = new FakeSignInManager(_userManager);
//         }

//         [Fact]
//         public async Task OnPostAsync_WithValidCredentials_ShouldRedirectToIndex()
//         {
//             var loginModel = new LoginModel(_signInManager, _userManager)
//             {
//                 Input = new LoginModel.InputModel
//                 {
//                     Email = "test@example.com",
//                     Password = "Password123!"
//                 }
//             };

//             var result = await loginModel.OnPostAsync();

//             // Casting avant d'utiliser FluentAssertions
//             var redirectResult = result as RedirectToPageResult;
//             redirectResult.Should().NotBeNull();
//             redirectResult!.PageName.Should().Be("/Index");
//         }

//         [Fact]
//         public async Task OnPostAsync_WithInvalidPassword_ShouldReturnPageResult()
//         {
//             var loginModel = new LoginModel(_signInManager, _userManager)
//             {
//                 Input = new LoginModel.InputModel
//                 {
//                     Email = "test@example.com",
//                     Password = "WrongPassword!"
//                 }
//             };

//             var result = await loginModel.OnPostAsync();

//             var pageResult = result as PageResult;
//             pageResult.Should().NotBeNull(); // reste sur la page login
//         }

//         [Fact]
//         public async Task OnPostAsync_WithUnknownEmail_ShouldReturnPageResult()
//         {
//             var loginModel = new LoginModel(_signInManager, _userManager)
//             {
//                 Input = new LoginModel.InputModel
//                 {
//                     Email = "unknown@example.com",
//                     Password = "Password123!"
//                 }
//             };

//             var result = await loginModel.OnPostAsync();

//             var pageResult = result as PageResult;
//             pageResult.Should().NotBeNull();
//         }

//         [Fact]
//         public async Task OnPostAsync_WithInvalidModelState_ShouldReturnPageResult()
//         {
//             var loginModel = new LoginModel(_signInManager, _userManager);
//             loginModel.ModelState.AddModelError("Email", "Required");

//             var result = await loginModel.OnPostAsync();

//             var pageResult = result as PageResult;
//             pageResult.Should().NotBeNull();
//         }
//     }
// }
