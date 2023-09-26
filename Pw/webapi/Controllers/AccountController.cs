using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PwServer.Models;
using System.Security.Claims;
using webapi.Data;
using webapi.Helpers;
using webapi.Models;

namespace PwServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private webapiContext _webapiContext;

        public AccountController(webapiContext webapiContext)
        {
            _webapiContext = webapiContext;
        }


        [HttpPost]
        [Route("api/CreateAccount")]
        public async Task<IActionResult> CreateAccount(string name, string email, string password)
        {
            try
            {
                var user = new UserInfoModel
                {
                    Name = name,
                    Email = email,
                    Password = password.HashPassword(),
                    Amount = 500
                };
                _webapiContext.UserInfoModel.Add(user);
                _webapiContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = _webapiContext.UserInfoModel.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                    return StatusCode(400, "user do not exist");
                }
                if (user.Password != password.HashPassword())
                {
                    return StatusCode(300, "wrong password");
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "1"),
                };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)).Wait();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpGet]
        [Route("Logout")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Userinformation")]
        [ResponseCache(Duration = 30)]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Userinformation()
        {
            var userInfo = new UserInfoModel();
            try
            {
                var email = HttpContext.User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                    ?.Claims?.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
                userInfo = _webapiContext.UserInfoModel.FirstOrDefault(x => x.Email == email);
            }
            catch (Exception e)
            {
                return NotFound();
            }
            return Ok(userInfo);
        }

    }
}