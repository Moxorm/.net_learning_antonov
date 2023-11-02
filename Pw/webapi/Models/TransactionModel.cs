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
        public uint ID { get; set; }

        [Required]
        [Range(0, uint.MaxValue)]
        public uint Amount { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public uint Sender { get; set; }
        [Required]
        public uint Recipinent { get; set; }
    }
}
