using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PwServer.Models;
using System.Security.Claims;
using webapi.Data;
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
            var transactions = new List<TransactionModelHistory>();
            try
            {
                var id = HttpContext.User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                    ?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                transactions = _webapiContext.TransactionModel
                    .Where(x => x.Sender == int.Parse(id) || x.Recipinent == int.Parse(id))
                    .Select(x => new TransactionModelHistory(x)).ToList();
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
        public IActionResult CreateTransaction(TransactionModel transaction)
        {
            try
            {
                if (transaction.Sender == transaction.Recipinent)
                {
                    return StatusCode(400, "You cannot transfer to the same account");
                }

                var id = uint.Parse(HttpContext.User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
                    ?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "No user");
                
                var Sender = _webapiContext.UserInfoModel.Find(id);
                if (Sender.Amount < transaction.Amount)
                {
                    return StatusCode(300, "Not enought money");
                }
                Sender.Amount -= transaction.Amount;

                transaction.Sender = id;
                transaction.Date = DateTime.UtcNow;
                _webapiContext.UserInfoModel.Find(transaction.Recipinent).Amount += transaction.Amount;
                _webapiContext.TransactionModel.Add(transaction);
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