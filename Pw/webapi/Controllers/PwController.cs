using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public class PwController : ControllerBase
    {
        private webapiContext _webapiContext;

        public PwController(webapiContext webapiContext)
        {
            _webapiContext = webapiContext;
        }


        //[httpget]
        //public ienumerable<weatherforecast> transactionhistory()
        //{
        //    return enumerable.range(1, 5).select(index => new weatherforecast
        //    {
        //        date = dateonly.fromdatetime(datetime.now.adddays(index)),
        //        temperaturec = random.shared.next(-20, 55),
        //        summary = summaries[random.shared.next(summaries.length)]
        //    })
        //    .toarray();
        //}

        //[HttpGet]
        //[Route("api/Userinformation")]
        //public async Task<IActionResult> Userinformation(string name)
        //{
        //    var userInfo = new UserInfoModel();
        //    try
        //    {

        //        userInfo = _webapiContext.UserInfoModel.FirstOrDefault(x => EF.Functions.Like(x.Name, $"%{name}%"));
        //    }
        //    catch (Exception e)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(userInfo);
        //}
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
        [HttpGet]
        [Route("GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
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
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(
                        AccountService.Authenticate(user)));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/Login")]
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
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(
                        AccountService.Authenticate(user)));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

/*        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }*/


        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("CreateTransaction")]
        public IActionResult CreateTransaction(TransactionModel transactionModel)
        {
            try
            {
                var Sender = _webapiContext.UserInfoModel.Find(transactionModel.Sender);
                if (Sender.Amount < transactionModel.Amount)
                {
                    return StatusCode(300, "Not enought money");
                }
                Sender.Amount -= transactionModel.Amount;
                _webapiContext.UserInfoModel.Find(transactionModel.Recipinent).Amount += transactionModel.Amount;
                _webapiContext.TransactionModel.Add(transactionModel);
                _webapiContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }
    }
}