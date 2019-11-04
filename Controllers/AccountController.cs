using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System;
using System.Linq;
using Identity.ExternalClaims.Data;
namespace dotnet.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult SignInWithGitHub()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("GitHub", "http://localhost:4200/");
            return Challenge(authenticationProperties, "Github");
        }

        public async Task<IActionResult> HandleExternalLogin()
        {

            var info = await _signInManager.GetExternalLoginInfoAsync();
            
            var userFromManager = await _userManager.GetUserAsync(User);
            
            // var token = await _userManager.GetAuthenticationTokenAsync(userFromManager,"Github","access_token");
            // var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            // if (!result.Succeeded) //user does not exist yet
            // {
            //     var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            //     var newUser = new ApplicationUser {
            //         UserName = email,
            //         Email = email,
            //         EmailConfirmed = true
            //     };
            //     var createResult = await _userManager.CreateAsync(newUser);
            //     if (!createResult.Succeeded)
            //         throw new Exception(createResult.Errors.Select(e => e.Description).Aggregate((errors, error) => $"{errors}, {error}"));

            //     await _userManager.AddLoginAsync(newUser, info);
            //     var newUserClaims = info.Principal.Claims.Append(new Claim("userId", newUser.Id));
            //     await _userManager.AddClaimsAsync(newUser, newUserClaims);                                
            //     await _signInManager.SignInAsync(newUser, isPersistent: false);
            //     await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            // }
            // if (result.Succeeded)
            // {
            //     Console.WriteLine("--------------------------------------------------");
            //     Console.WriteLine("Working",info,userFromManager);
            //     Console.WriteLine("----------------------------------------------");

            //     return Redirect("http://localhost:4200/home");
            // }
            // else
            // {
                return Redirect("http://localhost:4200/home");
            // }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("http://localhost:4200");
        }
    }
}