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
        [Route("TransactionHistory")]
        public IActionResult TransactionHistory(int userId)
        {
            try
            {
                var Sender = _webapiContext.UserInfoModel.Find(userId);
                _webapiContext.TransactionModel.Select(x => x.Sender == userId || x.Recipinent == userId);
                _webapiContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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