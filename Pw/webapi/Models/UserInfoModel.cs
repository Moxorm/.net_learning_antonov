using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PwServer.Models
{
    [Table("Users")]
    public class UserInfoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string? Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}
