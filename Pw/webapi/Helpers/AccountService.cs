using PwServer.Models;
using System.Security.Claims;

namespace webapi.Helpers
{
    public class AccountService
    {
        public static ClaimsIdentity Authenticate(UserInfoModel user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "1"),
                };
            return new ClaimsIdentity(
                            claims,
                            "ApplicationCookie",
                            ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType);

        }
    }
}
