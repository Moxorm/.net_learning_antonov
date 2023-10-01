using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PwServer.Models
{
    [Table("Users")]
    public class UserInfoModel
    {
        public UserInfoModel()
        {

        }
        public UserInfoModel(UserInfoModel userInfoModel)
        {
            ID = userInfoModel.ID;
            Name = userInfoModel.Name;
            Email = userInfoModel.Email;
            Password = userInfoModel.Password;
            Amount = userInfoModel.Amount;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string? Name { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Index(IsUnique = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}
