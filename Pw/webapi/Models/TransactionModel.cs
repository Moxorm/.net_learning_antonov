using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PwServer.Models
{
    [Table("Transactions")]
    public class TransactionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Sender { get; set; }
        [Required]
        public int Recipinent { get; set; }
    }
}
