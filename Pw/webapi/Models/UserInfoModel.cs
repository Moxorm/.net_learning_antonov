using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PwServer.Models
{
    [Table("Users")]
    [Microsoft.EntityFrameworkCore.Index(nameof(UserInfoModel.Email),IsUnique = true)]
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
        public uint ID { get; set; }
        [Required]

        [MaxLength(40)]
        [MinLength(7)]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Required]
        [MinLength(10)]
        public string? Password { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public uint Amount { get; set; }
    }
}
