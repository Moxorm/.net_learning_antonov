using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PwServer.Models
{
    [Table("TransactionModel")]
    public class TransactionModel
    {
        public TransactionModel()
        {

        }
        public TransactionModel(TransactionModel transactionModel)
        {
            ID = transactionModel.ID;
            Amount = transactionModel.Amount;
            Sender = transactionModel.Sender;
            Recipinent = transactionModel.Recipinent;
        }
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
