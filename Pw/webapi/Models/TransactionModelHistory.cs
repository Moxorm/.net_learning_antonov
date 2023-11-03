using PwServer.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class TransactionModelHistory
    {
        public TransactionModelHistory()
        {

        }
        public TransactionModelHistory(TransactionModel transactionModel)
        {
            ID = transactionModel.ID;
            Amount = transactionModel.Amount;
            Sender = transactionModel.Sender;
            Recipinent = transactionModel.Recipinent;
            Date = transactionModel.Date;
        }
        public uint ID { get; set; }
        public uint Amount { get; set; }
        public uint Sender { get; set; }
        public uint Recipinent { get; set; }
        public DateTime Date { get; set; }
    }
}
