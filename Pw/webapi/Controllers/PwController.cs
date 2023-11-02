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
    public class PwController : ControllerBase
    {
        private webapiContext _webapiContext;

        public PwController(webapiContext webapiContext)
        {
            _webapiContext = webapiContext;
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ResponseCache(Duration = 30)]
        [Route("TransactionHistory")]
        public IActionResult TransactionHistory()
        {
            var transactions = new List<TransactionModel>();
            try
            {
                var id = HttpContext.User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                    ?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                transactions = _webapiContext.TransactionModel
                    .Where(x => x.Sender == int.Parse(id) || x.Recipinent == int.Parse(id))
                    .Select(x => new TransactionModel(x)).ToList();
                _webapiContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok(transactions);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Route("CreateTransaction")]
        public IActionResult CreateTransaction(int amount, int recipinent)
        {
            try
            {
                var id = int.Parse(HttpContext.User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                    ?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                
                var Sender = _webapiContext.UserInfoModel.Find(id);
                if (Sender.Amount < amount)
                {
                    return StatusCode(300, "Not enought money");
                }
                Sender.Amount -= amount;

                var transactionModel = new TransactionModel
                {
                    Amount = amount,
                    Recipinent = recipinent,
                    Sender = id,
                };

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