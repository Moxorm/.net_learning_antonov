using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PwServer.Models;
using webapi.Data;

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

        [HttpGet]
        [Route("api/Userinformation")]
        public IActionResult Userinformation(string name)
        {
            var userInfo = new UserInfoModel();
            try
            {

                userInfo = _webapiContext.UserInfoModel.FirstOrDefault(x => EF.Functions.Like(x.Name, $"%{name}%"));
            }
            catch (Exception e)
            {
                return NotFound();
            }
            return Ok(userInfo);
        }

        [HttpPost]
        [Route("api/CreateAccount")]
        public IActionResult CreateAccount(string name, string email, string password)
        {
            try
            {
                _webapiContext.UserInfoModel.Add(new UserInfoModel
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    Amount = 500
                });
                _webapiContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/Login")]
        public IActionResult Login(string email, string password)
        {
            var msg = "error";
            var result = !string.IsNullOrEmpty(msg)
                ? BadRequest(msg)
                : (IActionResult)Ok();
            return result;
        }


        [HttpPost]
        [Route("api/CreateTransaction")]
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