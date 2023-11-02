using PwServer.Models;

namespace webapi.Models
{
    public class UsersInfo
    {
        public UsersInfo() { }
        public UsersInfo(UserInfoModel user)
        {
            ID = user.ID;
            Name = user.Name;
            Email = user.Email;
        }
        public uint ID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
