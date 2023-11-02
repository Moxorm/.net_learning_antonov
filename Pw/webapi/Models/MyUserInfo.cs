using PwServer.Models;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class MyUserInfo
    {
        public MyUserInfo() { }
        public MyUserInfo(UserInfoModel user)
        {
            ID = user.ID;
            Name = user.Name;
            Email = user.Email;
            Amount = user.Amount;
        }
        public uint ID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public uint Amount { get; set; }
    }
}
