using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PwServer.Models;
using System.Net;
using System.Security.Claims;
using System.Web.Http.ModelBinding;
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
        [Route("CreateAccount")]
        public IActionResult CreateAccount(UserInfoModel user)
        {
            try
            {
                user.Amount = 500;
                user.Password = user.Password.HashPassword();

                if (_webapiContext.UserInfoModel.Where(x => x.Email == user.Email).Count() != 0)
                {
                    return StatusCode(409, "thim email alredy used");
                }

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
        public IActionResult Login(string email, string password)
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
                    new Claim(ClaimTypes.NameIdentifier,user.ID.ToString()),
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

        [HttpPost]
        [Route("Logout")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet] 
        [Route("UserInformation")]
        [ResponseCache(Duration = 30)]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UserInformation()
        {
            var userInfo = new MyUserInfo();
            try
            {
                var email = HttpContext.User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                    ?.Claims?.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
                userInfo = new MyUserInfo(_webapiContext.UserInfoModel.FirstOrDefault(x => x.Email == email));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok(userInfo);
        }

        [HttpGet]
        [Route("AllUsersInformation")]
        [ResponseCache(Duration = 30)]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AllUsersInformation()
        {
            var userInfo = new List<UsersInfo>();
            try
            {
                var email = HttpContext.User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                    ?.Claims?.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
                userInfo = _webapiContext.UserInfoModel.Select(x => new UsersInfo(x)).ToList();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok(userInfo);
        }

    }
}